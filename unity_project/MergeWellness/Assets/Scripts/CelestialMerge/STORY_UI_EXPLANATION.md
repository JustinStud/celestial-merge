# 📖 Story UI - Warum ist nichts sichtbar?

## ✅ Das ist normal!

Die Story-UI-Elemente (DialogPanel, ChapterUnlockPanel, LoreNotificationPanel) sind **standardmäßig inaktiv** und werden nur angezeigt, wenn:

1. **Ein Story Beat getriggert wird** (z.B. bei Level 1, 5, 10)
2. **Ein Chapter freigeschaltet wird** (z.B. bei Level 11, 26, 46)
3. **Eine Lore Entry freigeschaltet wird** (durch Story Beats)

---

## 🎮 Wie teste ich die Story UI?

### **Option 1: Level erreichen (natürlich)**

1. **Spiele das Spiel** und erreiche **Level 1**
2. **StoryManager** sollte automatisch den ersten Story Beat triggern
3. **DialogPanel** sollte erscheinen mit Stella's Dialog

### **Option 2: Manuell testen (für Entwicklung)**

1. **Wähle `StoryManager` GameObject** in der Hierarchy
2. **Im Inspector:** Suche nach **"Story Manager"** Component
3. **Falls vorhanden:** Du könntest einen Test-Button hinzufügen (siehe unten)

### **Option 3: Level direkt setzen**

1. **Wähle `CelestialProgressionManager` GameObject**
2. **Im Inspector:** Setze **"Player Level"** auf **1** (oder 5, 10, etc.)
3. **Play-Button drücken**
4. **Story Beat sollte getriggert werden**

---

## 🔍 Prüfen ob Story System funktioniert

### **Checkliste:**

- [ ] `StoryManager` GameObject existiert in Scene
- [ ] `StoryManager` hat **"Story Database"** zugewiesen
- [ ] `StoryUIManager` GameObject existiert
- [ ] `StoryUIManager` hat alle UI-Elemente zugewiesen
- [ ] `CelestialProgressionManager` existiert und funktioniert
- [ ] Console zeigt keine Fehler für Story System

### **Console-Logs prüfen:**

Wenn Story System funktioniert, solltest du bei Level 1 sehen:
```
📚 Story Beat getriggert: 101 - Stella
```

---

## 🐛 Wenn Story UI nicht erscheint

### **Problem 1: StoryManager fehlt**

**Lösung:**
1. Erstelle GameObject: `StoryManager`
2. Füge Component hinzu: `Story Manager`
3. Zuweisungen:
   - **Story Database:** Ziehe `StoryDatabase` Asset
   - **Progression Manager:** Ziehe `CelestialProgressionManager` GameObject
   - **Story UI:** Ziehe `StoryUIManager` GameObject

### **Problem 2: StoryDatabase nicht initialisiert**

**Lösung:**
1. Wähle `StoryDatabase` Asset im Project-Fenster
2. Im Inspector: Rechtsklick → **"Initialize Story Content"**
3. Prüfe ob Chapters und Beats erstellt wurden

### **Problem 3: Story Beats triggern nicht**

**Lösung:**
- Prüfe ob `CelestialProgressionManager.OnLevelUp` Event funktioniert
- Prüfe ob `StoryManager` auf das Event subscribed ist
- Prüfe Console für Fehler

---

## ✅ Zusammenfassung

**Die UI hat sich nicht geändert, weil:**
- Story-UI-Elemente sind **inaktiv** (SetActive = false)
- Sie werden nur bei **Story Events** aktiviert
- Das ist **korrekt** und **gewollt**

**Um die UI zu sehen:**
- Erreiche **Level 1** → Dialog sollte erscheinen
- Oder teste manuell (siehe oben)

---

**Viel Erfolg! 🚀**
