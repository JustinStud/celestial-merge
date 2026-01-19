# 🎯 UI-Komponenten Prioritäten - Was ist wichtig?

## ✅ Status: Progression UI ist bereits zugewiesen!

Du hast bereits:
- ✅ **Level Text** → zugewiesen
- ✅ **Chapter Text** → zugewiesen
- ✅ **XP Progress Bar** → zugewiesen
- ✅ **XP Text** → zugewiesen

**Das ist perfekt für Level/XP System!**

---

## 🔴 PRIORITÄT 1: Currency UI (WICHTIG!)

**Warum:** Spieler müssen Stardust und Crystals sehen können!

### **Was erstellen:**

#### **1. Stardust Text**

```
Canvas → Rechtsklick → UI → Text - TextMeshPro
Name: "StardustText"
```

**RectTransform:**
- Anchor: **Top-Right**
- Pos X: **-10** (10 Pixel von rechts)
- Pos Y: **-10** (10 Pixel von oben)
- Width: **150**
- Height: **30**

**TextMeshProUGUI:**
- Text: **"0"** (Placeholder)
- Font Size: **20**
- Alignment: **Right**
- Color: **Gold (255, 215, 0)**

#### **2. Crystals Text**

```
Canvas → Rechtsklick → UI → Text - TextMeshPro
Name: "CrystalsText"
```

**RectTransform:**
- Anchor: **Top-Right**
- Pos X: **-10**
- Pos Y: **-40** (unter Stardust)
- Width: **150**
- Height: **30**

**TextMeshProUGUI:**
- Text: **"0"** (Placeholder)
- Font Size: **20**
- Alignment: **Right**
- Color: **Cyan (0, 255, 255)**

#### **3. Icons (Optional, aber empfohlen)**

**Stardust Icon:**
- Erstelle `Image` GameObject
- Name: `StardustIcon`
- Position: Links neben Stardust Text
- Größe: 30×30 Pixel

**Crystals Icon:**
- Erstelle `Image` GameObject
- Name: `CrystalsIcon`
- Position: Links neben Crystals Text
- Größe: 30×30 Pixel

**Zuweisung:**
- `StardustText` → **Stardust Text**
- `CrystalsText` → **Crystals Text**
- `StardustIcon` → **Stardust Icon** (optional)
- `CrystalsIcon` → **Crystals Icon** (optional)

---

## 🟡 PRIORITÄT 2: Merge UI (Optional, aber nützlich)

**Warum:** Zeigt Merge-Ergebnisse und Rewards an.

### **Was erstellen:**

#### **Merge Result Panel**

```
Canvas → Rechtsklick → UI → Panel
Name: "MergeResultPanel"
```

**RectTransform:**
- Anchor: **Center**
- Width: **400**
- Height: **200**
- Standard: **Inaktiv** (SetActive = false)

**Inhalt:**
- `MergeResultText` (TextMeshPro) - "Merge erfolgreich!"
- `MergeRewardText` (TextMeshPro) - "+50 Stardust, +5 XP"
- `MergeResultItemImage` (Image) - Zeigt gemergtes Item

**Zuweisung:**
- `MergeResultPanel` → **Merge Result Panel**
- `MergeResultText` → **Merge Result Text**
- `MergeRewardText` → **Merge Reward Text**
- `MergeResultItemImage` → **Merge Result Item**

---

## 🟢 PRIORITÄT 3: Restliche UI (Später)

Diese können später hinzugefügt werden:

- **Daily UI:** Daily Login, Daily Quests
- **Mini-Game UI:** Energy, Play Button
- **Board UI:** Board Size, Free Slots
- **Idle UI:** Production Rate, Offline Rewards

**Warum später?**
- Spiel funktioniert auch ohne diese
- Du kannst sie Schritt für Schritt hinzufügen
- Fokus auf Kern-Funktionalität (Level, XP, Currency)

---

## 📋 Empfohlene Reihenfolge

### **Phase 1: Jetzt (Wichtig)**
1. ✅ Progression UI (bereits erledigt!)
2. 🔴 Currency UI (Stardust, Crystals Text)

### **Phase 2: Später (Optional)**
3. 🟡 Merge UI (Merge Results)
4. 🟢 Daily UI (wenn Daily System aktiv ist)
5. 🟢 Mini-Game UI (wenn Mini-Games aktiv sind)

---

## ✅ Quick Setup: Currency UI

**Minimal-Setup (5 Minuten):**

1. **StardustText erstellen:**
   - Canvas → UI → Text - TextMeshPro
   - Name: "StardustText"
   - Position: Top-Right, Font Size: 20, Color: Gold

2. **CrystalsText erstellen:**
   - Canvas → UI → Text - TextMeshPro
   - Name: "CrystalsText"
   - Position: Unter Stardust, Font Size: 20, Color: Cyan

3. **Zuweisung:**
   - Ziehe `StardustText` in **Stardust Text**
   - Ziehe `CrystalsText` in **Crystals Text**

4. **Testen:**
   - Play-Button drücken
   - Items mergen
   - Stardust sollte sich aktualisieren

---

## 🎯 Zusammenfassung

**Jetzt machen:**
- ✅ Progression UI (bereits erledigt)
- 🔴 Currency UI (Stardust, Crystals Text)

**Später machen:**
- 🟡 Merge UI
- 🟢 Daily UI
- 🟢 Mini-Game UI
- 🟢 Board UI
- 🟢 Idle UI

**Empfehlung:** Fange mit Currency UI an, dann kannst du später die anderen hinzufügen, wenn du sie brauchst.

---

**Viel Erfolg! 🚀**
