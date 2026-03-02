# LangLe — Product Requirements Document

## 1. Overview

**LangLe** is a simple, fun language-learning web application that helps anyone pick up a new language through bite-sized daily lessons using pictures, sentences, and interactive exercises. Think of it as a lighter, friendlier Duolingo — focused on practical, everyday vocabulary and phrases.

**Launch languages:** Spanish & Telugu (with English as the base language). Users can flip seamlessly between Telugu ↔ English ↔ Spanish translations.

---

## 2. Problem Statement

Learning a new language feels overwhelming. Most people give up because courses are too long, too academic, or too boring. LangLe solves this by keeping lessons short (5–10 minutes), visual, and rewarding — so users build a daily habit without even noticing.

---

## 3. Target Users

| Persona | Description |
|---------|-------------|
| **Casual Learner** | Wants to learn basic Spanish for travel or fun, 5–10 min/day |
| **Beginner Student** | Needs structured daily practice alongside a class |
| **Curious Explorer** | Wants to try a new language with zero commitment pressure |

---

## 4. Core Features

### 4.1 User Authentication & Profile
- Sign up / Log in (email + password)
- User profile with display name and avatar selection
- Choose target language (Spanish for v1)
- Set a daily learning goal (5 / 10 / 15 min per day)

### 4.2 Multi-Language Flip Translation
- **Tri-language support:** Every word/phrase is stored in English, Spanish, and Telugu
- **Flip Cards UI:** Tap/click to flip between languages on any card:
  - Side 1 → English | Side 2 → Spanish | Side 3 → Telugu (swipe or tap to rotate)
- **Language Selector Toggle:** Global toggle in the header to set your "learning from → learning to" pair:
  - English → Spanish, English → Telugu, Telugu → Spanish, Telugu → English, etc.
- Seamless switching — no page reload, instant flip animations

### 4.3 Lessons & Learning Content
- **Topics** organized by real-life themes:
  - Greetings, Food & Drink, Numbers, Colors, Family, Travel, Shopping, Weather, Time, Animals
- Each topic contains **5–8 short lessons**
- Each lesson has **5–10 interactive exercises** mixing:
  - 📸 **Picture Match** — see an image, pick the correct word in target language
  - 📝 **Translate Sentence** — translate between any two of the three languages (multiple choice)
  - 🔊 **Listen & Choose** — hear a word/phrase, pick the right answer
  - 🧩 **Fill in the Blank** — complete a sentence with the missing word
  - 🔤 **Word Scramble** — arrange scrambled letters into the correct word
  - 🔄 **Tri-Flip Challenge** — shown a word in one language, type it in the other two

### 4.4 Rich User Interface & Visual Design
- **Vibrant, playful color palette** — gradient backgrounds (purple → blue → teal), rounded corners, soft shadows
- **Animated mascot character** — a friendly parrot named "LeLe" that reacts to user actions:
  - 🦜 Celebrates correct answers (jumps, confetti)
  - 😢 Encourages on wrong answers ("Try again! You got this!")
  - 💤 Reminds you to practice if you miss a day
- **Flip Card Animations** — smooth 3D CSS flip cards for language switching (English ↔ Spanish ↔ Telugu)
- **Illustrated vocabulary** — every word paired with a colorful cartoon illustration:
  - 🍎 Apple / Manzana / ఆపిల్ with a cute apple drawing
  - 🐕 Dog / Perro / కుక్క with an adorable dog illustration
  - ☀️ Sun / Sol / ఎండ with a smiling sun character
- **Gamification visuals:**
  - 💎 XP crystals that float and collect into a jar
  - 🔥 Animated fire streak counter
  - ⭐ Star ratings (1–3 stars) per lesson based on accuracy
  - 🏅 Shiny animated badges that unlock with a "pop" effect
  - 📊 Colorful progress rings around each topic card
- **Confetti & celebration explosions** on milestones (lesson complete, new streak record, level up)
- **Micro-interactions everywhere:**
  - Buttons with bounce/ripple on tap
  - Cards that tilt slightly on hover (parallax)
  - Progress bars that fill with a liquid animation
  - XP counter that ticks up number-by-number
  - Hearts (lives) that pulse when lost
- **Dark mode / Light mode** with smooth theme transition
- **Fun sound effects** (optional, toggleable):
  - ✅ Correct answer: cheerful "ding!"
  - ❌ Wrong answer: gentle "whomp"
  - 🎉 Lesson complete: trumpet fanfare
  - 🔥 Streak milestone: fire crackle
- **Emoji-rich navigation** — topic cards use large emoji + illustrations as icons
- **Skeleton loading states** with shimmer animation for smooth perceived performance
- **Responsive & touch-friendly** — swipe gestures for card flips on mobile, tap-friendly large buttons

### 4.5 Progress & Streaks
- **Daily streak counter** — consecutive days of completing at least one lesson
- **XP points** earned per correct answer (10 XP each)
- **Topic progress bar** — shows % completion per topic
- **Overall level** — Beginner → Elementary → Intermediate (based on total XP)

### 4.6 Daily Learning Dashboard
- Today's suggested lesson (next incomplete lesson)
- Streak count and XP earned today
- Quick stats: lessons completed, words learned, current level
- **Weekly activity chart** — simple bar chart showing XP earned each day

### 4.7 Goals & Motivation
- Set a personal end goal (e.g., "Learn 200 words", "Complete all Travel lessons")
- Visual goal progress tracker on dashboard
- Simple achievement badges:
  - 🔥 7-Day Streak, 🔥 30-Day Streak
  - 📚 First Topic Complete
  - 🌟 100 Words Learned
  - 🏆 All Topics Complete

### 4.8 Word Bank (Review)
- List of all words/phrases the user has learned
- Each entry shows: Spanish word, English translation, picture (if available)
- Simple flashcard review mode for previously learned words

---

## 5. Technical Architecture

### 5.1 Tech Stack

| Layer | Technology |
|-------|-----------|
| **Orchestration** | .NET Aspire 9.0 (AppHost + ServiceDefaults) |
| **Backend API** | ASP.NET Core Web API (.NET 9) |
| **Frontend** | Blazor Web App (Interactive Server + WebAssembly) |
| **Database** | PostgreSQL (via Aspire `Aspire.Hosting.PostgreSQL`) |
| **ORM** | Entity Framework Core 9 + Npgsql provider |
| **Caching** | Redis (via Aspire `Aspire.Hosting.Redis`) — streaks, leaderboard, session |
| **Auth** | ASP.NET Core Identity + Cookie auth (Blazor-native) |
| **Image Storage** | wwwroot static files (v1), Azure Blob Storage (v2) |
| **CSS / UI** | MudBlazor component library + custom CSS animations |
| **Charts** | MudBlazor Charts (weekly XP bar chart, progress rings) |
| **Animations** | CSS @keyframes + JS interop for confetti/celebrations |
| **Sound Effects** | Howler.js via JS interop (optional, toggleable) |
| **Testing** | xUnit + bUnit (Blazor component tests) + Aspire integration tests |
| **CI/CD** | GitHub Actions |

### 5.2 .NET Aspire Architecture

```
┌─────────────────────────────────────────────┐
│              LangLe.AppHost                  │
│          (.NET Aspire Orchestrator)          │
│                                              │
│  ┌──────────┐  ┌──────────┐  ┌───────────┐ │
│  │PostgreSQL│  │  Redis   │  │ LangLe.Api│ │
│  │ (langdb) │  │ (cache)  │  │  (API)    │ │
│  └────┬─────┘  └────┬─────┘  └─────┬─────┘ │
│       │              │              │        │
│       └──────────────┼──────────────┘        │
│                      │                       │
│              ┌───────┴────────┐              │
│              │  LangLe.Web   │              │
│              │ (Blazor App)  │              │
│              └───────────────┘              │
└─────────────────────────────────────────────┘
```

**AppHost wiring (Program.cs):**
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("langdb");

var redis = builder.AddRedis("cache");

var api = builder.AddProject<Projects.LangLe_Api>("api")
    .WithReference(postgres)
    .WithReference(redis);

builder.AddProject<Projects.LangLe_Web>("web")
    .WithReference(api);

builder.Build().Run();
```

### 5.3 Solution Structure

```
LangLe/
├── LangLe.sln                         # Solution file
├── PRD.md                              # This document
│
├── src/
│   ├── LangLe.AppHost/                # .NET Aspire orchestrator
│   │   └── Program.cs                 # Wires PostgreSQL, Redis, API, Web
│   │
│   ├── LangLe.ServiceDefaults/        # Shared Aspire service config
│   │   └── Extensions.cs             # OpenTelemetry, health checks, resilience
│   │
│   ├── LangLe.Api/                    # ASP.NET Core Web API (.NET 9)
│   │   ├── Controllers/              # REST API controllers
│   │   ├── Services/                  # Business logic services
│   │   ├── Data/                      # EF Core DbContext, migrations
│   │   │   ├── LangLeDbContext.cs
│   │   │   ├── SeedData.cs           # Spanish & Telugu seed content
│   │   │   └── Migrations/
│   │   └── Models/                    # Entity models
│   │
│   ├── LangLe.Web/                    # Blazor Web App (Interactive Server + WASM)
│   │   ├── Components/
│   │   │   ├── Layout/               # MainLayout, NavMenu
│   │   │   ├── Pages/                # Dashboard, Topics, Lesson, WordBank, Profile
│   │   │   └── Shared/               # FlipCard, ProgressRing, XpCounter, Mascot
│   │   ├── Services/                  # HttpClient services to call API
│   │   └── wwwroot/
│   │       ├── css/                   # Custom animations, themes
│   │       ├── images/               # Vocabulary illustrations, mascot, badges
│   │       ├── sounds/               # Sound effects (ding, whomp, fanfare)
│   │       └── js/                    # Confetti, sound interop
│   │
│   └── LangLe.Shared/                # Shared DTOs & contracts
│       ├── DTOs/                      # Request/Response models
│       └── Enums/                     # ExerciseType, GoalType, ThemeMode
│
└── tests/
    ├── LangLe.Api.Tests/             # xUnit API unit & integration tests
    └── LangLe.Web.Tests/             # bUnit Blazor component tests
```

### 5.3 Key Data Models

```
User            → Id, Email, DisplayName, AvatarUrl, DailyGoalMinutes, PreferredTheme, CreatedAt
Language        → Id, Code ("es","te","en"), Name ("Spanish","Telugu","English"), FlagEmoji, ScriptType
Topic           → Id, LanguageGroupId, Name, Description, IconUrl, SortOrder
Lesson          → Id, TopicId, Title, SortOrder
Exercise        → Id, LessonId, Type (enum), Question, CorrectAnswer, Options[], ImageUrl
WordEntry       → Id, TopicId, English, Spanish, Telugu, ImageUrl, AudioUrlEs, AudioUrlTe
UserProgress    → Id, UserId, LessonId, CompletedAt, XpEarned, SourceLang, TargetLang
UserStreak      → Id, UserId, CurrentStreak, LongestStreak, LastActivityDate
UserGoal        → Id, UserId, GoalType, TargetValue, CurrentValue
UserWordBank    → Id, UserId, WordEntryId, LearnedAt, ConfidenceLevel
Achievement     → Id, Name, Description, IconUrl, Criteria
UserAchievement → Id, UserId, AchievementId, UnlockedAt
UserPreference  → Id, UserId, SourceLanguage, TargetLanguage, ThemeMode (light/dark)
```

### 5.4 API Endpoints (Key)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login, returns JWT |
| GET | `/api/profile` | Get current user profile |
| PUT | `/api/profile/goal` | Set daily goal |
| GET | `/api/languages` | List available languages |
| GET | `/api/topics?lang=es` | List topics for a language |
| GET | `/api/topics/{id}/lessons` | List lessons in a topic |
| GET | `/api/lessons/{id}/exercises` | Get exercises for a lesson |
| POST | `/api/lessons/{id}/complete` | Submit lesson results |
| GET | `/api/progress/dashboard` | Dashboard stats |
| GET | `/api/progress/weekly` | Weekly XP chart data |
| GET | `/api/wordbank` | User's learned words |
| GET | `/api/achievements` | User's achievements |

---

## 6. UI Pages

| Page | Description |
|------|-------------|
| **Landing / Login** | Simple sign-up / log-in page |
| **Dashboard** | Daily stats, streak, suggested lesson, weekly chart |
| **Topics** | Grid of topic cards with progress bars |
| **Lesson** | Interactive exercise flow (one exercise at a time, progress bar at top) |
| **Lesson Complete** | Summary screen — XP earned, words learned, streak updated |
| **Word Bank** | Scrollable list of learned words with flashcard review |
| **Profile** | User info, goals, achievements, settings |

---

## 7. Content Strategy (v1 — Spanish)

Start with **10 topics**, each with **5 lessons**, each with **8 exercises** = **400 exercises total**.

Seed data will be included in the project so the app works out of the box with no external content service.

| # | Topic | English | Spanish | Telugu |
|---|-------|---------|---------|--------|
| 1 | Greetings | hello, goodbye, good morning, please, thank you | hola, adiós, buenos días, por favor, gracias | హలో, వీడ్కోలు, శుభోదయం, దయచేసి, ధన్యవాదాలు |
| 2 | Numbers | one, two, three ... ten, hundred | uno, dos, tres ... diez, cien | ఒకటి, రెండు, మూడు ... పది, వంద |
| 3 | Colors | red, blue, green, yellow, black | rojo, azul, verde, amarillo, negro | ఎరుపు, నీలం, ఆకుపచ్చ, పసుపు, నలుపు |
| 4 | Food & Drink | water, bread, milk, apple, coffee | agua, pan, leche, manzana, café | నీళ్ళు, రొట్టె, పాలు, ఆపిల్, కాఫీ |
| 5 | Family | mother, father, brother, sister, son | madre, padre, hermano, hermana, hijo | అమ్మ, నాన్న, అన్న, అక్క, కొడుకు |
| 6 | Animals | dog, cat, bird, fish, horse | perro, gato, pájaro, pez, caballo | కుక్క, పిల్లి, పక్షి, చేప, గుర్రం |
| 7 | Travel | airport, hotel, map, passport, train | aeropuerto, hotel, mapa, pasaporte, tren | విమానాశ్రయం, హోటల్, మ్యాప్, పాస్‌పోర్ట్, రైలు |
| 8 | Shopping | shop, price, money, cheap, expensive | tienda, precio, dinero, barato, caro | దుకాణం, ధర, డబ్బు, చౌక, ఖరీదు |
| 9 | Weather | sun, rain, snow, heat, cold | sol, lluvia, nieve, calor, frío | ఎండ, వర్షం, మంచు, వేడి, చలి |
| 10 | Time | hour, minute, today, tomorrow, yesterday | hora, minuto, hoy, mañana, ayer | గంట, నిమిషం, ఈరోజు, రేపు, నిన్న |

---

## 8. Non-Functional Requirements

- **Performance:** Pages load in < 2 seconds
- **Responsive:** Works on desktop and mobile browsers
- **Offline:** Not required for v1
- **Accessibility:** Basic WCAG compliance (alt text on images, keyboard nav)
- **Extensibility:** Adding a new language = new seed data + language record (no code changes)

---

## 9. Future Enhancements (Out of Scope for v1)

- Additional languages (French, German, Japanese, etc.)
- Speech recognition for pronunciation practice
- Leaderboards and social features
- Spaced repetition algorithm for smarter review
- Mobile native apps (MAUI or React Native)
- AI-generated exercises and conversations
- Premium tier with advanced content

---

## 10. Success Metrics

| Metric | Target |
|--------|--------|
| User completes first lesson | > 80% of sign-ups |
| 7-day retention | > 40% |
| Average session length | 5–10 minutes |
| Lessons completed per user per week | ≥ 5 |

---

*Created: March 2026 | Version: 1.0 | Status: Draft*
