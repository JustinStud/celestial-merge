# 🔄 Migration von MergeWellness zu CelestialMerge

## ⚠️ Aktuelles Problem

Du hast **zwei parallele Systeme** laufen:
1. **Altes System** (MergeWellness): `GridManager`, `ItemDatabase`, `WellnessItem`
2. **Neues System** (CelestialMerge): `ExpandableBoardManager`, `CelestialItemDatabase`, `CelestialItem`

Das führt zu:
- ❌ Zwei Grids übereinander (sieht komisch aus)
- ❌ Merge stoppt bei T3 (alte ItemDatabase nicht initialisiert)
- ❌ Kein funktionierendes CelestialMerge-Game

## 🎯 Lösung: Schnelle Reparatur (für jetzt)

### Schritt 1: Alte ItemDatabase initialisieren

1. **Project-Fenster** öffnen
2. Suche nach: `ItemDatabase` Asset (nicht CelestialItemDatabase!)
3. **Wähle das Asset** aus
4. Im Inspector: **Rechtsklick** auf das Script → `Initialize Default Items`
5. Prüfe: Sollte jetzt Items bis Tier 5 haben

**Das behebt:** Merge funktioniert jetzt bis Tier 5

### Schritt 2: ExpandableBoardManager deaktivieren (temporär)

1. **Hierarchy** öffnen
2. Finde `ExpandableBoardManager` GameObject
3. **Deaktiviere** es (Checkbox oben links im Inspector)
   - Oder: Rechtsklick → `Set Active` → Deaktivieren

**Das behebt:** Nur noch ein Grid sichtbar

## 🚀 Vollständige Migration (später)

Um vollständig auf CelestialMerge umzusteigen:

### Option A: Altes System komplett ersetzen

1. **Deaktiviere alte Systeme:**
   - `GridManager` GameObject deaktivieren
   - `GameplayManager` GameObject deaktivieren
   - `QuickItemSpawner` GameObject deaktivieren

2. **Aktiviere neue Systeme:**
   - `ExpandableBoardManager` GameObject aktivieren
   - `CelestialGameManager` sollte bereits aktiv sein

3. **Erstelle neuen ItemSpawner:**
   - Erstelle `CelestialItemSpawner` (siehe unten)

### Option B: Beide Systeme parallel (nicht empfohlen)

- Beide Systeme laufen, aber das führt zu Verwirrung
- Nur für Testing/Entwicklung

## 📝 Nächste Schritte

Nach der schnellen Reparatur:
1. ✅ Merge funktioniert bis Tier 5
2. ✅ Nur ein Grid sichtbar
3. ⚠️ Aber: Es ist noch das alte System (MergeWellness)

Für vollständiges CelestialMerge:
- Siehe "Vollständige Migration" oben
- Oder: Warte auf weitere Anleitung
