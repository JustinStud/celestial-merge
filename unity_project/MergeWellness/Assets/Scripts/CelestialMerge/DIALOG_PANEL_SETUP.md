# 🎨 DialogPanel Setup - Anchor & Größe konfigurieren

## Problem: Anchor-Bereich und Größen-Einstellungen nicht sichtbar

**Lösung:** Die Anchor-Einstellungen sind im **Rect Transform Component** versteckt. Hier ist, wie du sie findest und konfigurierst.

---

## 📍 Schritt 1: Rect Transform Component finden

1. **Wähle `DialogPanel`** in der Hierarchy
2. **Im Inspector** (rechts) siehst du das **Rect Transform Component**
3. **Klicke auf den Pfeil** links neben "Rect Transform" um es zu erweitern (falls es zusammengeklappt ist)

---

## 🎯 Schritt 2: Anchor-Modus wechseln

Unity hat **2 Modi** für Rect Transform:

### **Modus 1: Stretch-Modus** (aktuell aktiv)
- Zeigt: **Left, Top, Right, Bottom** (Stretch-Offsets)
- Anchors sind als **Min X/Y, Max X/Y** sichtbar
- **Problem:** Wenn alle Werte 0 sind, nimmt Panel die volle Bildschirmgröße ein

### **Modus 2: Position-Modus** (für zentriertes Panel)
- Zeigt: **Pos X, Pos Y, Width, Height**
- Anchors sind als visuelles Icon sichtbar

---

## ✅ Lösung: DialogPanel richtig konfigurieren

### **Option A: Zentriertes Panel (800×500 Pixel)**

1. **Wähle `DialogPanel`** im Inspector
2. **Im Rect Transform Component:**
   - Klicke auf das **Anchor-Icon** (oben links im Rect Transform)
   - Wähle **"Middle Center"** (oder Alt + Shift + Klick auf Center-Anchor)

3. **Jetzt siehst du:**
   - **Pos X:** 0
   - **Pos Y:** 0
   - **Width:** (sollte sichtbar sein)
   - **Height:** (sollte sichtbar sein)

4. **Setze die Größe:**
   - **Width:** `800`
   - **Height:** `500`
   - **Pos X:** `0` (zentriert horizontal)
   - **Pos Y:** `0` (zentriert vertikal)

---

### **Option B: Manuell über Anchor-Werte**

Falls du die Anchor-Werte direkt setzen willst:

1. **Im Rect Transform Component:**
   - Scrolle nach unten zu **"Anchors"**
   - **Min X:** `0.5` (Center)
   - **Min Y:** `0.5` (Center)
   - **Max X:** `0.5` (Center)
   - **Max Y:** `0.5` (Center)

2. **Jetzt erscheinen Width/Height Felder:**
   - **Width:** `800`
   - **Height:** `500`

---

## 🎨 Visuelle Anchor-Ansicht aktivieren

Um die Anchors **visuell im Scene View** zu sehen:

1. **Scene View öffnen** (falls nicht sichtbar: Window → General → Scene)
2. **Wähle `DialogPanel`** in der Hierarchy
3. **Im Scene View** siehst du jetzt:
   - **4 kleine Dreiecke** (die Anchors)
   - **Grüne Linien** (die Verbindungen)
   - **Blaues Rechteck** (das Panel)

4. **Anchors visuell verschieben:**
   - Klicke und ziehe die **4 kleinen Dreiecke** im Scene View
   - Oder: Rechtsklick auf Anchor-Icon → Anchor-Preset wählen

---

## 🔧 Schnell-Fix: Anchor-Presets verwenden

**Die einfachste Methode:**

1. **Wähle `DialogPanel`**
2. **Im Rect Transform Component:**
   - Klicke auf das **Anchor-Icon** (oben links, zeigt aktuell "Stretch")
   - Ein **Pop-up-Menü** erscheint mit Presets

3. **Wähle:**
   - **"Middle Center"** (für zentriertes Panel)
   - Oder **"Top Center"** (für Panel oben)
   - Oder **"Bottom Center"** (für Panel unten)

4. **Nach Auswahl:**
   - **Width/Height** Felder erscheinen automatisch
   - Setze: **Width = 800, Height = 500**

---

## 📐 Detaillierte Konfiguration für DialogPanel

### **Zentriertes DialogPanel (Empfohlen):**

```
Rect Transform:
├── Anchor Preset: Middle Center
├── Pos X: 0
├── Pos Y: 0
├── Width: 800
├── Height: 500
├── Pivot: (0.5, 0.5)
└── Anchors:
    ├── Min X: 0.5
    ├── Min Y: 0.5
    ├── Max X: 0.5
    └── Max Y: 0.5
```

**So erreichst du das:**

1. **Anchor Preset:** Klicke auf Anchor-Icon → "Middle Center"
2. **Width:** `800`
3. **Height:** `500`
4. **Pos X:** `0`
5. **Pos Y:** `0`

---

## 🐛 Troubleshooting

### **Problem 1: Anchor-Icon nicht sichtbar**

**Lösung:**
- Stelle sicher, dass **Rect Transform Component erweitert** ist
- Das Anchor-Icon ist **oben links** im Rect Transform (kleines Quadrat mit 4 Linien)

### **Problem 2: Width/Height Felder fehlen**

**Lösung:**
- Anchors müssen **nicht gestreckt** sein (Min X ≠ Max X oder Min Y ≠ Max Y)
- Setze **Min X = Max X = 0.5** und **Min Y = Max Y = 0.5**
- Dann erscheinen Width/Height automatisch

### **Problem 3: Panel ist zu groß/klein**

**Lösung:**
- Wenn Panel den ganzen Bildschirm einnimmt:
  - Anchors sind auf **Stretch** (Min=0, Max=1)
  - Wechsle zu **Center Anchor** (siehe oben)

### **Problem 4: Panel ist nicht zentriert**

**Lösung:**
- Setze **Pos X = 0** und **Pos Y = 0**
- Stelle sicher, dass **Pivot = (0.5, 0.5)** ist
- Anchors sollten **Center** sein (0.5, 0.5)

---

## ✅ Finale Checkliste

- [ ] DialogPanel in Hierarchy ausgewählt
- [ ] Rect Transform Component erweitert
- [ ] Anchor Preset auf "Middle Center" gesetzt
- [ ] Width = 800
- [ ] Height = 500
- [ ] Pos X = 0
- [ ] Pos Y = 0
- [ ] Panel ist im Game View zentriert sichtbar

---

## 🎮 Testen

1. **Game View öffnen** (falls nicht sichtbar: Window → General → Game)
2. **Play-Button drücken** (optional, für Live-Vorschau)
3. **Panel sollte zentriert sein** (800×500 Pixel)

---

**Viel Erfolg! 🚀**
