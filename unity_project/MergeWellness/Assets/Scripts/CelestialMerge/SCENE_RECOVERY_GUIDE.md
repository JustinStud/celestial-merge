# 🔄 Scene Recovery Guide - Deine GameObjects wiederfinden

## Problem: Scene ist leer (nur Main Camera + Directional Light)

**Ursache:** Unity hat eine neue "Untitled" Scene geöffnet statt deiner Gameplay-Scene.

---

## ✅ Lösung: Richtige Scene öffnen

### **Schritt 1: Scene-Datei finden**

Du hast **2 Scene-Dateien** in deinem Projekt:
- `Assets/Gameplay.unity` 
- `Assets/Gameplay_v2.unity`

**Eine davon enthält deine GameObjects!**

### **Schritt 2: Scene öffnen**

**Option A: Über Project-Fenster**
1. Im **Project-Fenster** (unten links) navigiere zu `Assets/`
2. Suche nach `Gameplay.unity` oder `Gameplay_v2.unity`
3. **Doppelklick** auf die Scene-Datei
4. Unity fragt: "Save current scene?" → **"Don't Save"** (wenn du nichts geändert hast)

**Option B: Über File-Menü**
1. **File** → **Open Scene**
2. Navigiere zu `Assets/Gameplay.unity` oder `Assets/Gameplay_v2.unity`
3. Öffne die Datei

**Option C: Über Scene-Tab**
1. Oben in Unity siehst du den **Scene-Tab** (wahrscheinlich "Untitled")
2. Klicke auf den **Dropdown-Pfeil** neben "Untitled"
3. Wähle `Gameplay` oder `Gameplay_v2` aus der Liste

---

## 📋 Was sollte in deiner Scene sein?

Nach dem Öffnen der richtigen Scene solltest du diese GameObjects sehen:

### **Kern-Manager:**
- ✅ `CelestialGameManager` (oder `GameplayManager`)
- ✅ `CurrencyManager`
- ✅ `CelestialProgressionManager`
- ✅ `CelestialMergeManager`
- ✅ `ExpandableBoardManager`
- ✅ `DailySystemManager`
- ✅ `IdleProductionManager`
- ✅ `CraftingSystem`
- ✅ `ItemSynergySystem`
- ✅ `MiniGameManager`

### **UI-Elemente:**
- ✅ `Canvas` (mit UI-Elementen)
- ✅ `EventSystem` (automatisch erstellt)
- ✅ `BoardParent` (für ExpandableBoardManager)
- ✅ `SlotPrefab` (Prefab für Board-Slots)

### **Optional:**
- ✅ `CelestialItemSpawner` (zum Testen)
- ✅ `AudioListenerManager`
- ✅ `MenuManager`

---

## 🔍 Wenn die Scene immer noch leer ist

### **Möglichkeit 1: Scene wurde nicht gespeichert**
- Leider sind die GameObjects dann verloren
- Du musst sie neu erstellen (siehe unten)

### **Möglichkeit 2: Scene ist in einem anderen Ordner**
- Suche im Project-Fenster nach `*.unity` Dateien
- Prüfe auch `Assets/Scenes/` Ordner (falls vorhanden)

### **Möglichkeit 3: Unity Safe Mode Problem**
- Wenn Unity im Safe Mode startet, können Scenes nicht richtig geladen werden
- **Lösung:** Behebe alle Kompilierfehler (siehe unten)

---

## 🛠️ Scene neu aufbauen (falls nötig)

Falls deine GameObjects wirklich weg sind, hier die **Quick-Setup Checkliste**:

### **Schritt 1: Manager-GameObjects erstellen**

Für jedes System ein GameObject:

```
Hierarchy → Rechtsklick → Create Empty
Name: "CurrencyManager"
Add Component → CurrencyManager
```

**Erstelle diese GameObjects:**
1. `CurrencyManager` → `CurrencyManager.cs`
2. `CelestialProgressionManager` → `CelestialProgressionManager.cs`
3. `CelestialMergeManager` → `CelestialMergeManager.cs`
4. `ExpandableBoardManager` → `ExpandableBoardManager.cs`
5. `DailySystemManager` → `DailySystemManager.cs`
6. `IdleProductionManager` → `IdleProductionManager.cs`
7. `CraftingSystem` → `CraftingSystem.cs`
8. `ItemSynergySystem` → `ItemSynergySystem.cs`
9. `MiniGameManager` → `MiniGameManager.cs`
10. `CelestialGameManager` → `CelestialGameManager.cs`

### **Schritt 2: ExpandableBoardManager konfigurieren**

1. Wähle `ExpandableBoardManager` GameObject
2. Im Inspector:
   - **Slot Prefab:** Erstelle ein Prefab oder ziehe vorhandenes
   - **Board Parent:** Erstelle UI Panel → `BoardParent`
   - **Grid Layout:** Wird automatisch erstellt
   - **Progression Manager:** Ziehe `CelestialProgressionManager` GameObject

### **Schritt 3: CelestialGameManager konfigurieren**

1. Wähle `CelestialGameManager` GameObject
2. Im Inspector:
   - **Auto Initialize:** ✅
   - **Debug Mode:** ✅

### **Schritt 4: Scene speichern**

1. **File** → **Save As**
2. Name: `Gameplay` oder `Gameplay_v2`
3. Speichere in `Assets/`

---

## ⚠️ Warnungen beheben (Optional)

Die Warnungen sind nicht kritisch, aber hier die Fixes:

### **Warning 1: MenuManager.pauseOnStart**

**Datei:** `Assets/Scripts/MenuManager.cs` Zeile 18

**Fix:** Entferne oder verwende das Feld:

```csharp
// Option 1: Entfernen
// [SerializeField] private bool pauseOnStart = false;

// Option 2: Verwenden
private void Start()
{
    if (pauseOnStart)
    {
        Time.timeScale = 0f;
    }
    // ... rest of code
}
```

### **Warning 2: GridManager.gridHeight**

**Datei:** `Assets/Scripts/GridManager.cs` Zeile 14

**Fix:** Entferne oder verwende das Feld (falls es nicht gebraucht wird).

---

## ✅ Finale Checkliste

- [ ] Richtige Scene geöffnet (`Gameplay.unity` oder `Gameplay_v2.unity`)
- [ ] Alle Manager-GameObjects vorhanden
- [ ] ExpandableBoardManager konfiguriert
- [ ] CelestialGameManager konfiguriert
- [ ] Scene gespeichert
- [ ] Spiel getestet (Play-Button)

---

## 🎮 Testen

1. **Play-Button drücken**
2. **Console öffnen** (Window → General → Console)
3. **Sollte sehen:**
   ```
   ✅ Celestial Merge - Initialisierung
   ✅ Spiel erfolgreich initialisiert!
   ```

---

**Viel Erfolg! 🚀**
