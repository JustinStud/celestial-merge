# 🔧 Visual Fix Guide - Items nicht sichtbar

## Problem: Items werden gespawnt, aber nicht angezeigt

### Ursache:
- Slots haben keine UI-Komponenten (ItemImage, ItemText)
- Board-Größe ist falsch (5×4 statt 4×5)
- Visual Update wird nicht korrekt aufgerufen

## ✅ Lösung Schritt für Schritt

### Schritt 1: Board-Größe korrigieren

1. **Erstelle BoardVisualFix GameObject:**
   - Hierarchy → Rechtsklick → `Create Empty`
   - Name: `BoardVisualFix`
   - Add Component → `BoardVisualFix`

2. **Fixe Board-Größe:**
   - Wähle `BoardVisualFix` GameObject
   - Im Inspector: Rechtsklick auf Script → `Fix Board Size (4x5)`
   - Oder: Wähle `ExpandableBoardManager` → Setze `Current Width = 4`, `Current Height = 5`

### Schritt 2: Slot Visuals reparieren

1. **Fixe alle Slots:**
   - Wähle `BoardVisualFix` GameObject
   - Im Inspector: Rechtsklick auf Script → `Fix All Board Slots - Visual`
   - Sollte alle Slots reparieren

### Schritt 3: Progression Manager verbinden

1. **Wähle `ExpandableBoardManager` GameObject**
2. **Im Inspector:**
   - Ziehe `CelestialProgressionManager` GameObject in das Feld `Progression Manager`
   - Oder: Lasse es leer - wird automatisch gefunden

### Schritt 4: Items neu spawnen

1. **Lösche alte Slots (falls nötig):**
   - Im Play-Mode: BoardParent GameObject → Alle Children löschen
   - Oder: Stoppe Play-Mode, lösche BoardParent Children manuell

2. **Spawne Items neu:**
   - Drücke `Space` im Play-Mode
   - Oder: Rechtsklick auf `CelestialItemSpawner` → `Spawn Celestial Item - Quick Test`

## 🔍 Debugging

### Prüfe ob Items gesetzt sind:
1. Wähle einen Slot im Hierarchy (während Play-Mode)
2. Im Inspector: Prüfe `CelestialBoardSlot` Component
3. `Current Item` sollte nicht null sein

### Prüfe ob UI-Komponenten existieren:
1. Wähle einen Slot
2. Prüfe ob `ItemImage` und `ItemText` Children existieren
3. Falls nicht: `BoardVisualFix` → `Fix All Board Slots - Visual`

### Prüfe Board-Größe:
1. Wähle `ExpandableBoardManager`
2. Prüfe: `Current Width = 4`, `Current Height = 5`
3. Falls falsch: `BoardVisualFix` → `Fix Board Size (4x5)`

## ✅ Checkliste

- [ ] Board-Größe ist 4×5 (nicht 5×4)
- [ ] Progression Manager ist verbunden
- [ ] BoardVisualFix wurde ausgeführt
- [ ] Items werden gespawnt (Console zeigt "✅ Item erfolgreich zum Board hinzugefügt")
- [ ] Slots haben ItemImage und ItemText Children
- [ ] Items sind sichtbar im Game View

## 🎯 Falls Items immer noch nicht sichtbar

1. **Prüfe Canvas:**
   - Canvas → Render Mode = `Screen Space - Overlay`
   - Canvas Scaler aktiviert

2. **Prüfe BoardParent:**
   - Sollte im Canvas sein
   - Sollte RectTransform haben
   - Anchor: Center, Pivot: Center

3. **Prüfe Slots:**
   - Jeder Slot sollte Image Component haben
   - ItemImage sollte enabled sein wenn Item vorhanden
   - ItemText sollte Text anzeigen

4. **Force Update:**
   - Wähle einen Slot mit Item
   - Im Inspector: Rechtsklick auf `CelestialBoardSlot` → `Set Item` (falls verfügbar)
   - Oder: `BoardVisualFix` → `Fix All Board Slots - Visual`

Viel Erfolg! 🚀
