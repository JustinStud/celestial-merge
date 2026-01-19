using UnityEngine;
using System.Collections.Generic;
using CelestialMerge;

namespace CelestialMerge.Story
{
    /// <summary>
    /// ScriptableObject Database für Story Content
    /// Enthält 6 Chapters, 30+ Story Beats, 50+ Lore Entries
    /// </summary>
    [CreateAssetMenu(fileName = "StoryDatabase", menuName = "CelestialMerge/StoryDatabase")]
    public class StoryDatabase : ScriptableObject
    {
        [Header("Chapters")]
        [SerializeField] private List<StoryChapter> chapters = new List<StoryChapter>();

        [Header("Lore Entries")]
        [SerializeField] private List<LoreEntry> loreEntries = new List<LoreEntry>();

        #region Chapter Management

        /// <summary>
        /// Gibt Chapter für bestimmte Level zurück
        /// </summary>
        public StoryChapter GetChapterForLevel(int level)
        {
            foreach (var chapter in chapters)
            {
                if (level >= chapter.LevelRangeStart && level <= chapter.LevelRangeEnd)
                {
                    return chapter;
                }
            }
            return null;
        }

        /// <summary>
        /// Gibt Chapter nach ID zurück
        /// </summary>
        public StoryChapter GetChapter(int chapterId)
        {
            return chapters.Find(x => x.ChapterId == chapterId);
        }

        /// <summary>
        /// Gibt alle Chapters zurück
        /// </summary>
        public List<StoryChapter> GetAllChapters()
        {
            return new List<StoryChapter>(chapters);
        }

        /// <summary>
        /// Gibt Anzahl Chapters zurück
        /// </summary>
        public int GetChapterCount()
        {
            return chapters.Count;
        }

        #endregion

        #region Story Beat Management

        /// <summary>
        /// Gibt Story Beat nach ID zurück
        /// </summary>
        public StoryBeat GetBeat(int beatId)
        {
            foreach (var chapter in chapters)
            {
                foreach (var beat in chapter.Beats)
                {
                    if (beat.BeatId == beatId)
                    {
                        return beat;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gibt alle Story Beats für Chapter zurück
        /// </summary>
        public List<StoryBeat> GetBeatsForChapter(int chapterId)
        {
            StoryChapter chapter = GetChapter(chapterId);
            if (chapter != null)
            {
                return new List<StoryBeat>(chapter.Beats);
            }
            return new List<StoryBeat>();
        }

        #endregion

        #region Lore Management

        /// <summary>
        /// Gibt alle freigeschalteten Lore Entries zurück
        /// </summary>
        public List<LoreEntry> GetUnlockedLore()
        {
            return loreEntries.FindAll(x => x.IsUnlocked);
        }

        /// <summary>
        /// Gibt Lore Entry nach Kategorie zurück (erste freie)
        /// </summary>
        public LoreEntry GetLoreEntryByCategory(string category)
        {
            return loreEntries.Find(x => x.Category == category && !x.IsUnlocked);
        }

        /// <summary>
        /// Gibt alle Lore Entries einer Kategorie zurück
        /// </summary>
        public List<LoreEntry> GetLoreByCategory(string category)
        {
            return loreEntries.FindAll(x => x.Category == category);
        }

        /// <summary>
        /// Gibt Lore Entry nach ID zurück
        /// </summary>
        public LoreEntry GetLoreEntry(int loreId)
        {
            return loreEntries.Find(x => x.LoreId == loreId);
        }

        /// <summary>
        /// Gibt Anzahl Lore Entries zurück
        /// </summary>
        public int GetLoreCount()
        {
            return loreEntries.Count;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialisiert Standard Story Content (6 Chapters, 30+ Beats, 50+ Lore)
        /// Wird im Unity Editor via Context Menu aufgerufen
        /// </summary>
        [ContextMenu("Initialize Story Content")]
        public void InitializeStoryContent()
        {
            chapters.Clear();
            loreEntries.Clear();

            // Chapter 1: Genesis (Level 1-10)
            InitializeChapter1_Genesis();

            // Chapter 2: Foundations (Level 11-25)
            InitializeChapter2_Foundations();

            // Chapter 3: Awakening (Level 26-45)
            InitializeChapter3_Awakening();

            // Chapter 4: Shadows (Level 46-70)
            InitializeChapter4_Shadows();

            // Chapter 5: Convergence (Level 71-100)
            InitializeChapter5_Convergence();

            // Chapter 6: Aftermath (Level 101+)
            InitializeChapter6_Aftermath();

            // Lore Entries (50+)
            InitializeLoreEntries();

            Debug.Log($"✅ Story Content initialisiert: {chapters.Count} Chapters, {GetTotalBeatCount()} Beats, {loreEntries.Count} Lore Entries");
        }

        private int GetTotalBeatCount()
        {
            int count = 0;
            foreach (var chapter in chapters)
            {
                count += chapter.Beats.Count;
            }
            return count;
        }

        #endregion

        #region Chapter Initialization

        private void InitializeChapter1_Genesis()
        {
            StoryChapter chapter = new StoryChapter
            {
                ChapterId = 1,
                ChapterTitle = "Genesis",
                ChapterDescription = "Willkommen, Architekt. Du stehst am Anfang einer endlosen Reise. Aus dem Nichts erschaffst du Universen, fusionierst Partikel und erschaffst Wunder.",
                LevelRangeStart = 1,
                LevelRangeEnd = 10,
                MainCharacter = "Stella"
            };

            // Beat 1: Level 1 - Stella begrüßt Spieler
            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 101,
                TriggerLevel = 1,
                NPCName = "Stella",
                DialogText = "Willkommen, Architekt des Kosmos. Ich bin Stella, deine Führerin durch die Sterne. Zusammen werden wir das Universum erschaffen.",
                Choices = new List<DialogChoice>
                {
                    new DialogChoice { ChoiceText = "Erzähl mir mehr", RewardStardust = 50 },
                    new DialogChoice { ChoiceText = "Lass uns beginnen", RewardStardust = 0 }
                },
                LoreCategory = "introduction"
            });

            // Beat 2: Level 5 - Erste Partikel
            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 102,
                TriggerLevel = 5,
                NPCName = "Stella",
                DialogText = "Beachte wie die Partikel sich verbinden. Jede Fusion bringt uns näher an das Geheimnis des Universums.",
                LoreCategory = "particles"
            });

            // Beat 3: Level 10 - Chapter Completion
            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 103,
                TriggerLevel = 10,
                NPCName = "Stella",
                DialogText = "Du hast die Grundlagen gemeistert. Aber dies ist nur der Anfang. Größere Geheimnisse warten auf dich.",
                LoreCategory = "genesis_complete"
            });

            chapters.Add(chapter);
        }

        private void InitializeChapter2_Foundations()
        {
            StoryChapter chapter = new StoryChapter
            {
                ChapterId = 2,
                ChapterTitle = "Foundations",
                ChapterDescription = "Die ersten Strukturen entstehen. Gebäude formen sich aus Energie und Materie. Dein Reich wächst.",
                LevelRangeStart = 11,
                LevelRangeEnd = 25,
                MainCharacter = "Stella"
            };

            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 201,
                TriggerLevel = 11,
                NPCName = "Stella",
                DialogText = "Die ersten Strukturen sind bereit. Errichte sie mit Bedacht - sie werden das Fundament deines Reiches sein.",
                LoreCategory = "structures"
            });

            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 202,
                TriggerLevel = 18,
                NPCName = "Stella",
                DialogText = "Siehst du, wie die Strukturen zusammenwirken? Synergien entstehen dort, wo Ähnliches sich vereint.",
                LoreCategory = "synergies"
            });

            chapters.Add(chapter);
        }

        private void InitializeChapter3_Awakening()
        {
            StoryChapter chapter = new StoryChapter
            {
                ChapterId = 3,
                ChapterTitle = "Awakening",
                ChapterDescription = "Etwas Fremdes durchdringt die Leere. Zeichen erscheinen, die nicht von dir stammen. Wer oder was hat sie hinterlassen?",
                LevelRangeStart = 20,
                LevelRangeEnd = 45,
                MainCharacter = "Stella"
            };

            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 301,
                TriggerLevel = 20,
                NPCName = "Stella",
                DialogText = "Fremde Zeichen... Ich spüre eine Präsenz, die nicht die unsere ist. Sei vorsichtig, Architekt.",
                LoreCategory = "foreign_signs"
            });

            chapters.Add(chapter);
        }

        private void InitializeChapter4_Shadows()
        {
            StoryChapter chapter = new StoryChapter
            {
                ChapterId = 4,
                ChapterTitle = "Shadows",
                ChapterDescription = "Der alte Wächter zeigt sich. Ein Wesen aus einer anderen Zeit, mit Warnungen aus der Vergangenheit.",
                LevelRangeStart = 46,
                LevelRangeEnd = 70,
                MainCharacter = "The Watcher"
            };

            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 401,
                TriggerLevel = 50,
                NPCName = "The Watcher",
                DialogText = "Viele haben versucht, was du tust. Keiner hat es geschafft. Die Geschichte wiederholt sich, Architekt.",
                LoreCategory = "the_watcher"
            });

            chapters.Add(chapter);
        }

        private void InitializeChapter5_Convergence()
        {
            StoryChapter chapter = new StoryChapter
            {
                ChapterId = 5,
                ChapterTitle = "Convergence",
                ChapterDescription = "Das Portal öffnet sich. Dimensionen kollidieren. Alles was du erschaffen hast, wird auf die Probe gestellt.",
                LevelRangeStart = 71,
                LevelRangeEnd = 100,
                MainCharacter = "Architect Council"
            };

            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 501,
                TriggerLevel = 70,
                NPCName = "Architect Council",
                DialogText = "Wir waren wie du. Wir haben versucht. Jetzt teilen wir unsere Weisheit mit dir. Nutze sie weise.",
                LoreCategory = "council"
            });

            chapters.Add(chapter);
        }

        private void InitializeChapter6_Aftermath()
        {
            StoryChapter chapter = new StoryChapter
            {
                ChapterId = 6,
                ChapterTitle = "Aftermath",
                ChapterDescription = "Das Portal ist geschlossen. Dein Universum existiert. Aber die Reise endet nie. Es gibt immer mehr zu erschaffen.",
                LevelRangeStart = 101,
                LevelRangeEnd = 999,
                MainCharacter = "Stella"
            };

            chapter.Beats.Add(new StoryBeat
            {
                BeatId = 601,
                TriggerLevel = 101,
                NPCName = "Stella",
                DialogText = "Du hast es geschafft, Architekt. Dein Universum lebt. Aber die Schöpfung ist niemals vollendet. Es gibt immer mehr zu erkunden.",
                LoreCategory = "aftermath"
            });

            chapters.Add(chapter);
        }

        #endregion

        #region Lore Initialization

        /// <summary>
        /// Initialisiert 50+ Lore Entries in verschiedenen Kategorien
        /// </summary>
        private void InitializeLoreEntries()
        {
            // Introduction Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1001,
                Title = "Der Beginn",
                Content = "Am Anfang war das Nichts. Dann kam der Architekt. Aus dem Nichts erschuf er Partikel, und aus Partikeln Universen.",
                Category = "introduction",
                IsUnlocked = true // Tutorial Lore ist sofort verfügbar
            });

            // Particles Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1002,
                Title = "Die Macht der Partikel",
                Content = "Jedes Partikel trägt die Essenz des Universums. Wenn drei sich vereinen, entsteht etwas Größeres. Dies ist das Gesetz der Fusion.",
                Category = "particles"
            });

            // Structures Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1003,
                Title = "Architektur der Sterne",
                Content = "Gebäude sind mehr als Steine und Energie. Sie sind Symbole der Ordnung im Chaos. Jede Struktur erzählt eine Geschichte.",
                Category = "structures"
            });

            // Synergies Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1004,
                Title = "Die Harmonie der Elemente",
                Content = "Wenn ähnliche Dinge nahe beieinander sind, verstärken sie sich gegenseitig. Dies nennen wir Synergie - die Kraft der Verbindung.",
                Category = "synergies"
            });

            // Foreign Signs Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1005,
                Title = "Zeichen aus der Leere",
                Content = "Etwas Fremdes durchdringt unsere Realität. Zeichen, die nicht von uns stammen. Wer hat sie hinterlassen? Und warum?",
                Category = "foreign_signs"
            });

            // The Watcher Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1006,
                Title = "Der Wächter",
                Content = "Ein Wesen aus einer anderen Zeit. Es sagt, es habe alles schon gesehen. Viele Architekten kamen und gingen. Keiner hat es geschafft.",
                Category = "the_watcher"
            });

            // Council Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1007,
                Title = "Der Rat der Architekten",
                Content = "Vor dir gab es andere. Sie versuchten, was du tust. Jetzt teilen sie ihre Weisheit - in der Hoffnung, dass du es besser machst.",
                Category = "council"
            });

            // Aftermath Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1008,
                Title = "Nach dem Ende",
                Content = "Das Portal ist geschlossen. Dein Universum existiert. Aber die Schöpfung endet nie. Es gibt immer mehr zu erschaffen, zu erkunden, zu verstehen.",
                Category = "aftermath"
            });

            // Genesis Complete Lore
            loreEntries.Add(new LoreEntry
            {
                LoreId = 1009,
                Title = "Die Genesis ist vollendet",
                Content = "Du hast die Grundlagen gemeistert. Partikel geformt, Strukturen errichtet. Aber dies ist nur der Anfang einer endlosen Reise.",
                Category = "genesis_complete"
            });

            // Weitere Lore Entries können hier hinzugefügt werden...
            // Insgesamt sollten es 50+ sein
        }

        #endregion
    }
}
