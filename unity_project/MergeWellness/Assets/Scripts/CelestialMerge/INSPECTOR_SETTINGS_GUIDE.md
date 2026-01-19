# 🔍 Inspector-Einstellungen finden - Schritt für Schritt

## Frage 1: Image Component Einstellungen für NPCPortrait

### **Wo finde ich die Image Component?**

1. **Wähle `NPCPortrait`** in der Hierarchy
   - Falls nicht vorhanden: `DialogPanel` → Rechtsklick → `UI` → `Image` → Benenne es `NPCPortrait`

2. **Im Inspector** (rechts) siehst du mehrere Components:
   - `Rect Transform` (immer vorhanden bei UI)
   - `Canvas Renderer` (automatisch)
   - **`Image`** ← **DAS IST ES!**

3. **Klicke auf `Image` Component** um es zu erweitern (falls zusammengeklappt)

4. **Jetzt siehst du alle Einstellungen:**
   ```
   Image Component:
   ├── Source Image: [None (Sprite)]
   ├── Color: [Weiß]
   ├── Material: [None]
   ├── Raycast Target: ☑️
   ├── Maskable: ☑️
   ├── Image Type: [Simple] ← HIER!
   ├── Preserve Aspect: ☑️ ← HIER!
   └── Set Native Size: [Button]
   ```

### **Die spezifischen Einstellungen:**

- **Image Type:**
  - Dropdown-Menü (Standard: "Simple")
  - Wähle: **"Simple"**

- **Preserve Aspect:**
  - Checkbox direkt unter "Image Type"
  - Aktiviere: **☑️** (Häkchen setzen)

- **Raycast Target:**
  - Checkbox (sollte bereits aktiviert sein)
  - Für UI-Interaktionen wichtig

---

## Frage 2: Image Component nicht sichtbar bei ChapterImage

### **Problem:** Image Component fehlt oder ist nicht sichtbar

### **Lösung A: Image Component hinzufügen**

1. **Wähle `ChapterImage`** in der Hierarchy
2. **Im Inspector:**
   - Klicke auf **"Add Component"** (unten im Inspector)
   - Suche nach **"Image"**
   - Klicke auf **"Image"** → Component wird hinzugefügt

### **Lösung B: Image Component ist versteckt**

1. **Wähle `ChapterImage`** in der Hierarchy
2. **Im Inspector:**
   - Scrolle nach unten
   - Suche nach **"Image"** Component
   - Falls es einen **Pfeil** (▶) hat: **Klicke darauf** um es zu erweitern

### **Lösung C: Falsches GameObject ausgewählt**

- Stelle sicher, dass du **`ChapterImage`** (nicht `ChapterUnlockPanel`) ausgewählt hast
- `ChapterImage` ist ein **Child** von `ChapterUnlockPanel`

---

## Frage 3: StoryUIManager GameObject finden/erstellen

### **Problem:** Welches GameObject hat die StoryUIManager Component?

### **Lösung A: Prüfen ob bereits vorhanden**

1. **In der Hierarchy:**
   - Suche nach einem GameObject namens **"StoryUIManager"** oder **"StoryUI"**
   - Falls vorhanden: Wähle es aus
   - Im Inspector sollte **"Story UI Manager"** Component sichtbar sein

2. **Falls nicht gefunden:**
   - Siehe "Lösung B" (neu erstellen)

### **Lösung B: StoryUIManager GameObject erstellen**

**Schritt-für-Schritt:**

1. **Erstelle leeres GameObject:**
   ```
   Hierarchy → Rechtsklick → Create Empty
   Name: "StoryUIManager"
   ```

2. **Füge StoryUIManager Component hinzu:**
   - Wähle `StoryUIManager` GameObject
   - Im Inspector: **"Add Component"**
   - Suche nach: **"Story UI Manager"** (oder "StoryUIManager")
   - Klicke darauf → Component wird hinzugefügt

3. **Jetzt siehst du im Inspector:**
   ```
   Story UI Manager (Script)
   ├── Dialog Panel: [None (GameObject)]
   ├── NPC Portrait Image: [None (Image)]
   ├── NPC Name Text: [None (TextMeshProUGUI)]
   ├── Dialog Text: [None (TextMeshProUGUI)]
   ├── Choice Buttons: [Size: 0]
   ├── Dialog Canvas Group: [None (CanvasGroup)]
   ├── Chapter Unlock Panel: [None (GameObject)]
   ├── Chapter Image: [None (Image)]
   ├── Chapter Title Text: [None (TextMeshProUGUI)]
   ├── Chapter Description Text: [None (TextMeshProUGUI)]
   ├── Lore Notification Panel: [None (GameObject)]
   ├── Lore Notification Text: [None (TextMeshProUGUI)]
   └── Typewriter Speed: 0.05
   ```

4. **Zuweisungen:**
   - Ziehe `DialogPanel` in **"Dialog Panel"**
   - Ziehe `NPCPortrait` (Image) in **"NPC Portrait Image"**
   - Ziehe `NPCName` (Text) in **"NPC Name Text"**
   - Ziehe `DialogText` (Text) in **"Dialog Text"**
   - Ziehe `ChoiceButton1`, `ChoiceButton2`, `ChoiceButton3` in **"Choice Buttons"** Array
   - etc.

---

## 📋 Quick Reference: Component-Felder im Inspector

### **Image Component Felder:**

| Feld | Wo? | Was? |
|------|-----|------|
| **Source Image** | Oben im Image Component | Sprite-Bild (optional) |
| **Color** | Unter Source Image | Farbe (Standard: Weiß) |
| **Image Type** | Dropdown-Menü | "Simple", "Sliced", "Tiled", "Filled" |
| **Preserve Aspect** | Checkbox unter Image Type | Verhältnis beibehalten |
| **Raycast Target** | Checkbox | Für UI-Interaktionen |
| **Maskable** | Checkbox | Für Masking |

### **StoryUIManager Component Felder:**

| Feld | Typ | Was reinziehen? |
|------|-----|-----------------|
| **Dialog Panel** | GameObject | `DialogPanel` GameObject |
| **NPC Portrait Image** | Image | `NPCPortrait` Image Component |
| **NPC Name Text** | TextMeshProUGUI | `NPCName` Text Component |
| **Dialog Text** | TextMeshProUGUI | `DialogText` Text Component |
| **Choice Buttons** | Button[] | `ChoiceButton1`, `ChoiceButton2`, `ChoiceButton3` |
| **Chapter Unlock Panel** | GameObject | `ChapterUnlockPanel` GameObject |
| **Chapter Image** | Image | `ChapterImage` Image Component |
| **Lore Notification Panel** | GameObject | `LoreNotificationPanel` GameObject |

---

## 🐛 Troubleshooting

### **Problem: Image Component fehlt komplett**

**Lösung:**
1. Wähle GameObject
2. `Add Component` → Suche "Image" → Hinzufügen

### **Problem: Image Type Dropdown ist leer**

**Lösung:**
- Stelle sicher, dass du die **Image Component** (nicht Rect Transform) ausgewählt hast
- Falls immer noch leer: Unity neu starten

### **Problem: StoryUIManager Script nicht gefunden**

**Lösung:**
1. Prüfe ob `StoryUIManager.cs` in `Assets/Scripts/CelestialMerge/Story/` existiert
2. Falls nicht: Warte auf Unity-Kompilierung (10-30 Sekunden)
3. Falls immer noch nicht: Prüfe Console auf Kompilierfehler

### **Problem: Zuweisungen funktionieren nicht**

**Lösung:**
- Stelle sicher, dass du das **richtige GameObject** auswählst
- `NPCPortrait` muss ein **Image Component** haben (nicht nur Rect Transform)
- `NPCName` muss ein **TextMeshProUGUI** Component haben

---

## ✅ Checkliste

- [ ] NPCPortrait hat Image Component
- [ ] Image Type = "Simple"
- [ ] Preserve Aspect = ✅
- [ ] ChapterImage hat Image Component
- [ ] StoryUIManager GameObject erstellt
- [ ] StoryUIManager Component hinzugefügt
- [ ] Alle UI-Elemente im StoryUIManager zugewiesen

---

**Viel Erfolg! 🚀**
