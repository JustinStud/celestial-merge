# Merge Wellness - Game Design Document

**Version:** 1.0  
**Last Updated:** 2024  
**Document Status:** Active Development

---

## Table of Contents

1. [Game Overview](#1-game-overview)
2. [Core Gameplay](#2-core-gameplay)
3. [UI Design](#3-ui-design)
4. [Monetization](#4-monetization)
5. [Content Details](#5-content-details)
6. [Technical Requirements](#6-technical-requirements)
7. [Art Style & Audio](#7-art-style--audio)
8. [Progression Systems](#8-progression-systems)
9. [Social Features](#9-social-features)
10. [Development Roadmap](#10-development-roadmap)

---

## 1. Game Overview

### 1.1 Game Concept
**Merge Wellness** is a casual merge puzzle game that combines the satisfying mechanics of item merging with a wellness and self-care theme. Players merge wellness-related items to create more advanced items, build their wellness sanctuary, and progress through various themed areas.

### 1.2 Genre
- **Primary Genre:** Casual Merge Puzzle
- **Secondary Genres:** Idle/Incremental, Collection

### 1.3 Target Audience
- **Primary:** Ages 12-45
- **Demographics:** 
  - Wellness enthusiasts
  - Casual mobile gamers
  - Players seeking relaxing gameplay
  - Fans of merge/puzzle games
- **Player Motivations:**
  - Stress relief and relaxation
  - Collection and completion
  - Progression and achievement
  - Aesthetic satisfaction

### 1.4 Platforms
- **Primary:** Mobile
  - iOS (iOS 13.0+)
  - Android (Android 8.0+)
- **Future Considerations:**
  - Tablet optimization
  - Potential web version

### 1.5 Unique Selling Points
- Wellness-focused theme promoting mindfulness
- Beautiful, calming visual design
- Non-intrusive monetization (no pay-to-win)
- Daily challenges and events
- Multiple themed areas to explore

---

## 2. Core Gameplay

### 2.1 Core Mechanics

#### 2.1.1 Item Merging
- **Basic Merge:** Combine 2 identical items to create 1 item of the next tier
- **Merge Rules:**
  - Items must be identical to merge
  - Merging requires 2 items minimum
  - Merged items are removed and replaced with the higher-tier item
  - Merge animations provide visual feedback
  - Sound effects confirm successful merges

#### 2.1.2 Grid-Based Inventory
- **Grid Size:** 4x4 (16 slots)
- **Grid Management:**
  - Drag and drop items to rearrange
  - Items can be moved freely within the grid
  - Full grid prevents new item generation
  - Strategic planning required for optimal merges

#### 2.1.3 Resource Gathering
- **Generation Methods:**
  - Time-based generation (items spawn automatically)
  - Tap-to-generate (limited uses, regenerates over time)
  - Merge rewards (merging items may generate bonus resources)
  - Daily rewards
  - Challenge completions

### 2.2 Gameplay Flow

1. **Item Generation:** Items spawn on the grid over time
2. **Grid Management:** Player arranges items strategically
3. **Merging:** Player merges items to create higher-tier items
4. **Progression:** Higher-tier items unlock new areas and content
5. **Collection:** Player builds their wellness sanctuary
6. **Challenges:** Daily and special challenges provide goals

### 2.3 Controls
- **Touch/Drag:** Move items on grid
- **Tap:** Select item (shows merge preview)
- **Long Press:** Item information/details
- **Swipe:** Quick actions (sell item, etc.)

### 2.4 Game Modes

#### 2.4.1 Main Game Mode
- Continuous merge gameplay
- No time limits
- Relaxed, self-paced experience

#### 2.4.2 Daily Challenges
- Time-limited challenges
- Specific objectives (e.g., "Merge 10 yoga mats")
- Rewards upon completion

#### 2.4.3 Special Events
- Limited-time themed events
- Exclusive items and rewards
- Community goals

---

## 3. UI Design

### 3.1 Main Menu Screen

**Layout:**
```
┌─────────────────────────┐
│   [Logo/Title]          │
│   Merge Wellness        │
│                         │
│   ┌─────────────────┐   │
│   │   [PLAY]        │   │
│   └─────────────────┘   │
│                         │
│   ┌─────────────────┐   │
│   │   [INVENTORY]   │   │
│   └─────────────────┘   │
│                         │
│   ┌─────────────────┐   │
│   │   [SETTINGS]    │   │
│   └─────────────────┘   │
│                         │
│   [Daily Challenge]     │
│   [Events]              │
└─────────────────────────┘
```

**Elements:**
- **Play Button:** Large, prominent, leads to gameplay screen
- **Inventory Button:** Access to item collection and management
- **Settings Button:** Game configuration options
- **Daily Challenge Indicator:** Shows current challenge status
- **Event Banner:** Displays active events
- **Currency Display:** Top-right corner (coins, gems)

### 3.2 Gameplay Screen

**Layout:**
```
┌─────────────────────────┐
│ [Back] [Score] [Coins]  │
│                         │
│ ┌─────────────────────┐ │
│ │                     │ │
│ │   4x4 Item Grid     │ │
│ │                     │ │
│ └─────────────────────┘ │
│                         │
│ [Merge Button]          │
│ [Auto-Merge Toggle]     │
│                         │
│ [Area Progress]         │
│ [Next Item Preview]     │
└─────────────────────────┘
```

**Elements:**
- **Top Bar:**
  - Back button (returns to main menu)
  - Current score display
  - Currency counters (coins, gems)
- **Main Grid:** 4x4 item placement area
- **Action Buttons:**
  - Merge button (manual merge trigger)
  - Auto-merge toggle (optional automation)
- **Progress Indicators:**
  - Current area progress bar
  - Next unlockable item preview
  - Achievement notifications

### 3.3 Inventory Screen

**Layout:**
```
┌─────────────────────────┐
│ [Back]    [Collection]  │
│                         │
│ ┌─────────────────────┐ │
│ │  Item Categories   │ │
│ │  [All] [Tier 1-5]  │ │
│ └─────────────────────┘ │
│                         │
│ ┌─────────────────────┐ │
│ │                     │ │
│ │  Scrollable Item    │ │
│ │  Collection Grid    │ │
│ │                     │ │
│ └─────────────────────┘ │
│                         │
│ [Item Details Panel]    │
└─────────────────────────┘
```

**Features:**
- **Item Collection:** View all discovered items
- **Filtering:** Filter by tier, category, or area
- **Item Details:** Tap items to see:
  - Item name and description
  - Tier level
  - Merge requirements
  - Discovery date
  - Collection statistics

### 3.4 Settings Screen

**Layout:**
```
┌─────────────────────────┐
│ [Back]    [Settings]    │
│                         │
│ Sound Effects           │
│ [Toggle: ON/OFF]        │
│                         │
│ Background Music        │
│ [Toggle: ON/OFF]        │
│                         │
│ Language                │
│ [Dropdown: English]     │
│                         │
│ Notifications           │
│ [Toggle: ON/OFF]        │
│                         │
│ ┌─────────────────────┐ │
│ │   [Reset Progress]  │ │
│ └─────────────────────┘ │
│                         │
│ Version: 1.0.0          │
│ [Privacy Policy]        │
│ [Terms of Service]      │
└─────────────────────────┘
```

**Options:**
- **Sound Effects:** Toggle merge/item sounds
- **Background Music:** Toggle ambient music
- **Language:** Multi-language support
- **Notifications:** Daily challenge reminders
- **Reset Progress:** Clear all data (with confirmation)
- **Legal Links:** Privacy policy, terms of service

### 3.5 Visual Design Principles
- **Color Palette:** Calming pastels, nature-inspired greens and blues
- **Typography:** Clean, readable sans-serif fonts
- **Icons:** Rounded, friendly iconography
- **Animations:** Smooth, gentle transitions
- **Feedback:** Visual and audio feedback for all actions

---

## 4. Monetization

### 4.1 Business Model
**Free-to-Play (F2P)** with optional in-app purchases and ad-based rewards.

### 4.2 Core Principles
- **No Pay-to-Win:** All gameplay content accessible without purchases
- **Player-Friendly:** Purchases enhance experience but don't create barriers
- **Transparent:** Clear pricing and value proposition
- **Respectful:** No aggressive monetization tactics

### 4.3 In-App Purchases

#### 4.3.1 Currency Packs
- **Coins:** Used for cosmetic upgrades
- **Gems:** Premium currency for special items
- **Packages:**
  - Starter Pack: $2.99
  - Value Pack: $9.99
  - Premium Pack: $19.99

#### 4.3.2 Cosmetic Items
- **Themes:** Alternative visual themes for the game
- **Item Skins:** Cosmetic variations of items
- **Backgrounds:** Unlockable background themes
- **Price Range:** $0.99 - $4.99

#### 4.3.3 Convenience Items
- **Speed-Ups:** Reduce item generation time (temporary)
- **Extra Grid Slots:** Additional inventory space (permanent)
- **Auto-Merge Pass:** 7-day auto-merge subscription ($4.99)

### 4.4 Ad-Based Rewards

#### 4.4.1 Rewarded Video Ads
- **Double Rewards:** Watch ad to double daily challenge rewards
- **Bonus Items:** Watch ad for bonus item generation
- **Speed Boost:** Watch ad for temporary generation speed increase
- **Frequency:** Limited to prevent ad fatigue (max 5-10 per day)

#### 4.4.2 Optional Interstitial Ads
- **Between Sessions:** Optional ads when returning to game
- **Non-Intrusive:** Player can skip after 5 seconds
- **Frequency:** Maximum once per session

### 4.5 Monetization Balance
- **Free Players:** Can enjoy 100% of gameplay content
- **Paying Players:** Get convenience and cosmetic benefits
- **Ad Watchers:** Receive bonuses without spending money
- **Revenue Targets:** Average revenue per user (ARPU) through volume, not high individual spending

---

## 5. Content Details

### 5.1 Wellness Items (20 Items, Tier 1-5)

#### Tier 1 (Basic Items)
1. **Yoga Mat** - Basic yoga equipment
2. **Meditation Stone** - Simple meditation aid
3. **Essential Oil Bottle** - Aromatherapy starter
4. **Herbal Tea Bag** - Basic wellness drink

#### Tier 2 (Intermediate Items)
5. **Yoga Space** - Merged from 2 Yoga Mats
6. **Meditation Cushion** - Merged from 2 Meditation Stones
7. **Aromatherapy Diffuser** - Merged from 2 Essential Oil Bottles
8. **Wellness Tea Set** - Merged from 2 Herbal Tea Bags

#### Tier 3 (Advanced Items)
9. **Yoga Studio** - Merged from 2 Yoga Spaces
10. **Meditation Garden** - Merged from 2 Meditation Cushions
11. **Wellness Spa** - Merged from 2 Aromatherapy Diffusers
12. **Tea Ceremony Room** - Merged from 2 Wellness Tea Sets

#### Tier 4 (Expert Items)
13. **Wellness Retreat** - Merged from 2 Yoga Studios
14. **Zen Sanctuary** - Merged from 2 Meditation Gardens
15. **Luxury Spa** - Merged from 2 Wellness Spas
16. **Tea House** - Merged from 2 Tea Ceremony Rooms

#### Tier 5 (Master Items)
17. **Wellness Paradise** - Merged from 2 Wellness Retreats
18. **Temple of Peace** - Merged from 2 Zen Sanctuaries
19. **Ultimate Spa Resort** - Merged from 2 Luxury Spas
20. **Grand Tea Palace** - Merged from 2 Tea Houses

### 5.2 Merge Chains

#### Example Chain: Yoga Path
```
Yoga Mat (Tier 1)
    ↓ (2x merge)
Yoga Space (Tier 2)
    ↓ (2x merge)
Yoga Studio (Tier 3)
    ↓ (2x merge)
Wellness Retreat (Tier 4)
    ↓ (2x merge)
Wellness Paradise (Tier 5)
```

#### Example Chain: Meditation Path
```
Meditation Stone (Tier 1)
    ↓ (2x merge)
Meditation Cushion (Tier 2)
    ↓ (2x merge)
Meditation Garden (Tier 3)
    ↓ (2x merge)
Zen Sanctuary (Tier 4)
    ↓ (2x merge)
Temple of Peace (Tier 5)
```

### 5.3 Game Areas (5 Main Areas)

#### Area 1: Serenity Garden
- **Theme:** Nature and tranquility
- **Unlock:** Starting area
- **Items:** Yoga Mat, Meditation Stone
- **Goal:** Create your first Tier 3 item

#### Area 2: Aromatherapy Haven
- **Theme:** Scent and relaxation
- **Unlock:** Complete Area 1
- **Items:** Essential Oil Bottle, Aromatherapy Diffuser
- **Goal:** Merge 5 Tier 2 items

#### Area 3: Tea Ceremony Pavilion
- **Theme:** Traditional wellness
- **Unlock:** Complete Area 2
- **Items:** Herbal Tea Bag, Wellness Tea Set
- **Goal:** Create 2 Tier 4 items

#### Area 4: Spa Oasis
- **Theme:** Luxury and pampering
- **Unlock:** Complete Area 3
- **Items:** All Tier 3+ items
- **Goal:** Complete one full merge chain to Tier 5

#### Area 5: Ultimate Sanctuary
- **Theme:** Master wellness destination
- **Unlock:** Complete Area 4
- **Items:** All Tier 5 items
- **Goal:** Collect all 4 Tier 5 items

### 5.4 Daily Challenges

#### Challenge Types
1. **Merge Master:** Merge X items of a specific type
2. **Speed Runner:** Complete merges within time limit
3. **Collection Quest:** Discover X new items
4. **Chain Builder:** Complete a full merge chain
5. **Grid Master:** Achieve specific grid arrangement

#### Challenge Rewards
- **Completion:** Coins, gems, bonus items
- **Streak Bonus:** Consecutive day completion bonuses
- **Weekly Challenge:** Special weekly objectives with premium rewards

### 5.5 Progression System

#### Experience & Levels
- **XP Gain:** Earned through merges and challenges
- **Level Benefits:** Unlock new areas, increase generation speed
- **Milestone Rewards:** Special rewards at level milestones

#### Achievements
- **Collection Achievements:** Collect all items in a tier
- **Merge Achievements:** Perform X merges
- **Time Achievements:** Play for X days
- **Special Achievements:** Unique challenges and discoveries

---

## 6. Technical Requirements

### 6.1 Performance Targets
- **Frame Rate:** 60 FPS on mid-range devices
- **Load Times:** < 3 seconds initial load
- **Memory Usage:** < 200MB on average
- **Battery Efficiency:** Optimized for extended play sessions

### 6.2 Platform Requirements

#### iOS
- **Minimum:** iOS 13.0
- **Devices:** iPhone 6s and newer, iPad (5th gen) and newer
- **Storage:** 100MB initial, 500MB with all content

#### Android
- **Minimum:** Android 8.0 (API 26)
- **RAM:** 2GB minimum, 3GB recommended
- **Storage:** 100MB initial, 500MB with all content

### 6.3 Technical Features
- **Cloud Save:** Cross-device progress sync
- **Offline Play:** Full functionality without internet
- **Analytics:** Player behavior tracking (privacy-compliant)
- **Crash Reporting:** Automatic error reporting

---

## 7. Art Style & Audio

### 7.1 Visual Art Style
- **Style:** Soft, hand-drawn aesthetic with digital polish
- **Color Scheme:** 
  - Primary: Calming greens (#7FB069, #A8D5BA)
  - Secondary: Soft blues (#6B9BD2, #B8D4E3)
  - Accent: Warm pastels (#F4A261, #E9C46A)
- **Art Direction:** Minimalist, clean, peaceful
- **Item Design:** Rounded, friendly shapes with subtle details

### 7.2 Audio Design

#### Sound Effects
- **Merge Sound:** Satisfying "pop" or "chime"
- **Item Generation:** Gentle "appear" sound
- **UI Interactions:** Soft click/tap sounds
- **Achievement:** Celebratory chime

#### Background Music
- **Style:** Ambient, calming instrumental
- **Tracks:** 
  - Main menu: Peaceful, welcoming
  - Gameplay: Subtle, non-intrusive
  - Areas: Themed variations
- **Volume Control:** Separate sliders for music and SFX

---

## 8. Progression Systems

### 8.1 Player Progression
- **Level System:** 1-100 levels
- **XP Sources:** Merges, challenges, discoveries
- **Level Rewards:** Coins, gems, unlockables

### 8.2 Collection Progression
- **Item Discovery:** Unlock items through merging
- **Collection Book:** Visual catalog of all items
- **Completion Rewards:** Special rewards for full collections

### 8.3 Area Progression
- **Unlock Requirements:** Complete previous area goals
- **Area-Specific Items:** Unique items per area
- **Area Completion:** Unlock next area + bonus rewards

---

## 9. Social Features

### 9.1 Leaderboards
- **Global Leaderboard:** Top players by score
- **Weekly Leaderboard:** Reset weekly with rewards
- **Friends Leaderboard:** Compare with friends

### 9.2 Sharing
- **Screenshot Sharing:** Share progress screenshots
- **Achievement Sharing:** Share milestone achievements
- **Social Media Integration:** Optional sharing to social platforms

### 9.3 Community Events
- **Global Goals:** Community-wide objectives
- **Seasonal Events:** Themed events (e.g., "Spring Wellness")
- **Collaborative Challenges:** Team-based objectives

---

## 10. Development Roadmap

### 10.1 Phase 1: Core Gameplay (Months 1-3)
- Basic merge mechanics
- Grid system
- 10 initial items (Tier 1-3)
- Main menu and gameplay UI
- Basic progression

### 10.2 Phase 2: Content Expansion (Months 4-6)
- Complete item set (20 items, Tier 1-5)
- All 5 game areas
- Daily challenges
- Achievement system
- Polish and optimization

### 10.3 Phase 3: Monetization & Polish (Months 7-9)
- In-app purchase integration
- Ad system implementation
- UI/UX refinements
- Audio implementation
- Beta testing

### 10.4 Phase 4: Launch & Post-Launch (Month 10+)
- Soft launch
- Marketing campaign
- Community features
- Regular content updates
- Event system

---

## Appendix

### A. Glossary
- **Merge:** Combining 2 identical items to create 1 higher-tier item
- **Tier:** Item level/rarity (1-5)
- **Grid:** 4x4 inventory space for items
- **Area:** Themed game section with specific items and goals

### B. Design Pillars
1. **Wellness First:** Promote relaxation and mindfulness
2. **Accessibility:** Easy to learn, enjoyable for all skill levels
3. **Respect:** Respect player time and money
4. **Beauty:** Create a visually pleasing, calming experience

---

**Document End**

*This document is a living document and will be updated as the game evolves during development.*
