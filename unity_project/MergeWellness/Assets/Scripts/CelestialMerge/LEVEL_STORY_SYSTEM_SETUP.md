# Level- und Story-System Setup Guide

## Übersicht

Dieser Guide erklärt, wie du das **Level-System** und **Story-System** für Celestial Merge einrichtest.

## ✅ Level-System

Das Level-System ist bereits vollständig implementiert und sollte automatisch funktionieren. Es umfasst:

- **XP-System**: XP wird beim Mergen von Items vergeben
- **Level-Progression**: Level 1-500 mit exponentieller XP-Kurve
- **Chapter-System**: 6 Chapters basierend auf Level
- **Milestones**: Merge-Milestones bei 10, 25, 50, 100, 250, 500, 1000 Merges
- **Board Expansion**: Automatische Board-Erweiterung alle 4 Level

### Automatische Funktionalität

Das Level-System funktioniert automatisch, wenn folgende Komponenten vorhanden sind:

1. ✅ `CelestialProgressionManager` in der Szene
2. ✅ `CelestialMergeManager` in der Szene
3. ✅ `ExpandableBoardManager` in der Szene (für Merges)

### Überprüfung

**Console-Logs beim Start:**
```
📊 Progression geladen: Level X, XP Y/Z, Chapter N, Merges M
```

**Console-Logs beim Mergen:**
```
⭐ XP Reward: +X (Vorher: Y, Nachher: Z, Level: N)
🎉 Level Up! Jetzt Level N
📖 Chapter N freigeschaltet!
```

**Falls Level-System nicht funktioniert:**

1. Prüfe ob `CelestialProgressionManager` in der Szene ist
2. Prüfe ob `CelestialMergeManager` vorhanden ist
3. Prüfe Console auf Fehler-Meldungen
4. Stelle sicher, dass `RegisterMerge()` aufgerufen wird (siehe `ExpandableBoardManager.cs`)

---

## 📖 Story-System Setup

Das Story-System benötigt einige manuelle Schritte zur Einrichtung.

### Schritt 1: StoryDatabase Asset erstellen (2 Min)

1. **Im Project-Fenster:**
   - Navigiere zu `Assets/Scripts/CelestialMerge/Story/` (oder erstelle den Ordner)
   - Rechtsklick → `Create` → `CelestialMerge` → `StoryDatabase`
   - Benenne es: `StoryDatabase`

2. **StoryDatabase initialisieren:**
   - Wähle das `StoryDatabase` Asset im Project-Fenster
   - Im Inspector: Rechtsklick auf das Script → `Initialize Story Content`
   - Oder: Im Inspector-Button klicken (falls vorhanden)
   - **Console sollte zeigen:** `✅ Story Content initialisiert: 6 Chapters, X Beats, Y Lore Entries`

### Schritt 2: StoryManager GameObject erstellen (1 Min)

1. **In der Szene:**
   - `Hierarchy` → Rechtsklick → `Create Empty`
   - Name: `StoryManager`
   - Füge Component hinzu: `StoryManager` (Script)

2. **Im Inspector:**
   - Ziehe `StoryDatabase` Asset in die `Story Database` Referenz
   - `Progression Manager` wird automatisch gefunden (falls `CelestialProgressionManager` in der Szene ist)

### Schritt 3: StoryUIManager GameObject erstellen (Optional, für UI)

**Falls du Story-Dialoge anzeigen möchtest:**

1. **In der Szene:**
   - `Hierarchy` → Rechtsklick → `Create Empty`
   - Name: `StoryUIManager`
   - Füge Component hinzu: `StoryUIManager` (Script)

2. **UI-Elemente erstellen:**
   - Dialog Panel (GameObject mit Canvas Group)
   - NPC Portrait Image
   - NPC Name Text (TextMeshProUGUI)
   - Dialog Text (TextMeshProUGUI)
   - Choice Buttons (Array von Buttons)
   - Chapter Unlock Panel (optional)
   - Lore Notification Panel (optional)

3. **Im Inspector von StoryUIManager:**
   - Ziehe alle UI-Elemente in die entsprechenden Referenzen

**Hinweis:** Falls `StoryUIManager` nicht vorhanden ist, funktioniert das Story-System trotzdem, aber Dialoge werden nicht angezeigt.

### Schritt 4: Integration mit CelestialGameManager (Automatisch)

`CelestialGameManager` findet automatisch:
- ✅ `StoryManager` (falls in der Szene)
- ✅ `StoryDatabase` (falls vorhanden)

**Console-Logs beim Start:**
```
📖 Story Database geladen: 6 Chapters
📖 StoryManager initialisiert: Level X, Chapter Y
```

---

## 🎮 Story-System Funktionalität

### Automatische Trigger

Das Story-System triggert automatisch:

1. **Chapter-Unlock**: Wenn Spieler ein neues Level erreicht, das ein neues Chapter freischaltet
2. **Story Beats**: Wenn Spieler ein bestimmtes Level erreicht (z.B. Level 1, 5, 10 für Chapter 1)

### Story Beats

Jedes Chapter hat mehrere Story Beats, die bei bestimmten Leveln getriggert werden:

- **Chapter 1 (Genesis)**: Level 1, 5, 10
- **Chapter 2 (Foundations)**: Level 11, 18
- **Chapter 3 (Awakening)**: Level 26
- **Chapter 4 (Shadows)**: Level 50
- **Chapter 5 (Convergence)**: Level 70
- **Chapter 6 (Aftermath)**: Level 101

### Lore-System

Beim Triggern von Story Beats werden automatisch Lore Entries freigeschaltet, die in der Encyclopedia gesammelt werden können.

---

## 🔧 Troubleshooting

### Problem: Level-System funktioniert nicht

**Symptome:**
- Level bleibt bei 0 oder 1
- XP wird nicht vergeben
- Keine Level-Ups

**Lösung:**
1. Prüfe ob `CelestialProgressionManager` in der Szene ist
2. Prüfe ob `CelestialMergeManager` vorhanden ist
3. Prüfe Console auf Fehler
4. Stelle sicher, dass `RegisterMerge()` aufgerufen wird (siehe `ExpandableBoardManager.PerformMerge()`)

### Problem: Story-System funktioniert nicht

**Symptome:**
- Keine Story-Dialoge
- Keine Chapter-Unlocks
- Console-Warnung: "StoryDatabase nicht gefunden"

**Lösung:**
1. ✅ Erstelle `StoryDatabase` Asset (Schritt 1)
2. ✅ Initialisiere Story Content (Schritt 1)
3. ✅ Erstelle `StoryManager` GameObject (Schritt 2)
4. ✅ Weise `StoryDatabase` im Inspector zu
5. ✅ Prüfe Console auf Initialisierungs-Logs

### Problem: Story-Dialoge werden nicht angezeigt

**Symptome:**
- Story Beats werden getriggert (Console-Logs), aber keine UI

**Lösung:**
1. ✅ Erstelle `StoryUIManager` GameObject (Schritt 3)
2. ✅ Erstelle UI-Elemente (Dialog Panel, etc.)
3. ✅ Weise UI-Elemente im Inspector zu
4. ✅ Prüfe ob `StoryUIManager` in der Szene ist

### Problem: StoryDatabase ist leer

**Symptome:**
- Console: "StoryDatabase ist leer!"

**Lösung:**
1. Wähle `StoryDatabase` Asset im Project-Fenster
2. Im Inspector: Rechtsklick auf Script → `Initialize Story Content`
3. Prüfe Console: Sollte zeigen "✅ Story Content initialisiert"

---

## 📊 Überprüfung

### Level-System testen:

1. **Starte das Spiel**
2. **Führe einen Merge durch**
3. **Prüfe Console:**
   ```
   ⭐ XP Reward: +X
   📊 Level Text aktualisiert: N
   📊 XP Progress Bar aktualisiert: Y/Z
   ```

### Story-System testen:

1. **Starte das Spiel**
2. **Erreiche Level 1** (oder starte mit Level 1)
3. **Prüfe Console:**
   ```
   📖 Story Beat getriggert: 101 - Stella
   📖 Lore Entry freigeschaltet: Der Beginn
   ```

4. **Erreiche Level 10** (für Chapter 1 Completion)
5. **Prüfe Console:**
   ```
   📖 Chapter 1 freigeschaltet: Genesis
   ```

---

## ✅ Checkliste

### Level-System:
- [ ] `CelestialProgressionManager` in der Szene
- [ ] `CelestialMergeManager` in der Szene
- [ ] Console zeigt "📊 Progression geladen" beim Start
- [ ] XP wird beim Mergen vergeben (Console-Logs)
- [ ] Level-Ups funktionieren

### Story-System:
- [ ] `StoryDatabase` Asset erstellt
- [ ] Story Content initialisiert (6 Chapters, 30+ Beats, 50+ Lore)
- [ ] `StoryManager` GameObject in der Szene
- [ ] `StoryDatabase` im Inspector zugewiesen
- [ ] Console zeigt "📖 StoryManager initialisiert" beim Start
- [ ] Story Beats werden getriggert (Console-Logs)
- [ ] (Optional) `StoryUIManager` für Dialoge

---

## 🎉 Fertig!

Beide Systeme sollten jetzt funktionieren. Das Level-System ist vollständig automatisch, das Story-System benötigt nur die oben beschriebenen Setup-Schritte.

**Nächste Schritte:**
- Teste das Level-System durch Mergen von Items
- Teste das Story-System durch Erreichen verschiedener Level
- Passe Story Content in `StoryDatabase` an (falls gewünscht)
