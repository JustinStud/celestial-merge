# 🎨 Sprite Setup Guide - Items visuell verbessern

## Problem: Items sind alle grau

Die Items werden aktuell als graue Quadrate angezeigt, weil keine Sprites zugewiesen sind. Hier ist, wie du das beheben kannst:

## ✅ Lösung 1: Sprites zu Items hinzufügen (Empfohlen)

### Schritt 1: Sprites erstellen/importieren

1. **Erstelle oder importiere Sprites:**
   - Unity → `Assets` → Rechtsklick → `Create` → `Folder` → Name: `Sprites`
   - Füge deine Sprite-Bilder hinzu (PNG, JPG, etc.)
   - Unity konvertiert sie automatisch zu Sprites

2. **Sprite-Einstellungen:**
   - Wähle ein Sprite-Bild im Project-Fenster
   - Im Inspector: `Texture Type` = `Sprite (2D and UI)`
   - Klicke `Apply`

### Schritt 2: Sprites zu CelestialItemDatabase zuweisen

1. **Öffne CelestialItemDatabase Asset:**
   - Project → Finde `CelestialItemDatabase` Asset
   - Wähle es aus

2. **Im Inspector:**
   - Du siehst alle Items in der Liste
   - Für jedes Item gibt es ein `Item Sprite` Feld
   - **Ziehe deine Sprites** aus dem Project-Fenster in die entsprechenden Felder

### Schritt 3: Alternative - Code-basierte Zuweisung

Falls du viele Items hast, kannst du auch im Code Sprites zuweisen:

```csharp
// In CelestialItemDatabase.cs, in der AddItem-Methode:
public void AddItem(string id, string name, int level, string category, 
    ItemRarity rarity, string lore, int stardust, int crystal, int xp, 
    Sprite sprite = null) // Sprite-Parameter hinzufügen
{
    // ... existierender Code ...
    itemData.itemSprite = sprite; // Sprite zuweisen
}
```

## ✅ Lösung 2: Bessere Farben für Rarity (Bereits implementiert)

Die Rarity-Farben wurden bereits verbessert:
- **Common**: Hellgrau (0.8, 0.8, 0.8) - jetzt sichtbarer
- **Uncommon**: Grün
- **Rare**: Blau
- **Epic**: Lila
- **Legendary**: Orange
- **Mythic**: Rot

## ✅ Lösung 3: Temporäre Sprite-Generierung (Bereits implementiert)

Das System erstellt jetzt automatisch einfarbige Sprites basierend auf Rarity, wenn kein Sprite zugewiesen ist. Die Items sollten jetzt **farbige Quadrate** sein statt grau.

## 🔍 Debugging: Warum sind Items immer noch grau?

1. **Prüfe ItemImage:**
   - Wähle einen Slot während Play-Mode
   - Prüfe `ItemImage` Component
   - `Color` sollte nicht grau sein (außer Common Items)
   - `Enabled` sollte `true` sein

2. **Prüfe Rarity:**
   - Alle Starter-Items sind `Common` → daher grau
   - Das ist normal! Höhere Rarity = bessere Farben

3. **Force Update:**
   - Stoppe Play-Mode
   - Lösche alle Slots
   - Starte Play-Mode neu

## 📋 Checkliste

- [ ] Sprites erstellt/importiert
- [ ] Sprites zu CelestialItemDatabase zugewiesen
- [ ] Items haben jetzt farbige Quadrate (basierend auf Rarity)
- [ ] Common Items = Hellgrau (normal)
- [ ] Höhere Rarity = bessere Farben

## 🎯 Nächste Schritte

1. **Erstelle einfache Sprites** für jedes Item (kannst auch Placeholder verwenden)
2. **Weise sie zu** in CelestialItemDatabase
3. **Teste** - Items sollten jetzt Sprites haben statt farbige Quadrate

Viel Erfolg! 🚀
