# 📊 XP/Level System - UI Setup Guide

## ✅ Status: System ist implementiert!

Das **XP/Level System** ist bereits vollständig implementiert:
- ✅ `CelestialProgressionManager` verwaltet Level und XP
- ✅ XP wird beim Mergen vergeben (`CelestialMergeManager`)
- ✅ Level-Ups funktionieren automatisch
- ✅ `RegisterMerge()` wird jetzt aufgerufen (für Milestones)

**Problem:** Die UI-Elemente müssen erstellt und zugewiesen werden!

---

## 🎨 UI-Elemente erstellen

### **Schritt 1: Level/XP UI Panel erstellen**

```
Hierarchy → Canvas → Rechtsklick → UI → Panel
Name: "ProgressionPanel"
```

**RectTransform:**
- Anchor: **Top-Left**
- Pos X: **10** (10 Pixel von links)
- Pos Y: **-10** (10 Pixel von oben)
- Width: **300**
- Height: **150**

**Image Component:**
- Color: **Dunkelblau (20, 30, 60, 200)** mit Transparenz

---

### **Schritt 2: Level Text erstellen**

```
ProgressionPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "LevelText"
```

**RectTransform:**
- Anchor: **Top-Left**
- Pos X: **10**
- Pos Y: **-10**
- Width: **280**
- Height: **30**

**TextMeshProUGUI:**
- Text: **"Level 1"** (Placeholder)
- Font Size: **24**
- Font Style: **Bold**
- Alignment: **Left**
- Color: **Gold (255, 215, 0)**

---

### **Schritt 3: Chapter Text erstellen**

```
ProgressionPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "ChapterText"
```

**RectTransform:**
- Anchor: **Top-Left**
- Pos X: **10**
- Pos Y: **-40**
- Width: **280**
- Height: **25**

**TextMeshProUGUI:**
- Text: **"Chapter 1"** (Placeholder)
- Font Size: **18**
- Alignment: **Left**
- Color: **Weiß (255, 255, 255)**

---

### **Schritt 4: XP Progress Bar erstellen**

```
ProgressionPanel → Rechtsklick → UI → Slider
Name: "XPProgressBar"
```

**RectTransform:**
- Anchor: **Top-Left**
- Pos X: **10**
- Pos Y: **-75**
- Width: **280**
- Height: **20**

**Slider Component:**
- Min Value: **0**
- Max Value: **1**
- Value: **0.5** (Placeholder)
- Whole Numbers: ❌

**Fill Area (Child):**
- Hintergrundfarbe: **Dunkelgrau (50, 50, 50)**

**Fill (Child of Fill Area):**
- Hintergrundfarbe: **Blau (50, 150, 255)** oder **Gold (255, 215, 0)**

---

### **Schritt 5: XP Text erstellen**

```
ProgressionPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "XPText"
```

**RectTransform:**
- Anchor: **Top-Left**
- Pos X: **10**
- Pos Y: **-100**
- Width: **280**
- Height: **20**

**TextMeshProUGUI:**
- Text: **"0 / 100 XP"** (Placeholder)
- Font Size: **14**
- Alignment: **Left**
- Color: **Weiß (255, 255, 255)**

---

## 🔗 Zuweisung im CelestialUIManager

### **WICHTIG: CelestialUIManager erstellen (falls nicht vorhanden)**

**CelestialUIManager ist NICHT dasselbe wie StoryUIManager!**

- **StoryUIManager:** Für Story-Dialoge (bereits erstellt)
- **CelestialUIManager:** Für Level/XP UI (muss erstellt werden)

**CelestialUIManager erstellen:**
1. **Hierarchy → Rechtsklick → Create Empty**
2. **Name:** `CelestialUIManager`
3. **Add Component → Celestial UI Manager** (Script)
4. **Game Manager:** Ziehe `CelestialGameManager` GameObject hinein

**Detaillierte Anleitung:** Siehe `UI_MANAGER_EXPLANATION.md`

---

### **Zuweisung:**

1. **Wähle `CelestialUIManager` GameObject** in der Hierarchy

2. **Im Inspector, ziehe die UI-Elemente:**

   **Progression UI:**
   - `LevelText` → **Level Text**
   - `ChapterText` → **Chapter Text**
   - `XPProgressBar` → **XP Progress Bar**
   - `XPText` → **XP Text**

3. **Testen:**
   - Play-Button drücken
   - Items mergen
   - XP sollte steigen, Progress Bar sollte sich füllen
   - Bei Level-Up sollte Level Text sich aktualisieren

---

## ✅ Was sollte funktionieren

Nach dem Setup:

1. **XP wird vergeben:**
   - Beim Mergen: `+X XP` in Console
   - Progress Bar füllt sich

2. **Level-Up:**
   - Console zeigt: `🎉 Level Up! Jetzt Level X`
   - Level Text aktualisiert sich
   - XP Progress Bar resetet

3. **Chapter-Unlock:**
   - Bei Level 11, 26, 46, etc.
   - Chapter Text aktualisiert sich

---

## 🐛 Troubleshooting

### **Problem: XP wird nicht angezeigt**

**Lösung:**
- Prüfe ob `CelestialUIManager` alle UI-Elemente zugewiesen hat
- Prüfe Console: Sollte zeigen `+X XP` beim Mergen
- Prüfe ob `CelestialProgressionManager` existiert

### **Problem: Progress Bar füllt sich nicht**

**Lösung:**
- Prüfe ob `XPProgressBar` zugewiesen ist
- Prüfe ob `UpdateProgressionUI()` aufgerufen wird
- Prüfe Console für Fehler

### **Problem: Level-Up funktioniert nicht**

**Lösung:**
- Prüfe ob genug XP gesammelt wurde (siehe Console)
- Prüfe ob `OnLevelUp` Event subscribed ist
- Prüfe Console: Sollte zeigen `🎉 Level Up!`

---

## 📋 Quick Reference: XP-Werte

**Standard XP pro Merge:**
- Level 1 Items: **1-2 XP**
- Level 2 Items: **2-4 XP**
- Level 3 Items: **5-10 XP**
- Level 4+ Items: **10-20+ XP**

**XP für Level-Up:**
- Level 1→2: **100 XP**
- Level 2→3: **110 XP** (exponentiell)
- Level 10→11: **~259 XP**
- Level 50→51: **~11,739 XP**

**Formel:** `100 * (1.1 ^ (level - 1))`

---

## ✅ Finale Checkliste

- [ ] ProgressionPanel erstellt
- [ ] LevelText erstellt und zugewiesen
- [ ] ChapterText erstellt und zugewiesen
- [ ] XPProgressBar erstellt und zugewiesen
- [ ] XPText erstellt und zugewiesen
- [ ] Alle UI-Elemente im CelestialUIManager zugewiesen
- [ ] XP wird beim Mergen vergeben (Console prüfen)
- [ ] Progress Bar füllt sich
- [ ] Level-Up funktioniert

---

**Viel Erfolg! 🚀**
