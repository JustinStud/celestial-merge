/**
 * Main Firebase Manager - Central Export Point
 * This file exports all Firebase-related functionality for Merge Wellness
 * 
 * Usage:
 *   import { signUp, signIn, mergeItems, subscribeToInventory } from './backend/firebase_manager';
 */

// Authentication
export {
  signUp,
  signIn,
  logout,
  getCurrentUser,
  isAuthenticated,
  onAuthStateChange,
  getCurrentUserId,
  sendPasswordResetEmail,
} from './auth_manager';

// Firestore Operations
export {
  createUserDocument,
  getUserData,
  updateUserData,
  updateUserLastLogin,
  addItemToInventory,
  removeItemsFromInventory,
  getItemDefinition,
  getAllItemDefinitions,
  createGameEvent,
  getUserGameEvents,
  batchUpdateUserData,
  subscribeToUserData,
  subscribeToInventory,
} from './firestore_manager';

// Real-time Sync
export {
  RealtimeSyncManager,
  getSyncManager,
  initializeSync,
  ConflictResolutionStrategy,
} from './realtime_sync';

// Cloud Functions
export {
  mergeItems,
  handleLevelUp,
  processPurchase,
} from './cloud_functions';

// Configuration
export {
  initializeFirebase,
  getFirestoreInstance,
  getAuthInstance,
  getFunctionsInstance,
  getFirebaseConfig,
  resetFirebaseInstances,
} from './firebase_config';

// Types
export type {
  UserData,
  InventoryItem,
  ItemDefinition,
  GameEvent,
  AuthResult,
  MergeResult,
  LevelUpResult,
  PurchaseResult,
  SyncConflict,
} from './types';

export {
  COLLECTIONS,
  EventType,
  ErrorCode,
} from './types';
