/**
 * Real-time Sync System with Conflict Resolution
 * Handles offline mode, conflict resolution, and real-time updates
 * Production-ready with robust error handling
 */

import { Timestamp } from 'firebase/firestore';
import {
  subscribeToUserData,
  getUserData,
  updateUserData,
  batchUpdateUserData,
} from './firestore_manager';
import { UserData, InventoryItem, SyncConflict, ErrorCode } from './types';
import { getCurrentUserId } from './auth_manager';

/**
 * Local storage key for offline data
 */
const OFFLINE_DATA_KEY = 'merge_wellness_offline_data';
const LAST_SYNC_KEY = 'merge_wellness_last_sync';

/**
 * Conflict resolution strategy
 */
export enum ConflictResolutionStrategy {
  SERVER_WINS = 'server_wins',
  CLIENT_WINS = 'client_wins',
  MERGE = 'merge',
  TIMESTAMP = 'timestamp',
}

/**
 * Real-time sync manager
 */
export class RealtimeSyncManager {
  private userId: string | null = null;
  private unsubscribe: (() => void) | null = null;
  private localData: UserData | null = null;
  private serverData: UserData | null = null;
  private syncListeners: Array<(data: UserData) => void> = [];
  private conflictListeners: Array<(conflict: SyncConflict) => void> = [];
  private strategy: ConflictResolutionStrategy = ConflictResolutionStrategy.TIMESTAMP;

  /**
   * Initialize real-time sync
   * @param userId - User ID to sync
   * @param strategy - Conflict resolution strategy
   */
  async initialize(
    userId: string,
    strategy: ConflictResolutionStrategy = ConflictResolutionStrategy.TIMESTAMP
  ): Promise<{ success: boolean; error?: string }> {
    try {
      this.userId = userId;
      this.strategy = strategy;

      // Load offline data if available
      await this.loadOfflineData();

      // Subscribe to real-time updates
      this.unsubscribe = subscribeToUserData(userId, (data) => {
        this.handleServerUpdate(data);
      });

      // Sync with server
      await this.syncWithServer();

      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: error instanceof Error ? error.message : 'Unknown error',
      };
    }
  }

  /**
   * Stop real-time sync
   */
  stop(): void {
    if (this.unsubscribe) {
      this.unsubscribe();
      this.unsubscribe = null;
    }
    this.userId = null;
    this.localData = null;
    this.serverData = null;
  }

  /**
   * Subscribe to data updates
   * @param callback - Function to call when data updates
   * @returns Unsubscribe function
   */
  onDataUpdate(callback: (data: UserData) => void): () => void {
    this.syncListeners.push(callback);
    return () => {
      this.syncListeners = this.syncListeners.filter((listener) => listener !== callback);
    };
  }

  /**
   * Subscribe to conflict events
   * @param callback - Function to call when conflicts occur
   * @returns Unsubscribe function
   */
  onConflict(callback: (conflict: SyncConflict) => void): () => void {
    this.conflictListeners.push(callback);
    return () => {
      this.conflictListeners = this.conflictListeners.filter((listener) => listener !== callback);
    };
  }

  /**
   * Update local data (will sync to server)
   * @param updates - Partial user data to update
   * @returns Success status
   */
  async updateLocalData(updates: Partial<UserData>): Promise<{ success: boolean; error?: string }> {
    if (!this.userId) {
      return {
        success: false,
        error: 'Sync manager not initialized',
      };
    }

    try {
      // Update local data
      if (this.localData) {
        this.localData = { ...this.localData, ...updates };
      } else {
        // If no local data, fetch from server first
        const serverData = await getUserData(this.userId);
        if (serverData) {
          this.localData = { ...serverData, ...updates };
        } else {
          return {
            success: false,
            error: 'User data not found',
          };
        }
      }

      // Save to offline storage
      await this.saveOfflineData();

      // Try to sync with server
      const syncResult = await this.syncWithServer();
      if (!syncResult.success && syncResult.isOffline) {
        // If offline, data is saved locally and will sync when online
        return { success: true };
      }

      return syncResult;
    } catch (error) {
      return {
        success: false,
        error: error instanceof Error ? error.message : 'Unknown error',
      };
    }
  }

  /**
   * Get current local data
   */
  getLocalData(): UserData | null {
    return this.localData;
  }

  /**
   * Get current server data
   */
  getServerData(): UserData | null {
    return this.serverData;
  }

  /**
   * Handle server update
   * @param data - Updated data from server
   */
  private handleServerUpdate(data: UserData | null): void {
    if (!data) {
      return;
    }

    const previousServerData = this.serverData;
    this.serverData = data;

    // Check for conflicts
    if (this.localData && previousServerData) {
      const hasConflict = this.detectConflict(this.localData, data);
      if (hasConflict) {
        this.handleConflict(this.localData, data);
        return;
      }
    }

    // Update local data from server
    this.localData = data;
    this.saveOfflineData();

    // Notify listeners
    this.notifyListeners(data);
  }

  /**
   * Sync local data with server
   */
  private async syncWithServer(): Promise<{ success: boolean; isOffline?: boolean; error?: string }> {
    if (!this.userId) {
      return {
        success: false,
        error: 'Sync manager not initialized',
      };
    }

    try {
      // Check if we have local changes
      if (!this.localData) {
        // No local data, just fetch from server
        const serverData = await getUserData(this.userId);
        if (serverData) {
          this.localData = serverData;
          this.serverData = serverData;
          this.saveOfflineData();
        }
        return { success: true };
      }

      // Get server data
      const serverData = await getUserData(this.userId);
      if (!serverData) {
        return {
          success: false,
          error: 'User data not found on server',
        };
      }

      this.serverData = serverData;

      // Check for conflicts
      const hasConflict = this.detectConflict(this.localData, serverData);
      if (hasConflict) {
        const resolved = await this.resolveConflict(this.localData, serverData);
        if (resolved) {
          this.localData = resolved;
          await updateUserData(this.userId, resolved);
        } else {
          // Conflict not resolved, notify listeners
          this.handleConflict(this.localData, serverData);
          return { success: false, error: 'Conflict detected' };
        }
      } else {
        // No conflict, update server with local changes
        await updateUserData(this.userId, this.localData);
        this.localData = serverData; // Use server data as source of truth
      }

      this.saveOfflineData();
      this.notifyListeners(this.localData);

      return { success: true };
    } catch (error) {
      // Network error - we're offline
      if (error instanceof Error && error.message.includes('network')) {
        return {
          success: true,
          isOffline: true,
        };
      }

      return {
        success: false,
        error: error instanceof Error ? error.message : 'Unknown error',
      };
    }
  }

  /**
   * Detect conflicts between local and server data
   */
  private detectConflict(local: UserData, server: UserData): boolean {
    // Check if timestamps differ significantly (more than 1 second)
    const localTime = local.lastLogin?.toMillis() || 0;
    const serverTime = server.lastLogin?.toMillis() || 0;
    const timeDiff = Math.abs(localTime - serverTime);

    // If timestamps are very different, there might be a conflict
    if (timeDiff > 1000) {
      // Check if critical data differs
      if (
        local.score !== server.score ||
        local.level !== server.level ||
        JSON.stringify(local.inventory) !== JSON.stringify(server.inventory)
      ) {
        return true;
      }
    }

    return false;
  }

  /**
   * Handle conflict
   */
  private handleConflict(local: UserData, server: UserData): void {
    const conflict: SyncConflict = {
      localData: local,
      serverData: server,
      timestamp: Timestamp.now(),
    };

    this.conflictListeners.forEach((listener) => listener(conflict));
  }

  /**
   * Resolve conflict based on strategy
   */
  private async resolveConflict(
    local: UserData,
    server: UserData
  ): Promise<UserData | null> {
    switch (this.strategy) {
      case ConflictResolutionStrategy.SERVER_WINS:
        return server;

      case ConflictResolutionStrategy.CLIENT_WINS:
        return local;

      case ConflictResolutionStrategy.TIMESTAMP:
        const localTime = local.lastLogin?.toMillis() || 0;
        const serverTime = server.lastLogin?.toMillis() || 0;
        return localTime > serverTime ? local : server;

      case ConflictResolutionStrategy.MERGE:
        return this.mergeData(local, server);

      default:
        return null;
    }
  }

  /**
   * Merge local and server data intelligently
   */
  private mergeData(local: UserData, server: UserData): UserData {
    // Merge inventory items
    const mergedInventory = this.mergeInventory(local.inventory, server.inventory);

    // Use highest values for score, level, xp
    return {
      ...server, // Use server as base
      score: Math.max(local.score, server.score),
      level: Math.max(local.level, server.level),
      xp: Math.max(local.xp, server.xp),
      coins: Math.max(local.coins, server.coins),
      gems: Math.max(local.gems, server.gems),
      inventory: mergedInventory,
      unlockedAreas: [
        ...new Set([...local.unlockedAreas, ...server.unlockedAreas]),
      ].sort(),
      achievements: [...new Set([...local.achievements, ...server.achievements])],
    };
  }

  /**
   * Merge inventory arrays
   */
  private mergeInventory(local: InventoryItem[], server: InventoryItem[]): InventoryItem[] {
    const merged: Map<string, InventoryItem> = new Map();

    // Add server items
    server.forEach((item) => {
      merged.set(item.itemId, { ...item });
    });

    // Merge local items
    local.forEach((localItem) => {
      const existing = merged.get(localItem.itemId);
      if (existing) {
        existing.quantity = Math.max(existing.quantity, localItem.quantity);
        // Use earliest discovery time
        if (localItem.discoveredAt < existing.discoveredAt) {
          existing.discoveredAt = localItem.discoveredAt;
        }
      } else {
        merged.set(localItem.itemId, { ...localItem });
      }
    });

    return Array.from(merged.values());
  }

  /**
   * Save data to offline storage
   */
  private async saveOfflineData(): Promise<void> {
    if (!this.localData) {
      return;
    }

    try {
      const data = {
        userData: this.localData,
        timestamp: Date.now(),
      };
      localStorage.setItem(OFFLINE_DATA_KEY, JSON.stringify(data));
      localStorage.setItem(LAST_SYNC_KEY, Date.now().toString());
    } catch (error) {
      console.warn('Failed to save offline data:', error);
    }
  }

  /**
   * Load data from offline storage
   */
  private async loadOfflineData(): Promise<void> {
    try {
      const stored = localStorage.getItem(OFFLINE_DATA_KEY);
      if (stored) {
        const data = JSON.parse(stored);
        // Convert timestamps back to Timestamp objects if needed
        this.localData = data.userData;
      }
    } catch (error) {
      console.warn('Failed to load offline data:', error);
    }
  }

  /**
   * Notify all listeners of data update
   */
  private notifyListeners(data: UserData): void {
    this.syncListeners.forEach((listener) => {
      try {
        listener(data);
      } catch (error) {
        console.error('Error in sync listener:', error);
      }
    });
  }
}

/**
 * Global sync manager instance
 */
let syncManager: RealtimeSyncManager | null = null;

/**
 * Get or create sync manager instance
 */
export const getSyncManager = (): RealtimeSyncManager => {
  if (!syncManager) {
    syncManager = new RealtimeSyncManager();
  }
  return syncManager;
};

/**
 * Initialize sync for current user
 */
export const initializeSync = async (
  strategy: ConflictResolutionStrategy = ConflictResolutionStrategy.TIMESTAMP
): Promise<{ success: boolean; error?: string }> => {
  const userId = getCurrentUserId();
  if (!userId) {
    return {
      success: false,
      error: 'User not authenticated',
    };
  }

  const manager = getSyncManager();
  return await manager.initialize(userId, strategy);
};
