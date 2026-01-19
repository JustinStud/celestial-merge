/**
 * Firebase Authentication Module
 * Handles user sign up, login, logout, and session management
 * Production-ready with comprehensive error handling
 */

import {
  createUserWithEmailAndPassword,
  signInWithEmailAndPassword,
  signOut,
  onAuthStateChanged,
  User,
  UserCredential,
  AuthError,
} from 'firebase/auth';
import { getAuthInstance } from './firebase_config';
import { AuthResult, ErrorCode } from './types';
import { createUserDocument } from './firestore_manager';

/**
 * Sign up a new user with email and password
 * @param email - User email address
 * @param password - User password (should be validated client-side for length/complexity)
 * @param username - Display username
 * @returns Promise with authentication result
 */
export const signUp = async (
  email: string,
  password: string,
  username: string
): Promise<AuthResult> => {
  try {
    // Validate input
    if (!email || !password || !username) {
      return {
        success: false,
        error: {
          code: ErrorCode.INVALID_INPUT,
          message: 'Email, password, and username are required',
        },
      };
    }

    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      return {
        success: false,
        error: {
          code: ErrorCode.INVALID_INPUT,
          message: 'Invalid email format',
        },
      };
    }

    // Validate password strength (minimum 6 characters as per Firebase requirement)
    if (password.length < 6) {
      return {
        success: false,
        error: {
          code: ErrorCode.INVALID_INPUT,
          message: 'Password must be at least 6 characters',
        },
      };
    }

    // Validate username
    if (username.length < 3 || username.length > 20) {
      return {
        success: false,
        error: {
          code: ErrorCode.INVALID_INPUT,
          message: 'Username must be between 3 and 20 characters',
        },
      };
    }

    const auth = getAuthInstance();
    const userCredential: UserCredential = await createUserWithEmailAndPassword(
      auth,
      email,
      password
    );

    // Create user document in Firestore
    const createUserResult = await createUserDocument(userCredential.user.uid, {
      email,
      username,
    });

    if (!createUserResult.success) {
      // If Firestore creation fails, delete the auth user
      await userCredential.user.delete();
      return {
        success: false,
        error: {
          code: ErrorCode.UNKNOWN_ERROR,
          message: 'Failed to create user profile',
        },
      };
    }

    return {
      success: true,
      user: {
        uid: userCredential.user.uid,
        email: userCredential.user.email,
        displayName: username,
      },
    };
  } catch (error) {
    const authError = error as AuthError;
    return {
      success: false,
      error: {
        code: authError.code || ErrorCode.AUTH_FAILED,
        message: getAuthErrorMessage(authError.code),
      },
    };
  }
};

/**
 * Sign in an existing user with email and password
 * @param email - User email address
 * @param password - User password
 * @returns Promise with authentication result
 */
export const signIn = async (email: string, password: string): Promise<AuthResult> => {
  try {
    // Validate input
    if (!email || !password) {
      return {
        success: false,
        error: {
          code: ErrorCode.INVALID_INPUT,
          message: 'Email and password are required',
        },
      };
    }

    const auth = getAuthInstance();
    const userCredential: UserCredential = await signInWithEmailAndPassword(auth, email, password);

    // Update last login timestamp
    const { updateUserLastLogin } = await import('./firestore_manager');
    await updateUserLastLogin(userCredential.user.uid);

    return {
      success: true,
      user: {
        uid: userCredential.user.uid,
        email: userCredential.user.email,
        displayName: userCredential.user.displayName || undefined,
      },
    };
  } catch (error) {
    const authError = error as AuthError;
    return {
      success: false,
      error: {
        code: authError.code || ErrorCode.AUTH_FAILED,
        message: getAuthErrorMessage(authError.code),
      },
    };
  }
};

/**
 * Sign out the current user
 * @returns Promise with success status
 */
export const logout = async (): Promise<{ success: boolean; error?: string }> => {
  try {
    const auth = getAuthInstance();
    await signOut(auth);
    return { success: true };
  } catch (error) {
    const authError = error as AuthError;
    return {
      success: false,
      error: getAuthErrorMessage(authError.code),
    };
  }
};

/**
 * Get the current authenticated user
 * @returns Current user or null if not authenticated
 */
export const getCurrentUser = (): User | null => {
  const auth = getAuthInstance();
  return auth.currentUser;
};

/**
 * Check if user is authenticated
 * @returns True if user is authenticated, false otherwise
 */
export const isAuthenticated = (): boolean => {
  return getCurrentUser() !== null;
};

/**
 * Subscribe to authentication state changes
 * @param callback - Function to call when auth state changes
 * @returns Unsubscribe function
 */
export const onAuthStateChange = (
  callback: (user: User | null) => void
): (() => void) => {
  const auth = getAuthInstance();
  return onAuthStateChanged(auth, callback);
};

/**
 * Get user ID of current authenticated user
 * @returns User ID or null if not authenticated
 */
export const getCurrentUserId = (): string | null => {
  const user = getCurrentUser();
  return user ? user.uid : null;
};

/**
 * Convert Firebase auth error codes to user-friendly messages
 * @param code - Firebase error code
 * @returns User-friendly error message
 */
const getAuthErrorMessage = (code?: string): string => {
  switch (code) {
    case 'auth/email-already-in-use':
      return 'This email is already registered. Please sign in instead.';
    case 'auth/invalid-email':
      return 'Invalid email address.';
    case 'auth/operation-not-allowed':
      return 'Email/password accounts are not enabled.';
    case 'auth/weak-password':
      return 'Password is too weak. Please choose a stronger password.';
    case 'auth/user-disabled':
      return 'This account has been disabled.';
    case 'auth/user-not-found':
      return 'No account found with this email address.';
    case 'auth/wrong-password':
      return 'Incorrect password.';
    case 'auth/too-many-requests':
      return 'Too many failed attempts. Please try again later.';
    case 'auth/network-request-failed':
      return 'Network error. Please check your connection.';
    case 'auth/invalid-credential':
      return 'Invalid email or password.';
    default:
      return code || 'An authentication error occurred. Please try again.';
  }
};

/**
 * Reset password (sends password reset email)
 * Note: This requires Firebase Auth to be configured with email action handlers
 */
export const sendPasswordResetEmail = async (email: string): Promise<AuthResult> => {
  try {
    const { sendPasswordResetEmail: firebaseSendPasswordReset } = await import('firebase/auth');
    const auth = getAuthInstance();

    if (!email) {
      return {
        success: false,
        error: {
          code: ErrorCode.INVALID_INPUT,
          message: 'Email is required',
        },
      };
    }

    await firebaseSendPasswordReset(auth, email);

    return {
      success: true,
    };
  } catch (error) {
    const authError = error as AuthError;
    return {
      success: false,
      error: {
        code: authError.code || ErrorCode.AUTH_FAILED,
        message: getAuthErrorMessage(authError.code),
      },
    };
  }
};
