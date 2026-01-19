# Canvas Sort Order - Detaillierte Anleitung

## Was ist Canvas Sort Order?

Die **Canvas Sort Order** bestimmt, welche UI-Elemente über anderen gerendert werden. Ein Canvas mit höherer Sort Order wird über Canvas mit niedrigerer Sort Order gerendert.

**Beispiel:**
- Canvas für Board: Sort Order `0` (Standard)
- Canvas für Dialoge: Sort Order `100` (höher)
- **Ergebnis:** Dialoge erscheinen über dem Board ✅

---

## Schritt 1: Canvas finden

### Option A: Canvas des Dialog-Panels finden

1. **Öffne Unity Editor**
2. **In der Hierarchy:**
   - Suche nach deinem **Dialog-Panel** GameObject
   - Beispiel: `DialogPanel` oder `StoryDialogPanel`
3. **Klicke auf das Dialog-Panel** in der Hierarchy
4. **Im Inspector:**
   - Schaue nach oben in der Hierarchy-Struktur
   - Das Dialog-Panel sollte unter einem **Canvas** GameObject sein
   - Beispiel: `Canvas → DialogPanel`

**Tipp:** Falls das Dialog-Panel direkt unter `Canvas` ist, ist das Canvas das gesuchte Element.

### Option B: Canvas in Hierarchy suchen

1. **In der Hierarchy:**
   - Suche nach **Canvas** GameObjects
   - Es kann mehrere Canvas geben:
     - `Canvas` (Haupt-Canvas für Spiel-UI)
     - `DialogCanvas` (falls separat erstellt)
     - `StoryCanvas` (falls separat erstellt)

2. **Identifiziere das richtige Canvas:**
   - Das Canvas, unter dem dein Dialog-Panel ist
   - Oder: Das Canvas, das für Story-Dialoge verwendet wird

---

## Schritt 2: Canvas auswählen

1. **Klicke auf das Canvas GameObject** in der Hierarchy
   - Beispiel: `Canvas` oder `DialogCanvas`

2. **Im Inspector sollte jetzt die Canvas Component sichtbar sein**

---

## Schritt 3: Sort Order im Inspector finden

1. **Im Inspector:**
   - Scrolle nach unten zur **Canvas Component**
   - Die Canvas Component hat mehrere Einstellungen:
     - **Render Mode**: `Screen Space - Overlay` (oder andere)
     - **Pixel Perfect**: (Checkbox)
     - **Sort Order**: **← HIER IST ES!**

2. **Die Sort Order ist ein Zahlenfeld:**
   - Standardwert: `0`
   - Kann positive oder negative Zahlen sein
   - Höhere Zahlen = über anderen Canvas

---

## Schritt 4: Sort Order setzen

### Methode 1: Direkt im Inspector

1. **Klicke auf das Zahlenfeld** bei **Sort Order**
2. **Lösche den aktuellen Wert** (z.B. `0`)
3. **Tippe den neuen Wert ein**: `100`
4. **Drücke Enter** oder klicke außerhalb des Feldes

**Ergebnis:** Das Canvas hat jetzt Sort Order `100` und wird über anderen Canvas gerendert.

### Methode 2: Mit Pfeiltasten

1. **Klicke auf das Zahlenfeld** bei **Sort Order**
2. **Verwende Pfeiltasten**:
   - `↑` (Hoch) = erhöht Wert
   - `↓` (Runter) = verringert Wert
3. **Halte `Shift` + Pfeiltaste** für größere Schritte (z.B. +10)

---

## Schritt 5: Andere Canvas prüfen

Um sicherzustellen, dass dein Dialog-Canvas über allen anderen ist:

1. **Prüfe alle Canvas in der Szene:**
   - Gehe durch alle Canvas in der Hierarchy
   - Prüfe deren Sort Order im Inspector

2. **Vergleiche die Werte:**
   - **Board/Spiel-Canvas**: Sort Order `0` (oder niedriger)
   - **Dialog-Canvas**: Sort Order `100` (oder höher)
   - **Andere UI-Canvas**: Sort Order zwischen `0` und `100`

3. **Stelle sicher:**
   - Dialog-Canvas hat die **höchste Sort Order**
   - Mindestens `100` oder höher

---

## Schritt 6: Testen

1. **Starte das Spiel** (Play-Button)
2. **Triggere einen Dialog** (z.B. erreiche Level 1)
3. **Prüfe visuell:**
   - Dialog-Panel erscheint **über** dem Board
   - Text ist vollständig sichtbar
   - Keine Überlappung mit anderen UI-Elementen

---

## Detaillierte Inspector-Ansicht

### Canvas Component im Inspector:

```
┌─────────────────────────────────────┐
│ Canvas                               │
├─────────────────────────────────────┤
│ Render Mode: [Screen Space - Overlay]│
│ Pixel Perfect: [✓]                    │
│ Sort Order: [100] ← HIER!            │
│ Additional Shader Channels: [...]    │
└─────────────────────────────────────┘
```

**Wichtig:** Das **Sort Order** Feld ist ein **Integer-Feld** (Ganzzahl).

---

## Häufige Probleme und Lösungen

### Problem 1: Sort Order Feld ist nicht sichtbar

**Ursache:** Canvas Component ist nicht erweitert oder Inspector ist zu klein.

**Lösung:**
1. Klicke auf das **Pfeil-Symbol** neben "Canvas" im Inspector (um Component zu erweitern)
2. Scrolle im Inspector nach unten
3. Stelle sicher, dass Inspector-Fenster groß genug ist

### Problem 2: Sort Order ändert sich nicht

**Ursache:** Canvas ist möglicherweise ein Prefab oder Read-Only.

**Lösung:**
1. Prüfe ob Canvas ein Prefab ist (blaues Symbol in Hierarchy)
2. Falls Prefab: **Unpack Prefab** (Rechtsklick → Unpack Prefab)
3. Oder: Editiere das Prefab direkt

### Problem 3: Dialog erscheint immer noch hinter Board

**Mögliche Ursachen:**
1. Falsches Canvas ausgewählt
2. Anderes Canvas hat höhere Sort Order
3. Dialog-Panel ist nicht unter dem richtigen Canvas

**Lösung:**
1. Prüfe alle Canvas in der Szene
2. Stelle sicher, dass Dialog-Canvas die höchste Sort Order hat
3. Prüfe ob Dialog-Panel wirklich unter diesem Canvas ist
4. Versuche Sort Order noch höher zu setzen (z.B. `200`)

### Problem 4: Mehrere Canvas mit gleicher Sort Order

**Problem:** Wenn mehrere Canvas die gleiche Sort Order haben, wird die Hierarchy-Reihenfolge verwendet.

**Lösung:**
1. Setze Dialog-Canvas auf eindeutige, hohe Sort Order (z.B. `100`)
2. Oder: Verschiebe Dialog-Panel in Hierarchy nach unten (später = über anderen)

---

## Alternative: Automatische Sort Order (bereits implementiert)

Der `StoryUIManager` setzt automatisch die Sort Order beim Start:

```csharp
canvas.sortingOrder = 100;
```

**Falls dies nicht funktioniert:**
1. Prüfe Console auf Fehler-Meldungen
2. Stelle sicher, dass Dialog-Panel unter einem Canvas ist
3. Setze Sort Order manuell (siehe oben)

---

## Best Practices

### Empfohlene Sort Order Werte:

- **Haupt-Spiel-Canvas**: `0` (Standard)
- **UI-Panels (Currency, Progression)**: `10-50`
- **Dialog-Canvas**: `100` (oder höher)
- **Popups/Notifications**: `150`
- **Loading Screen**: `200` (höchste Priorität)

### Tipp: Sort Order Planung

Wenn du mehrere Canvas hast, plane die Sort Order im Voraus:

```
Canvas (Board/Spiel):        Sort Order 0
Canvas (UI Panels):          Sort Order 10
Canvas (Daily Rewards):      Sort Order 20
Canvas (Story Dialogs):      Sort Order 100  ← Dialoge über allem
Canvas (Popups):             Sort Order 150
```

---

## Visuelle Darstellung

### Vorher (Sort Order 0):
```
┌─────────────────┐
│   Dialog        │  ← Wird hinter Board gerendert
│   (unsichtbar)  │
└─────────────────┘
┌─────────────────┐
│   Board         │  ← Wird zuerst gerendert
│   (sichtbar)    │
└─────────────────┘
```

### Nachher (Sort Order 100):
```
┌─────────────────┐
│   Board         │  ← Wird zuerst gerendert (Sort Order 0)
│   (hinten)      │
└─────────────────┘
┌─────────────────┐
│   Dialog        │  ← Wird über Board gerendert (Sort Order 100)
│   (sichtbar)    │
└─────────────────┘
```

---

## Zusammenfassung

1. **Finde Canvas** des Dialog-Panels in der Hierarchy
2. **Wähle Canvas** aus
3. **Im Inspector:** Finde **Canvas Component**
4. **Sort Order Feld:** Setze Wert auf `100` (oder höher)
5. **Teste:** Dialog sollte über Board erscheinen

**Das war's!** Die Sort Order bestimmt die Rendering-Reihenfolge: Höhere Zahlen = über anderen.

---

## Quick Reference

| Aktion | Schritte |
|--------|----------|
| **Canvas finden** | Hierarchy → Suche nach Canvas GameObject |
| **Canvas auswählen** | Klicke auf Canvas in Hierarchy |
| **Sort Order finden** | Inspector → Canvas Component → Sort Order |
| **Sort Order setzen** | Klicke auf Feld → Tippe `100` → Enter |
| **Testen** | Play → Triggere Dialog → Prüfe visuell |

---

## ✅ Checkliste

- [ ] Canvas des Dialog-Panels gefunden
- [ ] Canvas im Inspector ausgewählt
- [ ] Canvas Component sichtbar
- [ ] Sort Order Feld gefunden
- [ ] Sort Order auf `100` gesetzt
- [ ] Andere Canvas geprüft (niedrigere Sort Order)
- [ ] Getestet: Dialog erscheint über Board

---

**Fertig!** Dein Dialog-Panel sollte jetzt korrekt über dem Board gerendert werden. 🎉
