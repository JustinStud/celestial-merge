# 🔧 Quick Fix: Board zentrieren & Items spawnen

## Problem 1: Board nicht zentriert

### Lösung:

1. **Erstelle BoardSetupHelper GameObject:**
   - Hierarchy → Rechtsklick → `Create Empty`
   - Name: `BoardSetupHelper`
   - Add Component → `BoardSetupHelper`

2. **Führe Setup aus:**
   - Wähle `BoardSetupHelper` GameObject
   - Im Inspector: Rechtsklick auf Script → `Setup Board Parent - Zentrieren`
   - Oder: Rechtsklick → `Create Slot Prefab`

3. **Prüfe BoardParent:**
   - Im Canvas sollte jetzt `BoardParent` GameObject sein
   - Sollte zentriert sein (Anchor: Center, Pivot: Center)

## Problem 2: Keine Items auf dem Board

### Lösung:

1. **Erstelle CelestialItemSpawner:**
   - Hierarchy → Rechtsklick → `Create Empty`
   - Name: `CelestialItemSpawner`
   - Add Component → `CelestialItemSpawner`

2. **Stelle sicher, dass CelestialItemDatabase initialisiert ist:**
   - Project → Finde `CelestialItemDatabase` Asset
   - Inspector → Rechtsklick → `Initialize Celestial Items`

3. **Items spawnen:**
   - **Automatisch:** Beim Start werden 3 Starter-Items gespawnt
   - **Manuell:** Drücke `Space` im Play-Mode
   - **Context Menu:** Rechtsklick auf Script → `Spawn Celestial Item - Quick Test`

## Problem 3: Merge funktioniert nicht

### Lösung:

1. **Stelle sicher, dass alle Referenzen gesetzt sind:**
   - `ExpandableBoardManager` → `Board Parent` = BoardParent GameObject
   - `ExpandableBoardManager` → `Slot Prefab` = BoardSlotPrefab (wird automatisch erstellt)
   - `CelestialMergeManager` → `Item Database` = CelestialItemDatabase Asset
   - `CelestialMergeManager` → `Currency Manager` = CurrencyManager GameObject
   - `CelestialMergeManager` → `Progression Manager` = CelestialProgressionManager GameObject

2. **Teste Merge:**
   - Spawne 2x gleiche Items (Space drücken)
   - Ziehe ein Item auf das andere
   - Merge sollte funktionieren

## ✅ Checkliste

- [ ] BoardSetupHelper erstellt und Setup ausgeführt
- [ ] BoardParent ist zentriert (im Canvas)
- [ ] CelestialItemSpawner erstellt
- [ ] CelestialItemDatabase Asset initialisiert
- [ ] Items werden gespawnt (automatisch oder mit Space)
- [ ] Merge funktioniert (Drag-Drop)

## 🎮 Keyboard Shortcuts

- **Space:** Spawne zufälliges Item
- **M:** Spawne 2x gleiche Items (für Merge-Test)

## 📝 Falls es nicht funktioniert

1. **Console prüfen:** Gibt es Fehler?
2. **Referenzen prüfen:** Sind alle Referenzen im Inspector gesetzt?
3. **BoardParent prüfen:** Existiert es im Canvas?
4. **Slot Prefab prüfen:** Wurde es erstellt?

Viel Erfolg! 🚀
