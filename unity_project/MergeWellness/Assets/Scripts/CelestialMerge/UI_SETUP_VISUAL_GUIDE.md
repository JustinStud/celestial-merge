# 🎨 UI Setup - Visuelle Anleitung

## Übersicht: Benötigte UI-Elemente

```
Canvas (bereits vorhanden)
├── DialogPanel
│   ├── Background (Image)
│   ├── NPCPortrait (Image)
│   ├── NPCName (TextMeshPro)
│   ├── DialogText (TextMeshPro)
│   └── ChoiceButtons (Panel)
│       ├── ChoiceButton1 (Button)
│       ├── ChoiceButton2 (Button)
│       └── ChoiceButton3 (Button)
├── ChapterUnlockPanel
│   ├── Background (Image)
│   ├── ChapterImage (Image)
│   ├── ChapterTitle (TextMeshPro)
│   └── ChapterDescription (TextMeshPro)
└── LoreNotificationPanel
    ├── Background (Image)
    └── LoreNotificationText (TextMeshPro)
```

---

## 📋 Detaillierte UI-Erstellung

### **1. DialogPanel - Schritt für Schritt**

#### **Schritt 1: Panel erstellen**
```
Hierarchy → Rechtsklick → UI → Panel
Name: "DialogPanel"
```

**RectTransform Einstellungen:**
- Anchor: **Center** (Alt + Shift + Klick auf Center-Anchor)
- Width: **800**
- Height: **500**
- Pos X: **0**
- Pos Y: **0**

**Image Component:**
- Color: **Schwarz (0, 0, 0, 200)** → Alpha = 200 für Transparenz

**CanvasGroup Component hinzufügen:**
- `Add Component` → `Canvas Group`
- Interactable: ✅
- Blocks Raycasts: ✅
- Alpha: **1**

---

#### **Schritt 2: NPCPortrait (Image)**
```
DialogPanel → Rechtsklick → UI → Image
Name: "NPCPortrait"
```

**RectTransform:**
- Anchor: **Left** (Alt + Klick auf Left-Anchor)
- Pos X: **-300** (300 Pixel von links)
- Pos Y: **0** (zentriert vertikal)
- Width: **200**
- Height: **200**

**Image Component:**
- Image Type: **Simple**
- Preserve Aspect: ✅
- Raycast Target: ✅

---

#### **Schritt 3: NPCName (Text)**
```
DialogPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "NPCName"
```

**RectTransform:**
- Anchor: **Top-Left**
- Pos X: **-300** (unter Portrait)
- Pos Y: **120** (20 Pixel unter Portrait)
- Width: **200**
- Height: **30**

**TextMeshProUGUI Component:**
- Text: **"Stella"** (Placeholder)
- Font Size: **24**
- Font Style: **Bold**
- Alignment: **Center**
- Color: **Weiß (255, 255, 255)**

---

#### **Schritt 4: DialogText (Text)**
```
DialogPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "DialogText"
```

**RectTransform:**
- Anchor: **Left**
- Pos X: **-50** (rechts neben Portrait)
- Pos Y: **50** (etwas nach oben)
- Width: **500**
- Height: **300**

**TextMeshProUGUI Component:**
- Text: **"Dialog Text hier..."** (Placeholder)
- Font Size: **18**
- Font Style: **Normal**
- Alignment: **Top-Left**
- Word Wrapping: ✅
- Color: **Weiß (255, 255, 255)**

---

#### **Schritt 5: ChoiceButtons Container**
```
DialogPanel → Rechtsklick → UI → Panel
Name: "ChoiceButtons"
```

**RectTransform:**
- Anchor: **Bottom**
- Pos X: **0**
- Pos Y: **-200** (200 Pixel über unterem Rand)
- Width: **700**
- Height: **200**

**Vertical Layout Group Component hinzufügen:**
- `Add Component` → `Vertical Layout Group`
- Spacing: **10**
- Child Alignment: **Middle Center**
- Child Force Expand: ✅ Width, ❌ Height

---

#### **Schritt 6: Choice Buttons erstellen**

**Für jeden Button (3x):**

```
ChoiceButtons → Rechtsklick → UI → Button - TextMeshPro
Name: "ChoiceButton1", "ChoiceButton2", "ChoiceButton3"
```

**RectTransform (für jeden Button):**
- Width: **400**
- Height: **50**

**Button Component:**
- Interactable: ✅
- Transition: **Color Tint**
- Normal Color: **Grau (128, 128, 128)**
- Highlighted Color: **Hellgrau (200, 200, 200)**
- Pressed Color: **Dunkelgrau (100, 100, 100)**

**TextMeshProUGUI (im Button):**
- Text: **"Choice Text"** (Placeholder)
- Font Size: **16**
- Alignment: **Center**
- Color: **Weiß (255, 255, 255)**

---

### **2. ChapterUnlockPanel - Schritt für Schritt**

#### **Schritt 1: Panel erstellen**
```
Hierarchy → Rechtsklick → UI → Panel
Name: "ChapterUnlockPanel"
```

**RectTransform:**
- Anchor: **Center**
- Width: **900**
- Height: **600**
- Pos X: **0**
- Pos Y: **0**

**Image Component:**
- Color: **Dunkelblau (20, 30, 60, 240)**

**CanvasGroup Component:**
- Alpha: **1**

---

#### **Schritt 2: ChapterImage (Image)**
```
ChapterUnlockPanel → Rechtsklick → UI → Image
Name: "ChapterImage"
```

**RectTransform:**
- Anchor: **Top**
- Pos X: **0**
- Pos Y: **-50** (50 Pixel von oben)
- Width: **400**
- Height: **300**

**Image Component:**
- Image Type: **Simple**
- Preserve Aspect: ✅

---

#### **Schritt 3: ChapterTitle (Text)**
```
ChapterUnlockPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "ChapterTitle"
```

**RectTransform:**
- Anchor: **Top**
- Pos X: **0**
- Pos Y: **-380** (unter Image)
- Width: **800**
- Height: **50**

**TextMeshProUGUI:**
- Text: **"Chapter 1: Genesis"** (Placeholder)
- Font Size: **32**
- Font Style: **Bold**
- Alignment: **Center**
- Color: **Gold (255, 215, 0)**

---

#### **Schritt 4: ChapterDescription (Text)**
```
ChapterUnlockPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "ChapterDescription"
```

**RectTransform:**
- Anchor: **Top**
- Pos X: **0**
- Pos Y: **-450** (unter Title)
- Width: **800**
- Height: **100**

**TextMeshProUGUI:**
- Text: **"Chapter Description hier..."** (Placeholder)
- Font Size: **18**
- Alignment: **Center**
- Word Wrapping: ✅
- Color: **Weiß (255, 255, 255)**

---

### **3. LoreNotificationPanel - Schritt für Schritt**

#### **Schritt 1: Panel erstellen**
```
Hierarchy → Rechtsklick → UI → Panel
Name: "LoreNotificationPanel"
```

**RectTransform:**
- Anchor: **Top-Center**
- Pos X: **0**
- Pos Y: **-50** (50 Pixel von oben)
- Width: **500**
- Height: **100**

**Image Component:**
- Color: **Gold (255, 215, 0, 230)** → Alpha = 230

---

#### **Schritt 2: LoreNotificationText (Text)**
```
LoreNotificationPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "LoreNotificationText"
```

**RectTransform:**
- Anchor: **Center**
- Width: **480**
- Height: **80**

**TextMeshProUGUI:**
- Text: **"📖 Lore freigeschaltet: Title"** (Placeholder)
- Font Size: **20**
- Font Style: **Bold**
- Alignment: **Center**
- Color: **Schwarz (0, 0, 0)**

---

## 🔗 Zuweisung im StoryUIManager

1. **Wähle das GameObject mit `StoryUIManager` Component**

2. **Im Inspector, ziehe die UI-Elemente:**

   **Dialog UI:**
   - `DialogPanel` → **Dialog Panel**
   - `NPCPortrait` (Image) → **NPC Portrait Image**
   - `NPCName` (Text) → **NPC Name Text**
   - `DialogText` (Text) → **Dialog Text**
   - `ChoiceButton1` → **Choice Buttons [0]**
   - `ChoiceButton2` → **Choice Buttons [1]**
   - `ChoiceButton3` → **Choice Buttons [2]**

   **Chapter Unlock UI:**
   - `ChapterUnlockPanel` → **Chapter Unlock Panel**
   - `ChapterImage` (Image) → **Chapter Image**
   - `ChapterTitle` (Text) → **Chapter Title Text**
   - `ChapterDescription` (Text) → **Chapter Description Text**

   **Lore Notification UI:**
   - `LoreNotificationPanel` → **Lore Notification Panel**
   - `LoreNotificationText` (Text) → **Lore Notification Text**

3. **Typewriter Speed einstellen:**
   - **Typewriter Speed:** `0.05` (Sekunden pro Zeichen)

---

## ✅ Finale Checkliste

- [ ] DialogPanel erstellt und konfiguriert
- [ ] ChapterUnlockPanel erstellt und konfiguriert
- [ ] LoreNotificationPanel erstellt und konfiguriert
- [ ] Alle UI-Elemente im StoryUIManager zugewiesen
- [ ] EventSystem vorhanden (Unity erstellt automatisch)
- [ ] Canvas vorhanden (Unity erstellt automatisch)
- [ ] Alle Panels initial als **inaktiv** (SetActive = false) - wird automatisch von StoryUIManager gehandhabt

---

## 🎯 Quick Reference: RectTransform Shortcuts

- **Center Anchor:** Alt + Shift + Klick auf Center-Anchor
- **Left Anchor:** Alt + Klick auf Left-Anchor
- **Top Anchor:** Alt + Klick auf Top-Anchor
- **Bottom Anchor:** Alt + Klick auf Bottom-Anchor
- **Stretch (Full Screen):** Alt + Shift + Klick auf Stretch-Anchor

---

**Viel Erfolg beim Setup! 🚀**
