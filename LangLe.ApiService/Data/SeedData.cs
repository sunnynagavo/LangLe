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
                ("sorry", "lo siento", "క్షమించండి", "😔"), ("yes", "sí", "అవును", "✅"),
                ("no", "no", "కాదు", "❌"), ("nice to meet you", "mucho gusto", "మిమ్మల్ని కలిసి సంతోషం", "🤝"),
                ("see you later", "hasta luego", "తర్వాత కలుద్దాం", "👋"), ("good afternoon", "buenas tardes", "శుభ మధ్యాహ్నం", "🌤️"),
                ("excuse me", "disculpe", "క్షమించండి", "🙇"), ("of course", "por supuesto", "తప్పకుండా", "💯"),
                ("congratulations", "felicidades", "అభినందనలు", "🎉"), ("good luck", "buena suerte", "శుభం కలగాలి", "🍀"),
                ("cheers", "salud", "ఆరోగ్యం", "🥂"), ("take care", "cuídate", "జాగ్రత్తగా ఉండు", "💙")
            }),
            ("Numbers", "Count from one to ten and beyond!", "🔢", new[]
            {
                ("one", "uno", "ఒకటి", "1️⃣"), ("two", "dos", "రెండు", "2️⃣"),
                ("three", "tres", "మూడు", "3️⃣"), ("four", "cuatro", "నాలుగు", "4️⃣"),
                ("five", "cinco", "ఐదు", "5️⃣"), ("six", "seis", "ఆరు", "6️⃣"),
                ("seven", "siete", "ఏడు", "7️⃣"), ("eight", "ocho", "ఎనిమిది", "8️⃣"),
                ("nine", "nueve", "తొమ్మిది", "9️⃣"), ("ten", "diez", "పది", "🔟"),
                ("eleven", "once", "పదకొండు", "🔢"), ("twelve", "doce", "పన్నెండు", "🔢"),
                ("twenty", "veinte", "ఇరవై", "🔢"), ("thirty", "treinta", "ముప్పై", "🔢"),
                ("fifty", "cincuenta", "యాభై", "🔢"), ("hundred", "cien", "వంద", "💯"),
                ("thousand", "mil", "వెయ్యి", "🔢"), ("first", "primero", "మొదటి", "🥇"),
                ("second", "segundo", "రెండవ", "🥈"), ("third", "tercero", "మూడవ", "🥉")
            }),
            ("Colors", "Learn all the colors of the rainbow!", "🎨", new[]
            {
                ("red", "rojo", "ఎరుపు", "🔴"), ("blue", "azul", "నీలం", "🔵"),
                ("green", "verde", "ఆకుపచ్చ", "🟢"), ("yellow", "amarillo", "పసుపు", "🟡"),
                ("black", "negro", "నలుపు", "⚫"), ("white", "blanco", "తెలుపు", "⚪"),
                ("orange", "naranja", "నారింజ", "🟠"), ("purple", "morado", "ఊదా", "🟣"),
                ("pink", "rosa", "గులాబీ", "🩷"), ("brown", "marrón", "గోధుమ", "🟤"),
                ("gray", "gris", "బూడిద", "🩶"), ("gold", "dorado", "బంగారం", "🌟"),
                ("silver", "plateado", "వెండి", "🪙"), ("light blue", "celeste", "లేత నీలం", "💎"),
                ("dark", "oscuro", "చీకటి", "🌑"), ("light", "claro", "వెలుతురు", "💡"),
                ("bright", "brillante", "ప్రకాశవంతం", "✨"), ("colorful", "colorido", "రంగులమయం", "🌈"),
                ("navy blue", "azul marino", "నేవీ నీలం", "🫐"), ("turquoise", "turquesa", "పచ్చని నీలం", "🧊")
            }),
            ("Food & Drink", "Delicious words to know!", "🍕", new[]
            {
                ("water", "agua", "నీళ్ళు", "💧"), ("bread", "pan", "రొట్టె", "🍞"),
                ("milk", "leche", "పాలు", "🥛"), ("apple", "manzana", "ఆపిల్", "🍎"),
                ("coffee", "café", "కాఫీ", "☕"), ("rice", "arroz", "అన్నం", "🍚"),
                ("egg", "huevo", "గుడ్డు", "🥚"), ("chicken", "pollo", "కోడి", "🍗"),
                ("fish", "pescado", "చేప", "🐟"), ("fruit", "fruta", "పండు", "🍇"),
                ("cheese", "queso", "చీజ్", "🧀"), ("meat", "carne", "మాంసం", "🥩"),
                ("soup", "sopa", "సూప్", "🍲"), ("salad", "ensalada", "సలాడ్", "🥗"),
                ("sugar", "azúcar", "పంచదార", "🍬"), ("salt", "sal", "ఉప్పు", "🧂"),
                ("beer", "cerveza", "బీర్", "🍺"), ("wine", "vino", "వైన్", "🍷"),
                ("ice cream", "helado", "ఐస్ క్రీం", "🍦"), ("cake", "pastel", "కేక్", "🎂")
            }),
            ("Family", "Meet the family!", "👨‍👩‍👧‍👦", new[]
            {
                ("mother", "madre", "అమ్మ", "👩"), ("father", "padre", "నాన్న", "👨"),
                ("brother", "hermano", "అన్న", "👦"), ("sister", "hermana", "అక్క", "👧"),
                ("son", "hijo", "కొడుకు", "👶"), ("daughter", "hija", "కూతురు", "👶"),
                ("grandfather", "abuelo", "తాత", "👴"), ("grandmother", "abuela", "నానమ్మ", "👵"),
                ("uncle", "tío", "మామ", "👨"), ("aunt", "tía", "పిన్ని", "👩"),
                ("cousin", "primo", "బంధువు", "🧑"), ("husband", "esposo", "భర్త", "💑"),
                ("wife", "esposa", "భార్య", "💑"), ("baby", "bebé", "బిడ్డ", "👶"),
                ("family", "familia", "కుటుంబం", "👨‍👩‍👧‍👦"), ("parents", "padres", "తల్లిదండ్రులు", "👫"),
                ("children", "hijos", "పిల్లలు", "👧👦"), ("nephew", "sobrino", "మేనల్లుడు", "👦"),
                ("niece", "sobrina", "మేనకోడలు", "👧"), ("godfather", "padrino", "గాడ్‌ఫాదర్", "🤵")
            }),
            ("Animals", "Adorable creatures everywhere!", "🐾", new[]
            {
                ("dog", "perro", "కుక్క", "🐕"), ("cat", "gato", "పిల్లి", "🐈"),
                ("bird", "pájaro", "పక్షి", "🐦"), ("fish", "pez", "చేప", "🐠"),
                ("horse", "caballo", "గుర్రం", "🐴"), ("cow", "vaca", "ఆవు", "🐄"),
                ("elephant", "elefante", "ఏనుగు", "🐘"), ("monkey", "mono", "కోతి", "🐒"),
                ("rabbit", "conejo", "కుందేలు", "🐇"), ("lion", "león", "సింహం", "🦁"),
                ("tiger", "tigre", "పులి", "🐅"), ("bear", "oso", "ఎలుగుబంటి", "🐻"),
                ("snake", "serpiente", "పాము", "🐍"), ("butterfly", "mariposa", "సీతాకోకచిలుక", "🦋"),
                ("frog", "rana", "కప్ప", "🐸"), ("turtle", "tortuga", "తాబేలు", "🐢"),
                ("sheep", "oveja", "గొర్రె", "🐑"), ("pig", "cerdo", "పంది", "🐷"),
                ("duck", "pato", "బాతు", "🦆"), ("wolf", "lobo", "తోడేలు", "🐺")
            }),
            ("Travel", "Explore the world!", "✈️", new[]
            {
                ("airport", "aeropuerto", "విమానాశ్రయం", "✈️"), ("hotel", "hotel", "హోటల్", "🏨"),
                ("map", "mapa", "మ్యాప్", "🗺️"), ("passport", "pasaporte", "పాస్‌పోర్ట్", "📕"),
                ("train", "tren", "రైలు", "🚂"), ("bus", "autobús", "బస్సు", "🚌"),
                ("taxi", "taxi", "టాక్సీ", "🚕"), ("ticket", "boleto", "టిక్కెట్", "🎫"),
                ("beach", "playa", "బీచ్", "🏖️"), ("mountain", "montaña", "కొండ", "⛰️"),
                ("luggage", "equipaje", "సామాను", "🧳"), ("restaurant", "restaurante", "రెస్టారెంట్", "🍽️"),
                ("museum", "museo", "మ్యూజియం", "🏛️"), ("hospital", "hospital", "ఆసుపత్రి", "🏥"),
                ("pharmacy", "farmacia", "ఫార్మసీ", "💊"), ("bank", "banco", "బ్యాంక్", "🏦"),
                ("police", "policía", "పోలీసు", "👮"), ("embassy", "embajada", "రాయబార కార్యాలయం", "🏛️"),
                ("reservation", "reservación", "రిజర్వేషన్", "📋"), ("guide", "guía", "గైడ్", "🧭")
            }),
            ("Shopping", "Time to shop!", "🛍️", new[]
            {
                ("shop", "tienda", "దుకాణం", "🏪"), ("price", "precio", "ధర", "💲"),
                ("money", "dinero", "డబ్బు", "💰"), ("cheap", "barato", "చౌక", "👍"),
                ("expensive", "caro", "ఖరీదు", "💎"), ("buy", "comprar", "కొనుక్కోవడం", "🛒"),
                ("sell", "vender", "అమ్మడం", "🏷️"), ("market", "mercado", "మార్కెట్", "🏬"),
                ("bag", "bolsa", "సంచి", "👜"), ("size", "tamaño", "సైజు", "📏"),
                ("receipt", "recibo", "రసీదు", "🧾"), ("cash", "efectivo", "నగదు", "💵"),
                ("credit card", "tarjeta de crédito", "క్రెడిట్ కార్డ్", "💳"), ("discount", "descuento", "తగ్గింపు", "🏷️"),
                ("exchange", "cambio", "మార్పిడి", "🔄"), ("clothing", "ropa", "బట్టలు", "👕"),
                ("shoes", "zapatos", "బూట్లు", "👟"), ("gift", "regalo", "బహుమతి", "🎁"),
                ("open", "abierto", "తెరిచి ఉంది", "🔓"), ("closed", "cerrado", "మూసివేసి ఉంది", "🔒")
            }),
            ("Weather", "What's the weather like?", "⛅", new[]
            {
                ("sun", "sol", "ఎండ", "☀️"), ("rain", "lluvia", "వర్షం", "🌧️"),
                ("snow", "nieve", "మంచు", "❄️"), ("hot", "calor", "వేడి", "🔥"),
                ("cold", "frío", "చలి", "🥶"), ("wind", "viento", "గాలి", "💨"),
                ("cloud", "nube", "మేఘం", "☁️"), ("storm", "tormenta", "తుఫాను", "⛈️"),
                ("rainbow", "arcoíris", "ఇంద్రధనుస్సు", "🌈"), ("temperature", "temperatura", "ఉష్ణోగ్రత", "🌡️"),
                ("fog", "niebla", "పొగమంచు", "🌫️"), ("thunder", "trueno", "ఉరుము", "⚡"),
                ("lightning", "relámpago", "మెరుపు", "🌩️"), ("humidity", "humedad", "తేమ", "💦"),
                ("dry", "seco", "పొడి", "🏜️"), ("frost", "escarcha", "మంచుగడ్డ", "🧊"),
                ("breeze", "brisa", "చల్లగాలి", "🍃"), ("sunny", "soleado", "ఎండగా", "🌞"),
                ("cloudy", "nublado", "మేఘావృతం", "🌥️"), ("rainy", "lluvioso", "వర్షంతో", "🌦️")
            }),
            ("Time", "Every moment counts!", "⏰", new[]
            {
                ("hour", "hora", "గంట", "🕐"), ("minute", "minuto", "నిమిషం", "⏱️"),
                ("today", "hoy", "ఈరోజు", "📅"), ("tomorrow", "mañana", "రేపు", "📆"),
                ("yesterday", "ayer", "నిన్న", "📋"), ("morning", "mañana", "ఉదయం", "🌅"),
                ("afternoon", "tarde", "మధ్యాహ్నం", "🌤️"), ("night", "noche", "రాత్రి", "🌙"),
                ("week", "semana", "వారం", "📊"), ("month", "mes", "నెల", "🗓️"),
                ("year", "año", "సంవత్సరం", "📆"), ("second", "segundo", "సెకను", "⏰"),
                ("clock", "reloj", "గడియారం", "🕰️"), ("early", "temprano", "ముందుగా", "🌄"),
                ("late", "tarde", "ఆలస్యం", "🕐"), ("always", "siempre", "ఎల్లప్పుడూ", "♾️"),
                ("never", "nunca", "ఎప్పటికీ కాదు", "🚫"), ("sometimes", "a veces", "కొన్నిసార్లు", "🔀"),
                ("now", "ahora", "ఇప్పుడు", "⏳"), ("soon", "pronto", "త్వరలో", "🔜")
            }),
            ("Body & Health", "Know your body!", "🏥", new[]
            {
                ("head", "cabeza", "తల", "🗣️"), ("hand", "mano", "చేయి", "✋"),
                ("eye", "ojo", "కన్ను", "👁️"), ("ear", "oreja", "చెవి", "👂"),
                ("nose", "nariz", "ముక్కు", "👃"), ("mouth", "boca", "నోరు", "👄"),
                ("heart", "corazón", "గుండె", "❤️"), ("stomach", "estómago", "కడుపు", "🫃"),
                ("foot", "pie", "పాదం", "🦶"), ("arm", "brazo", "చేతి భుజం", "💪"),
                ("leg", "pierna", "కాలు", "🦵"), ("back", "espalda", "వీపు", "🔙"),
                ("teeth", "dientes", "దంతాలు", "🦷"), ("hair", "cabello", "జుట్టు", "💇"),
                ("finger", "dedo", "వేలు", "☝️"), ("knee", "rodilla", "మోకాలు", "🦵"),
                ("doctor", "doctor", "డాక్టర్", "👨‍⚕️"), ("medicine", "medicina", "మందు", "💊"),
                ("pain", "dolor", "నొప్పి", "🤕"), ("healthy", "saludable", "ఆరోగ్యకరమైన", "💪")
            }),
            ("Clothing", "Dress to impress!", "👗", new[]
            {
                ("shirt", "camisa", "చొక్కా", "👔"), ("pants", "pantalones", "ప్యాంటు", "👖"),
                ("dress", "vestido", "దుస్తులు", "👗"), ("hat", "sombrero", "టోపీ", "🎩"),
                ("jacket", "chaqueta", "జాకెట్", "🧥"), ("shoes", "zapatos", "బూట్లు", "👟"),
                ("socks", "calcetines", "సాక్సులు", "🧦"), ("skirt", "falda", "లంగా", "👗"),
                ("coat", "abrigo", "కోటు", "🧥"), ("tie", "corbata", "టై", "👔"),
                ("gloves", "guantes", "చేతి తొడుగులు", "🧤"), ("scarf", "bufanda", "మఫ్లర్", "🧣"),
                ("belt", "cinturón", "బెల్ట్", "🪢"), ("boots", "botas", "బూట్లు", "🥾"),
                ("uniform", "uniforme", "యూనిఫాం", "🎽"), ("sweater", "suéter", "స్వెటర్", "🧶"),
                ("underwear", "ropa interior", "లోదుస్తులు", "🩲"), ("glasses", "gafas", "కళ్ళద్దాలు", "👓"),
                ("watch", "reloj", "గడియారం", "⌚"), ("ring", "anillo", "ఉంగరం", "💍")
            }),
            ("Emotions", "Express how you feel!", "😊", new[]
            {
                ("happy", "feliz", "సంతోషం", "😊"), ("sad", "triste", "దుఃఖం", "😢"),
                ("angry", "enojado", "కోపం", "😠"), ("scared", "asustado", "భయం", "😨"),
                ("tired", "cansado", "అలసిపోయిన", "😴"), ("excited", "emocionado", "ఉత్సాహం", "🤩"),
                ("nervous", "nervioso", "ఆందోళన", "😰"), ("surprised", "sorprendido", "ఆశ్చర్యం", "😲"),
                ("bored", "aburrido", "విసుగు", "😐"), ("proud", "orgulloso", "గర్వం", "😤"),
                ("love", "amor", "ప్రేమ", "❤️"), ("hope", "esperanza", "ఆశ", "🌟"),
                ("fear", "miedo", "భయం", "😱"), ("joy", "alegría", "ఆనందం", "🥳"),
                ("calm", "tranquilo", "ప్రశాంతం", "😌"), ("lonely", "solitario", "ఒంటరి", "🥺"),
                ("grateful", "agradecido", "కృతజ్ఞత", "🙏"), ("jealous", "celoso", "అసూయ", "😒"),
                ("confused", "confundido", "గందరగోళం", "😵"), ("curious", "curioso", "ఉత్సుకత", "🤔")
            }),
            ("House & Home", "Around the house!", "🏠", new[]
            {
                ("house", "casa", "ఇల్లు", "🏠"), ("room", "habitación", "గది", "🚪"),
                ("kitchen", "cocina", "వంటగది", "🍳"), ("bedroom", "dormitorio", "పడకగది", "🛏️"),
                ("bathroom", "baño", "బాత్రూమ్", "🚿"), ("door", "puerta", "తలుపు", "🚪"),
                ("window", "ventana", "కిటికీ", "🪟"), ("table", "mesa", "బల్ల", "🪑"),
                ("chair", "silla", "కుర్చీ", "💺"), ("bed", "cama", "మంచం", "🛏️"),
                ("garden", "jardín", "తోట", "🌻"), ("roof", "techo", "పైకప్పు", "🏗️"),
                ("floor", "piso", "నేల", "🏢"), ("stairs", "escaleras", "మెట్లు", "🪜"),
                ("garage", "garaje", "గ్యారేజ్", "🅿️"), ("key", "llave", "తాళం", "🔑"),
                ("lamp", "lámpara", "దీపం", "💡"), ("mirror", "espejo", "అద్దం", "🪞"),
                ("sofa", "sofá", "సోఫా", "🛋️"), ("television", "televisión", "టీవీ", "📺")
            }),
            ("School & Work", "Learn and earn!", "🎓", new[]
            {
                ("school", "escuela", "పాఠశాల", "🏫"), ("teacher", "profesor", "ఉపాధ్యాయుడు", "👨‍🏫"),
                ("student", "estudiante", "విద్యార్థి", "👩‍🎓"), ("book", "libro", "పుస్తకం", "📚"),
                ("pen", "bolígrafo", "కలం", "🖊️"), ("paper", "papel", "కాగితం", "📄"),
                ("class", "clase", "తరగతి", "🏫"), ("exam", "examen", "పరీక్ష", "📝"),
                ("homework", "tarea", "ఇంటి పని", "📓"), ("computer", "computadora", "కంప్యూటర్", "💻"),
                ("office", "oficina", "కార్యాలయం", "🏢"), ("meeting", "reunión", "సమావేశం", "🤝"),
                ("boss", "jefe", "బాస్", "👔"), ("salary", "salario", "జీతం", "💰"),
                ("job", "trabajo", "ఉద్యోగం", "💼"), ("email", "correo electrónico", "ఈమెయిల్", "📧"),
                ("project", "proyecto", "ప్రాజెక్ట్", "📊"), ("team", "equipo", "జట్టు", "👥"),
                ("phone", "teléfono", "ఫోన్", "📱"), ("calendar", "calendario", "క్యాలెండర్", "📅")
            }),
            ("Sports & Hobbies", "Stay active and have fun!", "⚽", new[]
            {
                ("soccer", "fútbol", "ఫుట్‌బాల్", "⚽"), ("basketball", "baloncesto", "బాస్కెట్‌బాల్", "🏀"),
                ("tennis", "tenis", "టెన్నిస్", "🎾"), ("swimming", "natación", "ఈత", "🏊"),
                ("running", "correr", "పరుగు", "🏃"), ("cycling", "ciclismo", "సైక్లింగ్", "🚴"),
                ("dance", "baile", "నృత్యం", "💃"), ("music", "música", "సంగీతం", "🎵"),
                ("painting", "pintura", "చిత్రకళ", "🎨"), ("reading", "lectura", "చదవడం", "📖"),
                ("cooking", "cocinar", "వంట", "👨‍🍳"), ("photography", "fotografía", "ఫోటోగ్రఫీ", "📷"),
                ("game", "juego", "ఆట", "🎮"), ("team", "equipo", "జట్టు", "👥"),
                ("winner", "ganador", "విజేత", "🏆"), ("exercise", "ejercicio", "వ్యాయామం", "🏋️"),
                ("yoga", "yoga", "యోగా", "🧘"), ("hiking", "senderismo", "హైకింగ్", "🥾"),
                ("fishing", "pesca", "చేపలు పట్టడం", "🎣"), ("camping", "acampar", "క్యాంపింగ్", "🏕️")
            }),
            ("Nature", "Explore the natural world!", "🌿", new[]
            {
                ("tree", "árbol", "చెట్టు", "🌳"), ("flower", "flor", "పువ్వు", "🌸"),
                ("river", "río", "నది", "🏞️"), ("ocean", "océano", "సముద్రం", "🌊"),
                ("mountain", "montaña", "కొండ", "🏔️"), ("forest", "bosque", "అడవి", "🌲"),
                ("sky", "cielo", "ఆకాశం", "🌌"), ("star", "estrella", "నక్షత్రం", "⭐"),
                ("moon", "luna", "చంద్రుడు", "🌙"), ("earth", "tierra", "భూమి", "🌍"),
                ("grass", "hierba", "గడ్డి", "🌿"), ("lake", "lago", "సరస్సు", "🏞️"),
                ("desert", "desierto", "ఎడారి", "🏜️"), ("island", "isla", "దీవి", "🏝️"),
                ("volcano", "volcán", "అగ్నిపర్వతం", "🌋"), ("waterfall", "cascada", "జలపాతం", "💦"),
                ("rock", "roca", "రాయి", "🪨"), ("sand", "arena", "ఇసుక", "🏖️"),
                ("leaf", "hoja", "ఆకు", "🍃"), ("seed", "semilla", "విత్తనం", "🌱")
            }),
            ("City Life", "Navigate the urban jungle!", "🏙️", new[]
            {
                ("city", "ciudad", "నగరం", "🏙️"), ("street", "calle", "వీధి", "🛣️"),
                ("building", "edificio", "భవనం", "🏢"), ("park", "parque", "పార్కు", "🏞️"),
                ("library", "biblioteca", "గ్రంథాలయం", "📚"), ("church", "iglesia", "చర్చి", "⛪"),
                ("bridge", "puente", "వంతెన", "🌉"), ("traffic", "tráfico", "ట్రాఫిక్", "🚦"),
                ("corner", "esquina", "మూల", "📐"), ("plaza", "plaza", "ప్లాజా", "🏛️"),
                ("apartment", "apartamento", "అపార్ట్‌మెంట్", "🏬"), ("sidewalk", "acera", "ఫుట్‌పాత్", "🚶"),
                ("subway", "metro", "మెట్రో", "🚇"), ("tower", "torre", "టవర్", "🗼"),
                ("stadium", "estadio", "స్టేడియం", "🏟️"), ("cinema", "cine", "సినిమా", "🎬"),
                ("hospital", "hospital", "ఆసుపత్రి", "🏥"), ("fire station", "estación de bomberos", "ఫైర్ స్టేషన్", "🚒"),
                ("post office", "oficina de correos", "పోస్ట్ ఆఫీస్", "📮"), ("zoo", "zoológico", "జంతు ప్రదర్శనశాల", "🦁")
            }),
            ("Verbs & Actions", "Express what you do!", "🏃", new[]
            {
                ("eat", "comer", "తినడం", "🍽️"), ("drink", "beber", "తాగడం", "🥤"),
                ("sleep", "dormir", "నిద్ర", "😴"), ("walk", "caminar", "నడవడం", "🚶"),
                ("run", "correr", "పరుగెత్తడం", "🏃"), ("read", "leer", "చదవడం", "📖"),
                ("write", "escribir", "రాయడం", "✍️"), ("speak", "hablar", "మాట్లాడటం", "🗣️"),
                ("listen", "escuchar", "వినడం", "👂"), ("see", "ver", "చూడటం", "👀"),
                ("open", "abrir", "తెరవడం", "📂"), ("close", "cerrar", "మూయడం", "📕"),
                ("give", "dar", "ఇవ్వడం", "🎁"), ("take", "tomar", "తీసుకోవడం", "🤲"),
                ("think", "pensar", "ఆలోచించడం", "🤔"), ("know", "saber", "తెలుసు", "💡"),
                ("want", "querer", "కావాలి", "🙋"), ("need", "necesitar", "అవసరం", "❗"),
                ("help", "ayudar", "సహాయం", "🆘"), ("work", "trabajar", "పని చేయడం", "💼")
            }),
            ("Descriptive Words", "Describe your world!", "📝", new[]
            {
                ("big", "grande", "పెద్ద", "🐘"), ("small", "pequeño", "చిన్న", "🐜"),
                ("fast", "rápido", "వేగం", "⚡"), ("slow", "lento", "నెమ్మది", "🐌"),
                ("new", "nuevo", "కొత్త", "✨"), ("old", "viejo", "పాత", "📜"),
                ("good", "bueno", "మంచి", "👍"), ("bad", "malo", "చెడు", "👎"),
                ("beautiful", "hermoso", "అందమైన", "🌹"), ("ugly", "feo", "అసహ్యమైన", "👹"),
                ("tall", "alto", "పొడవైన", "🦒"), ("short", "bajo", "పొట్టి", "🐁"),
                ("heavy", "pesado", "బరువైన", "🏋️"), ("easy", "fácil", "సులభం", "✅"),
                ("difficult", "difícil", "కష్టం", "🧩"), ("clean", "limpio", "శుభ్రం", "🧹"),
                ("dirty", "sucio", "మురికి", "🗑️"), ("strong", "fuerte", "బలమైన", "💪"),
                ("weak", "débil", "బలహీనమైన", "🥀"), ("rich", "rico", "ధనవంతుడు", "💎")
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
        int wordsPerLesson = 4;
        int lessonIndex = 0;

        // All exercise generators
        var exerciseGenerators = new List<Func<(string En, string Es, string Te, string? Img), (string En, string Es, string Te, string? Img)[], Exercise>>
        {
            // PictureMatch: show emoji, pick correct Spanish
            (w, all) =>
            {
                var wrong = all.Where(x => x.En != w.En).OrderBy(_ => Random.Shared.Next()).Take(3).Select(x => x.Es).ToList();
                wrong.Add(w.Es);
                return new Exercise
                {
                    Type = ExerciseType.PictureMatch,
                    Question = $"{w.Img} What is this in Spanish?",
                    CorrectAnswer = w.Es,
                    OptionsJson = JsonSerializer.Serialize(wrong.OrderBy(_ => Random.Shared.Next())),
                    ImageUrl = w.Img,
                    HintText = w.En
                };
            },
            // TranslateSentence: En → Es (multiple choice)
            (w, all) =>
            {
                var wrong = all.Where(x => x.En != w.En).OrderBy(_ => Random.Shared.Next()).Take(3).Select(x => x.Es).ToList();
                wrong.Add(w.Es);
                return new Exercise
                {
                    Type = ExerciseType.TranslateSentence,
                    Question = $"Translate to Spanish: \"{w.En}\"",
                    CorrectAnswer = w.Es,
                    OptionsJson = JsonSerializer.Serialize(wrong.OrderBy(_ => Random.Shared.Next()))
                };
            },
            // FillInTheBlank: type Spanish
            (w, _) => new Exercise
            {
                Type = ExerciseType.FillInTheBlank,
                Question = $"The Spanish word for \"{w.En}\" is ____",
                CorrectAnswer = w.Es,
                OptionsJson = "[]",
                HintText = $"Starts with '{w.Es[0]}'"
            },
            // TriFlipChallenge: Telugu → Spanish
            (w, _) => new Exercise
            {
                Type = ExerciseType.TriFlipChallenge,
                Question = $"🇮🇳 Telugu: {w.Te} → What is this in Spanish?",
                CorrectAnswer = w.Es,
                OptionsJson = "[]",
                HintText = $"In English: {w.En}"
            },
            // Reverse PictureMatch: show emoji, pick correct English
            (w, all) =>
            {
                var wrong = all.Where(x => x.En != w.En).OrderBy(_ => Random.Shared.Next()).Take(3).Select(x => x.En).ToList();
                wrong.Add(w.En);
                return new Exercise
                {
                    Type = ExerciseType.PictureMatch,
                    Question = $"{w.Img} What is this in English?",
                    CorrectAnswer = w.En,
                    OptionsJson = JsonSerializer.Serialize(wrong.OrderBy(_ => Random.Shared.Next())),
                    ImageUrl = w.Img,
                    HintText = w.Es
                };
            },
            // Reverse Translate: Es → En (multiple choice)
            (w, all) =>
            {
                var wrong = all.Where(x => x.En != w.En).OrderBy(_ => Random.Shared.Next()).Take(3).Select(x => x.En).ToList();
                wrong.Add(w.En);
                return new Exercise
                {
                    Type = ExerciseType.TranslateSentence,
                    Question = $"What does \"{w.Es}\" mean in English?",
                    CorrectAnswer = w.En,
                    OptionsJson = JsonSerializer.Serialize(wrong.OrderBy(_ => Random.Shared.Next())),
                    HintText = $"Telugu: {w.Te}"
                };
            },
            // Telugu → English (multiple choice)
            (w, all) =>
            {
                var wrong = all.Where(x => x.En != w.En).OrderBy(_ => Random.Shared.Next()).Take(3).Select(x => x.En).ToList();
                wrong.Add(w.En);
                return new Exercise
                {
                    Type = ExerciseType.TriFlipChallenge,
                    Question = $"🇮🇳 Telugu: {w.Te} → What is this in English?",
                    CorrectAnswer = w.En,
                    OptionsJson = JsonSerializer.Serialize(wrong.OrderBy(_ => Random.Shared.Next())),
                    HintText = $"Spanish: {w.Es}"
                };
            },
            // FillInTheBlank: type English from Spanish
            (w, _) => new Exercise
            {
                Type = ExerciseType.FillInTheBlank,
                Question = $"\"{w.Es}\" in English is ____",
                CorrectAnswer = w.En,
                OptionsJson = "[]",
                HintText = $"Starts with '{w.En[0]}'"
            }
        };

        for (int i = 0; i < words.Length; i += wordsPerLesson)
        {
            lessonIndex++;
            var lessonWords = words.Skip(i).Take(wordsPerLesson).ToArray();

            var exercises = new List<Exercise>();

            // For each word, pick 2 unique exercise types (from 8 generators)
            foreach (var w in lessonWords)
            {
                var pickedGenerators = exerciseGenerators
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(2)
                    .ToList();

                foreach (var gen in pickedGenerators)
                {
                    exercises.Add(gen(w, words));
                }
            }

            // Shuffle all exercises so same word doesn't appear back-to-back
            exercises = exercises.OrderBy(_ => Random.Shared.Next()).ToList();

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
        new() { Name = "All Star", Description = "Complete all topics", IconEmoji = "🏆", CriteriaType = "topics_completed", CriteriaValue = 20 },
        new() { Name = "XP Hunter", Description = "Earn 1000 XP", IconEmoji = "💎", CriteriaType = "total_xp", CriteriaValue = 1000 }
    ];
}
