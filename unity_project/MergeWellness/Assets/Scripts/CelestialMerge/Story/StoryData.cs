using UnityEngine;
using System.Collections.Generic;

namespace CelestialMerge.Story
{
    /// <summary>
    /// Story Chapter Datenstruktur
    /// </summary>
    [System.Serializable]
    public class StoryChapter
    {
        public int ChapterId;
        public string ChapterTitle;
        [TextArea(3, 5)]
        public string ChapterDescription;
        public int LevelRangeStart;
        public int LevelRangeEnd;
        public List<StoryBeat> Beats = new List<StoryBeat>();
        public string MainCharacter;
        public Sprite ChapterImage;
    }

    /// <summary>
    /// Story Beat (narrativer Event)
    /// </summary>
    [System.Serializable]
    public class StoryBeat
    {
        public int BeatId;
        public int TriggerLevel;
        public string NPCName;
        [TextArea(3, 6)]
        public string DialogText;
        public List<DialogChoice> Choices = new List<DialogChoice>();
        public string LoreCategory;
        public Sprite NPCPortrait;
        public bool IsCompleted = false;
    }

    /// <summary>
    /// Dialog Choice (für Branching Narrative - optional)
    /// </summary>
    [System.Serializable]
    public class DialogChoice
    {
        public string ChoiceText;
        public int RewardStardust = 0;
        public string UnlockCosmetic = "";
        public int NextBeatId = -1; // -1 = Ende Dialog
    }

    /// <summary>
    /// Lore Entry für Encyclopedia
    /// </summary>
    [System.Serializable]
    public class LoreEntry
    {
        public int LoreId;
        public string Title;
        [TextArea(5, 10)]
        public string Content;
        public string Category;
        public bool IsUnlocked = false;
        public CelestialItem RelatedItem; // Optional: Zusammenhang mit Item
        public Sprite LoreImage; // Optional: Bild für Lore Entry
    }
}
