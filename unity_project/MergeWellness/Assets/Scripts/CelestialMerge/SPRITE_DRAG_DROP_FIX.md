# 🎨 Sprite Drag & Drop Fix - Sprites zu Items hinzufügen

## 🔴 Problem: Sprites können nicht in das "Item Sprite" Feld gezogen werden

**Ursache:** Die Textures sind nicht als "Sprite (2D and UI)" importiert!

---

## ✅ Lösung: Textures richtig importieren

### **Schritt 1: Texture Type prüfen und ändern**

1. **Wähle ein Bild** im Project-Fenster (z.B. `Stardust_Particle...`)
2. **Im Inspector** siehst du die **Import Settings**
3. **Texture Type:** Muss **`Sprite (2D and UI)`** sein!
   - Falls es **`Default`** oder **`Texture`** ist → ändere es!
4. **Klicke `Apply`** (unten rechts im Inspector)

**Wichtig:** 
- Jedes Bild muss einzeln geändert werden
- Oder: Wähle mehrere Bilder aus (Strg+Klick) und ändere alle auf einmal

---

### **Schritt 2: Sprite Mode prüfen**

1. **Wähle ein Bild** im Project-Fenster
2. **Im Inspector:**
   - **Sprite Mode:** Sollte **`Single`** sein (für einzelne Sprites)
   - Falls **`Multiple`** → ändere auf **`Single`**
3. **Klicke `Apply`**

---

### **Schritt 3: Sprite zu Item zuweisen**

**Jetzt sollte es funktionieren:**

1. **Wähle `CelestialItemDatabase` Asset** im Project-Fenster
2. **Im Inspector:** Erweitere die Items-Liste
3. **Für jedes Item:**
   - Finde das Feld **`Item Sprite`**
   - **Ziehe dein Sprite** aus dem Project-Fenster in das Feld
   - **Oder:** Klicke auf den **Kreis-Button** neben "None (Sprite)" → Wähle Sprite aus

**Tipp:** 
- Du kannst mehrere Sprites gleichzeitig zuweisen
- Wähle mehrere Items aus (Strg+Klick) und weise Sprites zu

---

## 🔍 Alternative Methoden

### **Methode 1: Object Picker verwenden**

1. **Klicke auf den Kreis-Button** neben "None (Sprite)"
2. **Object Picker öffnet sich**
3. **Suche nach deinem Sprite** (z.B. "Stardust")
4. **Wähle es aus**

---

### **Methode 2: Sprite per Name zuweisen (Editor Script)**

Falls Drag & Drop immer noch nicht funktioniert, kann ich ein Editor-Script erstellen, das Sprites automatisch per Name zuweist.

**Sag mir Bescheid, wenn du das brauchst!**

---

## 🚨 Häufige Probleme

### **Problem 1: "Texture Type kann nicht geändert werden"**

**Lösung:**
- Stelle sicher, dass das Bild nicht in einem **Read-Only** Ordner ist
- Prüfe ob Unity gerade kompiliert (warte bis fertig)

---

### **Problem 2: "Sprite wird nicht angezeigt"**

**Lösung:**
1. **Prüfe ob Sprite zugewiesen ist:**
   - `CelestialItemDatabase` → Item → `Item Sprite` sollte nicht "None" sein

2. **Prüfe Sprite-Import:**
   - Wähle Sprite im Project-Fenster
   - Inspector → **Max Size:** Sollte mindestens 256 sein
   - **Compression:** Kann "None" sein für bessere Qualität

3. **Force Reimport:**
   - Rechtsklick auf Sprite → **Reimport**

---

### **Problem 3: "Sprites werden im Spiel nicht angezeigt"**

**Lösung:**
1. **Prüfe ItemImage Component:**
   - Wähle einen Slot während Play-Mode
   - Prüfe ob `ItemImage` → `Sprite` gesetzt ist

2. **Prüfe ob Sprite richtig übergeben wird:**
   - Console sollte keine Fehler zeigen
   - Prüfe `CelestialItemDatabase.CreateItem()` → Sprite wird übergeben

---

## ✅ Quick Fix: Alle Textures auf einmal ändern

1. **Project-Fenster:** Wähle den **`Sprites`** Ordner
2. **Im Inspector:** Du siehst alle Bilder
3. **Wähle alle Bilder aus** (Strg+A oder Cmd+A)
4. **Im Inspector:**
   - **Texture Type:** `Sprite (2D and UI)`
   - **Sprite Mode:** `Single`
5. **Klicke `Apply`**

**Jetzt sollten alle Bilder als Sprites importiert sein!**

---

## 📋 Checkliste

- [ ] Textures sind als "Sprite (2D and UI)" importiert
- [ ] Sprite Mode ist "Single"
- [ ] `Apply` wurde geklickt
- [ ] Sprites können in "Item Sprite" Feld gezogen werden
- [ ] Sprites werden im Spiel angezeigt

---

**Viel Erfolg! 🎨**
