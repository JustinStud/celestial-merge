# 🎮 125 Items Setup Guide

## ✅ Was wurde gemacht:

1. **Datenbank erweitert** mit allen 125 Items aus `ItemStack_125Items.md`
2. **Sprite Auto-Assigner** erstellt für automatische Sprite-Zuweisung

---

## 🚀 Schritt 1: Items initialisieren

### **In Unity:**

1. **Wähle `CelestialItemDatabase` Asset** im Project-Fenster
2. **Im Inspector:** Rechtsklick auf das Script
3. **Context Menu:** `Initialize All 125 Items (Designer)`
4. **Console sollte zeigen:** `✅ CelestialItemDatabase initialisiert: 125 Items geladen`

**Wichtig:** Dies ersetzt alle bestehenden Items! Falls du bereits Items hast, die du behalten willst, mache vorher ein Backup.

---

## 🎨 Schritt 2: Sprites automatisch zuweisen

### **Option A: Automatisches Tool (Empfohlen)**

1. **Unity Menü:** `CelestialMerge` → `Tools` → `Auto-Assign Sprites`
2. **Im Fenster:**
   - **Celestial Item Database:** Ziehe `CelestialItemDatabase` Asset rein
   - **Sprite Search Path:** `Assets/Sprites` (sollte automatisch sein)
   - **Klicke:** `Auto-Assign Sprites by Name`
3. **Console zeigt:** Wie viele Sprites zugewiesen wurden
4. **Dialog:** Bestätigt die Anzahl der zugewiesenen Sprites

**Das Tool sucht Sprites basierend auf:**
- Item Name (z.B. "Stardust Particle" → "stardust_particle_l1_common")
- Level und Rarity (z.B. Level 1, Common → "l1_common")
- Fuzzy Matching (ähnliche Namen)

---

### **Option B: Manuell zuweisen**

1. **Wähle `CelestialItemDatabase` Asset**
2. **Im Inspector:** Erweitere die Items-Liste
3. **Für jedes Item:**
   - Finde das Feld **`Item Sprite`**
   - **Ziehe Sprite** aus `Assets/Sprites` in das Feld
   - **Oder:** Klicke auf den Kreis-Button → Wähle Sprite aus

**Tipp:** Die Sprite-Namen sollten dem Format entsprechen:
- `stardust_particle_l1_common.png`
- `cosmic_dust_cloud_l2_uncommon.png`
- etc.

---

## 📋 Item-Kategorien (125 Items)

### **Category 1: Celestial Bodies (25 items)**
- Items 1-25
- Level 1-25
- Beispiele: Stardust Particle, Proto-Star, Black Hole, Multiverse Nexus

### **Category 2: Structures (20 items)**
- Items 26-45
- Level 1-20
- Beispiele: Energy Beacon, Dyson Sphere Segment, Reality Engine

### **Category 3: Lifeforms (18 items)**
- Items 46-63
- Level 1-18
- Beispiele: Microbe Spore, Star Whale, Galactic Phoenix

### **Category 4: Artifacts (15 items)**
- Items 64-80
- Level 1-15
- Beispiele: Ancient Rune, Infinity Stone, Genesis Cradle

### **Category 5: Elements (17 items)**
- Items 81-97
- Level 1-5
- Beispiele: Fire Chain, Water Vortex, Solar Flare

### **Category 6: Decorations (15 items)**
- Items 98-115
- Level 1-15
- Beispiele: Nebula Cloud, Galaxy Spiral, Starfield

**UI Icons (116-125)** werden nicht als spielbare Items hinzugefügt.

---

## 🔍 Troubleshooting

### **Problem: "Initialize All 125 Items" funktioniert nicht**

**Lösung:**
1. Prüfe ob `CelestialItemDatabase` Asset ausgewählt ist
2. Prüfe Console auf Fehler
3. Stelle sicher dass das Script kompiliert wurde

---

### **Problem: Sprites werden nicht automatisch zugewiesen**

**Lösung:**
1. **Prüfe Sprite-Pfad:**
   - Sollte `Assets/Sprites` sein
   - Prüfe ob Sprites wirklich dort sind

2. **Prüfe Sprite-Namen:**
   - Sollten dem Format entsprechen: `{name}_l{level}_{rarity}.png`
   - Beispiel: `stardust_particle_l1_common.png`

3. **Manuell zuweisen:**
   - Falls automatisch nicht funktioniert, weise Sprites manuell zu

---

### **Problem: Items werden im Spiel nicht angezeigt**

**Lösung:**
1. **Prüfe ob Sprites zugewiesen sind:**
   - `CelestialItemDatabase` → Item → `Item Sprite` sollte nicht "None" sein

2. **Prüfe ItemImage Component:**
   - Wähle einen Slot während Play-Mode
   - Prüfe ob `ItemImage` → `Sprite` gesetzt ist

3. **Prüfe Console:**
   - Sollte keine Fehler zeigen
   - Prüfe ob Items richtig geladen werden

---

## ✅ Checkliste

- [ ] `CelestialItemDatabase` Asset ausgewählt
- [ ] `Initialize All 125 Items (Designer)` ausgeführt
- [ ] Console zeigt: `125 Items geladen`
- [ ] `Auto-Assign Sprites` ausgeführt (oder manuell zugewiesen)
- [ ] Sprites sind in `Item Sprite` Feldern zugewiesen
- [ ] Items werden im Spiel angezeigt

---

## 🎯 Nächste Schritte

Nach dem Setup:

1. **Teste Items im Spiel:**
   - Spawne Items
   - Prüfe ob Sprites angezeigt werden
   - Teste Merges

2. **Prüfe Merge-Chains:**
   - Stelle sicher dass alle Merge-Chains funktionieren
   - Teste verschiedene Level

3. **Visual Polish:**
   - Prüfe ob alle Sprites gut aussehen
   - Passe Größen/Positionen an falls nötig

---

**Viel Erfolg! 🚀**
