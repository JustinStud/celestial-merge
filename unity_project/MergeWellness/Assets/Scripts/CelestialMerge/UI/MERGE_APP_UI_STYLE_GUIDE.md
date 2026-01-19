# 🎨 Merge-App UI Style Guide - Professionelles Design

## 📋 Übersicht

Dieser Style Guide basiert auf Best Practices von erfolgreichen Merge-Apps wie:
- **Merge Dragons** (Metacore)
- **Merge Mansion** (Metacore)
- **Merge Garden** (Tripledot Studios)

## 🎯 Kern-Prinzipien

### 1. Klare Hierarchie
- **Titel oben** - Groß, fett, zentriert
- **Hauptinhalt mitte** - Große Icons, klare Belohnungen
- **Action Button unten** - Groß, auffällig, klar getrennt

### 2. Lesbarkeit (Contrast & Size)
- **Schriftgröße:** Mindestens 24pt für Buttons, 28-32pt für Titel
- **Kontrast:** Weiße Schrift auf dunklem Hintergrund ODER dunkle Schrift auf hellem
- **Abstände:** Mindestens 20px zwischen Elementen

### 3. Professionelles Layout (Merge Dragons Stil)

```
┌─────────────────────────────────────┐
│  Daily Login Bonus          [X]    │  ← Titel + Close Button
│                                     │
│  ┌─────────────────────────────┐   │
│  │  Tag 3 von 7                │   │  ← Day Info (oben)
│  └─────────────────────────────┘   │
│                                     │
│  ┌─────────────────────────────┐   │
│  │  💰 150 Stardust            │   │  ← Reward Display
│  │  💎 10 Crystals             │   │     (groß, klar)
│  └─────────────────────────────┘   │
│                                     │
│  ┌─────────────────────────────┐   │
│  │    [  Tag 1  Tag 2  TAG 3  ]│   │  ← Calendar View
│  │    [  Tag 4  Tag 5  Tag 6  ]│   │     (aktueller Tag hervorgehoben)
│  │    [       Tag 7            ]│   │
│  └─────────────────────────────┘   │
│                                     │
│  ┌─────────────────────────────┐   │
│  │      [  🎁 Abholen  ]       │   │  ← Claim Button
│  └─────────────────────────────┘   │     (groß, unten, farbig)
│                                     │
│  Streak: 3 Tage                    │  ← Info (klein, unten)
└─────────────────────────────────────┘
```

## 📐 Layout-Spezifikationen

### Daily Login Panel

#### Panel Setup
- **Breite:** 700-800px (80% Screen Width)
- **Höhe:** 600-700px (70% Screen Height)
- **Position:** Center (0, 0)
- **Hintergrund:** Dunkel mit Transparenz (z.B. RGBA(20, 20, 30, 0.95))
- **Border:** 2px Rahmen in Akzentfarbe

#### Titel-Bereich (oben, 15% Höhe)
- **Text:** "Daily Login Bonus" oder "Tägliche Belohnung"
- **Font Size:** 36pt
- **Farbe:** Weiß (#FFFFFF)
- **Font Style:** Bold
- **Position:** Top-Center, Padding: 20px von oben

#### Day Info (unter Titel, 10% Höhe)
- **Text:** "Tag 3 von 7"
- **Font Size:** 28pt
- **Farbe:** Hellblau oder Akzentfarbe (#4A9EFF)
- **Position:** Unter Titel, zentriert

#### Reward Display (Mitte, 25% Höhe)
- **Layout:** Vertikale Liste
- **Format:** 
  ```
  💰 150 Stardust
  💎 10 Crystals
  ```
- **Font Size:** 32pt für Zahlen, 24pt für Labels
- **Farbe:** Gold-Gelb für Zahlen (#FFD700), Weiß für Labels
- **Icons:** Emoji oder Sprite-Icons (💰, 💎)
- **Position:** Center, zwischen Day Info und Calendar

#### Calendar View (Mitte-Unter, 35% Höhe)
- **Layout:** 7 Boxen (1×7 oder 2×4 Layout)
- **Aktueller Tag:** Hervorgehoben (Glow, größer, farbiger Border)
- **Vergangene Tage:** Grau, mit Checkmark-Icon
- **Zukünftige Tage:** Gesperrt-Icon, niedrigere Opacity

#### Claim Button (unten, 15% Höhe)
- **Größe:** 300px breit, 60px hoch
- **Position:** Bottom-Center, 30px von unten
- **Farbe:** Grün (#33CC66) wenn verfügbar, Grau (#888888) wenn abgeholt
- **Text:** "🎁 Abholen" (verfügbar) oder "✓ Bereits abgeholt" (abgeholt)
- **Font Size:** 24pt
- **Font Style:** Bold

#### Info-Text (ganz unten, 5% Höhe)
- **Text:** "Streak: 3 Tage" oder "Nächstes Reset: in 12h"
- **Font Size:** 18pt
- **Farbe:** Grau (#AAAAAA)
- **Position:** Bottom-Center, 10px von unten

---

## 🎨 Farb-Palette (Merge-App-Stil)

### Primär-Farben
- **Hintergrund Panel:** RGBA(20, 20, 30, 0.95) - Dunkelblau/Schwarz
- **Akzentfarbe:** #4A9EFF - Hellblau (für Highlight)
- **Reward-Text:** #FFD700 - Gold (für Belohnungen)
- **Button Aktiv:** #33CC66 - Grün (für "Abholen")
- **Button Inaktiv:** #888888 - Grau (für "Bereits abgeholt")

### Text-Farben
- **Titel:** #FFFFFF - Weiß
- **Body Text:** #FFFFFF - Weiß (auf dunklem Hintergrund)
- **Info Text:** #AAAAAA - Grau
- **Error/Warning:** #FF4444 - Rot

---

## 📏 Abstände & Padding

### Panel-Padding
- **Außen:** 20px auf allen Seiten
- **Zwischen Elementen:** Mindestens 20px vertikal

### Button-Padding
- **Innen:** 15px horizontal, 10px vertikal
- **Außen:** 20px Abstand zu anderen Elementen

---

## 🔧 Unity Inspector Setup

### Daily Login Panel (Beispiel)

#### Hierarchie-Struktur
```
DailyLoginPanel (Panel)
├── TitleText (TextMeshPro)         ← "Daily Login Bonus"
├── DayInfoText (TextMeshPro)       ← "Tag 3 von 7"
├── RewardContainer (Panel)
│   ├── StardustReward (TextMeshPro) ← "💰 150 Stardust"
│   └── CrystalsReward (TextMeshPro) ← "💎 10 Crystals"
├── CalendarContainer (Panel)        ← 7 Day Boxes
│   ├── Day1Box (Panel)
│   ├── Day2Box (Panel)
│   └── ...
├── ClaimButton (Button)            ← "🎁 Abholen"
└── StreakText (TextMeshPro)        ← "Streak: 3 Tage"
```

#### RectTransform Einstellungen

**DailyLoginPanel:**
- Anchor: Center
- Pos: (0, 0)
- Size: (800, 700)

**TitleText:**
- Anchor: Top-Center
- Pos: (0, -30)
- Font Size: 36

**DayInfoText:**
- Anchor: Top-Center
- Pos: (0, -80)
- Font Size: 28

**RewardContainer:**
- Anchor: Center
- Pos: (0, 50)
- Size: (600, 150)

**CalendarContainer:**
- Anchor: Center
- Pos: (0, -150)
- Size: (700, 200)

**ClaimButton:**
- Anchor: Bottom-Center
- Pos: (0, 50)
- Size: (300, 60)

**StreakText:**
- Anchor: Bottom-Center
- Pos: (0, 10)
- Font Size: 18

---

## ✅ Checkliste für Professionelles Design

### Layout
- [ ] Titel oben, klar getrennt
- [ ] Belohnungen in der Mitte, groß und lesbar
- [ ] Calendar View zeigt 7 Tage
- [ ] Claim Button unten, groß und auffällig
- [ ] Keine Überlappungen zwischen Elementen

### Lesbarkeit
- [ ] Schriftgröße mindestens 24pt für Buttons
- [ ] Hoher Kontrast (weiß auf dunkel ODER dunkel auf hell)
- [ ] Ausreichend Abstand zwischen Elementen (mind. 20px)

### Visual Feedback
- [ ] Aktueller Tag ist hervorgehoben
- [ ] Abgeholte Tage haben Checkmark
- [ ] Gesperrte Tage haben gesperrt-Icon
- [ ] Button-Farbe ändert sich je nach Status

### User Experience
- [ ] Panel kann geschlossen werden (X-Button oben rechts)
- [ ] Belohnungen sind klar erkennbar (große Icons/Text)
- [ ] Claim Button ist leicht erreichbar (unten, mittig)

---

## 🚀 Nächste Schritte

1. **Implementiere Layout in DailyUIPanel.cs** ✅ (bereits verbessert)
2. **Erstelle UI-Elemente im Unity Editor** nach diesem Guide
3. **Teste auf verschiedenen Bildschirmgrößen**
4. **Animiere Claim-Button** (Pulse, Glow bei verfügbar)

---

**Viel Erfolg beim Umsetzen! 🎮✨**
