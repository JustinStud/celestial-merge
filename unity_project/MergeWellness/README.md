# 🧘 Merge Wellness - Unity Game

Ein Merge-Game mit Wellness-Thema, implementiert in Unity mit C#.

## 🎮 Features

### Kern-Mechaniken
- **5×5 Grid-System** (25 Slots)
- **Drag-Drop-Mechanik** für Items
- **Merge-System**: 2× Level-N Items → 1× Level-(N+1) Item
- **Tier-System**: Items von Tier 1 bis Tier 10
- **Overflow-Inventory**: Für Items wenn Grid voll ist

### Progression
- **Daily Rewards**: 1 Starter-Item pro Tag
- **Merge-Milestones**: Achievements bei 10, 25, 50, 100, 250, 500 Merges
- **Wellness-Facts**: Pop-ups mit interessanten Fakten bei neuen Items
- **Score-System**: Punkte basierend auf Merge-Tier

### Backend-Integration
- **Firebase Cloud Functions**: Merge-Event-Verarbeitung
- **Cloud Save**: Spielstand-Synchronisation
- **Leaderboard**: Optional (Social Features)

## 📁 Projektstruktur

```
Assets/
├── Scripts/
│   ├── WellnessItem.cs          # Item-Datenstruktur
│   ├── ItemDatabase.cs          # Item-Datenbank (ScriptableObject)
│   ├── GridSlot.cs              # Einzelner Grid-Slot mit Drag-Drop
│   ├── GridManager.cs           # 5×5 Grid-Verwaltung
│   ├── GameplayManager.cs       # Progression, Daily Rewards, Milestones
│   ├── UIManager.cs             # UI-Verwaltung
│   ├── FirebaseManager.cs       # Backend-Integration
│   └── EventSystemSetup.cs      # EventSystem-Initialisierung
├── Prefabs/                     # UI-Prefabs
├── Scenes/                      # Game-Scenes
└── Resources/                   # Item-Sprites, etc.
```

## 🎮 Items erstellen und spielen

### Schnellstart: Items spawnen

**Option 1: ItemSpawner Script (Empfohlen)**
1. Erstelle leeres GameObject: `ItemSpawner`
2. Füge `ItemSpawner` Script hinzu
3. Im Inspector: Ziehe `GridManager` und `ItemDatabase` in die Referenzen
4. Klicke auf die Buttons im Inspector:
   - **"🎲 Spawn Random Item"** - Spawnt 1 zufälliges Item
   - **"🎲 Spawn 3 Random"** - Spawnt 3 zufällige Items
   - **"📦 Fill Grid"** - Füllt das Grid mit Items
   - **"🔗 Merge Test (2x)"** - Spawnt 2x gleiches Item zum Mergen

**Option 2: Daily Reward Button**
- Klicke auf den "Daily Reward" Button im Spiel
- Erhält 1 zufälliges Starter-Item pro Tag

**Option 3: Context Menu (im Play-Mode)**
- Rechtsklick auf `ItemSpawner` GameObject
- Wähle: "Spawn Random Starter Item" oder "Spawn Merge Test Items"

**Option 4: Code (für Entwickler)**
```csharp
// Im GameplayManager oder ItemSpawner:
itemSpawner.SpawnRandomStarterItem();
itemSpawner.SpawnItemById("yoga_mat_tier1");
itemSpawner.SpawnMergeTestItems();
```

## 🚀 Setup-Anleitung

### 1. Unity-Projekt öffnen
1. Öffne Unity Hub
2. Öffne das Projekt: `unity_project/MergeWellness/`

### 2. Item-Datenbank initialisieren
1. Im Project-Fenster: Rechtsklick → `Create → MergeWellness → ItemDatabase`
2. Wähle das erstellte Asset aus
3. Im Inspector: Klicke auf den Button **"Initialize Default Items"** (oben im Inspector)
4. Item-Datenbank ist jetzt mit Standard-Items gefüllt

**Alternative:** Falls der Button nicht erscheint, stelle sicher dass:
- Das `ItemDatabaseEditor.cs` Script im `Assets/Editor/` Ordner liegt
- Unity den Editor-Ordner neu kompiliert hat (warten bis Compile abgeschlossen)

### 3. Scene-Setup
1. Erstelle neue Scene: `Scenes/Gameplay.unity`
2. Erstelle Canvas (UI → Canvas)
3. Erstelle leeres GameObject: `GameplayManager` → Füge `GameplayManager` Script hinzu
4. Erstelle leeres GameObject: `GridManager` → Füge `GridManager` Script hinzu
5. Erstelle leeres GameObject: `UIManager` → Füge `UIManager` Script hinzu
6. Erstelle leeres GameObject: `EventSystemSetup` → Füge `EventSystemSetup` Script hinzu

### 4. Grid-Setup
1. Im Canvas: Erstelle leeres GameObject `GridPanel`
2. Füge `GridLayoutGroup` Component hinzu
3. Im `GridManager` Script: Ziehe `GridPanel` in `Grid Parent` Feld
4. `GridManager` erstellt automatisch Slots zur Laufzeit

### 5. UI-Setup
1. Erstelle UI-Text für Score: `ScoreText`
2. Erstelle UI-Text für Merge-Count: `MergeCountText`
3. Erstelle Button für Daily Reward: `DailyRewardButton`
4. Im `UIManager` Script: Ziehe alle UI-Elemente in entsprechende Felder

### 6. Referenzen verbinden
- `GameplayManager`:
  - `Grid Manager`: Ziehe GridManager GameObject
  - `Item Database`: Ziehe ItemDatabase Asset
  - `UI Manager`: Ziehe UIManager GameObject

- `GridManager`:
  - `Item Database`: Ziehe ItemDatabase Asset

## 🎯 Item-Types

- **yoga**: Yoga-bezogene Items
- **meditation**: Meditation-bezogene Items
- **herbal**: Kräuter-bezogene Items

## 🔧 Erweiterte Features

### Snap-to-Grid
Das Grid-System verwendet `GridLayoutGroup` für automatisches Snap-to-Grid.

### Overflow-Handling
Wenn das Grid voll ist, werden neue Items im Overflow-Inventory gespeichert (max. 10 Slots).

### Firebase-Integration
1. Installiere Firebase SDK für Unity
2. Konfiguriere Firebase-Projekt
3. Aktiviere `enableFirebase` im `FirebaseManager`

## 📝 Code-Beispiele

### Item erstellen
```csharp
WellnessItem item = itemDatabase.CreateItem("yoga_mat_tier1");
gridManager.AddItemToGrid(item);
```

### Merge durchführen
```csharp
// Automatisch durch Drag-Drop
// Oder programmatisch:
gridManager.HandleSlotDrop(slot1, slot2);
```

### Daily Reward abrufen
```csharp
gameplayManager.ClaimDailyReward();
```

## 🐛 Troubleshooting

### Items werden nicht angezeigt
- Prüfe ob `ItemDatabase` initialisiert ist
- Prüfe ob `GridManager` korrekt referenziert ist
- Prüfe ob Canvas und EventSystem vorhanden sind

### Drag-Drop funktioniert nicht
- Stelle sicher, dass `EventSystem` vorhanden ist
- Prüfe ob `Canvas` auf `Screen Space - Overlay` gesetzt ist
- Prüfe ob `GraphicRaycaster` am Canvas vorhanden ist

### Merge funktioniert nicht
- Prüfe ob beide Items gleichen Typ und Tier haben
- Prüfe ob `ItemDatabase.GetMergedItemId()` korrekt funktioniert

## 📚 Nächste Schritte

1. **Sprites hinzufügen**: Erstelle/Importiere Item-Sprites
2. **UI-Design**: Verbessere UI mit eigenen Designs
3. **Mehr Items**: Erweitere Item-Datenbank mit mehr Tiers
4. **Firebase Setup**: Konfiguriere Firebase für Cloud-Features
5. **Leaderboard**: Implementiere Leaderboard-UI

## 📄 Lizenz

Dieses Projekt ist Teil des Merge Wellness Game-Projekts.
