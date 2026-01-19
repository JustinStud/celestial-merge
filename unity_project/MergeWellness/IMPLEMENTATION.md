# 🎮 Merge Wellness - Implementierungsübersicht

## ✅ Implementierte Features

### 1. **WellnessItem Datenstruktur** ✅
- `WellnessItem.cs`: Item-Klasse mit Tier-System (1-10)
- Properties: ItemId, ItemName, Tier, ItemType, WellnessFact
- Methoden: `CanMergeWith()`, `GetMergedResultId()`

### 2. **ItemDatabase** ✅
- `ItemDatabase.cs`: ScriptableObject für Item-Verwaltung
- Standard-Items initialisiert (Tier 1-3)
- Methoden: `CreateItem()`, `GetStarterItemIds()`, `GetMergedItemId()`
- Erweiterbar für mehr Items und Tiers

### 3. **5×5 Grid-System** ✅
- `GridManager.cs`: Verwaltet 25 Slots
- Automatische Slot-Erstellung zur Laufzeit
- `GridLayoutGroup` für Snap-to-Grid
- Overflow-Inventory für volle Grids (max. 10 Items)

### 4. **Drag-Drop-Mechanik** ✅
- `GridSlot.cs`: Implementiert `IBeginDragHandler`, `IDragHandler`, `IEndDragHandler`, `IDropHandler`
- Visuelles Drag-Objekt während Drag
- Snap-to-Grid durch `GridLayoutGroup`
- Item-Verschiebung und Swap-Funktionalität

### 5. **Merge-Mechanik (2×N → N+1)** ✅
- `GridManager.PerformMerge()`: Führt Merge durch
- Validierung: Gleicher Typ und Tier
- Erstellt gemergtes Item (Tier + 1)
- Entfernt beide Quell-Items
- Platziert gemergtes Item im ersten Slot

### 6. **Progression-System** ✅
- `GameplayManager.cs`: Verwaltet Progression
- **Daily Rewards**: 1 Starter-Item pro Tag
- **Merge-Milestones**: 10, 25, 50, 100, 250, 500 Merges
- **Wellness-Facts**: Pop-ups bei neuen Items
- **Score-System**: Punkte basierend auf Tier

### 7. **UI-System** ✅
- `UIManager.cs`: Verwaltet alle UI-Elemente
- Score-Anzeige
- Merge-Count-Anzeige
- Daily Reward Button
- Wellness-Fact Pop-ups
- Milestone-Benachrichtigungen
- Leaderboard (vorbereitet)

### 8. **Firebase Backend-Integration** ✅
- `FirebaseManager.cs`: Backend-Integration
- `LogMergeEvent()`: Sendet Merge-Events
- `SaveGameStateToCloud()`: Cloud Save
- `UpdateLeaderboard()`: Leaderboard-Updates
- Vorbereitet für Firebase SDK

### 9. **EventSystem Setup** ✅
- `EventSystemSetup.cs`: Stellt EventSystem sicher
- Automatische Erstellung falls fehlend
- Notwendig für Drag-Drop

## 📋 Code-Struktur

```
Scripts/
├── WellnessItem.cs          # Item-Datenstruktur
├── ItemDatabase.cs           # Item-Datenbank (ScriptableObject)
├── GridSlot.cs               # Einzelner Slot (Drag-Drop)
├── GridManager.cs            # Grid-Verwaltung (5×5)
├── GameplayManager.cs        # Progression, Daily Rewards
├── UIManager.cs              # UI-Verwaltung
├── FirebaseManager.cs        # Backend-Integration
└── EventSystemSetup.cs       # EventSystem-Initialisierung
```

## 🔄 Workflow

### Merge-Prozess:
1. Spieler zieht Item von Slot A
2. Spieler lässt Item auf Slot B fallen
3. `GridManager.HandleSlotDrop()` wird aufgerufen
4. Prüfung: Gleicher Typ & Tier?
5. Wenn ja: `PerformMerge()` → Erstellt Tier+1 Item
6. `GameplayManager.OnItemMerged()` → Score, Milestones, Facts
7. `FirebaseManager.LogMergeEvent()` → Backend-Logging

### Daily Reward:
1. `GameplayManager.CheckDailyReward()` beim Start
2. Prüft ob letzter Reward heute war
3. Zeigt Daily Reward Button
4. Spieler klickt → `ClaimDailyReward()`
5. Zufälliges Starter-Item wird zum Grid hinzugefügt

## 🎯 Item-Types & Tiers

### Starter Items (Tier 1):
- `yoga_mat_tier1`: Yoga Mat
- `meditation_stone_tier1`: Meditation Stone
- `herbal_tea_tier1`: Herbal Tea

### Merge-Beispiele:
- 2× Yoga Mat (Tier 1) → Meditation Space (Tier 2)
- 2× Meditation Space (Tier 2) → Yoga Studio (Tier 3)
- etc.

## 🔧 Erweiterungsmöglichkeiten

### Mehr Items hinzufügen:
1. Öffne `ItemDatabase` Asset
2. Rechtsklick → `Initialize Default Items` (erweitern)
3. Oder manuell Items in Inspector hinzufügen

### Mehr Tiers:
- Erweitere `ItemDatabase.InitializeDefaultItems()`
- Füge Items für Tier 4-10 hinzu

### Firebase Setup:
1. Installiere Firebase SDK für Unity
2. Konfiguriere Firebase-Projekt
3. Aktiviere `enableFirebase` im `FirebaseManager`

## 📝 Nächste Schritte

1. **Unity-Projekt öffnen** und Scene erstellen
2. **ItemDatabase Asset** erstellen und initialisieren
3. **Scene-Setup** nach README-Anleitung
4. **UI-Elemente** erstellen und verbinden
5. **Testen** der Merge-Mechanik
6. **Sprites hinzufügen** für Items
7. **Firebase konfigurieren** (optional)

## ✨ Features im Detail

### Snap-to-Grid
- Automatisch durch `GridLayoutGroup`
- Cell Size: 100×100 Pixel
- Spacing: 10 Pixel
- Padding: 10 Pixel

### Overflow-Handling
- Wenn Grid voll (25 Items)
- Neue Items → Overflow-Inventory
- Max. 10 Overflow-Slots
- UI-Update für Overflow-Anzeige

### Wellness-Facts
- Jedes Item hat `WellnessFact` Property
- Pop-up bei neuem gemergtem Item
- Informative Fakten über Wellness

### Milestones
- Automatische Belohnungen bei:
  - 10, 25, 50, 100, 250, 500 Merges
- Belohnung: Zufälliges Starter-Item

## 🐛 Bekannte Einschränkungen

1. **Sprites**: Items haben noch keine Sprites (Farbige Quadrate als Fallback)
2. **Firebase**: Benötigt Firebase SDK Installation
3. **Overflow-UI**: Overflow-Inventory UI muss noch implementiert werden
4. **Leaderboard-UI**: Leaderboard-Panel muss noch gestylt werden

## 📚 Dokumentation

- Siehe `README.md` für Setup-Anleitung
- Code ist vollständig kommentiert
- Alle öffentlichen Methoden dokumentiert
