# 🎨 Canvas Group Alpha einstellen - Schritt-für-Schritt

## Wo stelle ich Canvas Group Alpha ein?

### Schritt 1: Canvas Group Component hinzufügen

1. **Hierarchy** → Wähle das Panel, das transparent sein soll (z.B. `OfflineRewardPanel` oder `DailyQuestPanel`)
2. **Inspector** → Klicke auf **"Add Component"** Button (unten)
3. Suche nach: `Canvas Group`
4. Klicke auf **"Canvas Group"** → Component wird hinzugefügt

### Schritt 2: Alpha-Wert einstellen

1. **Inspector** → Scrolle zu **Canvas Group** Component
2. Finde das Feld **"Alpha"**
3. Setze Wert auf: `0.95` (oder zwischen 0.0 und 1.0)
   - `1.0` = Vollständig opak (nicht transparent)
   - `0.95` = 95% opak, 5% transparent
   - `0.0` = Vollständig transparent (unsichtbar)

### Schritt 3: Weitere Einstellungen (Optional)

**Canvas Group** Component hat weitere Optionen:

- **Alpha**: `0.95` ← **Das ist was du suchst!**
- **Interactable**: ✅ (aktiviert) - UI-Elemente können interagiert werden
- **Blocks Raycasts**: ✅ (aktiviert) - Blockiert Klicks durch Panel
- **Ignore Parent Groups**: ❌ (deaktiviert) - Erbt Alpha von Parent

---

## Visuelle Anleitung

### Unity Editor Inspector

```
┌─────────────────────────────────────┐
│  OfflineRewardPanel (GameObject)    │
│  ┌───────────────────────────────┐ │
│  │ Transform                      │ │
│  │ Rect Transform                 │ │
│  │ Image                          │ │
│  │ Canvas Group        ← HIER!     │ │
│  │   Alpha: [0.95]    ← SETZE     │ │
│  │   Interactable: ✓              │ │
│  │   Blocks Raycasts: ✓           │ │
│  └───────────────────────────────┘ │
└─────────────────────────────────────┘
```

---

## Für welches Panel?

### Offline Reward Panel (Phase 3)
- **Panel**: `OfflineRewardPanel`
- **Alpha**: `0.95` (etwas transparent, aber lesbar)

### Daily Quest Panel (Phase 2)
- **Panel**: `DailyQuestPanel`
- **Alpha**: `0.90` - `0.95` (optional, für besseres Design)

### Daily Login Panel (Phase 2)
- **Panel**: `DailyLoginPanel`
- **Alpha**: `0.95` (optional)

---

## Häufige Fehler

### Fehler 1: Canvas Group fehlt
**Problem:** Alpha-Feld ist nicht sichtbar
**Lösung:** Füge **Canvas Group** Component hinzu (Add Component → Canvas Group)

### Fehler 2: Alpha hat keine Wirkung
**Problem:** Panel ist immer noch vollständig opak
**Lösung:** 
- Prüfe ob **Canvas Group** auf dem richtigen GameObject ist
- Prüfe ob **Ignore Parent Groups** deaktiviert ist
- Prüfe ob Parent-Panel auch Canvas Group hat (kann Alpha überschreiben)

### Fehler 3: Panel ist zu transparent
**Problem:** Panel ist kaum sichtbar
**Lösung:** Erhöhe Alpha auf `0.90` - `0.95` (nicht unter 0.85)

---

## Alternative: Image Component Alpha

Falls du **keine Canvas Group** verwenden möchtest:

1. **Inspector** → **Image** Component
2. **Color** → Klicke auf Color-Feld
3. Setze **Alpha (A)** auf `242` (entspricht 0.95 × 255)

**Nachteil:** Funktioniert nur für das Image selbst, nicht für Child-Elemente.

**Vorteil Canvas Group:** Beeinflusst alle Child-Elemente gleichzeitig.

---

## ✅ Checkliste

- [ ] Canvas Group Component hinzugefügt
- [ ] Alpha auf `0.95` gesetzt
- [ ] Panel ist etwas transparent (Hintergrund schimmert durch)
- [ ] Text ist noch lesbar (nicht zu transparent)

---

**Viel Erfolg! 🎨**
