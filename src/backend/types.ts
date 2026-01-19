/**
 * TypeScript Types and Interfaces for Merge Wellness Firebase Backend
 * Production-ready type definitions
 */

import { Timestamp } from 'firebase/firestore';

/**
 * User data structure stored in Firestore
 */
export interface UserData {
  username: string;
  email: string;
  level: number;
  score: number;
  inventory: InventoryItem[];
  lastLogin: Timestamp;
  createdAt: Timestamp;
  xp: number;
  coins: number;
  gems: number;
  unlockedAreas: number[];
  achievements: string[];
}

/**
 * Inventory item structure
 */
export interface InventoryItem {
  itemId: string;
  itemName: string;
  tier: number;
  quantity: number;
  discoveredAt: Timestamp;
}

/**
 * Item definition structure (master data)
 */
export interface ItemDefinition {
  itemId: string;
  name: string;
  type: 'wellness_item';
  tier: number; // 1-5
  description: string;
  mergeResult?: string; // itemId of the result when merged
  mergeRequirement?: number; // number of items needed (default: 2)
  xpReward: number;
  coinReward: number;
  area: number; // which area this item belongs to
}

/**
 * Game event structure
 */
export interface GameEvent {
  eventId: string;
  userId: string;
  eventType: 'merge' | 'levelup' | 'purchase' | 'discover' | 'challenge_complete';
  timestamp: Timestamp;
  metadata?: {
    itemId?: string;
    itemName?: string;
    tier?: number;
    newLevel?: number;
    purchaseType?: string;
    amount?: number;
    challengeId?: string;
    [key: string]: any;
  };
}

/**
 * Authentication result
 */
export interface AuthResult {
  success: boolean;
  user?: {
    uid: string;
    email: string | null;
    displayName?: string | null;
  };
  error?: {
    code: string;
    message: string;
  };
}

/**
 * Merge operation result
 */
export interface MergeResult {
  success: boolean;
  mergedItem?: InventoryItem;
  removedItems?: string[]; // itemIds that were removed
  xpGained?: number;
  coinsGained?: number;
  error?: string;
}

/**
 * Level up result
 */
export interface LevelUpResult {
  success: boolean;
  newLevel?: number;
  rewards?: {
    coins?: number;
    gems?: number;
    items?: InventoryItem[];
  };
  error?: string;
}

/**
 * Purchase result
 */
export interface PurchaseResult {
  success: boolean;
  transactionId?: string;
  itemsGranted?: InventoryItem[];
  currencyGranted?: {
    coins?: number;
    gems?: number;
  };
  error?: string;
}

/**
 * Real-time sync conflict resolution data
 */
export interface SyncConflict {
  localData: Partial<UserData>;
  serverData: Partial<UserData>;
  timestamp: Timestamp;
}

/**
 * Firestore collection paths
 */
export const COLLECTIONS = {
  USERS: 'users',
  ITEMS: 'items',
  GAME_EVENTS: 'gameEvents',
} as const;

/**
 * Event types enum
 */
export enum EventType {
  MERGE = 'merge',
  LEVELUP = 'levelup',
  PURCHASE = 'purchase',
  DISCOVER = 'discover',
  CHALLENGE_COMPLETE = 'challenge_complete',
}

/**
 * Error codes enum
 */
export enum ErrorCode {
  AUTH_FAILED = 'auth/failed',
  INVALID_INPUT = 'invalid/input',
  INSUFFICIENT_ITEMS = 'merge/insufficient_items',
  ITEM_NOT_FOUND = 'item/not_found',
  NETWORK_ERROR = 'network/error',
  PERMISSION_DENIED = 'permission/denied',
  UNKNOWN_ERROR = 'unknown/error',
}
