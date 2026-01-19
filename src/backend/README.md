# Merge Wellness - Firebase Backend System

Production-ready Firebase backend system for the Merge Wellness game.

## Features

- ✅ **Authentication Module** - Email/password sign up, login, logout, session management
- ✅ **Firestore Database** - User data, items, game events with proper structure
- ✅ **Cloud Functions** - Server-side merge logic, level up rewards, IAP processing
- ✅ **Real-time Sync** - Live updates with offline support and conflict resolution
- ✅ **Security Rules** - Comprehensive Firestore security rules

## Project Structure

```
src/backend/
├── types.ts                 # TypeScript types and interfaces
├── firebase_config.ts       # Firebase initialization and configuration
├── auth_manager.ts          # Authentication module
├── firestore_manager.ts     # Firestore CRUD operations
├── realtime_sync.ts         # Real-time sync with conflict resolution
├── cloud_functions.ts       # Cloud Functions client interface
├── firebase_manager.ts      # Main export file
├── firestore.rules          # Firestore security rules
├── functions/
│   └── index.ts            # Cloud Functions server code
├── package.json            # Dependencies
└── README.md               # This file
```

## Setup

### 1. Install Dependencies

```bash
npm install
```

### 2. Configure Firebase

Create a `.env` file in the project root with your Firebase configuration:

```env
REACT_APP_FIREBASE_API_KEY=your_api_key
REACT_APP_FIREBASE_AUTH_DOMAIN=your_project.firebaseapp.com
REACT_APP_FIREBASE_PROJECT_ID=your_project_id
REACT_APP_FIREBASE_STORAGE_BUCKET=your_project.appspot.com
REACT_APP_FIREBASE_MESSAGING_SENDER_ID=your_sender_id
REACT_APP_FIREBASE_APP_ID=your_app_id
```

### 3. Initialize Firebase

```typescript
import { initializeFirebase } from './backend/firebase_manager';

// Initialize Firebase (call this once at app startup)
initializeFirebase();
```

## Usage

### Authentication

```typescript
import { signUp, signIn, logout, onAuthStateChange } from './backend/firebase_manager';

// Sign up
const result = await signUp('user@example.com', 'password123', 'username');
if (result.success) {
  console.log('User created:', result.user);
}

// Sign in
const loginResult = await signIn('user@example.com', 'password123');

// Logout
await logout();

// Listen to auth state changes
onAuthStateChange((user) => {
  if (user) {
    console.log('User logged in:', user.uid);
  } else {
    console.log('User logged out');
  }
});
```

### User Data Management

```typescript
import { getUserData, updateUserData, addItemToInventory } from './backend/firebase_manager';

// Get user data
const userData = await getUserData(userId);

// Update user data
await updateUserData(userId, {
  level: 5,
  score: 1000,
});

// Add item to inventory
await addItemToInventory(userId, {
  itemId: 'yoga_mat',
  itemName: 'Yoga Mat',
  tier: 1,
  quantity: 2,
  discoveredAt: Timestamp.now(),
});
```

### Real-time Sync

```typescript
import { initializeSync, getSyncManager, ConflictResolutionStrategy } from './backend/firebase_manager';

// Initialize sync
await initializeSync(ConflictResolutionStrategy.TIMESTAMP);

// Get sync manager
const syncManager = getSyncManager();

// Subscribe to data updates
syncManager.onDataUpdate((userData) => {
  console.log('Data updated:', userData);
});

// Update local data (automatically syncs to server)
await syncManager.updateLocalData({
  score: 1500,
  level: 6,
});
```

### Cloud Functions

```typescript
import { mergeItems, handleLevelUp, processPurchase } from './backend/firebase_manager';

// Merge items
const mergeResult = await mergeItems('yoga_mat', 2);
if (mergeResult.success) {
  console.log('Merged item:', mergeResult.mergedItem);
  console.log('XP gained:', mergeResult.xpGained);
}

// Handle level up
const levelUpResult = await handleLevelUp(5);
if (levelUpResult.success) {
  console.log('Rewards:', levelUpResult.rewards);
}

// Process purchase
const purchaseResult = await processPurchase(
  'coins',
  'coins_100',
  'transaction_123',
  9.99
);
```

### Real-time Inventory Updates

```typescript
import { subscribeToInventory } from './backend/firebase_manager';

// Subscribe to inventory changes
const unsubscribe = subscribeToInventory(userId, (inventory) => {
  console.log('Inventory updated:', inventory);
  // Update UI with new inventory
});

// Unsubscribe when done
unsubscribe();
```

## Firestore Database Structure

### `/users/{userId}`
```typescript
{
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
```

### `/items/{itemId}`
```typescript
{
  itemId: string;
  name: string;
  type: 'wellness_item';
  tier: number; // 1-5
  description: string;
  mergeResult?: string;
  mergeRequirement?: number;
  xpReward: number;
  coinReward: number;
  area: number;
}
```

### `/gameEvents/{eventId}`
```typescript
{
  userId: string;
  eventType: 'merge' | 'levelup' | 'purchase' | 'discover' | 'challenge_complete';
  timestamp: Timestamp;
  metadata?: {
    [key: string]: any;
  };
}
```

## Security Rules

Security rules are defined in `firestore.rules`. Key features:

- Users can only read/write their own data
- Admins have full access
- Input validation on all writes
- Immutable game events (users can create, not update/delete)
- Public read access to items (game data)

Deploy rules with:
```bash
firebase deploy --only firestore:rules
```

## Cloud Functions

### Deploy Functions

```bash
npm run deploy:functions
```

### Available Functions

1. **onUserMergeItems** - Handles item merging logic
   - Validates merge requirements
   - Updates inventory
   - Awards XP and coins
   - Creates game event

2. **onUserLevelUp** - Handles level progression
   - Validates level progression
   - Awards level-up rewards
   - Creates game event

3. **onItemPurchase** - Processes in-app purchases
   - Validates transactions
   - Prevents duplicate purchases
   - Grants items/currency
   - Creates game event

## Conflict Resolution

The real-time sync system supports multiple conflict resolution strategies:

- **SERVER_WINS** - Server data always takes precedence
- **CLIENT_WINS** - Local data always takes precedence
- **TIMESTAMP** - Most recent data wins (default)
- **MERGE** - Intelligently merges data (e.g., inventory items)

## Offline Support

The system automatically:
- Saves data to local storage when offline
- Syncs changes when connection is restored
- Resolves conflicts using the configured strategy
- Provides real-time updates when online

## Error Handling

All functions return structured results:
```typescript
{
  success: boolean;
  data?: T;
  error?: string;
}
```

Always check `success` before using `data`.

## Best Practices

1. **Always validate user input** before calling Firebase functions
2. **Handle errors gracefully** - check `success` flag
3. **Use real-time subscriptions** for live updates
4. **Initialize sync** after user authentication
5. **Clean up subscriptions** when components unmount
6. **Use batch operations** for atomic updates
7. **Validate data** on both client and server

## Testing

Run tests with:
```bash
npm test
```

## Deployment

Deploy everything:
```bash
npm run deploy:all
```

Deploy only functions:
```bash
npm run deploy:functions
```

Deploy only rules:
```bash
npm run deploy:rules
```

## Environment Variables

Required environment variables:
- `REACT_APP_FIREBASE_API_KEY`
- `REACT_APP_FIREBASE_AUTH_DOMAIN`
- `REACT_APP_FIREBASE_PROJECT_ID`
- `REACT_APP_FIREBASE_STORAGE_BUCKET`
- `REACT_APP_FIREBASE_MESSAGING_SENDER_ID`
- `REACT_APP_FIREBASE_APP_ID`

## Support

For issues or questions, refer to:
- [Firebase Documentation](https://firebase.google.com/docs)
- [Firestore Security Rules](https://firebase.google.com/docs/firestore/security/get-started)
- [Cloud Functions](https://firebase.google.com/docs/functions)
