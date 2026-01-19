# Celestial Merge - Detaillierte Integrations-Anleitung

## 📋 Übersicht

Diese Anleitung führt dich Schritt für Schritt durch:
1. **Erstellung des zentralen CelestialGameManager**
2. **Implementierung der UI für alle Systeme**

---

## 🎯 Schritt 1: Zentrale GameManager-Integration

### 1.1 GameManager-Setup in Unity

#### Schritt 1.1.1: GameManager GameObject erstellen

1. **Öffne Unity** und navigiere zu deiner Gameplay-Szene
2. **Erstelle ein leeres GameObject:**
   - Rechtsklick in Hierarchy → `Create Empty`
   - Benenne es: `CelestialGameManager`
   - Position: (0, 0, 0) - Position ist egal, da es nur Scripts enthält

#### Schritt 1.1.2: CelestialGameManager Script hinzufügen

1. **Füge das Script hinzu:**
   - Wähle `CelestialGameManager` GameObject
   - Im Inspector: `Add Component` → Suche nach `CelestialGameManager`
   - Das Script wurde bereits erstellt (`CelestialGameManager.cs`)

2. **Aktiviere Auto-Initialize:**
   - Im Inspector: `Auto Initialize` = ✅ (aktiviert)
   - `Debug Mode` = ✅ (für Entwicklung)

### 1.2 System-Manager erstellen

#### Schritt 1.2.1: Alle Manager-GameObjects erstellen

Erstelle für jedes System ein GameObject:

```
Hierarchy:
├── CelestialGameManager (Haupt-Manager)
├── CurrencyManager
├── CelestialProgressionManager
├── CelestialMergeManager
├── ExpandableBoardManager
├── IdleProductionManager
├── DailySystemManager
├── CraftingSystem
├── ItemSynergySystem
└── MiniGameManager
```

**Vorgehen:**
1. Für jedes System: `Create Empty` GameObject
2. Benenne es entsprechend (z.B. `CurrencyManager`)
3. Füge das entsprechende Script hinzu (`Add Component`)

#### Schritt 1.2.2: Scripts zu GameObjects hinzufügen

**Für jedes GameObject:**

1. **CurrencyManager:**
   - Script: `CurrencyManager.cs`
   - Keine speziellen Einstellungen nötig

2. **CelestialProgressionManager:**
   - Script: `CelestialProgressionManager.cs`
   - Keine speziellen Einstellungen nötig

3. **CelestialMergeManager:**
   - Script: `CelestialMergeManager.cs`
   - **WICHTIG:** Ziehe `CurrencyManager` und `CelestialProgressionManager` in die Referenzen

4. **ExpandableBoardManager:**
   - Script: `ExpandableBoardManager.cs`
   - **WICHTIG:** 
     - Erstelle ein UI Canvas (falls noch nicht vorhanden)
     - Erstelle ein leeres GameObject als `BoardParent` (unter Canvas)
     - Ziehe `BoardParent` in die `Board Parent` Referenz
     - Erstelle ein Slot-Prefab (siehe unten)

5. **IdleProductionManager:**
   - Script: `IdleProductionManager.cs`
   - Keine speziellen Einstellungen nötig

6. **DailySystemManager:**
   - Script: `DailySystemManager.cs`
   - Keine speziellen Einstellungen nötig

7. **CraftingSystem:**
   - Script: `CraftingSystem.cs`
   - **WICHTIG:** Ziehe `CelestialItemDatabase` in die Referenz

8. **ItemSynergySystem:**
   - Script: `ItemSynergySystem.cs`
   - **WICHTIG:** Ziehe `CurrencyManager` und `CelestialProgressionManager` in die Referenzen

9. **MiniGameManager:**
   - Script: `MiniGameManager.cs`
   - **WICHTIG:** Ziehe `CurrencyManager` und `DailySystemManager` in die Referenzen

### 1.3 ItemDatabase Setup

#### Schritt 1.3.1: ItemDatabase Asset erstellen

1. **Im Project-Fenster:**
   - Navigiere zu `Assets/Scripts/CelestialMerge/`
   - Rechtsklick → `Create` → `CelestialMerge` → `ItemDatabase`
   - Benenne es: `CelestialItemDatabase`

2. **ItemDatabase initialisieren:**
   - Wähle das Asset im Project-Fenster
   - Im Inspector: Rechtsklick auf das Script → `Initialize Celestial Items`
   - Oder: Im Inspector-Button klicken (falls vorhanden)

#### Schritt 1.3.2: ItemDatabase zu GameManager verbinden

1. **Wähle `CelestialGameManager` GameObject**
2. **Im Inspector:**
   - Ziehe `CelestialItemDatabase` Asset in die `Item Database` Referenz

### 1.4 System-Verbindungen

#### Schritt 1.4.1: GameManager mit allen Systemen verbinden

1. **Wähle `CelestialGameManager` GameObject**
2. **Im Inspector:** Ziehe alle Manager-GameObjects in die entsprechenden Referenzen:
   - `Currency Manager` → `CurrencyManager` GameObject
   - `Progression Manager` → `CelestialProgressionManager` GameObject
   - `Merge Manager` → `CelestialMergeManager` GameObject
   - etc.

**ODER:** Lasse `Auto Find Systems` aktiviert - dann findet der GameManager alle Systeme automatisch!

#### Schritt 1.4.2: System-Inter-Verbindungen

**CelestialMergeManager:**
- `Item Database` → `CelestialItemDatabase` Asset
- `Currency Manager` → `CurrencyManager` GameObject
- `Progression Manager` → `CelestialProgressionManager` GameObject

**ExpandableBoardManager:**
- `Progression Manager` → `CelestialProgressionManager` GameObject
- `Board Parent` → UI GameObject (Grid Parent)
- `Slot Prefab` → Slot Prefab (siehe unten)

### 1.5 Slot Prefab erstellen

#### Schritt 1.5.1: Board Slot Prefab

1. **Erstelle ein UI Image:**
   - Canvas → Rechtsklick → `UI` → `Image`
   - Benenne es: `BoardSlot`

2. **Füge Script hinzu:**
   - `Add Component` → `CelestialBoardSlot` (wird automatisch erstellt)

3. **Erstelle Prefab:**
   - Ziehe `BoardSlot` GameObject in Project-Fenster
   - Lösche das GameObject aus der Szene (Prefab bleibt)

4. **Verbinde Prefab:**
   - Wähle `ExpandableBoardManager`
   - Ziehe `BoardSlot` Prefab in `Slot Prefab` Referenz

### 1.6 Test der Integration

#### Schritt 1.6.1: Console-Logs prüfen

1. **Starte das Spiel** (Play-Button)
2. **Öffne Console** (Window → General → Console)
3. **Prüfe Logs:**
   - Sollte sehen: `=== Celestial Merge - Initialisierung ===`
   - Sollte sehen: `✅ Spiel erfolgreich initialisiert!`
   - Falls Fehler: Prüfe welche Systeme fehlen

#### Schritt 1.6.2: System-Status prüfen

1. **Wähle `CelestialGameManager` während Play-Mode**
2. **Im Inspector:** Prüfe ob alle Referenzen gesetzt sind
3. **Console:** Sollte `=== System Status ===` mit allen ✓ zeigen

---

## 🎨 Schritt 2: UI-Implementierung

### 2.1 UI-Canvas Setup

#### Schritt 2.1.1: Canvas erstellen

1. **Erstelle Canvas:**
   - Rechtsklick in Hierarchy → `UI` → `Canvas`
   - Canvas Scaler: `Scale With Screen Size`
   - Reference Resolution: `1920 × 1080`

2. **Erstelle EventSystem:**
   - Unity erstellt es automatisch
   - Falls nicht: Rechtsklick → `UI` → `Event System`

### 2.2 Currency UI

#### Schritt 2.2.1: Stardust Display

1. **Erstelle UI-Panel:**
   - Canvas → Rechtsklick → `UI` → `Panel`
   - Benenne es: `CurrencyPanel`
   - Position: Top-Right

2. **Erstelle Stardust Display:**
   - `CurrencyPanel` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `StardustText`
   - Text: `0`
   - Font Size: `24`
   - Color: Gold (#FFD700)

3. **Erstelle Icon:**
   - `CurrencyPanel` → Rechtsklick → `UI` → `Image`
   - Benenne es: `StardustIcon`
   - Position: Links vom Text
   - Sprite: Lade ein Stern-Icon (oder erstelle temporär)

#### Schritt 2.2.2: Crystals Display

1. **Erstelle Crystals Display:**
   - `CurrencyPanel` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `CrystalsText`
   - Text: `0`
   - Font Size: `24`
   - Color: Purple (#7B4397)

2. **Erstelle Icon:**
   - `CurrencyPanel` → Rechtsklick → `UI` → `Image`
   - Benenne es: `CrystalsIcon`
   - Position: Links vom Text

### 2.3 Progression UI

#### Schritt 2.3.1: Level Display

1. **Erstelle Panel:**
   - Canvas → Rechtsklick → `UI` → `Panel`
   - Benenne es: `ProgressionPanel`
   - Position: Top-Left

2. **Erstelle Level Text:**
   - `ProgressionPanel` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `LevelText`
   - Text: `Level 1`
   - Font Size: `32`

3. **Erstelle Chapter Text:**
   - `ProgressionPanel` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `ChapterText`
   - Text: `Chapter 1`
   - Font Size: `20`

#### Schritt 2.3.2: XP Progress Bar

1. **Erstelle Progress Bar:**
   - `ProgressionPanel` → Rechtsklick → `UI` → `Slider`
   - Benenne es: `XPProgressBar`
   - Min Value: `0`
   - Max Value: `1`
   - Value: `0`

2. **Erstelle XP Text:**
   - `XPProgressBar` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `XPText`
   - Text: `0 / 100 XP`
   - Position: Über der Progress Bar

### 2.4 Daily UI

#### Schritt 2.4.1: Daily Login Panel

1. **Erstelle Panel:**
   - Canvas → Rechtsklick → `UI` → `Panel`
   - Benenne es: `DailyLoginPanel`
   - Position: Center
   - Standard: Deaktiviert (nur bei Login sichtbar)

2. **Erstelle Button:**
   - `DailyLoginPanel` → Rechtsklick → `UI` → `Button - TextMeshPro`
   - Benenne es: `DailyLoginButton`
   - Text: `Claim Daily Bonus`

3. **Erstelle Day Text:**
   - `DailyLoginPanel` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `DailyLoginDayText`
   - Text: `Day 1`

#### Schritt 2.4.2: Daily Quests Panel

1. **Erstelle Panel:**
   - Canvas → Rechtsklick → `UI` → `Panel`
   - Benenne es: `DailyQuestPanel`
   - Position: Left-Side

2. **Erstelle Container:**
   - `DailyQuestPanel` → Rechtsklick → `UI` → `Panel`
   - Benenne es: `DailyQuestContainer`
   - Füge `Vertical Layout Group` hinzu
   - Spacing: `10`

### 2.5 Mini-Game UI

#### Schritt 2.5.1: Energy Display

1. **Erstelle Panel:**
   - Canvas → Rechtsklick → `UI` → `Panel`
   - Benenne es: `MiniGamePanel`
   - Position: Bottom-Right

2. **Erstelle Energy Text:**
   - `MiniGamePanel` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `EnergyText`
   - Text: `Energy: 5/10`

3. **Erstelle Play Button:**
   - `MiniGamePanel` → Rechtsklick → `UI` → `Button - TextMeshPro`
   - Benenne es: `PlayMiniGameButton`
   - Text: `Play Mini-Game`

### 2.6 Merge Result UI

#### Schritt 2.6.1: Merge Result Panel

1. **Erstelle Panel:**
   - Canvas → Rechtsklick → `UI` → `Panel`
   - Benenne es: `MergeResultPanel`
   - Position: Center
   - Standard: Deaktiviert

2. **Erstelle Result Text:**
   - `MergeResultPanel` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `MergeResultText`
   - Text: `Merge erfolgreich!`

3. **Erstelle Reward Text:**
   - `MergeResultPanel` → Rechtsklick → `UI` → `Text - TextMeshPro`
   - Benenne es: `MergeRewardText`
   - Text: `+50 Stardust`

### 2.7 CelestialUIManager Setup

#### Schritt 2.7.1: UI Manager GameObject

1. **Erstelle GameObject:**
   - Canvas → Rechtsklick → `Create Empty`
   - Benenne es: `CelestialUIManager`

2. **Füge Script hinzu:**
   - `Add Component` → `CelestialUIManager`

#### Schritt 2.7.2: UI-Referenzen verbinden

1. **Wähle `CelestialUIManager` GameObject**
2. **Im Inspector:** Ziehe alle UI-Elemente in die entsprechenden Referenzen:
   - `Stardust Text` → `StardustText` GameObject
   - `Crystals Text` → `CrystalsText` GameObject
   - `Level Text` → `LevelText` GameObject
   - etc.

3. **Game Manager Referenz:**
   - `Game Manager` → `CelestialGameManager` GameObject

### 2.8 UI-Testing

#### Schritt 2.8.1: Test im Play-Mode

1. **Starte das Spiel**
2. **Prüfe UI-Updates:**
   - Currency sollte angezeigt werden
   - Level sollte angezeigt werden
   - XP Bar sollte sich füllen (wenn XP hinzugefügt wird)

#### Schritt 2.8.2: Manuelle Tests

1. **Currency Test:**
   - Im Inspector: `CurrencyManager` → `Add Stardust` (Button falls vorhanden)
   - UI sollte sich aktualisieren

2. **Level Up Test:**
   - Im Inspector: `CelestialProgressionManager` → `Add XP` (Button falls vorhanden)
   - UI sollte sich aktualisieren

---

## ✅ Checkliste

### GameManager Integration:
- [ ] CelestialGameManager GameObject erstellt
- [ ] Alle System-Manager erstellt
- [ ] Alle Scripts zu GameObjects hinzugefügt
- [ ] ItemDatabase Asset erstellt und initialisiert
- [ ] Alle Referenzen verbunden
- [ ] Slot Prefab erstellt
- [ ] Console zeigt keine Fehler

### UI Implementation:
- [ ] Canvas erstellt
- [ ] Currency UI erstellt (Stardust + Crystals)
- [ ] Progression UI erstellt (Level + XP)
- [ ] Daily UI erstellt (Login + Quests)
- [ ] Mini-Game UI erstellt (Energy)
- [ ] Merge Result UI erstellt
- [ ] CelestialUIManager erstellt
- [ ] Alle UI-Referenzen verbunden
- [ ] UI aktualisiert sich im Play-Mode

---

## 🐛 Troubleshooting

### Problem: Systeme werden nicht gefunden
**Lösung:** 
- Prüfe ob alle Manager-GameObjects in der Szene sind
- Prüfe ob `Auto Find Systems` aktiviert ist
- Prüfe Console auf Fehler

### Problem: UI aktualisiert sich nicht
**Lösung:**
- Prüfe ob `CelestialUIManager` alle Referenzen hat
- Prüfe ob Events abonniert sind (Console-Logs)
- Prüfe ob Systeme Events auslösen

### Problem: ItemDatabase ist leer
**Lösung:**
- Wähle ItemDatabase Asset
- Rechtsklick → `Initialize Celestial Items`
- Prüfe ob Items in der Liste sind

---

## 📝 Nächste Schritte

Nach erfolgreicher Integration:
1. **Teste alle Systeme** einzeln
2. **Implementiere Match-3 Mini-Game** vollständig
3. **Füge Physics Engine** hinzu
4. **Implementiere Story System**
5. **Erstelle Guild System**

Viel Erfolg! 🚀
