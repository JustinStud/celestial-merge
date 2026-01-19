/**
 * Cloud Functions Client Interface
 * TypeScript client for calling Firebase Cloud Functions
 * Production-ready with error handling
 */

import { httpsCallable, HttpsCallableResult } from 'firebase/functions';
import { getFunctionsInstance } from './firebase_config';
import { MergeResult, LevelUpResult, PurchaseResult, ErrorCode } from './types';
import { getCurrentUserId } from './auth_manager';

/**
 * Call a cloud function with error handling
 */
async function callCloudFunction<T = any, R = any>(
  functionName: string,
  data?: T
): Promise<{ success: boolean; data?: R; error?: string }> {
  try {
    const functions = getFunctionsInstance();
    const callFunction = httpsCallable<T, R>(functions, functionName);
    const result: HttpsCallableResult<R> = await callFunction(data);

    return {
      success: true,
      data: result.data,
    };
  } catch (error: any) {
    console.error(`Error calling cloud function ${functionName}:`, error);
    return {
      success: false,
      error: error.message || 'Unknown error',
    };
  }
}

/**
 * Merge items using cloud function
 * @param itemId - ID of the item to merge
 * @param quantity - Number of items to merge (must be >= 2)
 * @returns Merge result
 */
export const mergeItems = async (
  itemId: string,
  quantity: number = 2
): Promise<MergeResult> => {
  const userId = getCurrentUserId();
  if (!userId) {
    return {
      success: false,
      error: 'User not authenticated',
    };
  }

  if (quantity < 2) {
    return {
      success: false,
      error: 'At least 2 items are required to merge',
    };
  }

  const result = await callCloudFunction<{ itemId: string; quantity: number }, MergeResult>(
    'onUserMergeItems',
    { itemId, quantity }
  );

  if (!result.success) {
    return {
      success: false,
      error: result.error || 'Failed to merge items',
    };
  }

  return result.data || { success: false, error: 'No data returned' };
};

/**
 * Handle level up using cloud function
 * @param newLevel - The new level the user reached
 * @returns Level up result with rewards
 */
export const handleLevelUp = async (newLevel: number): Promise<LevelUpResult> => {
  const userId = getCurrentUserId();
  if (!userId) {
    return {
      success: false,
      error: 'User not authenticated',
    };
  }

  const result = await callCloudFunction<{ userId: string; newLevel: number }, LevelUpResult>(
    'onUserLevelUp',
    { userId, newLevel }
  );

  if (!result.success) {
    return {
      success: false,
      error: result.error || 'Failed to process level up',
    };
  }

  return result.data || { success: false, error: 'No data returned' };
};

/**
 * Process in-app purchase using cloud function
 * @param purchaseType - Type of purchase (e.g., 'coins', 'gems', 'cosmetic')
 * @param purchaseId - Purchase product ID
 * @param transactionId - Transaction ID from app store
 * @param amount - Purchase amount
 * @returns Purchase result
 */
export const processPurchase = async (
  purchaseType: string,
  purchaseId: string,
  transactionId: string,
  amount: number
): Promise<PurchaseResult> => {
  const userId = getCurrentUserId();
  if (!userId) {
    return {
      success: false,
      error: 'User not authenticated',
    };
  }

  const result = await callCloudFunction<
    {
      userId: string;
      purchaseType: string;
      purchaseId: string;
      transactionId: string;
      amount: number;
    },
    PurchaseResult
  >('onItemPurchase', {
    userId,
    purchaseType,
    purchaseId,
    transactionId,
    amount,
  });

  if (!result.success) {
    return {
      success: false,
      error: result.error || 'Failed to process purchase',
    };
  }

  return result.data || { success: false, error: 'No data returned' };
};
