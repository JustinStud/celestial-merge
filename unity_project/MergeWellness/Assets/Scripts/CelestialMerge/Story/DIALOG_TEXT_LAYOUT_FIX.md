# Dialog-Text Layout Fix

## Problem

Der Dialog-Text und der NPC-Name werden **außerhalb** des dunkelgrauen Dialog-Panels angezeigt, obwohl sie **innerhalb** des Panels erscheinen sollten.

## Lösung

### Automatische Lösung (Empfohlen)

1. **Öffne das Editor-Tool:**
   - Menu: `CelestialMerge` → `Story UI` → `Fix Dialog Text Layout`
   - Klicke auf `Finde StoryUIManager und fixe Layout`

2. **Das Tool korrigiert automatisch:**
   - Stellt sicher, dass `dialogText` ein **Child** von `dialogPanel` ist
   - Setzt korrekte **RectTransform-Einstellungen** (Anchor, Position, Größe)
   - Passt **Text-Alignment** an (oben-links)

### Manuelle Lösung (Falls automatisch nicht funktioniert)

#### Schritt 1: Prüfe Hierarchy-Struktur

1. Öffne die Scene im Hierarchy-Fenster
2. Finde das GameObject mit `StoryUIManager` Component
3. Prüfe ob `dialogText` (TextMeshProUGUI) ein **Child** von `dialogPanel` (GameObject) ist

**Richtige Struktur:**
```
dialogPanel (GameObject)
  ├── dialogText (TextMeshProUGUI)
  ├── npcNameText (TextMeshProUGUI) [optional]
  └── continueButton (Button) [optional]
```

**Falsche Struktur (Text außerhalb):**
```
Canvas
  ├── dialogPanel (GameObject)
  └── dialogText (TextMeshProUGUI) ← FEHLER: Sollte Child von dialogPanel sein!
```

#### Schritt 2: Verschiebe dialogText in dialogPanel (falls nötig)

1. Wähle `dialogText` GameObject im Hierarchy
2. **Ziehe es per Drag & Drop** auf `dialogPanel` (es wird dann ein Child)
3. ODER: Rechtsklick auf `dialogText` → `Change Parent` → `dialogPanel`

#### Schritt 3: Korrigiere RectTransform von dialogText

1. Wähle `dialogText` GameObject (sollte jetzt Child von `dialogPanel` sein)
2. Im Inspector: **RectTransform** Component

**Anchor Presets:**
- Rechtsklick auf **Anchor Preset Icon** (oben links im RectTransform)
- Wähle: **Stretch** → **Stretch** (fülle Parent)

**ODER manuell setzen:**

**Anchor Min:** `(0.1, 0.2)`  
**Anchor Max:** `(0.9, 0.8)`  
**Pos X:** `0`  
**Pos Y:** `0`  
**Width:** Automatisch (sollte leer sein)  
**Height:** Automatisch (sollte leer sein)  

**Offset Min:** `(0, 0)`  
**Offset Max:** `(0, 0)`  

#### Schritt 4: Korrigiere Text-Komponente

1. Im Inspector: **TextMeshPro - Text (UI)** Component
2. **Alignment:** Oben-Links (⬉)
3. **Enable Word Wrapping:** ✅ (aktiviert)

#### Schritt 5: Korrigiere npcNameText (falls vorhanden)

1. Wähle `npcNameText` GameObject
2. Stelle sicher, dass es **Child von dialogPanel** ist
3. **RectTransform:**
   - **Anchor Min:** `(0.1, 0.85)`
   - **Anchor Max:** `(0.9, 0.95)`
   - **Pos X:** `0`
   - **Pos Y:** `0`

#### Schritt 6: Teste im Game View

1. Starte das Spiel
2. Triggere einen Story Beat (z.B. Level 20 erreichen)
3. Prüfe ob Dialog-Text **innerhalb** des dunkelgrauen Panels erscheint

## Erwartetes Ergebnis

✅ **Dialog-Text** und **NPC-Name** erscheinen **innerhalb** des dunkelgrauen Dialog-Panels  
✅ Text ist korrekt positioniert (oben-links im Panel)  
✅ Text ist lesbar (weiße Schrift auf dunklem Hintergrund)  

## Häufige Fehler

❌ **dialogText ist nicht Child von dialogPanel**  
→ Lösung: Siehe Schritt 2

❌ **Anchor/Pivot ist falsch gesetzt**  
→ Lösung: Siehe Schritt 3 (Stretch-Stretch verwenden)

❌ **Text-Farbe ist unsichtbar** (z.B. schwarz auf schwarz)  
→ Lösung: Im Inspector → TextMeshPro Component → **Vertex Color** auf weiß setzen

❌ **Panel ist zu klein**  
→ Lösung: Vergrößere `dialogPanel` RectTransform (z.B. Width: 800, Height: 400)
