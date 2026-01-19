# 🔍 Weißer Balken im Hintergrund - Lösung

## Problem: Weißer Balken überlagert das Board

Der weiße Balken, der über das Board gelegt wird, ist wahrscheinlich ein **UI-Element** (Canvas, Panel, oder Image) in der Unity-Szene.

## ✅ Lösung: Weißen Balken finden und entfernen/anpassen

### Schritt 1: Balken in Hierarchy finden

1. **Öffne die Unity-Szene** (z.B. `MainScene`)
2. **In der Hierarchy:**
   - Suche nach Objekten wie:
     - `Canvas`
     - `Panel`
     - `Background`
     - `UI Panel`
     - `Overlay`
     - `WhiteBar` (falls benannt)

3. **Prüfe Canvas Children:**
   - Erweitere `Canvas` in der Hierarchy
   - Suche nach **Image** oder **Panel** Komponenten
   - Prüfe deren **Color** - ist es weiß/transparent?

### Schritt 2: Balken identifizieren

**Mögliche Ursachen:**

1. **Canvas Background:**
   - Canvas → `Canvas Scaler` oder `Graphic Raycaster`
   - Prüfe ob Canvas selbst ein `Image` Component hat

2. **UI Panel:**
   - Ein Panel-Element mit weißer/transparenter Farbe
   - Könnte für Menüs oder Overlays sein

3. **BoardParent Background:**
   - `BoardParent` GameObject könnte ein `Image` Component haben
   - Prüfe `BoardParent` → `Image` Component → `Color`

### Schritt 3: Balken entfernen/anpassen

**Option A: Balken entfernen (falls nicht benötigt)**

1. Wähle das GameObject mit dem weißen Balken
2. **Deaktiviere** es (Checkbox oben links im Inspector)
3. Oder **lösche** es komplett

**Option B: Balken transparent machen**

1. Wähle das GameObject
2. Im Inspector: `Image` Component
3. `Color` → Setze **Alpha = 0** (vollständig transparent)
4. Oder: `Color` → Setze auf gewünschte Farbe mit niedrigem Alpha

**Option C: Balken verschieben**

1. Wähle das GameObject
2. Im Inspector: `RectTransform`
3. Ändere `Position` oder `Anchors` so, dass es nicht über dem Board liegt

### Schritt 4: BoardParent prüfen

**Falls BoardParent selbst das Problem ist:**

1. Wähle `BoardParent` GameObject
2. Prüfe ob es ein `Image` Component hat
3. Falls ja:
   - Entferne `Image` Component (falls nicht benötigt)
   - Oder: Setze `Color` → Alpha = 0

## 🔍 Debugging-Tipps

### Visuell identifizieren:

1. **Wähle alle UI-Elemente** in der Hierarchy
2. **Im Scene View:**
   - Siehst du den weißen Balken als GameObject?
   - Klicke darauf → wird in Hierarchy markiert

2. **Im Game View:**
   - Wähle verschiedene GameObjects
   - Prüfe ob der Balken verschwindet wenn du ein bestimmtes GameObject deaktivierst

### Code-basierte Suche:

Falls der Balken programmatisch erstellt wird, suche nach:

```csharp
// Mögliche Code-Stellen:
- new GameObject("Panel")
- AddComponent<Image>()
- color = Color.white
- Canvas als Parent
```

## ✅ Schnelle Lösung

**Falls du den Balken nicht findest:**

1. **Erstelle ein neues GameObject:**
   - Hierarchy → Rechtsklick → `UI` → `Panel`
   - Name: `DebugPanel`

2. **Setze es als Child von Canvas:**
   - Ziehe es unter `Canvas`

3. **Deaktiviere es:**
   - Checkbox oben links = **unchecked**

4. **Prüfe ob der weiße Balken verschwunden ist**

## 📋 Checkliste

- [ ] Canvas und alle Children geprüft
- [ ] BoardParent geprüft (kein weißes Image)
- [ ] Alle UI Panels geprüft
- [ ] Weißer Balken identifiziert
- [ ] Balken entfernt/transparent gemacht/verschoben
- [ ] Game View zeigt kein weißes Overlay mehr

## 🎯 Erwartetes Ergebnis

Nach dem Fix:
- ✅ Kein weißer Balken über dem Board
- ✅ Board ist vollständig sichtbar
- ✅ Items sind klar erkennbar

Viel Erfolg! 🚀
