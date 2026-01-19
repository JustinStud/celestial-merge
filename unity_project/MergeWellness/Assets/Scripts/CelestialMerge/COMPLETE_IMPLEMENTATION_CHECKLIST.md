# ✅ Celestial Merge - Komplette Implementierungs-Checkliste

## 📋 Übersicht

Diese Checkliste führt dich Schritt für Schritt durch alle noch fehlenden Features, um das Spiel vollständig zu machen.

**Status:** 🟢 = Bereits implementiert | 🟡 = Script vorhanden, UI fehlt | 🔴 = Noch nicht implementiert

---

## 🎯 PHASE 1: Audio System Setup (30 Minuten)

### ✅ AudioManager Script
- ✅ `CelestialAudioManager.cs` ist implementiert
- ✅ Integration in `CelestialMergeManager` und `CelestialProgressionManager` vorhanden
- ✅ Integration in `SettingsMenu` vorhanden

### 🟡 Unity Editor Setup

#### Schritt 1.1: AudioManager GameObject erstellen
- [ ] **Hierarchy** → Rechtsklick → **Create Empty** → Name: `CelestialAudioManager`
- [ ] Wähle `CelestialAudioManager` GameObject
- [ ] **Inspector** → **Add Component** → `CelestialAudioManager` Script hinzufügen

#### Schritt 1.2: Audio Clips zuweisen (Optional)
- [ ] **Project** → Erstelle Ordner `Assets/Audio` falls nicht vorhanden
- [ ] Füge Audio Clips hinzu:
  - [ ] `BackgroundMusic` (AudioClip) → Ziehe in Inspector: `CelestialAudioManager` → **Background Music**
  - [ ] `MenuMusic` (AudioClip) → Ziehe in Inspector: **Menu Music**
  - [ ] `MergeSound` (AudioClip) → Ziehe in Inspector: **Merge Sound**
  - [ ] `LevelUpSound` (AudioClip) → Ziehe in Inspector: **Level Up Sound**
  - [ ] `ButtonClickSound` (AudioClip) → Ziehe in Inspector: **Button Click Sound**
  - [ ] `ErrorSound` (AudioClip) → Ziehe in Inspector: **Error Sound**
  - [ ] `CoinCollectSound` (AudioClip) → Ziehe in Inspector: **Coin Collect Sound**

**Hinweis:** Falls keine Audio Clips vorhanden, funktioniert das System auch, aber es gibt keine Sounds.

#### Schritt 1.3: Testen
- [ ] **Play** im Editor
- [ ] Prüfe Console: Sollte `✅ AudioManager initialisiert` zeigen
- [ ] Teste Merge → Sollte Merge Sound spielen
- [ ] Teste Level Up → Sollte Level Up Sound spielen

---

## 🎯 PHASE 2: Daily System UI (45 Minuten)

### ✅ DailyUIPanel Script
- ✅ `DailyUIPanel.cs` ist implementiert
- ✅ Integration mit `DailySystemManager` vorhanden
- ✅ Professionelles Layout (Merge-App-Stil)

### 🟡 Unity Editor Setup

#### Schritt 2.1: Daily Login Panel UI erstellen (Merge-App-Stil)

**WICHTIG:** Folgende Layout entspricht professionellen Merge-Apps (Merge Dragons, Merge Mansion Stil)

##### Panel Setup
- [ ] **Hierarchy** → Canvas → Rechtsklick → **UI → Panel** → Name: `DailyLoginPanel`
- [ ] **Inspector** → `DailyLoginPanel`:
  - **Anchor Presets**: Center (Alt+Shift+Center)
  - **Pos X**: `0`
  - **Pos Y**: `0`
  - **Width**: `800` (80% Screen Width)
  - **Height**: `700` (70% Screen Height)
  - **Image Component** → **Color**: RGBA(20, 20, 30, 250) - Dunkelblau/Schwarz mit Transparenz
  - **Raycast Target**: DEAKTIVIERT (wichtig für Button-Klickbarkeit)

##### Titel-Bereich (oben)
- [ ] Im `DailyLoginPanel` → **Create Empty** → Name: `TitleText`
- [ ] Füge **TextMeshPro - Text (UI)** Component hinzu
  - **Text**: `"Daily Login Bonus"`
  - **Font Size**: `36`
  - **Color**: Weiß (#FFFFFF)
  - **Font Style**: Bold
  - **Alignment**: Center
  - **RectTransform**: Anchor Top-Center, Pos (0, -30), Width 600

##### Day Info (unter Titel)
- [ ] Im `DailyLoginPanel` → **Create Empty** → Name: `DayText`
- [ ] Füge **TextMeshPro - Text (UI)** Component hinzu
  - **Text**: `"Tag 1 von 7"`
  - **Font Size**: `28`
  - **Color**: Hellblau (#4A9EFF)
  - **Alignment**: Center
  - **RectTransform**: Anchor Top-Center, Pos (0, -80), Width 600

##### Reward Display (Mitte, PROFESSIONELL LAYOUT)
- [ ] Im `DailyLoginPanel` → **Create Empty** → Name: `RewardContainer`
- [ ] **RectTransform**: Anchor Center, Pos (0, 50), Size (600, 150)
- [ ] Im `RewardContainer` → **Create Empty** → Name: `RewardText`
- [ ] Füge **TextMeshPro - Text (UI)** Component hinzu
  - **Text**: `"💰 100 Stardust\n💎 0 Crystals"`
  - **Font Size**: `32` (für Zahlen), Labels automatisch kleiner
  - **Color**: Gold-Gelb (#FFD700) für Rewards
  - **Font Style**: Bold
  - **Alignment**: Center
  - **WICHTIG**: Text ist oben im Panel, NICHT über Grid!

##### Calendar View (optional, für besseres Design)
- [ ] Im `DailyLoginPanel` → **Create Empty** → Name: `CalendarContainer`
- [ ] **RectTransform**: Anchor Center, Pos (0, -150), Size (700, 200)
- [ ] **Inspector** → **Add Component** → **Horizontal Layout Group**
  - **Spacing**: `10` (Abstand zwischen Day-Boxen)
  - **Child Force Expand**: Width DEAKTIVIERT, Height DEAKTIVIERT
  - **Child Control Size**: Width AKTIVIERT, Height AKTIVIERT

**Calendar Day Box Prefab erstellen:**
- [ ] **Hierarchy** → **Create Empty** → Name: `DayBoxPrefab`
- [ ] Füge **Image** Component hinzu (Background für Day Box)
  - **Color**: Grau (#444444) für vergangene Tage
  - **Width**: `80`, **Height**: `80`
- [ ] Im `DayBoxPrefab` → **Create Empty** → Name: `DayNumberText`
- [ ] Füge **TextMeshPro - Text (UI)** Component hinzu
  - **Text**: `"1"`
  - **Font Size**: `24`
  - **Color**: Weiß
  - **Alignment**: Center
- [ ] Im `DayBoxPrefab` → **Create Empty** → Name: `CheckmarkIcon` (optional)
- [ ] Füge **Image** Component hinzu → **Sprite**: Checkmark-Icon (falls vorhanden)
  - **Color**: Grün (#00FF00)
  - Standard: **Active** DEAKTIVIERT (wird nur bei abgeholten Tagen gezeigt)

**Day Box Prefab speichern:**
- [ ] **Project** → `Assets/Prefabs` (erstellen falls nötig) → Ziehe `DayBoxPrefab` hinein
- [ ] Lösche `DayBoxPrefab` aus Hierarchy

**Hinweis:** Calendar ist optional. Falls du den Calendar nicht möchtest, kannst du diesen Schritt überspringen.

##### Claim Button (unten, groß und auffällig)
- [ ] Im `DailyLoginPanel` → **Create Empty** → Name: `ClaimButton`
- [ ] Füge **Button** Component hinzu
- [ ] **RectTransform**: Anchor Bottom-Center, Pos (0, 50), Size (300, 60)
- [ ] **Button Color**: Grün (#33CC66) wenn verfügbar
- [ ] Füge **TextMeshPro - Text (UI)** Child hinzu
  - **Text**: `"🎁 Abholen"`
  - **Font Size**: `24`
  - **Font Style**: Bold
  - **Alignment**: Center

##### Streak Info (ganz unten, klein)
- [ ] Im `DailyLoginPanel` → **Create Empty** → Name: `StreakText`
- [ ] Füge **TextMeshPro - Text (UI)** Component hinzu
  - **Text**: `"Streak: 1 Tag"`
  - **Font Size**: `18`
  - **Color**: Grau (#AAAAAA)
  - **Alignment**: Center
  - **RectTransform**: Anchor Bottom-Center, Pos (0, 10)

#### Schritt 2.2: Daily Quests Panel UI erstellen

##### Panel Setup
- [ ] **Hierarchy** → Canvas → **UI → Panel** → Name: `DailyQuestPanel`
- [ ] **Inspector** → Anchor: Center
- [ ] **Width**: `800`, **Height**: `600`
- [ ] **Image Component** → **Raycast Target**: DEAKTIVIERT
- [ ] **Inspector** → **Active Checkbox** → **DEAKTIVIEREN** (Panel ist initial versteckt)
- [ ] **Inspector** → **Add Component** → **Canvas Group**
  - **Alpha**: `0.95` ← **HIER EINSTELLEN!**
  - **Interactable**: ✅ Aktiviert
  - **Blocks Raycasts**: ✅ Aktiviert

##### Titel & Close Button
- [ ] Im `DailyQuestPanel` → **Create Empty** → Name: `QuestTitleText`
- [ ] Füge **TextMeshPro - Text (UI)** Component hinzu
  - **Text**: `"Daily Quests"`
  - **Font Size**: `32`, **Color**: Weiß, **Font Style**: Bold
  - **RectTransform**: Anchor Top-Center, Pos (0, -30), Width 600

- [ ] Im `DailyQuestPanel` → **Button** → Name: `CloseQuestButton`
- [ ] **RectTransform**: Anchor Top-Right, Pos (-20, -20), Size (40, 40)
- [ ] **Text**: `"X"` oder Close-Icon, Font Size: `24`
- [ ] **Button Color**: Rot (#FF4444) oder Grau

##### Quest Container
- [ ] Im `DailyQuestPanel` → **Create Empty** → Name: `QuestContainer`
- [ ] **RectTransform**: Anchor Center, Pos (0, -20), Size (750, 500)
- [ ] Füge **Vertical Layout Group** Component hinzu
  - **Spacing**: `10`
  - **Padding**: `10` (alle Seiten)
  - **Child Force Expand**: Width AKTIVIERT, Height DEAKTIVIERT

##### Quest Prefab erstellen
- [ ] **Hierarchy** → **Create Empty** → Name: `QuestPrefab`
- [ ] **RectTransform**: Size (750, 100) - Breite etwas kleiner als Container
- [ ] Füge **Image** Component hinzu (Background)
  - **Color**: RGBA(40, 40, 50, 200) - Dunkelgrau mit Transparenz
- [ ] Child-Objekte erstellen:
  - [ ] `NameText` → **TextMeshPro** → Text: `"Quest Name"`, Font Size: `24`, Color: Weiß
  - [ ] `ProgressText` → **TextMeshPro** → Text: `"0/10"`, Font Size: `20`, Color: Grau
  - [ ] `ProgressBar` → **Slider** → **Fill Area** → **Fill** (Child)
    - Progress Bar RectTransform: Anchor Stretch-Stretch, Pos (10, 0), Size (-20, 0)
    - Fill Color: Grün (#33CC66)
  - [ ] `CompletedIcon` → **Image** (optional) → Checkmark-Icon, Standard: **Active** DEAKTIVIERT

**Quest Prefab speichern:**
- [ ] **Project** → `Assets/Prefabs` → Ziehe `QuestPrefab` hinein
- [ ] Lösche `QuestPrefab` aus Hierarchy

#### Schritt 2.2.5: Quest Button in Haupt-UI erstellen (WICHTIG!)

**Problem:** Es gibt keinen Button, um das Quest Panel zu öffnen!

##### Quest Button in Haupt-UI
- [ ] **Hierarchy** → Canvas → **UI → Button - TextMeshPro** → Name: `QuestButton`
- [ ] **RectTransform**: Anchor **Top-Right** (oder Top-Left, je nach Design)
  - **Pos X**: `-100` (wenn Top-Right)
  - **Pos Y**: `-50`
  - **Size**: `150×50` (oder größer für bessere Sichtbarkeit)
- [ ] **Button Text**: `"📋 Quests"` oder `"Tägliche Aufgaben"`
- [ ] **Font Size**: `22`, **Font Style**: Bold
- [ ] **Button Color**: Blau (#4A9EFF) oder Akzentfarbe
- [ ] **WICHTIG:** Button sollte **immer sichtbar** sein (nicht im Panel)

**Alternative Position:** Falls du einen Sidebar/Menu hast, platziere den Button dort.

#### Schritt 2.3: DailyUIPanel Script zuweisen

- [ ] **Hierarchy** → Finde `CelestialUIManager` oder erstelle neues GameObject → Name: `DailyUIPanel`
- [ ] Füge `DailyUIPanel` Script hinzu
- [ ] **Inspector** → `DailyUIPanel`:
  - [ ] `Daily Manager`: Ziehe `DailySystemManager` GameObject hinein
  - [ ] `Daily Login Panel`: Ziehe `DailyLoginPanel` hinein
  - [ ] `Claim Login Button`: Ziehe `ClaimButton` hinein
  - [ ] `Login Day Text`: Ziehe `DayText` hinein
  - [ ] `Login Reward Text`: Ziehe `RewardText` hinein
  - [ ] `Daily Quest Panel`: Ziehe `DailyQuestPanel` hinein
  - [ ] `Quest Container`: Ziehe `QuestContainer` hinein
  - [ ] `Quest Prefab`: Ziehe `QuestPrefab` Prefab hinein
  - [ ] **`Open Quest Button`**: Ziehe `QuestButton` (aus Haupt-UI) hinein ⭐ **WICHTIG!**
  - [ ] **`Close Quest Button`**: Ziehe `CloseQuestButton` (aus DailyQuestPanel) hinein

#### Schritt 2.4: UI-Layering & Button-Fixes

##### Problem 1: Weißer Kasten mittig (Panel ist sichtbar)
- [ ] **Hierarchy** → Wähle `DailyLoginPanel`
- [ ] **Inspector** → **Active Checkbox** oben links → **DEAKTIVIEREN** (unchecked)
  - **Wichtig:** Panel sollte beim Start **nicht** sichtbar sein
  - Panel wird nur angezeigt wenn `DailyUIPanel.ShowDailyLogin()` aufgerufen wird

##### Problem 2: Button kann nicht gedrückt werden (Raycast Blockierung)
- [ ] **Hierarchy** → Wähle `DailyLoginPanel`
- [ ] **Inspector** → Prüfe **Image** Component
  - Falls vorhanden: **Raycast Target** → **DEAKTIVIEREN** (unchecked)
  - Das verhindert, dass der Panel-Hintergrund Klicks blockiert

- [ ] **Hierarchy** → Wähle `ClaimButton` (unter DailyLoginPanel)
- [ ] **Inspector** → Prüfe **Button** Component
  - **Interactable** → **AKTIVIERT** (checked)
  - **Raycast Target** → **AKTIVIERT** (checked)

- [ ] **Hierarchy** → Prüfe Sibling Order
  - `ClaimButton` sollte **nach** `DailyLoginPanel` in der Hierarchy sein (unten)
  - Falls nicht: Rechtsklick auf `ClaimButton` → **Move To Last**

##### Canvas Sort Order (falls Problem besteht)
- [ ] **Hierarchy** → Finde **Canvas** GameObject
- [ ] **Inspector** → **Canvas** Component
  - **Sort Order**: `0` (oder niedriger als andere UI)
  - Falls Daily Panel auf separatem Canvas: **Sort Order** höher setzen (z.B. `10`)

#### Schritt 2.5: Close Button Problem beheben

##### Problem: "Close" Text erscheint im Grid/Container

**Ursache:** Close Button oder Text wurde versehentlich in den `QuestContainer` verschoben.

**Lösung:**
- [ ] **Hierarchy** → Erweitere `QuestContainer`
- [ ] **Suche** nach Objekten mit "Close" oder "Stardust" im Namen
- [ ] Falls gefunden: **Lösche** diese Objekte aus dem Container
- [ ] **Prüfe** ob `CloseQuestButton` direkt unter `DailyQuestPanel` ist (NICHT im Container!)

**Korrekte Position:**
```
DailyQuestPanel
├── CloseQuestButton  ← RICHTIG (direkt unter Panel)
└── QuestContainer
    └── (nur Quest-Objekte, KEIN Close Button!)
```

**Falsche Position:**
```
DailyQuestPanel
└── QuestContainer
    ├── CloseQuestButton  ← FALSCH! (sollte nicht hier sein)
    └── Quest_1
```

**Automatische Bereinigung:**
- Das Script bereinigt automatisch falsche Texte beim Öffnen des Panels
- Prüfe Console für Warnungen: `⚠️ Falscher Text im Quest Container gefunden`

#### Schritt 2.6: Testen
- [ ] **Play** im Editor
- [ ] **Prüfe:** Daily Login Panel sollte **nicht** sichtbar sein beim Start
- [ ] **Prüfe:** Quest Button sollte **sichtbar** sein in der Haupt-UI
- [ ] **Klicke** auf Quest Button → Daily Quest Panel sollte sich öffnen
- [ ] **Prüfe:** "Close" Text sollte **NICHT** im Grid/Container erscheinen
- [ ] **Klicke** auf Close Button (X) oben rechts im Quest Panel → Panel sollte sich schließen
- [ ] Teste Daily Login (falls Button vorhanden) → Button sollte funktionieren
- [ ] Prüfe Quests → Sollten angezeigt werden wenn Panel aktiviert wird

##### Debug: Button funktioniert immer noch nicht?
- [ ] Prüfe **Event System**: Hierarchy sollte **EventSystem** GameObject haben
- [ ] Falls fehlt: **Hierarchy** → Rechtsklick → **UI → Event System**
- [ ] Prüfe ob `DailyUIPanel` Script die Button-Referenzen hat (`Open Quest Button` und `Close Quest Button`)
- [ ] Prüfe Console für Fehler und Warnungen

---

## 🎯 PHASE 3: Idle Production UI (30 Minuten)

### ✅ IdleUIPanel Script
- ✅ `IdleUIPanel.cs` ist implementiert
- ✅ Integration mit `IdleProductionManager` vorhanden

### 🟡 Unity Editor Setup

#### Schritt 3.1: Idle Production Display erstellen

##### Production Display
- [ ] **Hierarchy** → Canvas → **UI → Panel** → Name: `IdleProductionPanel`
- [ ] **Inspector** → Anchor: **Top-Right**
- [ ] **Pos X**: `-100`, **Pos Y**: `-100`
- [ ] **Width**: `300`, **Height**: `100`

##### Text Display
- [ ] Im `IdleProductionPanel` → **Create Empty** → Name: `ProductionRateText`
- [ ] Füge **TextMeshPro** Component hinzu → Text: `"Production: 10.0 Stardust/Min"`, Font Size: `18`, Color: Weiß

- [ ] Im `IdleProductionPanel` → **Create Empty** → Name: `CurrentProductionText`
- [ ] Füge **TextMeshPro** Component hinzu → Text: `"+0.17 Stardust/Sek"`, Font Size: `16`, Color: Grau

#### Schritt 3.2: Offline Reward Panel erstellen

##### Panel Setup
- [ ] **Hierarchy** → Canvas → **UI → Panel** → Name: `OfflineRewardPanel`
- [ ] **Inspector** → Anchor: **Center**
- [ ] **Width**: `500`, **Height**: `300`
- [ ] **Inspector** → **Add Component** → **Canvas Group**
  - **Alpha**: `0.95` ← **HIER EINSTELLEN!**
  - **Interactable**: ✅ Aktiviert
  - **Blocks Raycasts**: ✅ Aktiviert

##### Text Display
- [ ] Im `OfflineRewardPanel` → **Create Empty** → Name: `OfflineTimeText`
- [ ] Füge **TextMeshPro** Component hinzu → Text: `"Offline: 2h 30m"`, **Font Size**: `28`, Color: Weiß

- [ ] Im `OfflineRewardPanel` → **Create Empty** → Name: `OfflineRewardText`
- [ ] Füge **TextMeshPro** Component hinzu → Text: `"+1500 Stardust"`, **Font Size**: `32`, Color: Gold

##### Buttons
- [ ] Im `OfflineRewardPanel` → **Button** → Name: `ClaimButton`
- [ ] **Text**: `"Abholen"`, Font Size: `24`

- [ ] Im `OfflineRewardPanel` → **Button** → Name: `CloseButton`
- [ ] **Text**: `"Schließen"`, Font Size: `20`

#### Schritt 3.3: IdleUIPanel Script zuweisen

- [ ] **Hierarchy** → Finde `CelestialUIManager` oder erstelle neues GameObject → Name: `IdleUIPanel`
- [ ] Füge `IdleUIPanel` Script hinzu
- [ ] **Inspector** → `IdleUIPanel`:
  - [ ] `Idle Manager`: Ziehe `IdleProductionManager` GameObject hinein
  - [ ] `Production Rate Text`: Ziehe `ProductionRateText` hinein
  - [ ] `Current Production Text`: Ziehe `CurrentProductionText` hinein
  - [ ] `Offline Reward Panel`: Ziehe `OfflineRewardPanel` hinein
  - [ ] `Offline Time Text`: Ziehe `OfflineTimeText` hinein
  - [ ] `Offline Reward Text`: Ziehe `OfflineRewardText` hinein
  - [ ] `Claim Offline Button`: Ziehe `ClaimButton` hinein
  - [ ] `Close Offline Panel Button`: Ziehe `CloseButton` hinein

#### Schritt 3.4: Testen
- [ ] **Play** im Editor
- [ ] Prüfe Production Display → Sollte Production Rate zeigen
- [ ] Teste Offline-Simulation → Panel sollte erscheinen

---

## 🎯 PHASE 4: Mini-Game UI (45 Minuten)

### ✅ MiniGameUIPanel Script
- ✅ `MiniGameUIPanel.cs` ist implementiert
- ✅ Integration mit `MiniGameManager` vorhanden

### 🟡 Unity Editor Setup

#### Schritt 4.1: Mini-Game Main Panel erstellen

##### Panel Setup
- [ ] **Hierarchy** → Canvas → **UI → Panel** → Name: `MiniGamePanel`
- [ ] **Inspector** → Anchor: **Center**
- [ ] **Width**: `800`, **Height**: `600`

##### Energy Display
- [ ] Im `MiniGamePanel` → **Create Empty** → Name: `EnergyDisplay`
- [ ] **TextMeshPro** → Name: `EnergyText` → Text: `"5/10 Energy"`, Font Size: `24`, Color: Weiß
- [ ] **Slider** → Name: `EnergyBar` → Value: `0.5`

##### Game Type Buttons
- [ ] Im `MiniGamePanel` → **Button** → Name: `EasyButton` → Text: `"Einfach"`, Font Size: `22`
- [ ] Im `MiniGamePanel` → **Button** → Name: `MediumButton` → Text: `"Mittel"`, Font Size: `22`
- [ ] Im `MiniGamePanel` → **Button** → Name: `HardButton` → Text: `"Schwer"`, Font Size: `22`

##### Description Text
- [ ] Im `MiniGamePanel` → **TextMeshPro** → Name: `GameTypeDescription` → Text: `"Wähle einen Schwierigkeitsgrad"`, Font Size: `20`

##### Close Button
- [ ] Im `MiniGamePanel` → **Button** → Name: `CloseButton` → Text: `"Schließen"`, Font Size: `22`

#### Schritt 4.2: Result Panel erstellen

##### Panel Setup
- [ ] **Hierarchy** → Canvas → **UI → Panel** → Name: `MiniGameResultPanel`
- [ ] **Inspector** → Anchor: **Center**
- [ ] **Width**: `600`, **Height**: `400`

##### Text Display
- [ ] Im `MiniGameResultPanel` → **TextMeshPro** → Name: `ResultText` → Text: `"✅ Gewonnen!"`, **Font Size**: `36`, Color: Grün
- [ ] Im `MiniGameResultPanel` → **TextMeshPro** → Name: `RewardText` → Text: `"Belohnung:\n50 Crystals\n500 Stardust"`, Font Size: `24`, Color: Weiß

##### Buttons
- [ ] Im `MiniGameResultPanel` → **Button** → Name: `CloseResultButton` → Text: `"Schließen"`
- [ ] Im `MiniGameResultPanel` → **Button** → Name: `PlayAgainButton` → Text: `"Nochmal spielen"`

#### Schritt 4.3: MiniGameUIPanel Script zuweisen

- [ ] **Hierarchy** → Finde `CelestialUIManager` oder erstelle neues GameObject → Name: `MiniGameUIPanel`
- [ ] Füge `MiniGameUIPanel` Script hinzu
- [ ] **Inspector** → `MiniGameUIPanel`:
  - [ ] `Mini Game Manager`: Ziehe `MiniGameManager` GameObject hinein
  - [ ] `Main Panel`: Ziehe `MiniGamePanel` hinein
  - [ ] `Close Button`: Ziehe `CloseButton` hinein
  - [ ] `Energy Text`: Ziehe `EnergyText` hinein
  - [ ] `Energy Bar`: Ziehe `EnergyBar` hinein
  - [ ] `Easy Game Button`: Ziehe `EasyButton` hinein
  - [ ] `Medium Game Button`: Ziehe `MediumButton` hinein
  - [ ] `Hard Game Button`: Ziehe `HardButton` hinein
  - [ ] `Game Type Description`: Ziehe `GameTypeDescription` hinein
  - [ ] `Result Panel`: Ziehe `MiniGameResultPanel` hinein
  - [ ] `Result Text`: Ziehe `ResultText` hinein
  - [ ] `Reward Text`: Ziehe `RewardText` hinein
  - [ ] `Close Result Button`: Ziehe `CloseResultButton` hinein
  - [ ] `Play Again Button`: Ziehe `PlayAgainButton` hinein

#### Schritt 4.4: Testen
- [ ] **Play** im Editor
- [ ] Öffne Mini-Game Panel → Buttons sollten funktionieren
- [ ] Teste Game Start → Energy sollte abnehmen
- [ ] Teste Result Panel → Sollte nach Game Completion erscheinen

---

## 🎯 PHASE 4.5: UI-Panel-Management Fix (KRITISCH!)

### ⚠️ Problem: Viele Panels übereinander, Buttons funktionieren nicht

**Ursache:** Kein zentrales Panel-Management, Panels überlappen sich, Raycast-Blockierung

### ✅ Lösung: CelestialUIPanelManager System

#### Schritt 4.5.1: PanelManager erstellen
- [ ] **Hierarchy** → Rechtsklick → **Create Empty** → Name: `CelestialUIPanelManager`
- [ ] **Inspector** → **Add Component** → `CelestialUIPanelManager` Script
- [ ] **Settings**:
  - **Auto Fix On Start**: ✅ Aktiviert
  - **Default Canvas Sort Order**: `0`
  - **Overlay Canvas Sort Order**: `100`

#### Schritt 4.5.2: Alle Panels deaktivieren (Initial State)
- [ ] **Hierarchy** → Finde `DailyLoginPanel` → **Active**: ❌ Deaktiviert
- [ ] **Hierarchy** → Finde `DailyQuestPanel` → **Active**: ❌ Deaktiviert
- [ ] **Hierarchy** → Finde `MiniGamePanel` → **Active**: ❌ Deaktiviert
- [ ] **Hierarchy** → Finde `OfflineRewardPanel` → **Active**: ❌ Deaktiviert
- [ ] **Hierarchy** → Finde `MergeResultPanel` → **Active**: ❌ Deaktiviert

**Oder automatisch:**
- [ ] **Unity Editor** → Menu: `CelestialMerge` → `UI` → `Fix All Panels Automatically`
- [ ] Klicke: **"📋 Deactivate All Modal Panels"**

#### Schritt 4.5.3: Automatisches Fixen (Empfohlen)
- [ ] **Unity Editor** → Menu: `CelestialMerge` → `UI` → `Fix All Panels Automatically`
- [ ] Klicke: **"🔧 Fix All Panels Now"**
- [ ] **Fertig!** Alle Panels sind jetzt gefixt

#### Schritt 4.5.4: Mini-Game Button in Haupt-UI (WICHTIG!)

**Problem:** Mini-Game Button funktioniert nicht, Panel öffnet sich nicht.

##### Mini-Game Button erstellen
- [ ] **Hierarchy** → Canvas → **UI → Button - TextMeshPro** → Name: `MiniGameButton`
- [ ] **RectTransform**: Anchor **Top-Right** (oder Top-Left)
  - **Pos X**: `-150` (wenn Top-Right)
  - **Pos Y**: `-100`
  - **Size**: `150×50`
- [ ] **Button Text**: `"🎮 Mini-Game"` oder `"Spiele"`
- [ ] **Font Size**: `22`, **Font Style**: Bold
- [ ] **Button Color**: Blau (#4A9EFF) oder Akzentfarbe

##### Script-Referenzen zuweisen
- [ ] **Hierarchy** → Wähle GameObject mit `CelestialUIManager` Script
- [ ] **Inspector** → `CelestialUIManager`:
  - [ ] **`Play Mini Game Button`**: Ziehe `MiniGameButton` hinein ⭐ **WICHTIG!**
  - [ ] **`Mini Game UI Panel`**: Ziehe GameObject mit `MiniGameUIPanel` Script hinein (falls vorhanden)

#### Schritt 4.5.5: Automatisches UI-Setup (App Store Ready) ⭐ **NEU!**

**Problem:** Mini-Game und Quest Buttons fehlen oder funktionieren nicht.

##### Option A: Editor-Tool (Empfohlen - 30 Sekunden)
- [ ] **Unity Editor** → Menu: `CelestialMerge` → `UI` → `Auto Setup Main UI (App Store Ready)`
- [ ] Klicke: **"🚀 Setup All UI Now"**
- [ ] **Fertig!** Alle Buttons werden automatisch erstellt und verbunden

##### Option B: Automatisch beim Start
- [ ] **Hierarchy** → Canvas → Rechtsklick → **Create Empty** → Name: `CelestialMainUIInitializer`
- [ ] **Inspector** → **Add Component** → `CelestialMainUIInitializer`
- [ ] **Auto Setup On Start**: ✅ Aktiviert
- [ ] **Play** → Buttons werden automatisch erstellt

##### Was wird erstellt?
- [ ] **Quest Button** (`📋 Quests`) - Top-Right, öffnet Daily Quest Panel
- [ ] **Mini-Game Button** (`🎮 Mini-Game`) - Top-Right, öffnet Mini-Game Panel
- [ ] **Daily Login Button** (`📅 Daily`) - Top-Right, öffnet Daily Login Panel (optional)
- [ ] Alle Buttons haben professionelles Design (Blau #4A9EFF, Hover States, etc.)

#### Schritt 4.5.6: Testen
- [ ] **Play** im Editor
- [ ] **Prüfe:** Keine Panels sollten beim Start sichtbar sein (außer Haupt-UI)
- [ ] **Prüfe:** Quest Button sollte **sichtbar** sein (Top-Right)
- [ ] **Prüfe:** Mini-Game Button sollte **sichtbar** sein (Top-Right)
- [ ] **Klicke** auf Quest Button → Daily Quest Panel sollte sich öffnen
- [ ] **Klicke** auf Mini-Game Button → Mini-Game Panel sollte sich öffnen
- [ ] **Teste Close Buttons** → Panels sollten sich schließen
- [ ] **Prüfe:** Nur ein Panel gleichzeitig sichtbar (keine Überlappung)
- [ ] **Prüfe:** Buttons funktionieren (können geklickt werden)

---

## 🎯 PHASE 5: Visual Polish (Optional, 60 Minuten)

### Merge Feedback (Particle Effects)

#### Schritt 5.1: Merge Particle System erstellen
- [ ] **Hierarchy** → **Effects → Particle System** → Name: `MergeParticles`
- [ ] **Inspector** → `Particle System`:
  - **Start Lifetime**: `0.5`
  - **Start Speed**: `5`
  - **Start Size**: `0.2`
  - **Start Color**: Gelb/Orange
  - **Emission → Rate over Time**: `50`
  - **Shape → Shape**: Circle, Radius: `0.5`

#### Schritt 5.2: Merge Feedback Script erstellen
- [ ] Erstelle Script `MergeFeedbackSystem.cs` (TODO: Falls gewünscht)
- [ ] Integriere in `CelestialMergeManager`

### Item Rarity Colors

#### Schritt 5.3: Item Display Script erweitern
- [ ] Finde Item Display Component (z.B. `CelestialItemDisplay.cs`)
- [ ] Füge Color-Logik hinzu:
  ```csharp
  Color GetRarityColor(ItemRarity rarity)
  {
      switch (rarity)
      {
          case ItemRarity.Common: return Color.gray;
          case ItemRarity.Uncommon: return Color.green;
          case ItemRarity.Rare: return Color.blue;
          case ItemRarity.Epic: return Color.magenta;
          case ItemRarity.Legendary: return Color.yellow;
          case ItemRarity.Mythic: return Color.red;
          default: return Color.white;
      }
  }
  ```

---

## 📝 FINALE CHECKLISTE

### Core Systems
- [x] Item System (125+ Items)
- [x] Currency System (Stardust, Crystals)
- [x] Merge System (2× und 3×)
- [x] Progression System (Level, XP, Chapters)
- [x] Board System (Expandable)
- [x] Story System (Chapters, Lore, Dialog)

### Audio System
- [x] AudioManager Script
- [ ] AudioManager GameObject erstellt
- [ ] Audio Clips zugewiesen (optional)
- [x] Integration in Merge/Level Up
- [x] Integration in SettingsMenu

### Daily System
- [x] DailySystemManager Script
- [x] DailyUIPanel Script
- [ ] Daily Login Panel UI erstellt
- [ ] Daily Quests Panel UI erstellt
- [ ] DailyUIPanel zugewiesen

### Idle System
- [x] IdleProductionManager Script
- [x] IdleUIPanel Script
- [ ] Idle Production Display UI erstellt
- [ ] Offline Reward Panel UI erstellt
- [ ] IdleUIPanel zugewiesen

### Mini-Game System
- [x] MiniGameManager Script
- [x] MiniGameUIPanel Script
- [ ] Mini-Game Main Panel UI erstellt
- [ ] Mini-Game Result Panel UI erstellt
- [ ] MiniGameUIPanel zugewiesen

### Visual Polish (Optional)
- [ ] Merge Particle Effects
- [ ] Item Rarity Colors
- [ ] Merge Animation (Scale/Pulse)

---

## 🚀 Nach Abschluss

1. **Teste alle Features**
   - [ ] Audio funktioniert
   - [ ] Daily System funktioniert
   - [ ] Idle Production funktioniert
   - [ ] Mini-Game UI funktioniert

2. **Build Testen**
   - [ ] Windows Build erstellen
   - [ ] Teste im Build

3. **Dokumentation**
   - [ ] Prüfe ob alle Features dokumentiert sind

---

## ⚠️ Wichtige Hinweise

- **Alle Scripts sind bereits implementiert** - Du musst nur noch die UI im Unity Editor erstellen
- **Auto-Find**: Alle Panels finden ihre Manager automatisch (falls nicht zugewiesen)
- **Testen**: Teste jeden Schritt einzeln, bevor du weitermachst
- **Prefabs**: Erstelle Prefabs für wiederverwendbare UI-Elemente
- **Layout-Stil**: Nutze den `MERGE_APP_UI_STYLE_GUIDE.md` für professionelles Design

---

**Viel Erfolg beim Umsetzen! 🎮✨**
