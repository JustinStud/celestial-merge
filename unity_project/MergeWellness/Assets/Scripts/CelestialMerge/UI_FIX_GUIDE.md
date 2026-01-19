# 🔧 UI-Fix Guide - Alle Probleme beheben

## 🔴 Problem 1: Stardust zeigt 5.0K und ändert sich nicht

### **Ursache:**
- Stardust Capacity ist bei **5000** (maxStardustCapacity)
- Wenn Stardust über Capacity geht, wird es auf Capacity gesetzt
- Capacity wird **nicht automatisch** mit Level erhöht

### **Lösung:**

**Option A: Capacity dynamisch erhöhen (Empfohlen)**

1. **Wähle `CurrencyManager` GameObject**
2. **Im Inspector:**
   - **Max Stardust Capacity:** Setze auf höheren Wert (z.B. `50000`)
   - Oder: **Unlimited Stardust:** ✅ aktivieren (für Testing)

**Option B: Capacity wird automatisch erhöht (bereits implementiert)**

- Capacity sollte sich automatisch mit Level erhöhen
- Prüfe ob `CelestialGameManager` die Events richtig verbindet

**Quick Fix für Testing:**
- Im Inspector: **Unlimited Stardust** = ✅
- Dann kann Stardust unbegrenzt steigen

---

## 🔴 Problem 2: Level zeigt 0 und ändert sich nicht

### **Ursache:**
- UI wird nicht beim Start aktualisiert
- Oder: `progressionManager` ist null
- Oder: UI-Elemente sind nicht richtig zugewiesen

### **Lösung:**

**Schritt 1: Prüfe Zuweisungen**

1. **Wähle `CelestialUIManager` GameObject**
2. **Im Inspector, prüfe:**
   - **Level Text:** Muss `LevelText` GameObject zugewiesen sein
   - **Progression Manager:** Wird automatisch gefunden (oder ziehe `CelestialProgressionManager`)

**Schritt 2: Prüfe Console**

Beim Start solltest du sehen:
```
🔍 UI Update: Level=1, XP=0/100
```

Falls nicht:
- `progressionManager` ist null
- Prüfe ob `CelestialGameManager` existiert

**Schritt 3: Manuell testen**

1. **Wähle `CelestialProgressionManager` GameObject**
2. **Im Inspector:** Setze **Player Level** auf **5**
3. **Play-Button drücken**
4. **Level Text sollte "Level 5" zeigen**

---

## 🔴 Problem 3: XP Progress Bar ist nicht richtig sichtbar

### **Ursache:**
- Progress Bar ist oben links, aber Position/Anker ist falsch
- Oder: Bar ist zu klein/unsichtbar

### **Lösung:**

**Schritt 1: Progress Bar richtig positionieren**

1. **Wähle `XPProgressBar`** (im ProgressionPanel)
2. **RectTransform:**
   - **Anchor:** Top-Left
   - **Pos X:** 10 (10 Pixel von links)
   - **Pos Y:** -50 (50 Pixel von oben)
   - **Width:** 280
   - **Height:** 20

**Schritt 2: Fill Area richtig einstellen**

1. **Wähle `Fill`** (Child von Fill Area)
2. **Image Component:**
   - **Color:** Blau (50, 150, 255) oder Gold (255, 215, 0)
   - **Image Type:** Filled (optional, für Animation)

**Schritt 3: Sichtbarkeit prüfen**

1. **Wähle `XPProgressBar`**
2. **Slider Component:**
   - **Value:** Setze auf **0.5** (für Test)
   - **Min Value:** 0
   - **Max Value:** 1
3. **Bar sollte jetzt halb gefüllt sein**

---

## ✅ Vollständige UI-Positionierung

### **ProgressionPanel Layout (Empfohlen):**

```
ProgressionPanel (Top-Left)
├── LevelText
│   Position: (10, -10) - Top-Left
│   Size: 280×30
├── ChapterText
│   Position: (10, -40) - Unter LevelText
│   Size: 280×25
├── XPProgressBar
│   Position: (10, -75) - Unter ChapterText
│   Size: 280×20
└── XPText
    Position: (10, -100) - Unter Progress Bar
    Size: 280×20
```

### **Currency UI Layout (Top-Right):**

```
Canvas (Top-Right)
├── StardustText
│   Position: (-10, -10) - Top-Right
│   Size: 150×30
└── CrystalsText
    Position: (-10, -40) - Unter Stardust
    Size: 150×30
```

---

## 🔍 Debug-Checkliste

### **Prüfe Console beim Start:**

Sollte zeigen:
```
🔍 UI Update: Level=1, XP=0/100
🔍 UI Update: Stardust=5000, Crystals=0
💰 Stardust UI aktualisiert: 5000 → 5.0K
📊 Level Text aktualisiert: 1
📊 XP Progress Bar aktualisiert: 0.00 (0/100)
```

**Falls nicht:**
- Prüfe ob `CelestialUIManager` existiert
- Prüfe ob UI-Elemente zugewiesen sind
- Prüfe ob `CelestialGameManager` existiert

### **Prüfe beim Mergen:**

Sollte zeigen:
```
✅ Merge erfolgreich: ... (+X XP)
💰 Stardust UI aktualisiert: 5100 → 5.1K
📊 XP Progress Bar aktualisiert: 0.05 (5/100)
```

**Falls nicht:**
- Events sind nicht subscribed
- Prüfe `SubscribeToEvents()` in `CelestialUIManager`

---

## 🎯 Quick Fixes

### **Fix 1: Stardust Capacity erhöhen**

1. **Wähle `CurrencyManager`**
2. **Max Stardust Capacity:** `50000` (statt 5000)
3. **Oder:** Unlimited Stardust = ✅

### **Fix 2: Level manuell setzen (für Test)**

1. **Wähle `CelestialProgressionManager`**
2. **Player Level:** Setze auf `5`
3. **Play-Button**
4. **Level sollte jetzt "Level 5" zeigen**

### **Fix 3: XP Progress Bar sichtbar machen**

1. **Wähle `XPProgressBar`**
2. **RectTransform:**
   - Anchor: **Top-Left**
   - Pos: **(10, -50)**
   - Size: **(280, 20)**
3. **Fill Color:** Blau oder Gold
4. **Value:** 0.5 (für Test)

---

## ✅ Finale Checkliste

- [ ] CurrencyManager: Unlimited Stardust = ✅ (oder Capacity erhöht)
- [ ] CelestialUIManager: Alle UI-Elemente zugewiesen
- [ ] LevelText zeigt richtiges Level
- [ ] XPProgressBar ist sichtbar und positioniert
- [ ] StardustText zeigt aktuellen Wert
- [ ] Console zeigt Debug-Logs beim Start
- [ ] Console zeigt Updates beim Mergen
- [ ] UI aktualisiert sich beim Mergen

---

**Viel Erfolg! 🚀**
