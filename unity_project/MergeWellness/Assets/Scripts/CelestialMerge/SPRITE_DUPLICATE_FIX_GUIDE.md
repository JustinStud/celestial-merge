# Sprite-Duplikat-Problem beheben

## Problem
Mehrere Items haben das gleiche Sprite zugewiesen bekommen, was dazu führt, dass visuell identische Items nicht gemerged werden können (da Merging auf `category`, `level` und `rarity` basiert, nicht auf dem Sprite).

## Lösung

### Schritt 1: Doppelte Zuweisungen finden
1. Öffne Unity Editor
2. Gehe zu `CelestialMerge > Tools > Auto-Assign Sprites`
3. Wähle deine `CelestialItemDatabase` aus
4. Klicke auf **"Find Duplicate Sprite Assignments"**
5. Die Console zeigt alle Sprites, die mehrfach zugewiesen wurden

### Schritt 2: Doppelte Zuweisungen beheben
1. Im gleichen Fenster klicke auf **"Fix Duplicate Assignments (Clear & Reassign)"**
2. Bestätige die Warnung
3. Alle Sprite-Zuweisungen werden entfernt und neu zugewiesen
4. Diesmal werden keine Duplikate erstellt, da das System verfolgt, welche Sprites bereits verwendet wurden

### Schritt 3: Überprüfen
1. Prüfe die Console-Logs - sie zeigen für jedes Item, welches Sprite zugewiesen wurde
2. Items ohne Sprite werden mit einer Warnung geloggt
3. Öffne die `CelestialItemDatabase` im Inspector und prüfe visuell

## Technische Details

### Verbesserte Matching-Logik
Der neue `CalculateMatchScore()` Algorithmus bewertet Matches nach:
- **Exakter Match mit Display-Name**: 1000 Punkte
- **Teilstring-Match mit Display-Name**: 500 Punkte
- **Level-Match**: 100 Punkte
- **Rarity-Match**: 50 Punkte
- **Fuzzy Word-Match**: 10 Punkte pro übereinstimmendem Wort

Das System wählt immer das Sprite mit dem höchsten Score, das noch nicht verwendet wurde.

### Verhindern von Duplikaten
- Ein `HashSet<Sprite>` verfolgt alle bereits zugewiesenen Sprites
- Beim Matching werden nur noch nicht verwendete Sprites berücksichtigt
- Jedes Sprite kann nur einmal zugewiesen werden

## Falls Probleme auftreten

### Problem: Viele Items haben kein Sprite
**Lösung**: 
- Prüfe, ob die Sprite-Dateien korrekt benannt sind
- Die Sprite-Namen sollten Teile der Item-ID oder des Display-Namens enthalten
- Beispiel: Item "Stardust Particle" sollte ein Sprite mit Namen wie "stardust_particle" oder "stardust_particle_l1_common" haben

### Problem: Falsche Sprites werden zugewiesen
**Lösung**:
- Nutze die "Find Duplicate" Funktion, um problematische Zuweisungen zu finden
- Du kannst Sprites manuell im Inspector der `CelestialItemDatabase` zuweisen
- Oder benenne die Sprite-Dateien um, damit sie besser matchen

## Nächste Schritte
Nachdem alle Sprites korrekt zugewiesen sind:
1. Teste das Merging im Spiel
2. Stelle sicher, dass Items mit gleichem Sprite auch die gleiche `category`, `level` und `rarity` haben, wenn sie mergeable sein sollen
3. Falls nicht, prüfe die Item-Daten in der Database
