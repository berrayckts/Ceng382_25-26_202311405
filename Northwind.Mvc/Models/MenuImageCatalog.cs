namespace Northwind.Mvc.Models;

public static class MenuImageCatalog
{
    private static readonly string[] KebabsAndMeats = [
        "/images/menu/01_ankara_tava.jpg", "/images/menu/02_adana_kebap.jpg", "/images/menu/03_izgara_kofte.jpg", 
        "/images/menu/04_firin_tavuk.jpg", "/images/menu/06_pilav_ustu_doner.jpg", "/images/menu/07_iskender_kebap.jpg", 
        "/images/menu/ızgara.jpeg", "/images/menu/meatbowl.jpeg"
    ];

    private static readonly string[] Soups = [
        "/images/menu/09_mercimek_corba.jpg", "/images/menu/10_tavuk_suyu_corba.jpg", "/images/menu/11_sebze_corba.jpg"
    ];

    private static readonly string[] Breakfasts = [
        "/images/menu/12_serpme_kahvalti.jpg", "/images/menu/14_menemen.jpg", "/images/menu/15_sucuklu_yumurta.jpg", 
        "/images/menu/breakfast.jpeg", "/images/menu/breakfast (2).jpeg", "/images/menu/breakfast 3.jpeg"
    ];

    private static readonly string[] PastriesAndBoreks = [
        "/images/menu/13_gozleme.jpg", "/images/menu/16_peynirli_borek.jpg", "/images/menu/17_kiymali_borek.jpg", 
        "/images/menu/18_sigara_boregi.jpg", "/images/menu/börek.jpeg"
    ];

    private static readonly string[] PidesAndLahmacuns = [
        "/images/menu/19_lahmacun.jpg", "/images/menu/20_pide_kiymali.jpg", "/images/menu/21_pide_kasarli.jpg"
    ];

    private static readonly string[] Pizzas = [
        "/images/menu/22_margherita_pizza.jpg", "/images/menu/23_karisik_pizza.jpg", "/images/menu/pizza.jpeg", 
        "/images/menu/pizza1.jpeg", "/images/menu/mozerallapizza.jpeg"
    ];

    private static readonly string[] Pastas = [
        "/images/menu/24_makarna_bolognese.jpg", "/images/menu/25_makarna_alfredo.jpg", "/images/menu/pasta.jpeg", "/images/menu/spagetti.jpeg"
    ];

    private static readonly string[] Salads = [
        "/images/menu/26_sezar_salata.jpg", "/images/menu/27_coban_salata.jpg", "/images/menu/28_mevsim_salata.jpg"
    ];

    private static readonly string[] Mezes = [
        "/images/menu/05_etli_yaprak_sarma.jpg", "/images/menu/29_humus.jpg", "/images/menu/30_acili_ezme.jpg", 
        "/images/menu/31_haydari.jpg", "/images/menu/32_patlican_salatasi.jpg", "/images/menu/meze.jpeg", "/images/menu/meze 2.jpeg"
    ];

    private static readonly string[] Drinks = [
        "/images/menu/33_ayran.jpg", "/images/menu/34_limonata.jpg", "/images/menu/35_kola.jpg", 
        "/images/menu/36_salgam.jpg", "/images/menu/37_turk_kahvesi.jpg", "/images/menu/38_cay.jpg"
    ];

    private static readonly string[] Desserts = [
        "/images/menu/39_sutlac.jpg", "/images/menu/40_kazandibi.jpg", "/images/menu/41_baklava.jpg", 
        "/images/menu/42_kunefe.jpg", "/images/menu/43_tiramisu.jpg", "/images/menu/44_cheesecake.jpg", 
        "/images/menu/45_meyve_tabagi.jpg", "/images/menu/46_ekler.jpg", "/images/menu/47_supangle.jpg", 
        "/images/menu/48_ice_cream.jpg", "/images/menu/dessert 2.jpeg", "/images/menu/dessertbowl.jpeg"
    ];

    private static readonly string[] Seafoods = [
        "/images/menu/49_fish_grill.jpg", "/images/menu/50_calamari.jpg", "/images/menu/51_shrimp.jpg", "/images/menu/52_midye_tava.jpg"
    ];

    private static readonly string[] VeganAndHealthy = [
        "/images/menu/53_sebze_tava.jpg", "/images/menu/54_vegan_tabak.jpg", "/images/menu/55_falafel.jpg", 
        "/images/menu/56_vegan_burger.jpg", "/images/menu/chickenbowl.jpeg", "/images/menu/blueberrybowl.jpeg"
    ];

    private static readonly string[] FastFoods = [
        "/images/menu/57_cheeseburger.jpg", "/images/menu/58_double_burger.jpg", "/images/menu/59_chicken_burger.jpg", 
        "/images/menu/60_nugget.jpg", "/images/menu/61_onion_rings.jpg", "/images/menu/62_french_fries.jpg"
    ];

    private static readonly string[] CocktailsAndCanapes = [
        "/images/menu/63_kokteyl_tabagi.jpg", "/images/menu/64_davet_menusu.jpg", "/images/menu/kanepe.jpeg", 
        "/images/menu/kanepe 3.jpeg", "/images/menu/kanepe 4.jpeg", "/images/menu/kokteyl.jpeg", 
        "/images/menu/sandwiches.jpeg", "/images/menu/sandwiches2.jpeg", "/images/menu/tatlı kanepe.jpeg", 
        "/images/menu/kanepe and ızgara.jpeg"
    ];

    private static readonly string[] Sushis = [
        "/images/menu/sushi.jpeg", "/images/menu/californiaroll.jpeg"
    ];

    private static readonly string[] Fallbacks = [
        "/images/menu/default.jpg", "/images/menu/08_sebzeli_guvec.jpg", "/images/menu/guvec.jpeg",
        "/images/menu/01_ankara_tava.jpg", "/images/menu/12_serpme_kahvalti.jpg", "/images/menu/22_margherita_pizza.jpg",
        "/images/menu/41_baklava.jpg", "/images/menu/57_cheeseburger.jpg", "/images/menu/63_kokteyl_tabagi.jpg"
    ];

    public static string ForItem(string? name, string? category, string? existingPath, int id = 0)
    {
        if (IsUsable(existingPath))
        {
            return existingPath!;
        }

        var text = $"{name} {category}";
        var specific = SpecificImageFor(text);
        if (specific is not null)
        {
            return specific;
        }

        var hashId = Math.Abs(id == 0 ? StableHash(text) : id);

        string Pick(string[] array) => array[hashId % array.Length];

        if (Has(text, "sushi", "nigiri", "roll")) return Pick(Sushis);
        if (Has(text, "pizza", "napoli", "margherita")) return Pick(Pizzas);
        if (Has(text, "breakfast", "kahvaltı", "kahvalti", "sofra", "sofrası", "sofrasi", "yumurta", "menemen", "kahvaltilik")) return Pick(Breakfasts);
        if (Has(text, "börek", "borek", "gözleme", "gozleme", "poğaça")) return Pick(PastriesAndBoreks);
        if (Has(text, "pide", "lahmacun")) return Pick(PidesAndLahmacuns);
        if (Has(text, "çorba", "corba", "soup")) return Pick(Soups);
        if (Has(text, "salata", "salad")) return Pick(Salads);
        if (Has(text, "burger", "hamburger", "cheeseburger", "nugget", "fries", "patates")) return Pick(FastFoods);
        if (Has(text, "adana", "kebab", "kebap", "köfte", "kofte", "lamb", "grill", "ızgara", "izgara", "döner", "doner", "tavuk", "chicken", "meat", "tava", "şiş", "sis")) return Pick(KebabsAndMeats);
        if (Has(text, "mezze", "meze", "humus", "haydari", "ezme", "sarma", "zeytinyağlı")) return Pick(Mezes);
        if (Has(text, "kanepe", "canape", "kokteyl", "cocktail", "davet", "sandwich", "sandviç", "atıştırmalık")) return Pick(CocktailsAndCanapes);
        if (Has(text, "pasta", "spagetti", "spaghetti", "makarna", "manti", "mantı")) return Pick(Pastas);
        if (Has(text, "dessert", "tatlı", "tatli", "sweet", "sütlaç", "baklava", "künefe", "cake", "ice cream", "dondurma")) return Pick(Desserts);
        if (Has(text, "drink", "içecek", "icecek", "su", "water", "cola", "kola", "ayran", "çay", "cay", "kahve", "coffee")) return Pick(Drinks);
        if (Has(text, "fish", "balık", "balik", "midye", "karides", "shrimp", "calamari", "kalamar")) return Pick(Seafoods);
        if (Has(text, "vegan", "green", "bowl", "healthy", "sebze", "falafel")) return Pick(VeganAndHealthy);
        if (Has(text, "güveç", "guvec")) return "/images/menu/guvec.jpeg";

        return Pick(Fallbacks);
    }

    public static string ForCaterer(string? name, string? category, string? existingPath, int id = 0)
    {
        if (IsUsable(existingPath))
        {
            return existingPath!;
        }

        return ForItem(name, category, null, id);
    }

    public static bool IsUsable(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        return !path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
    }

    private static string? SpecificImageFor(string text)
    {
        if (Has(text, "ayran")) return "/images/menu/33_ayran.jpg";
        if (Has(text, "limonata", "lemonade")) return "/images/menu/34_limonata.jpg";
        if (Has(text, "kola", "cola")) return "/images/menu/35_kola.jpg";
        if (Has(text, "salgam", "şalgam", "ÅŸalgam")) return "/images/menu/36_salgam.jpg";
        if (Has(text, "turkish coffee", "turk kahvesi", "türk kahvesi")) return "/images/menu/37_turk_kahvesi.jpg";
        if (Has(text, "turkish tea", "cay", "çay")) return "/images/menu/38_cay.jpg";

        if (Has(text, "adana")) return "/images/menu/02_adana_kebap.jpg";
        if (Has(text, "beyti")) return "/images/menu/07_iskender_kebap.jpg";
        if (Has(text, "mixed grill", "karisik izgara", "karışık ızgara")) return "/images/menu/ızgara.jpeg";
        if (Has(text, "tavuk şiş", "tavuk sis", "chicken skewer")) return "/images/menu/04_firin_tavuk.jpg";
        if (Has(text, "kofte", "köfte")) return "/images/menu/03_izgara_kofte.jpg";
        if (Has(text, "doner", "döner")) return "/images/menu/06_pilav_ustu_doner.jpg";
        if (Has(text, "lamb", "kuzu")) return "/images/menu/meatbowl.jpeg";

        if (Has(text, "baklava")) return "/images/menu/41_baklava.jpg";
        if (Has(text, "kunefe", "künefe")) return "/images/menu/42_kunefe.jpg";
        if (Has(text, "sutlac", "sütlaç")) return "/images/menu/39_sutlac.jpg";
        if (Has(text, "kazandibi")) return "/images/menu/40_kazandibi.jpg";
        if (Has(text, "ekler", "eclair")) return "/images/menu/46_ekler.jpg";
        if (Has(text, "tiramisu")) return "/images/menu/43_tiramisu.jpg";
        if (Has(text, "cheesecake")) return "/images/menu/44_cheesecake.jpg";

        if (Has(text, "mercimek")) return "/images/menu/09_mercimek_corba.jpg";
        if (Has(text, "tavuk suyu")) return "/images/menu/10_tavuk_suyu_corba.jpg";
        if (Has(text, "sebze corba", "sebze çorba")) return "/images/menu/11_sebze_corba.jpg";

        if (Has(text, "lahmacun")) return "/images/menu/19_lahmacun.jpg";
        if (Has(text, "kiymali pide", "kıymalı pide")) return "/images/menu/20_pide_kiymali.jpg";
        if (Has(text, "kasarli pide", "kaşarlı pide")) return "/images/menu/21_pide_kasarli.jpg";
        if (Has(text, "gozleme", "gözleme")) return "/images/menu/13_gozleme.jpg";
        if (Has(text, "borek", "börek")) return "/images/menu/16_peynirli_borek.jpg";

        if (Has(text, "sushi")) return "/images/menu/sushi.jpeg";
        if (Has(text, "california roll")) return "/images/menu/californiaroll.jpeg";
        if (Has(text, "pizza")) return "/images/menu/23_karisik_pizza.jpg";
        if (Has(text, "makarna", "pasta")) return "/images/menu/24_makarna_bolognese.jpg";

        return null;
    }

    private static bool Has(string text, params string[] needles) =>
        needles.Any(needle => text.Contains(needle, StringComparison.OrdinalIgnoreCase));

    private static int StableHash(string text)
    {
        unchecked
        {
            var hash = 17;
            foreach (var ch in text)
            {
                hash = (hash * 31) + ch;
            }
            return hash;
        }
    }
}
