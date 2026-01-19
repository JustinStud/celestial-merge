# Story Dialog UI Fix Guide

## Problem
1. Dialog-Panel wird hinter dem Board (schwarze Box) gerendert - Text ist nicht sichtbar
2. Keine Möglichkeit, in der Lore weiterzuklicken/navigieren

## ✅ Lösung

### Problem 1: Dialog-Panel über Board anzeigen

Das Dialog-Panel muss auf einem Canvas mit höherer **Sort Order** sein oder später in der Hierarchy stehen.

#### Automatische Lösung (bereits implementiert):
Der `StoryUIManager` setzt jetzt automatisch die Canvas Sort Order auf `100` beim Start.

#### Manuelle Lösung (falls nötig):

**Option 1: Canvas Sort Order anpassen**
1. Wähle das **Canvas** des Dialog-Panels in der Hierarchy
2. Im Inspector bei **Canvas Component**:
   - **Sort Order**: `100` (höher als andere Canvas)
   - Dies sorgt dafür, dass Dialog-Panel über allem anderen gerendert wird

**Option 2: Panel in Hierarchy nach unten verschieben**
1. Wähle das **Dialog Panel** GameObject in der Hierarchy
2. **Ziehe es nach unten** in der Hierarchy (später = über anderen)
3. Unity rendert UI-Elemente in der Reihenfolge der Hierarchy (oben = hinten, unten = vorne)

**Option 3: Separates Canvas für Dialoge**
1. Erstelle neues Canvas: `Hierarchy → Rechtsklick → UI → Canvas`
2. Benenne es: `DialogCanvas`
3. Im Inspector:
   - **Render Mode**: `Screen Space - Overlay`
   - **Sort Order**: `100` (höher als andere Canvas)
4. Verschiebe `DialogPanel` unter dieses neue Canvas

### Problem 2: Weiterklicken in Lore-Dialogen

Es gibt jetzt **zwei Möglichkeiten**, um in Dialoge weiterzuklicken:

#### 1. Continue-Button (empfohlen)
- Ein Button, der nach dem Typewriter-Effekt angezeigt wird
- erscheint nur, wenn keine Choices vorhanden sind

**Setup:**
1. Im Inspector von `StoryUIManager`:
   - Finde **Continue Button** Referenz
   - Erstelle einen Button: `DialogPanel → Rechtsklick → UI → Button - TextMeshPro`
   - Text: "Weiter" oder "Continue"
   - Ziehe den Button in die `Continue Button` Referenz

#### 2. Click-to-Continue (automatisch aktiviert)
- Klick auf das Dialog-Panel selbst
- Funktioniert nur, wenn Typewriter fertig ist und keine Choices vorhanden sind

**Hinweis:** Dies ist standardmäßig aktiviert (`Enable Click To Continue = true` im Inspector).

---

## 🎨 Dialog-Panel Setup (Vollständig)

### Schritt 1: Dialog-Panel erstellen

1. **Canvas auswählen** (oder neues Canvas erstellen)
2. **Panel erstellen:**
   ```
   Canvas → Rechtsklick → UI → Panel
   Name: "DialogPanel"
   ```
3. **Im Inspector von DialogPanel:**
   - **RectTransform**: 
     - Anchor: `Middle Center`
     - Position: `(0, 0, 0)`
     - Size: `(800, 300)` (anpassen nach Bedarf)
   - **Image Component**:
     - Color: Dunkelgrau/Transparent (z.B. `RGBA(0, 0, 0, 220)` für halbtransparent)
   - **CanvasGroup Component** hinzufügen:
     - `Add Component → Canvas Group`
     - Wird für Fade-Animationen verwendet

### Schritt 2: Dialog-Elemente erstellen

#### NPC Portrait Image
```
DialogPanel → Rechtsklick → UI → Image
Name: "NPCPortraitImage"
```
- Position: Links (z.B. `(-350, 0)`)
- Size: `(150, 150)`

#### NPC Name Text
```
DialogPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "NPCNameText"
```
- Text: "Stella"
- Font Size: `24`
- Font Style: `Bold`
- Position: Über Dialog-Text

#### Dialog Text
```
DialogPanel → Rechtsklick → UI → Text - TextMeshPro
Name: "DialogText"
```
- Text: (wird dynamisch gesetzt)
- Font Size: `20`
- Alignment: `Left`, `Top`
- Position: Rechts vom Portrait (z.B. `(-150, 0)`)
- Size: `(600, 200)`

#### Continue Button (optional, aber empfohlen)
```
DialogPanel → Rechtsklick → UI → Button - TextMeshPro
Name: "ContinueButton"
```
- Text: "Weiter" oder "Continue"
- Position: Unten rechts (z.B. `(350, -120)`)
- Size: `(120, 40)`

#### Choice Buttons (optional, für Branching Narrative)
```
DialogPanel → Rechtsklick → UI → Button - TextMeshPro
Name: "ChoiceButton1"
```
- Wiederhole für weitere Choices
- Position: Unter Dialog-Text, vertikal gestapelt

### Schritt 3: StoryUIManager verbinden

1. **Wähle StoryUIManager GameObject**
2. **Im Inspector:**
   - **Dialog Panel**: Ziehe `DialogPanel` GameObject
   - **NPC Portrait Image**: Ziehe `NPCPortraitImage`
   - **NPC Name Text**: Ziehe `NPCNameText`
   - **Dialog Text**: Ziehe `DialogText`
   - **Continue Button**: Ziehe `ContinueButton` (falls erstellt)
   - **Choice Buttons**: Ziehe alle Choice-Buttons in das Array (falls vorhanden)
   - **Dialog Canvas Group**: Ziehe `DialogPanel` (hat CanvasGroup Component)

### Schritt 4: Canvas Sort Order prüfen

1. **Wähle das Canvas** des Dialog-Panels
2. **Im Inspector:**
   - **Sort Order**: `100` (höher als andere Canvas)
   - Oder: Verschiebe Panel in Hierarchy nach unten

---

## ✅ Checkliste

- [ ] Dialog-Panel erstellt und konfiguriert
- [ ] NPC Portrait Image erstellt
- [ ] NPC Name Text erstellt
- [ ] Dialog Text erstellt
- [ ] Continue Button erstellt (empfohlen)
- [ ] CanvasGroup Component am Dialog-Panel
- [ ] StoryUIManager Referenzen zugewiesen
- [ ] Canvas Sort Order auf `100` gesetzt (oder Panel in Hierarchy nach unten)
- [ ] Test: Dialog erscheint über dem Board
- [ ] Test: Weiterklicken funktioniert (Button oder Click auf Panel)

---

## 🧪 Testen

### Dialog anzeigen:
1. Starte das Spiel
2. Erreiche Level 1 (oder ein Level mit Story Beat)
3. **Prüfe:**
   - Dialog-Panel erscheint über dem Board (nicht dahinter)
   - NPC Name und Text sind sichtbar
   - Typewriter-Effekt läuft

### Weiterklicken testen:
1. **Warte** bis Typewriter-Effekt fertig ist
2. **Option 1:** Klicke auf "Weiter"-Button
3. **Option 2:** Klicke auf das Dialog-Panel selbst
4. **Prüfe:** Dialog schließt sich

---

## 🔧 Troubleshooting

### Problem: Dialog-Panel erscheint noch hinter Board

**Lösung:**
1. Prüfe Canvas Sort Order (sollte `100` sein)
2. Verschiebe Dialog-Panel in Hierarchy nach unten
3. Oder: Erstelle separates Canvas für Dialoge mit höherer Sort Order

### Problem: Continue-Button erscheint nicht

**Ursachen:**
- Button nicht im Inspector zugewiesen
- Typewriter-Effekt läuft noch
- Choices sind vorhanden (dann werden Choice-Buttons angezeigt statt Continue)

**Lösung:**
1. Weise Continue-Button im Inspector zu
2. Prüfe ob Story Beat Choices hat (dann werden Choice-Buttons angezeigt)

### Problem: Click-to-Continue funktioniert nicht

**Ursachen:**
- `Enable Click To Continue` ist deaktiviert
- Typewriter-Effekt läuft noch (muss fertig sein)
- Choices sind vorhanden (dann funktioniert Click nicht)

**Lösung:**
1. Aktiviere `Enable Click To Continue` im Inspector
2. Warte bis Typewriter fertig ist
3. Falls Choices vorhanden, verwende Choice-Buttons

---

## 🎉 Fertig!

Das Dialog-System sollte jetzt korrekt funktionieren:
- ✅ Dialog-Panel erscheint über dem Board
- ✅ Weiterklicken funktioniert (Button oder Click)
- ✅ Text ist vollständig sichtbar
