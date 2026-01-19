# 🚀 Celestial Merge - Quick Start Guide

## Schnellstart in 5 Minuten

### Schritt 1: GameManager erstellen (2 Min)

1. **Unity öffnen** → Gameplay-Szene öffnen
2. **Leeres GameObject erstellen:**
   ```
   Hierarchy → Rechtsklick → Create Empty
   Name: "CelestialGameManager"
   ```
3. **Script hinzufügen:**
   ```
   Inspector → Add Component → "CelestialGameManager"
   ✅ Auto Initialize = aktiviert
   ```

### Schritt 2: Systeme erstellen (2 Min)

**Für jedes System:**
1. `Create Empty` GameObject
2. Script hinzufügen (z.B. `CurrencyManager`)
3. **WICHTIG:** Alle Manager müssen in der Szene sein!

**Erstelle diese GameObjects:**
- `CurrencyManager`
- `CelestialProgressionManager`
- `CelestialMergeManager`
- `ExpandableBoardManager`
- `DailySystemManager`
- `MiniGameManager`

### Schritt 3: ItemDatabase erstellen (1 Min)

1. **Project-Fenster:**
   ```
   Assets/Scripts/CelestialMerge/ → Rechtsklick
   → Create → CelestialMerge → ItemDatabase
   ```
2. **Initialisieren:**
   - Asset auswählen
   - Im Inspector: Rechtsklick → `Initialize Celestial Items`

### Schritt 4: Testen

1. **Play-Button drücken**
2. **Console öffnen** (Window → General → Console)
3. **Sollte sehen:**
   ```
   === Celestial Merge - Initialisierung ===
   ✅ Spiel erfolgreich initialisiert!
   ```

## ✅ Fertig!

Das Spiel ist jetzt grundlegend integriert. Für UI-Implementierung siehe `INTEGRATION_GUIDE.md`.
