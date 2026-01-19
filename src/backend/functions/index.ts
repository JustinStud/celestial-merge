/**
 * Firebase Cloud Functions
 * Server-side functions for Merge Wellness game logic
 * Deploy with: firebase deploy --only functions
 */

import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

// Initialize Firebase Admin if not already initialized
if (!admin.apps.length) {
  admin.initializeApp();
}

const db = admin.firestore();

/**
 * Cloud Function: Handle item merge logic
 * Validates merge requirements and updates user inventory
 */
export const onUserMergeItems = functions.https.onCall(async (data, context) => {
  // Verify authentication
  if (!context.auth) {
    throw new functions.https.HttpsError('unauthenticated', 'User must be authenticated');
  }

  const userId = context.auth.uid;
  const { itemId, quantity } = data;

  // Validate input
  if (!itemId || !quantity || quantity < 2) {
    throw new functions.https.HttpsError(
      'invalid-argument',
      'itemId and quantity (>= 2) are required'
    );
  }

  try {
    // Get user data
    const userRef = db.collection('users').doc(userId);
    const userDoc = await userRef.get();

    if (!userDoc.exists) {
      throw new functions.https.HttpsError('not-found', 'User not found');
    }

    const userData = userDoc.data()!;
    const inventory: Array<{ itemId: string; quantity: number }> = userData.inventory || [];

    // Find item in inventory
    const itemIndex = inventory.findIndex((item) => item.itemId === itemId);
    if (itemIndex === -1) {
      throw new functions.https.HttpsError('not-found', 'Item not found in inventory');
    }

    const item = inventory[itemIndex];
    if (item.quantity < quantity) {
      throw new functions.https.HttpsError(
        'failed-precondition',
        'Insufficient items in inventory'
      );
    }

    // Get item definition
    const itemDoc = await db.collection('items').doc(itemId).get();
    if (!itemDoc.exists) {
      throw new functions.https.HttpsError('not-found', 'Item definition not found');
    }

    const itemDef = itemDoc.data()!;
    const tier = itemDef.tier || 1;
    const mergeRequirement = itemDef.mergeRequirement || 2;

    // Validate merge requirement
    if (quantity < mergeRequirement) {
      throw new functions.https.HttpsError(
        'invalid-argument',
        `At least ${mergeRequirement} items are required to merge`
      );
    }

    // Calculate how many merges can be performed
    const mergeCount = Math.floor(quantity / mergeRequirement);
    const remainingItems = quantity % mergeRequirement;

    // Get merge result item
    if (!itemDef.mergeResult) {
      throw new functions.https.HttpsError(
        'failed-precondition',
        'This item cannot be merged further'
      );
    }

    const resultItemId = itemDef.mergeResult;
    const resultItemDoc = await db.collection('items').doc(resultItemId).get();
    if (!resultItemDoc.exists) {
      throw new functions.https.HttpsError('not-found', 'Merge result item not found');
    }

    const resultItemDef = resultItemDoc.data()!;

    // Update inventory
    const updatedInventory = [...inventory];

    // Remove merged items
    if (remainingItems === 0) {
      // Remove item completely
      updatedInventory.splice(itemIndex, 1);
    } else {
      // Update quantity
      updatedInventory[itemIndex].quantity = remainingItems;
    }

    // Add result items
    const resultItemIndex = updatedInventory.findIndex((item) => item.itemId === resultItemId);
    if (resultItemIndex === -1) {
      // Add new item
      updatedInventory.push({
        itemId: resultItemId,
        quantity: mergeCount,
        itemName: resultItemDef.name,
        tier: resultItemDef.tier,
        discoveredAt: admin.firestore.FieldValue.serverTimestamp(),
      });
    } else {
      // Update existing item
      updatedInventory[resultItemIndex].quantity += mergeCount;
    }

    // Calculate rewards
    const xpGained = (resultItemDef.xpReward || 0) * mergeCount;
    const coinsGained = (resultItemDef.coinReward || 0) * mergeCount;

    // Update user data
    const batch = db.batch();
    batch.update(userRef, {
      inventory: updatedInventory,
      xp: admin.firestore.FieldValue.increment(xpGained),
      coins: admin.firestore.FieldValue.increment(coinsGained),
      score: admin.firestore.FieldValue.increment(coinsGained), // Score increases with coins
    });

    // Create game event
    const eventRef = db.collection('gameEvents').doc();
    batch.set(eventRef, {
      userId,
      eventType: 'merge',
      timestamp: admin.firestore.FieldValue.serverTimestamp(),
      metadata: {
        itemId,
        itemName: itemDef.name,
        tier,
        quantityMerged: quantity,
        resultItemId,
        resultItemName: resultItemDef.name,
        xpGained,
        coinsGained,
      },
    });

    await batch.commit();

    return {
      success: true,
      mergedItem: {
        itemId: resultItemId,
        itemName: resultItemDef.name,
        tier: resultItemDef.tier,
        quantity: mergeCount,
        discoveredAt: admin.firestore.Timestamp.now(),
      },
      removedItems: [itemId],
      xpGained,
      coinsGained,
    };
  } catch (error: any) {
    console.error('Error in onUserMergeItems:', error);
    if (error instanceof functions.https.HttpsError) {
      throw error;
    }
    throw new functions.https.HttpsError('internal', 'Failed to merge items');
  }
});

/**
 * Cloud Function: Handle user level up
 * Awards rewards when user levels up
 */
export const onUserLevelUp = functions.https.onCall(async (data, context) => {
  // Verify authentication
  if (!context.auth) {
    throw new functions.https.HttpsError('unauthenticated', 'User must be authenticated');
  }

  const userId = context.auth.uid;
  const { newLevel } = data;

  // Validate input
  if (!newLevel || typeof newLevel !== 'number' || newLevel < 1) {
    throw new functions.https.HttpsError('invalid-argument', 'Valid newLevel is required');
  }

  try {
    const userRef = db.collection('users').doc(userId);
    const userDoc = await userRef.get();

    if (!userDoc.exists) {
      throw new functions.https.HttpsError('not-found', 'User not found');
    }

    const userData = userDoc.data()!;
    const currentLevel = userData.level || 1;

    // Validate level progression
    if (newLevel <= currentLevel) {
      throw new functions.https.HttpsError(
        'failed-precondition',
        'New level must be greater than current level'
      );
    }

    // Calculate rewards based on level
    const levelRewards = calculateLevelRewards(newLevel);
    const coinsReward = levelRewards.coins;
    const gemsReward = levelRewards.gems;

    // Update user data
    const batch = db.batch();
    batch.update(userRef, {
      level: newLevel,
      coins: admin.firestore.FieldValue.increment(coinsReward),
      gems: admin.firestore.FieldValue.increment(gemsReward),
      score: admin.firestore.FieldValue.increment(coinsReward),
    });

    // Create game event
    const eventRef = db.collection('gameEvents').doc();
    batch.set(eventRef, {
      userId,
      eventType: 'levelup',
      timestamp: admin.firestore.FieldValue.serverTimestamp(),
      metadata: {
        newLevel,
        previousLevel: currentLevel,
        coinsReward,
        gemsReward,
      },
    });

    await batch.commit();

    return {
      success: true,
      newLevel,
      rewards: {
        coins: coinsReward,
        gems: gemsReward,
      },
    };
  } catch (error: any) {
    console.error('Error in onUserLevelUp:', error);
    if (error instanceof functions.https.HttpsError) {
      throw error;
    }
    throw new functions.https.HttpsError('internal', 'Failed to process level up');
  }
});

/**
 * Cloud Function: Process in-app purchase
 * Validates and processes IAP transactions
 */
export const onItemPurchase = functions.https.onCall(async (data, context) => {
  // Verify authentication
  if (!context.auth) {
    throw new functions.https.HttpsError('unauthenticated', 'User must be authenticated');
  }

  const userId = context.auth.uid;
  const { purchaseType, purchaseId, transactionId, amount } = data;

  // Validate input
  if (!purchaseType || !purchaseId || !transactionId || !amount) {
    throw new functions.https.HttpsError('invalid-argument', 'All purchase fields are required');
  }

  try {
    // Check if transaction already processed (prevent duplicate purchases)
    const existingTransaction = await db
      .collection('gameEvents')
      .where('userId', '==', userId)
      .where('eventType', '==', 'purchase')
      .where('metadata.transactionId', '==', transactionId)
      .limit(1)
      .get();

    if (!existingTransaction.empty) {
      throw new functions.https.HttpsError(
        'already-exists',
        'This transaction has already been processed'
      );
    }

    // Process purchase based on type
    const purchaseResult = await processPurchaseByType(
      userId,
      purchaseType,
      purchaseId,
      amount
    );

    // Create game event
    const eventRef = db.collection('gameEvents').doc();
    await eventRef.set({
      userId,
      eventType: 'purchase',
      timestamp: admin.firestore.FieldValue.serverTimestamp(),
      metadata: {
        purchaseType,
        purchaseId,
        transactionId,
        amount,
        itemsGranted: purchaseResult.itemsGranted,
        currencyGranted: purchaseResult.currencyGranted,
      },
    });

    return {
      success: true,
      transactionId,
      itemsGranted: purchaseResult.itemsGranted,
      currencyGranted: purchaseResult.currencyGranted,
    };
  } catch (error: any) {
    console.error('Error in onItemPurchase:', error);
    if (error instanceof functions.https.HttpsError) {
      throw error;
    }
    throw new functions.https.HttpsError('internal', 'Failed to process purchase');
  }
});

/**
 * Calculate level rewards
 */
function calculateLevelRewards(level: number): { coins: number; gems: number } {
  // Base rewards
  const baseCoins = 100;
  const baseGems = 5;

  // Scale rewards with level
  const coins = baseCoins * level;
  const gems = Math.floor(baseGems * (level / 10)) + (level % 10 === 0 ? 10 : 0);

  return { coins, gems };
}

/**
 * Process purchase by type
 */
async function processPurchaseByType(
  userId: string,
  purchaseType: string,
  purchaseId: string,
  amount: number
): Promise<{ itemsGranted?: any[]; currencyGranted?: { coins?: number; gems?: number } }> {
  const userRef = db.collection('users').doc(userId);
  const userDoc = await userRef.get();
  const userData = userDoc.data()!;

  switch (purchaseType) {
    case 'coins':
      const coinsAmount = getCoinsFromPurchase(purchaseId, amount);
      await userRef.update({
        coins: admin.firestore.FieldValue.increment(coinsAmount),
      });
      return {
        currencyGranted: { coins: coinsAmount },
      };

    case 'gems':
      const gemsAmount = getGemsFromPurchase(purchaseId, amount);
      await userRef.update({
        gems: admin.firestore.FieldValue.increment(gemsAmount),
      });
      return {
        currencyGranted: { gems: gemsAmount },
      };

    case 'cosmetic':
      // Cosmetic purchases don't grant items, just unlock them
      // This would be handled by updating user's unlocked cosmetics
      return {
        itemsGranted: [],
      };

    case 'speed_up':
      // Speed ups are temporary, handled client-side
      return {
        itemsGranted: [],
      };

    default:
      throw new functions.https.HttpsError('invalid-argument', 'Invalid purchase type');
  }
}

/**
 * Get coins amount from purchase
 */
function getCoinsFromPurchase(purchaseId: string, amount: number): number {
  // Map purchase IDs to coin amounts
  // In production, this should match your IAP product IDs
  const coinMultipliers: Record<string, number> = {
    coins_100: 100,
    coins_500: 500,
    coins_1000: 1000,
    coins_5000: 5000,
  };

  return coinMultipliers[purchaseId] || Math.floor(amount * 10); // Fallback: $1 = 10 coins
}

/**
 * Get gems amount from purchase
 */
function getGemsFromPurchase(purchaseId: string, amount: number): number {
  // Map purchase IDs to gem amounts
  const gemMultipliers: Record<string, number> = {
    gems_10: 10,
    gems_50: 50,
    gems_100: 100,
    gems_500: 500,
  };

  return gemMultipliers[purchaseId] || Math.floor(amount); // Fallback: $1 = 1 gem
}
