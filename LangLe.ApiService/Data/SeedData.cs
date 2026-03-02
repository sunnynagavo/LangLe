using System.Text.Json;
using LangLe.ApiService.Models;
using LangLe.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LangLe.ApiService.Data;

public static class SeedData
{
    public static async Task InitializeAsync(LangLeDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.Topics.AnyAsync()) return;

        var topics = CreateTopics();
        db.Topics.AddRange(topics);

        var achievements = CreateAchievements();
        db.Achievements.AddRange(achievements);

        await db.SaveChangesAsync();
    }

    private static List<Topic> CreateTopics()
    {
        var topicData = new (string Name, string Desc, string Emoji, (string En, string Es, string Te, string? Img)[] Words)[]
        {
            ("Greetings", "Say hello in three languages!", "👋", new[]
            {
                ("hello", "hola", "హలో", "👋"), ("goodbye", "adiós", "వీడ్కోలు", "🤚"),
                ("good morning", "buenos días", "శుభోదయం", "🌅"), ("please", "por favor", "దయచేసి", "🙏"),
                ("thank you", "gracias", "ధన్యవాదాలు", "🙏"), ("how are you", "¿cómo estás?", "మీరు ఎలా ఉన్నారు?", "😊"),
                ("good night", "buenas noches", "శుభ రాత్రి", "🌙"), ("welcome", "bienvenido", "స్వాగతం", "🤗"),
                ("sorry", "lo siento", "క్షమించండి", "😔"), ("yes", "sí", "అవును", "✅")
            }),
            ("Numbers", "Count from one to ten and beyond!", "🔢", new[]
            {
                ("one", "uno", "ఒకటి", "1️⃣"), ("two", "dos", "రెండు", "2️⃣"),
                ("three", "tres", "మూడు", "3️⃣"), ("four", "cuatro", "నాలుగు", "4️⃣"),
                ("five", "cinco", "ఐదు", "5️⃣"), ("six", "seis", "ఆరు", "6️⃣"),
                ("seven", "siete", "ఏడు", "7️⃣"), ("eight", "ocho", "ఎనిమిది", "8️⃣"),
                ("nine", "nueve", "తొమ్మిది", "9️⃣"), ("ten", "diez", "పది", "🔟")
            }),
            ("Colors", "Learn all the colors of the rainbow!", "🎨", new[]
            {
                ("red", "rojo", "ఎరుపు", "🔴"), ("blue", "azul", "నీలం", "🔵"),
                ("green", "verde", "ఆకుపచ్చ", "🟢"), ("yellow", "amarillo", "పసుపు", "🟡"),
                ("black", "negro", "నలుపు", "⚫"), ("white", "blanco", "తెలుపు", "⚪"),
                ("orange", "naranja", "నారింజ", "🟠"), ("purple", "morado", "ఊదా", "🟣"),
                ("pink", "rosa", "గులాబీ", "🩷"), ("brown", "marrón", "గోధుమ", "🟤")
            }),
            ("Food & Drink", "Delicious words to know!", "🍕", new[]
            {
                ("water", "agua", "నీళ్ళు", "💧"), ("bread", "pan", "రొట్టె", "🍞"),
                ("milk", "leche", "పాలు", "🥛"), ("apple", "manzana", "ఆపిల్", "🍎"),
                ("coffee", "café", "కాఫీ", "☕"), ("rice", "arroz", "అన్నం", "🍚"),
                ("egg", "huevo", "గుడ్డు", "🥚"), ("chicken", "pollo", "కోడి", "🍗"),
                ("fish", "pescado", "చేప", "🐟"), ("fruit", "fruta", "పండు", "🍇")
            }),
            ("Family", "Meet the family!", "👨‍👩‍👧‍👦", new[]
            {
                ("mother", "madre", "అమ్మ", "👩"), ("father", "padre", "నాన్న", "👨"),
                ("brother", "hermano", "అన్న", "👦"), ("sister", "hermana", "అక్క", "👧"),
                ("son", "hijo", "కొడుకు", "👶"), ("daughter", "hija", "కూతురు", "👶"),
                ("grandfather", "abuelo", "తాత", "👴"), ("grandmother", "abuela", "నానమ్మ", "👵"),
                ("uncle", "tío", "మామ", "👨"), ("aunt", "tía", "పిన్ని", "👩")
            }),
            ("Animals", "Adorable creatures everywhere!", "🐾", new[]
            {
                ("dog", "perro", "కుక్క", "🐕"), ("cat", "gato", "పిల్లి", "🐈"),
                ("bird", "pájaro", "పక్షి", "🐦"), ("fish", "pez", "చేప", "🐠"),
                ("horse", "caballo", "గుర్రం", "🐴"), ("cow", "vaca", "ఆవు", "🐄"),
                ("elephant", "elefante", "ఏనుగు", "🐘"), ("monkey", "mono", "కోతి", "🐒"),
                ("rabbit", "conejo", "కుందేలు", "🐇"), ("lion", "león", "సింహం", "🦁")
            }),
            ("Travel", "Explore the world!", "✈️", new[]
            {
                ("airport", "aeropuerto", "విమానాశ్రయం", "✈️"), ("hotel", "hotel", "హోటల్", "🏨"),
                ("map", "mapa", "మ్యాప్", "🗺️"), ("passport", "pasaporte", "పాస్‌పోర్ట్", "📕"),
                ("train", "tren", "రైలు", "🚂"), ("bus", "autobús", "బస్సు", "🚌"),
                ("taxi", "taxi", "టాక్సీ", "🚕"), ("ticket", "boleto", "టిక్కెట్", "🎫"),
                ("beach", "playa", "బీచ్", "🏖️"), ("mountain", "montaña", "కొండ", "⛰️")
            }),
            ("Shopping", "Time to shop!", "🛍️", new[]
            {
                ("shop", "tienda", "దుకాణం", "🏪"), ("price", "precio", "ధర", "💲"),
                ("money", "dinero", "డబ్బు", "💰"), ("cheap", "barato", "చౌక", "👍"),
                ("expensive", "caro", "ఖరీదు", "💎"), ("buy", "comprar", "కొనుక్కోవడం", "🛒"),
                ("sell", "vender", "అమ్మడం", "🏷️"), ("market", "mercado", "మార్కెట్", "🏬"),
                ("bag", "bolsa", "సంచి", "👜"), ("size", "tamaño", "సైజు", "📏")
            }),
            ("Weather", "What's the weather like?", "⛅", new[]
            {
                ("sun", "sol", "ఎండ", "☀️"), ("rain", "lluvia", "వర్షం", "🌧️"),
                ("snow", "nieve", "మంచు", "❄️"), ("hot", "calor", "వేడి", "🔥"),
                ("cold", "frío", "చలి", "🥶"), ("wind", "viento", "గాలి", "💨"),
                ("cloud", "nube", "మేఘం", "☁️"), ("storm", "tormenta", "తుఫాను", "⛈️"),
                ("rainbow", "arcoíris", "ఇంద్రధనుస్సు", "🌈"), ("temperature", "temperatura", "ఉష్ణోగ్రత", "🌡️")
            }),
            ("Time", "Every moment counts!", "⏰", new[]
            {
                ("hour", "hora", "గంట", "🕐"), ("minute", "minuto", "నిమిషం", "⏱️"),
                ("today", "hoy", "ఈరోజు", "📅"), ("tomorrow", "mañana", "రేపు", "📆"),
                ("yesterday", "ayer", "నిన్న", "📋"), ("morning", "mañana", "ఉదయం", "🌅"),
                ("afternoon", "tarde", "మధ్యాహ్నం", "🌤️"), ("night", "noche", "రాత్రి", "🌙"),
                ("week", "semana", "వారం", "📊"), ("month", "mes", "నెల", "🗓️")
            })
        };

        var topics = new List<Topic>();
        for (int t = 0; t < topicData.Length; t++)
        {
            var (name, desc, emoji, words) = topicData[t];
            var topic = new Topic
            {
                Name = name,
                Description = desc,
                IconEmoji = emoji,
                SortOrder = t + 1,
                WordEntries = words.Select(w => new WordEntry
                {
                    English = w.En, Spanish = w.Es, Telugu = w.Te, ImageUrl = w.Img
                }).ToList(),
                Lessons = CreateLessonsForTopic(words, name)
            };
            topics.Add(topic);
        }
        return topics;
    }

    private static List<Lesson> CreateLessonsForTopic(
        (string En, string Es, string Te, string? Img)[] words, string topicName)
    {
        var lessons = new List<Lesson>();
        int wordsPerLesson = 2;
        int lessonIndex = 0;

        for (int i = 0; i < words.Length; i += wordsPerLesson)
        {
            lessonIndex++;
            var lessonWords = words.Skip(i).Take(wordsPerLesson).ToArray();
            var allWords = words;

            var exercises = new List<Exercise>();

            foreach (var w in lessonWords)
            {
                // PictureMatch: show emoji, pick correct English word
                var wrongOptions = allWords.Where(x => x.En != w.En).OrderBy(_ => Random.Shared.Next()).Take(3)
                    .Select(x => x.Es).ToList();
                wrongOptions.Add(w.Es);
                exercises.Add(new Exercise
                {
                    Type = ExerciseType.PictureMatch,
                    Question = $"{w.Img} What is this in Spanish?",
                    CorrectAnswer = w.Es,
                    OptionsJson = JsonSerializer.Serialize(wrongOptions.OrderBy(_ => Random.Shared.Next())),
                    ImageUrl = w.Img,
                    HintText = w.En
                });

                // TranslateSentence: En → Es
                var wrongTrans = allWords.Where(x => x.En != w.En).OrderBy(_ => Random.Shared.Next()).Take(3)
                    .Select(x => x.Es).ToList();
                wrongTrans.Add(w.Es);
                exercises.Add(new Exercise
                {
                    Type = ExerciseType.TranslateSentence,
                    Question = $"Translate to Spanish: \"{w.En}\"",
                    CorrectAnswer = w.Es,
                    OptionsJson = JsonSerializer.Serialize(wrongTrans.OrderBy(_ => Random.Shared.Next()))
                });

                // FillInTheBlank
                exercises.Add(new Exercise
                {
                    Type = ExerciseType.FillInTheBlank,
                    Question = $"The Spanish word for \"{w.En}\" is ____",
                    CorrectAnswer = w.Es,
                    OptionsJson = "[]",
                    HintText = $"Starts with '{w.Es[0]}'"
                });

                // TriFlipChallenge: show Telugu, type Spanish
                exercises.Add(new Exercise
                {
                    Type = ExerciseType.TriFlipChallenge,
                    Question = $"🇮🇳 Telugu: {w.Te} → What is this in Spanish?",
                    CorrectAnswer = w.Es,
                    OptionsJson = "[]",
                    HintText = $"In English: {w.En}"
                });
            }

            lessons.Add(new Lesson
            {
                Title = $"{topicName} - Lesson {lessonIndex}",
                SortOrder = lessonIndex,
                Exercises = exercises
            });
        }

        return lessons;
    }

    private static List<Achievement> CreateAchievements() =>
    [
        new() { Name = "First Steps", Description = "Complete your first lesson", IconEmoji = "🎯", CriteriaType = "lessons_completed", CriteriaValue = 1 },
        new() { Name = "Getting Started", Description = "Complete 5 lessons", IconEmoji = "📖", CriteriaType = "lessons_completed", CriteriaValue = 5 },
        new() { Name = "Dedicated Learner", Description = "Complete 25 lessons", IconEmoji = "📚", CriteriaType = "lessons_completed", CriteriaValue = 25 },
        new() { Name = "Week Warrior", Description = "Maintain a 7-day streak", IconEmoji = "🔥", CriteriaType = "streak", CriteriaValue = 7 },
        new() { Name = "Monthly Master", Description = "Maintain a 30-day streak", IconEmoji = "🔥", CriteriaType = "streak", CriteriaValue = 30 },
        new() { Name = "Word Collector", Description = "Learn 50 words", IconEmoji = "🌟", CriteriaType = "words_learned", CriteriaValue = 50 },
        new() { Name = "Vocabulary King", Description = "Learn 100 words", IconEmoji = "👑", CriteriaType = "words_learned", CriteriaValue = 100 },
        new() { Name = "Topic Explorer", Description = "Complete your first topic", IconEmoji = "🗺️", CriteriaType = "topics_completed", CriteriaValue = 1 },
        new() { Name = "All Star", Description = "Complete all topics", IconEmoji = "🏆", CriteriaType = "topics_completed", CriteriaValue = 10 },
        new() { Name = "XP Hunter", Description = "Earn 1000 XP", IconEmoji = "💎", CriteriaType = "total_xp", CriteriaValue = 1000 }
    ];
}
