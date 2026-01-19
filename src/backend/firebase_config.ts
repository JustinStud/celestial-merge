/**
 * Firebase Configuration and Initialization
 * Production-ready Firebase setup
 */

import { initializeApp, FirebaseApp } from 'firebase/app';
import { getAuth, Auth } from 'firebase/auth';
import { getFirestore, Firestore, connectFirestoreEmulator } from 'firebase/firestore';
import { getFunctions, Functions, connectFunctionsEmulator } from 'firebase/functions';

/**
 * Firebase configuration interface
 * Replace these values with your actual Firebase project config
 */
export interface FirebaseConfig {
  apiKey: string;
  authDomain: string;
  projectId: string;
  storageBucket: string;
  messagingSenderId: string;
  appId: string;
  measurementId?: string;
}

/**
 * Get Firebase configuration from environment variables
 * In production, these should be set via environment variables
 */
export const getFirebaseConfig = (): FirebaseConfig => {
  const config: FirebaseConfig = {
    apiKey: process.env.REACT_APP_FIREBASE_API_KEY || '',
    authDomain: process.env.REACT_APP_FIREBASE_AUTH_DOMAIN || '',
    projectId: process.env.REACT_APP_FIREBASE_PROJECT_ID || '',
    storageBucket: process.env.REACT_APP_FIREBASE_STORAGE_BUCKET || '',
    messagingSenderId: process.env.REACT_APP_FIREBASE_MESSAGING_SENDER_ID || '',
    appId: process.env.REACT_APP_FIREBASE_APP_ID || '',
  };

  // Validate configuration
  if (!config.apiKey || !config.projectId) {
    throw new Error('Firebase configuration is incomplete. Please check your environment variables.');
  }

  return config;
};

/**
 * Firebase app instance (singleton)
 */
let firebaseApp: FirebaseApp | null = null;
let firestoreInstance: Firestore | null = null;
let authInstance: Auth | null = null;
let functionsInstance: Functions | null = null;

/**
 * Initialize Firebase app
 * @param useEmulator - Whether to use Firebase emulators (for development)
 */
export const initializeFirebase = (useEmulator: boolean = false): FirebaseApp => {
  if (firebaseApp) {
    return firebaseApp;
  }

  const config = getFirebaseConfig();
  firebaseApp = initializeApp(config);

  // Initialize Firestore
  firestoreInstance = getFirestore(firebaseApp);
  if (useEmulator && process.env.NODE_ENV === 'development') {
    try {
      connectFirestoreEmulator(firestoreInstance, 'localhost', 8080);
    } catch (error) {
      console.warn('Firestore emulator already connected or not available');
    }
  }

  // Initialize Auth
  authInstance = getAuth(firebaseApp);

  // Initialize Functions
  functionsInstance = getFunctions(firebaseApp);
  if (useEmulator && process.env.NODE_ENV === 'development') {
    try {
      connectFunctionsEmulator(functionsInstance, 'localhost', 5001);
    } catch (error) {
      console.warn('Functions emulator already connected or not available');
    }
  }

  return firebaseApp;
};

/**
 * Get Firestore instance
 */
export const getFirestoreInstance = (): Firestore => {
  if (!firestoreInstance) {
    initializeFirebase();
  }
  return firestoreInstance!;
};

/**
 * Get Auth instance
 */
export const getAuthInstance = (): Auth => {
  if (!authInstance) {
    initializeFirebase();
  }
  return authInstance!;
};

/**
 * Get Functions instance
 */
export const getFunctionsInstance = (): Functions => {
  if (!functionsInstance) {
    initializeFirebase();
  }
  return functionsInstance!;
};

/**
 * Reset Firebase instances (useful for testing)
 */
export const resetFirebaseInstances = (): void => {
  firebaseApp = null;
  firestoreInstance = null;
  authInstance = null;
  functionsInstance = null;
};
