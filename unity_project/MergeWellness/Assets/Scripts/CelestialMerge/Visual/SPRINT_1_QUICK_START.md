# ⚡ Sprint 1 Quick Start - 10 Minuten Setup

## 🎯 Ziel
Spiel sieht professionell aus mit Partikeln, Animationen und Rarity-Farben.

---

## ✅ Schritt 1: DOTween installieren (5 Min)

**WICHTIG:** Für smooth Animationen

1. **Unity Editor** → **Window** → **Package Manager**
2. **"+"** → **"Add package from git URL"**
3. URL: `https://github.com/Demigiant/dotween.git?path=/DOTween`
4. Warte auf Installation

**Alternative:** Asset Store → "DOTween" → Import

**Hinweis:** System funktioniert auch OHNE DOTween (weniger smooth).

---

## ✅ Schritt 2: MergeFeedbackSystem erstellen (2 Min)

1. **Hierarchy** → Rechtsklick → **Create Empty** → Name: `MergeFeedbackSystem`
2. **Inspector** → **Add Component** → `MergeFeedbackSystem` Script

**✅ Fertig!** System funktioniert sofort (erstellt Partikel zur Laufzeit).

---

## ✅ Schritt 3: Item Visual Effects hinzufügen (30 Sek)

### Option A: Editor-Tool (Empfohlen)

1. **Unity Editor** → Menu: `CelestialMerge` → `Visual` → `Setup Item Visual Effects`
2. Klicke: **"🔧 Setup All Items"**
3. **Fertig!**

### Option B: Manuell (falls Tool nicht funktioniert)

1. **Hierarchy** → Finde Item-GameObjects
2. Für jedes Item: **Add Component** → `ItemVisualEffects`

---

## ✅ Schritt 4: Testen (2 Min)

1. **Play** im Editor
2. Führe einen Merge durch
3. **Prüfe:**
   - ✅ Partikel erscheinen
   - ✅ Reward Pop-ups erscheinen
   - ✅ Items haben Rarity-Farben
   - ✅ Epic+ Items haben Glow

---

## 🎉 Fertig!

**Total Zeit:** ~10 Minuten

**Ergebnis:**
- ✅ Professionelle Partikel-Effekte
- ✅ Rarity-basierte Farben
- ✅ Reward Pop-ups
- ✅ Screen Shake bei Epic+ Merges

---

## 🚨 Falls Probleme

### Keine Partikel sichtbar?
- Prüfe: `MergeFeedbackSystem` GameObject existiert
- Prüfe: Console für Fehler

### Items haben keine Farben?
- Verwende Editor-Tool: `CelestialMerge` → `Visual` → `Setup Item Visual Effects`

### DOTween Fehler?
- System funktioniert auch ohne DOTween (alternative Animationen)

---

**Viel Erfolg! 🎮✨**
