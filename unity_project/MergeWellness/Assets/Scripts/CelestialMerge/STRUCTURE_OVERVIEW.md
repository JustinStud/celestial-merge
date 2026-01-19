# 📐 Celestial Merge - Struktur-Übersicht

## 🎯 Unity Hierarchy Struktur

```
Scene: Gameplay
│
├── CelestialGameManager (Main Controller)
│   └── Script: CelestialGameManager
│
├── CurrencyManager
│   └── Script: CurrencyManager
│
├── CelestialProgressionManager
│   └── Script: CelestialProgressionManager
│
├── CelestialMergeManager
│   └── Script: CelestialMergeManager
│   └── References: CurrencyManager, ProgressionManager, ItemDatabase
│
├── ExpandableBoardManager
│   └── Script: ExpandableBoardManager
│   └── References: ProgressionManager, BoardParent, SlotPrefab
│
├── IdleProductionManager
│   └── Script: IdleProductionManager
│
├── DailySystemManager
│   └── Script: DailySystemManager
│
├── CraftingSystem
│   └── Script: CraftingSystem
│   └── References: ItemDatabase
│
├── ItemSynergySystem
│   └── Script: ItemSynergySystem
│   └── References: CurrencyManager, ProgressionManager
│
├── MiniGameManager
│   └── Script: MiniGameManager
│   └── References: CurrencyManager, DailySystemManager
│
└── Canvas (UI)
    ├── CelestialUIManager
    │   └── Script: CelestialUIManager
    │   └── References: Alle UI-Elemente
    │
    ├── CurrencyPanel
    │   ├── StardustText (TextMeshPro)
    │   ├── StardustIcon (Image)
    │   ├── CrystalsText (TextMeshPro)
    │   └── CrystalsIcon (Image)
    │
    ├── ProgressionPanel
    │   ├── LevelText (TextMeshPro)
    │   ├── ChapterText (TextMeshPro)
    │   ├── XPProgressBar (Slider)
    │   └── XPText (TextMeshPro)
    │
    ├── DailyLoginPanel
    │   ├── DailyLoginButton (Button)
    │   └── DailyLoginDayText (TextMeshPro)
    │
    ├── DailyQuestPanel
    │   └── DailyQuestContainer (Panel mit Vertical Layout Group)
    │
    ├── MiniGamePanel
    │   ├── EnergyText (TextMeshPro)
    │   └── PlayMiniGameButton (Button)
    │
    ├── MergeResultPanel
    │   ├── MergeResultText (TextMeshPro)
    │   └── MergeRewardText (TextMeshPro)
    │
    └── BoardParent (Grid Parent)
        └── GridLayoutGroup Component
        └── (Slots werden automatisch erstellt)
```

## 📦 Project Structure

```
Assets/
├── Scripts/
│   └── CelestialMerge/
│       ├── Core Systems/
│       │   ├── CelestialItem.cs
│       │   ├── CelestialItemDatabase.cs
│       │   ├── CelestialGameManager.cs
│       │   ├── CurrencyManager.cs
│       │   ├── CelestialProgressionManager.cs
│       │   └── CelestialMergeManager.cs
│       │
│       ├── Game Systems/
│       │   ├── ExpandableBoardManager.cs
│       │   ├── IdleProductionManager.cs
│       │   ├── DailySystemManager.cs
│       │   ├── CraftingSystem.cs
│       │   ├── ItemSynergySystem.cs
│       │   └── MiniGameManager.cs
│       │
│       └── UI/
│           └── CelestialUIManager.cs
│
├── Prefabs/
│   └── BoardSlot.prefab
│
└── Resources/
    └── CelestialItemDatabase.asset
```

## 🔗 System-Abhängigkeiten

```
CelestialGameManager (Zentrale Steuerung)
    │
    ├──→ CurrencyManager (Standalone)
    │
    ├──→ CelestialProgressionManager (Standalone)
    │
    ├──→ CelestialMergeManager
    │       ├──→ CurrencyManager
    │       ├──→ CelestialProgressionManager
    │       └──→ CelestialItemDatabase
    │
    ├──→ ExpandableBoardManager
    │       ├──→ CelestialProgressionManager
    │       └──→ BoardParent (UI)
    │
    ├──→ IdleProductionManager
    │       └──→ CurrencyManager
    │
    ├──→ DailySystemManager
    │       ├──→ CurrencyManager
    │       └──→ CelestialProgressionManager
    │
    ├──→ CraftingSystem
    │       └──→ CelestialItemDatabase
    │
    ├──→ ItemSynergySystem
    │       ├──→ CurrencyManager
    │       └──→ CelestialProgressionManager
    │
    ├──→ MiniGameManager
    │       ├──→ CurrencyManager
    │       └──→ DailySystemManager
    │
    └──→ CelestialUIManager
            └──→ CelestialGameManager (für alle Referenzen)
```

## ⚙️ Event-Flow

```
Player Action
    │
    ├──→ Merge Items
    │       └──→ CelestialMergeManager
    │               ├──→ CurrencyManager (Stardust Reward)
    │               ├──→ CelestialProgressionManager (XP + Register Merge)
    │               └──→ CelestialUIManager (Update UI)
    │
    ├──→ Claim Daily Login
    │       └──→ DailySystemManager
    │               ├──→ CurrencyManager (Rewards)
    │               └──→ CelestialUIManager (Update UI)
    │
    ├──→ Play Mini-Game
    │       └──→ MiniGameManager
    │               ├──→ CurrencyManager (Crystal Rewards)
    │               ├──→ DailySystemManager (Quest Progress)
    │               └──→ CelestialUIManager (Update UI)
    │
    └──→ Level Up
            └──→ CelestialProgressionManager
                    ├──→ CurrencyManager (Update Capacity)
                    ├──→ ExpandableBoardManager (Board Expansion)
                    └──→ CelestialUIManager (Update UI)
```

## 📋 Setup-Checkliste

### Phase 1: Core Systems
- [ ] CelestialGameManager erstellt
- [ ] Alle Manager-GameObjects erstellt
- [ ] Alle Scripts hinzugefügt
- [ ] ItemDatabase Asset erstellt und initialisiert

### Phase 2: System-Verbindungen
- [ ] GameManager mit allen Systemen verbunden
- [ ] System-Inter-Verbindungen gesetzt
- [ ] ItemDatabase zu allen Systemen verbunden

### Phase 3: UI Setup
- [ ] Canvas erstellt
- [ ] Alle UI-Panels erstellt
- [ ] CelestialUIManager erstellt
- [ ] Alle UI-Referenzen verbunden

### Phase 4: Testing
- [ ] Play-Mode: Keine Console-Fehler
- [ ] Play-Mode: UI aktualisiert sich
- [ ] Play-Mode: Systeme funktionieren

## 🎨 UI-Layout Vorschlag

```
┌─────────────────────────────────────────┐
│ [Level 1] [Chapter 1]    [⭐ 1000] [💎 50] │  ← Top Bar
├─────────────────────────────────────────┤
│                                         │
│                                         │
│         [Game Board - 4×5]              │
│                                         │
│                                         │
├─────────────────────────────────────────┤
│ [Daily Quests]  [Energy: 5/10] [Play] │  ← Bottom Bar
└─────────────────────────────────────────┘
```

## 🔧 Wichtige Einstellungen

### Canvas Settings:
- Render Mode: `Screen Space - Overlay`
- Canvas Scaler: `Scale With Screen Size`
- Reference Resolution: `1920 × 1080`
- Match: `0.5` (Width/Height)

### GridLayoutGroup Settings:
- Constraint: `Fixed Column Count`
- Constraint Count: `4` (startet mit 4×5)
- Cell Size: `100 × 100`
- Spacing: `10 × 10`

### TextMeshPro Settings:
- Font: `LegacyRuntime` (Standard) oder eigene Font
- Font Size: `24-32` (je nach Element)
- Alignment: `Center` oder `Left`

## 📝 Nächste Schritte nach Integration

1. **UI-Polish:** Animations, Sounds, Visual Effects
2. **Match-3 Game:** Vollständige Implementierung
3. **Physics Engine:** Gravity & Collisions
4. **Story System:** Narrative Integration
5. **Guild System:** Social Features
6. **Monetization:** IAP, Ads, Battle Pass

Viel Erfolg! 🚀
