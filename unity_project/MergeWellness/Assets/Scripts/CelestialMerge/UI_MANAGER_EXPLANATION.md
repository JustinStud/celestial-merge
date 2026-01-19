# 🎨 UI Manager Erklärung - StoryUIManager vs CelestialUIManager

## ❓ Frage: Ist StoryUIManager dasselbe wie CelestialUIManager?

**Antwort: NEIN!** Es sind **zwei verschiedene Manager** für unterschiedliche Zwecke:

---

## 📋 Die beiden UI Manager:

### **1. StoryUIManager** (für Story System)
- **Zweck:** Verwaltet Story-Dialoge, Chapter-Unlock Screens, Lore-Notifications
- **Verantwortlich für:**
  - DialogPanel (NPC-Dialoge)
  - ChapterUnlockPanel (Chapter-Freischaltung)
  - LoreNotificationPanel (Lore-Benachrichtigungen)
  - Typewriter-Effekt
- **Wann sichtbar:** Nur bei Story Events (Level 1, 5, 10, etc.)

### **2. CelestialUIManager** (für Game UI)
- **Zweck:** Verwaltet alle Game-UI-Elemente (Level, XP, Currency, etc.)
- **Verantwortlich für:**
  - **Level/XP Anzeige** ← **DAS BRAUCHST DU!**
  - Currency (Stardust, Crystals)
  - Progress Bar
  - Daily Quests
  - Mini-Game UI
  - Board Info
  - Merge Results
- **Wann sichtbar:** Immer während des Spiels

---

## ✅ Lösung: CelestialUIManager erstellen

### **Schritt 1: GameObject erstellen**

```
Hierarchy → Rechtsklick → Create Empty
Name: "CelestialUIManager"
```

### **Schritt 2: Component hinzufügen**

1. **Wähle `CelestialUIManager` GameObject**
2. **Im Inspector:**
   - `Add Component`
   - Suche nach: **"Celestial UI Manager"** (oder "CelestialUIManager")
   - Klicke darauf → Component wird hinzugefügt

### **Schritt 3: Game Manager zuweisen**

1. **Im Inspector** bei `CelestialUIManager` Component:
   - **Game Manager:** Ziehe `CelestialGameManager` GameObject hinein
   - (Falls nicht vorhanden: Wird automatisch gefunden)

### **Schritt 4: UI-Elemente zuweisen**

Jetzt kannst du die Level/XP UI-Elemente zuweisen:

1. **Erstelle die UI-Elemente** (siehe `XP_LEVEL_UI_SETUP.md`):
   - `ProgressionPanel`
   - `LevelText`
   - `ChapterText`
   - `XPProgressBar`
   - `XPText`

2. **Ziehe sie in die Felder:**
   - `LevelText` → **Level Text**
   - `ChapterText` → **Chapter Text**
   - `XPProgressBar` → **XP Progress Bar**
   - `XPText` → **XP Text**

---

## 📊 Übersicht: Welcher Manager für was?

| UI-Element | Manager | Zweck |
|------------|---------|-------|
| **Level Text** | CelestialUIManager | Zeigt Player Level |
| **XP Progress Bar** | CelestialUIManager | Zeigt XP-Fortschritt |
| **Stardust Text** | CelestialUIManager | Zeigt Stardust |
| **Dialog Panel** | StoryUIManager | Story-Dialoge |
| **Chapter Unlock Panel** | StoryUIManager | Chapter-Freischaltung |
| **Lore Notification** | StoryUIManager | Lore-Benachrichtigungen |

---

## 🎯 Zusammenfassung

**Du hast:**
- ✅ `StoryUIManager` (für Story)
- ❌ `CelestialUIManager` (für Level/XP) - **MUSS ERSTELLT WERDEN**

**Nächste Schritte:**
1. Erstelle `CelestialUIManager` GameObject
2. Füge `Celestial UI Manager` Component hinzu
3. Erstelle Level/XP UI-Elemente (siehe `XP_LEVEL_UI_SETUP.md`)
4. Weise UI-Elemente im `CelestialUIManager` zu

---

**Viel Erfolg! 🚀**
