# ⚡ Quick Fix: Fehlende Buttons (Mini-Game & Quest)

## Problem
- ❌ Mini-Game-Menü ist nicht sichtbar
- ❌ Quest-Menü lässt sich nicht öffnen
- ❌ Buttons fehlen in der Haupt-UI

## ✅ Lösung: 30 Sekunden Fix

### Schritt 1: Editor-Tool öffnen
1. **Unity Editor** → Menu: `CelestialMerge` → `UI` → `Auto Setup Main UI (App Store Ready)`

### Schritt 2: Setup ausführen
1. Klicke: **"🚀 Setup All UI Now"**
2. **Fertig!** ✅

### Schritt 3: Testen
1. **Play** im Editor
2. **Prüfe:** Quest Button sollte sichtbar sein (Top-Right)
3. **Prüfe:** Mini-Game Button sollte sichtbar sein (Top-Right)
4. **Klicke** auf Buttons → Panels sollten sich öffnen

---

## Was wird erstellt?

### Buttons (Top-Right)
- ✅ **Quest Button** (`📋 Quests`) - Öffnet Daily Quest Panel
- ✅ **Mini-Game Button** (`🎮 Mini-Game`) - Öffnet Mini-Game Panel

### Automatisch verbunden
- ✅ Quest Button → `DailyUIPanel.openQuestButton`
- ✅ Mini-Game Button → `CelestialUIManager.playMiniGameButton`

### Professionelles Design
- ✅ Blau (#4A9EFF)
- ✅ Hover/Pressed States
- ✅ Große Schrift (22px, Bold)
- ✅ Korrekte Positionierung

---

## Falls Buttons nicht sichtbar sind

### Prüfe 1: Canvas Sort Order
- **Hierarchy** → Canvas → **Inspector** → **Sort Order**: `0` oder höher

### Prüfe 2: Buttons aktiv?
- **Hierarchy** → `TopRightButtons` → Prüfe ob Buttons **Active** sind

### Prüfe 3: Event System
- **Hierarchy** → Prüfe ob `EventSystem` existiert
- Falls nicht: **Hierarchy** → Rechtsklick → **UI** → **Event System**

---

## Alternative: Manuelle Zuweisung

Falls Editor-Tool nicht funktioniert:

### Quest Button
1. **Hierarchy** → Finde `QuestButton` (oder erstelle: Canvas → UI → Button)
2. **Hierarchy** → Finde GameObject mit `DailyUIPanel` Script
3. **Inspector** → `DailyUIPanel` → **`Open Quest Button`**: Ziehe `QuestButton` hinein

### Mini-Game Button
1. **Hierarchy** → Finde `MiniGameButton` (oder erstelle: Canvas → UI → Button)
2. **Hierarchy** → Finde GameObject mit `CelestialUIManager` Script
3. **Inspector** → `CelestialUIManager` → **`Play Mini Game Button`**: Ziehe `MiniGameButton` hinein

---

**Viel Erfolg! 🎮✨**
