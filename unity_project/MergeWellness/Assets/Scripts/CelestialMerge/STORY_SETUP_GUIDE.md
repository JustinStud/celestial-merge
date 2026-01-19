# 📖 Story System Setup Guide

## Problem 1: "CelestialMerge" erscheint nicht im Create-Menü

**Lösung:**

1. **Warte auf Unity-Kompilierung**: Nach dem Beheben des Kompilierfehlers muss Unity neu kompilieren. Das kann 10-30 Sekunden dauern.

2. **Falls es immer noch nicht erscheint:**
   - **Methode A (Direkt):** Gehe zu `Assets` → `Create` → `ScriptableObject`
   - Wähle `StoryDatabase` aus der Liste
   - Benenne es `StoryDatabase` und speichere es in `Assets/Data/Story/`

3. **Alternative Methode:**
   - Öffne `StoryDatabase.cs` im Editor
   - Rechtsklick auf die Klasse → `Create` → `StoryDatabase`
   - Oder: Im Project-Fenster Rechtsklick → `Create` → `StoryDatabase`

4. **Nach Erstellung:**
   - Wähle das `StoryDatabase` Asset aus
   - Im Inspector: Rechtsklick auf den Script-Header → `Initialize Story Content`
   - Oder: Im Inspector oben rechts das Menü (⋮) → `Initialize Story Content`

---

## Problem 2: Benötigte UI-Elemente

### **Dialog Panel UI**

Erstelle ein neues GameObject in deiner Scene:

```
DialogPanel (GameObject)
├── CanvasGroup (Component)
├── Background (Image) - Dunkler Hintergrund mit Transparenz
├── NPCPortrait (Image) - 200x200 Pixel, zeigt NPC-Bild
├── NPCName (TextMeshProUGUI) - Name des NPCs
├── DialogText (TextMeshProUGUI) - Der Dialog-Text (mit Typewriter-Effekt)
└── ChoiceButtons (GameObject)
    ├── ChoiceButton1 (Button)
    │   └── Text (TextMeshProUGUI)
    ├── ChoiceButton2 (Button)
    │   └── Text (TextMeshProUGUI)
    └── ChoiceButton3 (Button) [Optional]
        └── Text (TextMeshProUGUI)
```

**Schritt-für-Schritt:**

1. **DialogPanel erstellen:**
   - Rechtsklick in Hierarchy → `UI` → `Panel`
   - Benenne es `DialogPanel`
   - Setze `RectTransform`: Anchor = Center, Width = 800, Height = 500

2. **CanvasGroup hinzufügen:**
   - Wähle `DialogPanel`
   - `Add Component` → `Canvas Group`
   - `Interactable` = true, `Blocks Raycasts` = true

3. **Background (optional, falls Panel nicht dunkel genug):**
   - Im Panel: `Image` Component
   - Color: Schwarz mit Alpha 200 (0.78)

4. **NPCPortrait:**
   - Rechtsklick auf `DialogPanel` → `UI` → `Image`
   - Benenne es `NPCPortrait`
   - Position: Links, Width = 200, Height = 200
   - `Image Type` = Simple

5. **NPCName:**
   - Rechtsklick auf `DialogPanel` → `UI` → `Text - TextMeshPro`
   - Benenne es `NPCName`
   - Position: Über NPCPortrait, Font Size = 24, Bold

6. **DialogText:**
   - Rechtsklick auf `DialogPanel` → `UI` → `Text - TextMeshPro`
   - Benenne es `DialogText`
   - Position: Rechts neben Portrait, Width = 500, Height = 300
   - Font Size = 18, Alignment = Top-Left, Word Wrap = true

7. **ChoiceButtons Container:**
   - Rechtsklick auf `DialogPanel` → `UI` → `Panel` (oder Empty GameObject)
   - Benenne es `ChoiceButtons`
   - Füge `Vertical Layout Group` hinzu (für automatische Anordnung)

8. **Choice Buttons:**
   - Für jeden Button: Rechtsklick auf `ChoiceButtons` → `UI` → `Button - TextMeshPro`
   - Benenne sie `ChoiceButton1`, `ChoiceButton2`, `ChoiceButton3`
   - Width = 400, Height = 50
   - Text: Font Size = 16

**Zuweisung im StoryUIManager:**
- Wähle das GameObject mit `StoryUIManager` Component
- Ziehe `DialogPanel` in `Dialog Panel`
- Ziehe `NPCPortrait` Image in `NPC Portrait Image`
- Ziehe `NPCName` Text in `NPC Name Text`
- Ziehe `DialogText` Text in `Dialog Text`
- Ziehe alle 3 `ChoiceButton` GameObjects in das `Choice Buttons` Array

---

### **Chapter Unlock Panel UI**

```
ChapterUnlockPanel (GameObject)
├── CanvasGroup (Component)
├── Background (Image) - Dunkler Hintergrund
├── ChapterImage (Image) - 400x300 Pixel, zeigt Chapter-Bild
├── ChapterTitle (TextMeshProUGUI) - "Chapter X: Title"
└── ChapterDescription (TextMeshProUGUI) - Beschreibung
```

**Schritt-für-Schritt:**

1. **ChapterUnlockPanel erstellen:**
   - Rechtsklick in Hierarchy → `UI` → `Panel`
   - Benenne es `ChapterUnlockPanel`
   - Setze `RectTransform`: Anchor = Center, Width = 900, Height = 600

2. **ChapterImage:**
   - Rechtsklick auf Panel → `UI` → `Image`
   - Position: Oben, Width = 400, Height = 300

3. **ChapterTitle:**
   - Rechtsklick auf Panel → `UI` → `Text - TextMeshPro`
   - Position: Unter Image, Font Size = 32, Bold, Center Alignment

4. **ChapterDescription:**
   - Rechtsklick auf Panel → `UI` → `Text - TextMeshPro`
   - Position: Unter Title, Width = 800, Font Size = 18, Center Alignment, Word Wrap = true

**Zuweisung:**
- Ziehe `ChapterUnlockPanel` in `Chapter Unlock Panel`
- Ziehe `ChapterImage` in `Chapter Image`
- Ziehe `ChapterTitle` in `Chapter Title Text`
- Ziehe `ChapterDescription` in `Chapter Description Text`

---

### **Lore Notification Panel UI**

```
LoreNotificationPanel (GameObject)
├── Background (Image) - Gold/Gelb mit Transparenz
└── LoreNotificationText (TextMeshProUGUI) - "📖 Lore freigeschaltet: Title"
```

**Schritt-für-Schritt:**

1. **LoreNotificationPanel erstellen:**
   - Rechtsklick in Hierarchy → `UI` → `Panel`
   - Benenne es `LoreNotificationPanel`
   - Setze `RectTransform`: Anchor = Top-Center, Width = 500, Height = 100
   - Position: Y = -50 (50 Pixel von oben)

2. **Background:**
   - Im Panel: `Image` Component
   - Color: Gold/Gelb (255, 215, 0) mit Alpha 230

3. **LoreNotificationText:**
   - Rechtsklick auf Panel → `UI` → `Text - TextMeshPro`
   - Position: Center, Font Size = 20, Bold, Center Alignment

**Zuweisung:**
- Ziehe `LoreNotificationPanel` in `Lore Notification Panel`
- Ziehe `LoreNotificationText` in `Lore Notification Text`

---

## Problem 3: Integration in Scene

### **Schritt 1: StoryManager hinzufügen**

1. **Erstelle GameObject:**
   - Rechtsklick in Hierarchy → `Create Empty`
   - Benenne es `StoryManager`
   - Position: (0, 0, 0)

2. **Füge Component hinzu:**
   - `Add Component` → `Story Manager` (Script)

3. **Zuweisungen:**
   - **Story Database:** Ziehe das `StoryDatabase` Asset (aus Project-Fenster) in das Feld
   - **Progression Manager:** Ziehe das GameObject mit `Celestial Progression Manager` Component
   - **Story UI:** Ziehe das GameObject mit `Story UI Manager` Component

---

### **Schritt 2: StoryUIManager hinzufügen**

1. **Erstelle GameObject:**
   - Rechtsklick in Hierarchy → `Create Empty`
   - Benenne es `StoryUIManager`
   - Position: (0, 0, 0)

2. **Füge Component hinzu:**
   - `Add Component` → `Story UI Manager` (Script)

3. **Zuweisungen (siehe oben für UI-Erstellung):**
   - Alle UI-Elemente zuweisen wie oben beschrieben

---

### **Schritt 3: Physics Manager hinzufügen (Optional)**

1. **Erstelle GameObject:**
   - Rechtsklick in Hierarchy → `Create Empty`
   - Benenne es `CelestialPhysicsManager`
   - Position: (0, 0, 0)

2. **Füge Component hinzu:**
   - `Add Component` → `Celestial Physics Manager` (Script)

3. **Zuweisungen:**
   - **Board Manager:** Ziehe das GameObject mit `Expandable Board Manager` Component

---

### **Schritt 4: CollisionFeedbackManager hinzufügen (Optional)**

1. **Erstelle GameObject:**
   - Rechtsklick in Hierarchy → `Create Empty`
   - Benenne es `CollisionFeedbackManager`
   - Position: (0, 0, 0)

2. **Füge Component hinzu:**
   - `Add Component` → `Collision Feedback Manager` (Script)

3. **Zuweisungen:**
   - **Audio Source:** Wird automatisch erstellt, oder ziehe eine vorhandene
   - **Main Camera:** Wird automatisch gefunden (Camera.main), oder ziehe manuell

---

## ✅ Checkliste

- [ ] Kompilierfehler behoben (Physics Manager)
- [ ] StoryDatabase Asset erstellt
- [ ] StoryDatabase initialisiert (Context Menu)
- [ ] DialogPanel UI erstellt und zugewiesen
- [ ] ChapterUnlockPanel UI erstellt und zugewiesen
- [ ] LoreNotificationPanel UI erstellt und zugewiesen
- [ ] StoryManager GameObject erstellt und konfiguriert
- [ ] StoryUIManager GameObject erstellt und konfiguriert
- [ ] Alle Referenzen im Inspector zugewiesen
- [ ] Unity kompiliert ohne Fehler
- [ ] Scene gespeichert

---

## 🎮 Testen

1. **Starte das Spiel**
2. **Level 1 erreichen** → Stella sollte erscheinen (Dialog)
3. **Level 5 erreichen** → Zweiter Dialog
4. **Level 10 erreichen** → Chapter 1 Completion
5. **Level 11 erreichen** → Chapter 2 Unlock Screen

---

## 🐛 Troubleshooting

**Problem:** Dialog erscheint nicht
- Prüfe ob `StoryManager` `StoryDatabase` zugewiesen hat
- Prüfe ob `StoryUIManager` alle UI-Elemente zugewiesen hat
- Prüfe Console für Fehler

**Problem:** Typewriter-Effekt funktioniert nicht
- Prüfe ob `DialogText` TextMeshProUGUI ist (nicht Legacy Text)
- Prüfe ob `Typewriter Speed` > 0 ist

**Problem:** Buttons funktionieren nicht
- Prüfe ob `EventSystem` in Scene vorhanden ist (Unity erstellt automatisch)
- Prüfe ob Buttons im `Choice Buttons` Array zugewiesen sind

---

**Viel Erfolg! 🚀**
