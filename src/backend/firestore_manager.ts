/**
 * Firestore Database Manager
 * Handles all Firestore CRUD operations for Merge Wellness
 * Production-ready with error handling and validation
 */

import {
  doc,
  getDoc,
  setDoc,
  updateDoc,
  collection,
  query,
  where,
  getDocs,
  addDoc,
  serverTimestamp,
  Timestamp,
  onSnapshot,
  Unsubscribe,
  writeBatch,
  DocumentData,
  QuerySnapshot,
} from 'firebase/firestore';
import { getFirestoreInstance } from './firebase_config';
import {
  UserData,
  ItemDefinition,
  GameEvent,
  InventoryItem,
  COLLECTIONS,
  EventType,
  ErrorCode,
  SyncConflict,
} from './types';

/**
 * Create a new user document in Firestore
 * @param userId - Firebase Auth user ID
 * @param userData - Initial user data
 * @returns Success status
 */
export const createUserDocument = async (
  userId: string,
  userData: { email: string; username: string }
): Promise<{ success: boolean; error?: string }> => {
  try {
    const db = getFirestoreInstance();
    const userRef = doc(db, COLLECTIONS.USERS, userId);

    // Check if user already exists
    const userSnap = await getDoc(userRef);
    if (userSnap.exists()) {
      return {
        success: false,
        error: 'User document already exists',
      };
    }

    // Create initial user data
    const initialUserData: UserData = {
      username: userData.username,
      email: userData.email,
      level: 1,
      score: 0,
      inventory: [],
      lastLogin: serverTimestamp() as Timestamp,
      createdAt: serverTimestamp() as Timestamp,
      xp: 0,
      coins: 0,
      gems: 0,
      unlockedAreas: [1], // Start with area 1 unlocked
      achievements: [],
    };

    await setDoc(userRef, initialUserData);

    return { success: true };
  } catch (error) {
    console.error('Error creating user document:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Unknown error',
    };
  }
};

/**
 * Get user data from Firestore
 * @param userId - Firebase Auth user ID
 * @returns User data or null if not found
 */
export const getUserData = async (userId: string): Promise<UserData | null> => {
  try {
    const db = getFirestoreInstance();
    const userRef = doc(db, COLLECTIONS.USERS, userId);
    const userSnap = await getDoc(userRef);

    if (!userSnap.exists()) {
      return null;
    }

    return userSnap.data() as UserData;
  } catch (error) {
    console.error('Error getting user data:', error);
    throw error;
  }
};

/**
 * Update user data in Firestore
 * @param userId - Firebase Auth user ID
 * @param updates - Partial user data to update
 * @returns Success status
 */
export const updateUserData = async (
  userId: string,
  updates: Partial<UserData>
): Promise<{ success: boolean; error?: string }> => {
  try {
    const db = getFirestoreInstance();
    const userRef = doc(db, COLLECTIONS.USERS, userId);

    // Validate user exists
    const userSnap = await getDoc(userRef);
    if (!userSnap.exists()) {
      return {
        success: false,
        error: 'User not found',
      };
    }

    // Prepare update data (exclude undefined values)
    const updateData: Partial<UserData> = {};
    Object.keys(updates).forEach((key) => {
      const value = updates[key as keyof UserData];
      if (value !== undefined) {
        updateData[key as keyof UserData] = value;
      }
    });

    await updateDoc(userRef, updateData);

    return { success: true };
  } catch (error) {
    console.error('Error updating user data:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Unknown error',
    };
  }
};

/**
 * Update user's last login timestamp
 * @param userId - Firebase Auth user ID
 */
export const updateUserLastLogin = async (userId: string): Promise<void> => {
  try {
    const db = getFirestoreInstance();
    const userRef = doc(db, COLLECTIONS.USERS, userId);
    await updateDoc(userRef, {
      lastLogin: serverTimestamp(),
    });
  } catch (error) {
    console.error('Error updating last login:', error);
    // Don't throw - this is a non-critical operation
  }
};

/**
 * Add item to user inventory
 * @param userId - Firebase Auth user ID
 * @param item - Inventory item to add
 * @returns Success status
 */
export const addItemToInventory = async (
  userId: string,
  item: InventoryItem
): Promise<{ success: boolean; error?: string }> => {
  try {
    const userData = await getUserData(userId);
    if (!userData) {
      return {
        success: false,
        error: 'User not found',
      };
    }

    // Check if item already exists in inventory
    const existingItemIndex = userData.inventory.findIndex((invItem) => invItem.itemId === item.itemId);

    if (existingItemIndex >= 0) {
      // Update quantity
      const updatedInventory = [...userData.inventory];
      updatedInventory[existingItemIndex].quantity += item.quantity;
      await updateUserData(userId, { inventory: updatedInventory });
    } else {
      // Add new item
      const updatedInventory = [...userData.inventory, item];
      await updateUserData(userId, { inventory: updatedInventory });
    }

    return { success: true };
  } catch (error) {
    console.error('Error adding item to inventory:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Unknown error',
    };
  }
};

/**
 * Remove items from user inventory
 * @param userId - Firebase Auth user ID
 * @param itemIds - Array of item IDs to remove
 * @param quantities - Optional quantities to remove (default: remove all)
 * @returns Success status
 */
export const removeItemsFromInventory = async (
  userId: string,
  itemIds: string[],
  quantities?: number[]
): Promise<{ success: boolean; error?: string }> => {
  try {
    const userData = await getUserData(userId);
    if (!userData) {
      return {
        success: false,
        error: 'User not found',
      };
    }

    const updatedInventory = userData.inventory.filter((invItem, index) => {
      const itemIndex = itemIds.indexOf(invItem.itemId);
      if (itemIndex === -1) {
        return true; // Keep item
      }

      // If quantity specified, reduce quantity instead of removing
      if (quantities && quantities[itemIndex] !== undefined) {
        const quantityToRemove = quantities[itemIndex];
        if (invItem.quantity > quantityToRemove) {
          invItem.quantity -= quantityToRemove;
          return true; // Keep item with reduced quantity
        }
        // Remove item if quantity becomes 0
        return false;
      }

      // Remove item completely
      return false;
    });

    await updateUserData(userId, { inventory: updatedInventory });

    return { success: true };
  } catch (error) {
    console.error('Error removing items from inventory:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Unknown error',
    };
  }
};

/**
 * Get item definition from Firestore
 * @param itemId - Item ID
 * @returns Item definition or null if not found
 */
export const getItemDefinition = async (itemId: string): Promise<ItemDefinition | null> => {
  try {
    const db = getFirestoreInstance();
    const itemRef = doc(db, COLLECTIONS.ITEMS, itemId);
    const itemSnap = await getDoc(itemRef);

    if (!itemSnap.exists()) {
      return null;
    }

    return itemSnap.data() as ItemDefinition;
  } catch (error) {
    console.error('Error getting item definition:', error);
    throw error;
  }
};

/**
 * Get all item definitions
 * @returns Array of all item definitions
 */
export const getAllItemDefinitions = async (): Promise<ItemDefinition[]> => {
  try {
    const db = getFirestoreInstance();
    const itemsRef = collection(db, COLLECTIONS.ITEMS);
    const itemsSnap = await getDocs(itemsRef);

    return itemsSnap.docs.map((doc) => doc.data() as ItemDefinition);
  } catch (error) {
    console.error('Error getting all item definitions:', error);
    throw error;
  }
};

/**
 * Create a game event
 * @param userId - Firebase Auth user ID
 * @param eventType - Type of event
 * @param metadata - Optional event metadata
 * @returns Created event ID
 */
export const createGameEvent = async (
  userId: string,
  eventType: EventType,
  metadata?: Record<string, any>
): Promise<{ success: boolean; eventId?: string; error?: string }> => {
  try {
    const db = getFirestoreInstance();
    const eventsRef = collection(db, COLLECTIONS.GAME_EVENTS);

    const event: Omit<GameEvent, 'eventId'> = {
      userId,
      eventType,
      timestamp: serverTimestamp() as Timestamp,
      metadata: metadata || {},
    };

    const docRef = await addDoc(eventsRef, event);

    return {
      success: true,
      eventId: docRef.id,
    };
  } catch (error) {
    console.error('Error creating game event:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Unknown error',
    };
  }
};

/**
 * Get game events for a user
 * @param userId - Firebase Auth user ID
 * @param limit - Maximum number of events to retrieve
 * @returns Array of game events
 */
export const getUserGameEvents = async (
  userId: string,
  limit: number = 50
): Promise<GameEvent[]> => {
  try {
    const db = getFirestoreInstance();
    const eventsRef = collection(db, COLLECTIONS.GAME_EVENTS);
    const q = query(eventsRef, where('userId', '==', userId));
    const eventsSnap = await getDocs(q);

    const events: GameEvent[] = [];
    eventsSnap.docs.forEach((doc) => {
      events.push({
        eventId: doc.id,
        ...doc.data(),
      } as GameEvent);
    });

    // Sort by timestamp (newest first) and limit
    return events
      .sort((a, b) => b.timestamp.toMillis() - a.timestamp.toMillis())
      .slice(0, limit);
  } catch (error) {
    console.error('Error getting user game events:', error);
    throw error;
  }
};

/**
 * Batch update user data (for atomic operations)
 * @param userId - Firebase Auth user ID
 * @param updates - Updates to apply
 * @returns Success status
 */
export const batchUpdateUserData = async (
  userId: string,
  updates: Partial<UserData>
): Promise<{ success: boolean; error?: string }> => {
  try {
    const db = getFirestoreInstance();
    const batch = writeBatch(db);
    const userRef = doc(db, COLLECTIONS.USERS, userId);

    // Validate user exists
    const userSnap = await getDoc(userRef);
    if (!userSnap.exists()) {
      return {
        success: false,
        error: 'User not found',
      };
    }

    // Prepare update data
    const updateData: Partial<UserData> = {};
    Object.keys(updates).forEach((key) => {
      const value = updates[key as keyof UserData];
      if (value !== undefined) {
        updateData[key as keyof UserData] = value;
      }
    });

    batch.update(userRef, updateData);
    await batch.commit();

    return { success: true };
  } catch (error) {
    console.error('Error in batch update:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Unknown error',
    };
  }
};

/**
 * Subscribe to real-time user data updates
 * @param userId - Firebase Auth user ID
 * @param callback - Function to call when data changes
 * @returns Unsubscribe function
 */
export const subscribeToUserData = (
  userId: string,
  callback: (userData: UserData | null) => void
): Unsubscribe => {
  const db = getFirestoreInstance();
  const userRef = doc(db, COLLECTIONS.USERS, userId);

  return onSnapshot(
    userRef,
    (snapshot) => {
      if (snapshot.exists()) {
        callback(snapshot.data() as UserData);
      } else {
        callback(null);
      }
    },
    (error) => {
      console.error('Error in user data subscription:', error);
      callback(null);
    }
  );
};

/**
 * Subscribe to real-time inventory changes
 * @param userId - Firebase Auth user ID
 * @param callback - Function to call when inventory changes
 * @returns Unsubscribe function
 */
export const subscribeToInventory = (
  userId: string,
  callback: (inventory: InventoryItem[]) => void
): Unsubscribe => {
  return subscribeToUserData(userId, (userData) => {
    if (userData) {
      callback(userData.inventory);
    } else {
      callback([]);
    }
  });
};
