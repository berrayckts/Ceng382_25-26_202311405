using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using System.Globalization;
using System.Text;

namespace Northwind.Mvc.Services;

public static class CateraIdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CateringProjectContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var logs = scope.ServiceProvider.GetRequiredService<ILogService>();

        await db.Database.MigrateAsync();

        foreach (var role in AppRole.All.Concat(AppRole.Legacy))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }

        var customer = await EnsureCustomerAsync(db, "Demo User", "user@caterease.local", "Istanbul", 41.0082m, 28.9784m);

        var bosphorus = await EnsureCatererAsync(db, "Bosphorus Kitchen", "bosphorus-kitchen", "Turkish/Ottoman", "Istanbul", "Besiktas", "Bebek, Istanbul", "owner.bosphorus@caterease.local", 41.0766m, 29.0438m);
        var napoli = await EnsureCatererAsync(db, "Napoli Express", "napoli-express", "Italian/Pizza", "Istanbul", "Kadikoy", "Moda, Kadikoy, Istanbul", "owner.napoli@caterease.local", 40.9877m, 29.0245m);
        var green = await EnsureCatererAsync(db, "Green Bowl", "green-bowl", "Healthy/Vegan", "Istanbul", "Sisli", "Nisantasi, Sisli, Istanbul", "owner.greenbowl@caterease.local", 41.0504m, 28.9910m);
        var sakura = await EnsureCatererAsync(db, "Sakura Sushi", "sakura-sushi", "Japanese", "Istanbul", "Beyoglu", "Karakoy, Beyoglu, Istanbul", "owner.sakura@caterease.local", 41.0245m, 28.9780m);
        var legacy = await db.Caterers.FirstOrDefaultAsync(c => c.Email == "caterer@caterease.local") ?? bosphorus;

        // Development-only demo passwords. Replace with secure onboarding in production.
        await EnsureUserAsync(userManager, "Vellora Admin", "admin@caterease.local", "Admin123!", AppRole.Admin, null, null);
        await EnsureUserAsync(userManager, "Bosphorus Owner", "owner.bosphorus@caterease.local", "Owner123!", AppRole.RestaurantOwner, bosphorus.Id, null);
        await EnsureUserAsync(userManager, "Napoli Owner", "owner.napoli@caterease.local", "Owner123!", AppRole.RestaurantOwner, napoli.Id, null);
        await EnsureUserAsync(userManager, "Green Bowl Owner", "owner.greenbowl@caterease.local", "Owner123!", AppRole.RestaurantOwner, green.Id, null);
        await EnsureUserAsync(userManager, "Sakura Owner", "owner.sakura@caterease.local", "Owner123!", AppRole.RestaurantOwner, sakura.Id, null);
        await EnsureUserAsync(userManager, "Demo User", "user@caterease.local", "User123!", AppRole.Customer, null, customer.Id);
        await EnsureUserAsync(userManager, "Istanbul Garden Manager", "caterer@caterease.local", "Owner123!", AppRole.RestaurantOwner, legacy.Id, null);

        await EnsureMenusAsync(db, bosphorus, "Turkish/Ottoman", new[]
        {
            ("Starters", "Hummus with pita", 65m), ("Starters", "Ezme salad", 55m), ("Starters", "Sigara böreği x5", 75m), ("Starters", "Cacık", 45m),
            ("Mains", "Lamb shank with bulgur", 320m), ("Mains", "Chicken güveç", 245m), ("Mains", "Vegetarian imam bayıldı", 195m), ("Mains", "Mercimek çorbası", 65m),
            ("Grills", "Adana kebab plate", 285m), ("Grills", "Beyti kebab", 295m), ("Grills", "Mixed grill for two", 520m), ("Grills", "Tavuk şiş", 235m),
            ("Desserts", "Künefe", 120m), ("Desserts", "Baklava 6 pcs", 110m), ("Desserts", "Sütlaç", 75m), ("Desserts", "Kazandibi", 70m),
            ("Drinks", "Ayran", 30m), ("Drinks", "Turkish tea", 20m), ("Drinks", "Turkish coffee", 45m), ("Drinks", "Şalgam", 35m)
        });
        await EnsureMenusAsync(db, napoli, "Italian/Pizza", new[]
        {
            ("Pizzas", "Margherita", 220m), ("Pizzas", "Pepperoni", 260m), ("Pizzas", "Four cheese", 275m), ("Pizzas", "Truffle mushroom", 295m), ("Pizzas", "Calzone", 250m),
            ("Pastas", "Spaghetti carbonara", 235m), ("Pastas", "Penne arrabbiata", 195m), ("Pastas", "Tagliatelle bolognese", 245m), ("Pastas", "Gnocchi al pesto", 225m),
            ("Salads", "Caesar", 165m), ("Salads", "Caprese", 175m), ("Salads", "Rucola e parmigiano", 185m),
            ("Desserts", "Tiramisu", 135m), ("Desserts", "Panna cotta", 115m), ("Desserts", "Cannoli x2", 125m),
            ("Drinks", "San Pellegrino", 55m), ("Drinks", "Limonata", 65m), ("Drinks", "Espresso", 50m), ("Drinks", "Cappuccino", 70m)
        });
        await EnsureMenusAsync(db, green, "Healthy/Vegan", new[]
        {
            ("Bowls", "Buddha bowl", 195m), ("Bowls", "Quinoa power bowl", 215m), ("Bowls", "Teriyaki tofu bowl", 205m), ("Bowls", "Falafel bowl", 185m),
            ("Salads", "Kale & avocado", 175m), ("Salads", "Chickpea fattoush", 165m), ("Salads", "Roasted beet", 155m),
            ("Soups", "Lentil & turmeric", 95m), ("Soups", "Roasted tomato", 90m), ("Soups", "Miso with tofu", 105m),
            ("Snacks", "Hummus & veggie sticks", 125m), ("Snacks", "Avocado toast x2", 145m), ("Snacks", "Energy balls x4", 95m),
            ("Drinks", "Cold-pressed green juice", 85m), ("Drinks", "Oat milk latte", 75m), ("Drinks", "Kombucha", 90m), ("Drinks", "Infused water", 35m)
        });
        await EnsureMenusAsync(db, sakura, "Japanese", new[]
        {
            ("Nigiri", "Salmon", 110m), ("Nigiri", "Tuna", 125m), ("Nigiri", "Ebi shrimp", 105m), ("Nigiri", "Tamago", 85m),
            ("Maki & Rolls", "California roll", 165m), ("Maki & Rolls", "Spicy tuna roll", 185m), ("Maki & Rolls", "Dragon roll", 215m), ("Maki & Rolls", "Rainbow roll", 225m), ("Maki & Rolls", "Veggie roll", 145m),
            ("Ramen", "Tonkotsu", 245m), ("Ramen", "Shoyu chicken", 225m), ("Ramen", "Vegan miso", 215m),
            ("Sides", "Edamame", 75m), ("Sides", "Gyoza x4", 125m), ("Sides", "Miso soup", 65m), ("Sides", "Seaweed salad", 95m),
            ("Drinks", "Matcha latte", 85m), ("Drinks", "Sake", 120m), ("Drinks", "Japanese beer", 110m), ("Drinks", "Ramune soda", 65m)
        });

        await EnsureAnkaraDistrictDemoAsync(db, userManager);
        await EnsureTurkeyMapDemoCaterersAsync(db);
        await EnsureSmartMenuOptionsAsync(db);
        await EnsureOrdersAsync(db, customer.Id, [bosphorus.Id, napoli.Id, green.Id, sakura.Id]);
        await logs.InfoAsync("Identity.Seed", "ASP.NET Identity roles, enriched demo users, menus, and orders ensured.");
    }

    private static async Task<CustomerProfile> EnsureCustomerAsync(CateringProjectContext db, string name, string email, string city, decimal lat, decimal lng)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Email == email);
        if (customer is null)
        {
            customer = new CustomerProfile { DisplayName = name, Email = email, City = city, Latitude = lat, Longitude = lng };
            db.Customers.Add(customer);
            await db.SaveChangesAsync();
        }
        return customer;
    }

    private static async Task<CatererProfile> EnsureCatererAsync(CateringProjectContext db, string name, string slug, string story, string city, string district, string address, string email, decimal lat, decimal lng)
    {
        var caterer = await db.Caterers.FirstOrDefaultAsync(c => c.Email == email || c.Slug == slug);
        if (caterer is null)
        {
            caterer = new CatererProfile { Name = name, Slug = slug, BrandStory = story, City = city, District = district, AddressLine = address, Email = email, Latitude = lat, Longitude = lng, IsApproved = true };
            db.Caterers.Add(caterer);
        }
        else
        {
            caterer.Name = name;
            caterer.BrandStory = story;
            caterer.City = city;
            caterer.District = district;
            caterer.AddressLine = address;
            caterer.Latitude = lat;
            caterer.Longitude = lng;
            caterer.IsApproved = true;
        }
        await db.SaveChangesAsync();
        return caterer;
    }

    private static async Task EnsureMenusAsync(CateringProjectContext db, CatererProfile caterer, string cuisine, IEnumerable<(string Category, string Name, decimal Price)> items)
    {
        foreach (var item in items)
        {
            var category = await EnsureCategoryAsync(db, $"{cuisine} - {item.Category}", Slugify($"{cuisine}-{item.Category}"));
            if (await db.MenuItems.AnyAsync(m => m.CatererId == caterer.Id && m.Name == item.Name))
            {
                continue;
            }
            db.MenuItems.Add(new MenuItemEntity
            {
                CatererId = caterer.Id,
                CategoryId = category.Id,
                Name = item.Name,
                Description = $"{item.Name} prepared by {caterer.Name} for catered events and group dining.",
                BasePrice = item.Price + 300,
                ImagePath = "/images/menu/mezze.svg",
                IsActive = true
            });
        }
        await db.SaveChangesAsync();

        async Task<CuisineCategory> EnsureCategoryAsync(CateringProjectContext context, string name, string slug)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Slug == slug);
            if (category is not null) return category;
            category = new CuisineCategory { Name = name, Slug = slug, IconKey = "plate" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return category;
        }
    }

    private static async Task EnsureAnkaraDistrictDemoAsync(CateringProjectContext db, UserManager<ApplicationUser> userManager)
    {
        var districts = new (string Name, decimal Lat, decimal Lng)[]
        {
            ("Akyurt", 40.1300m, 33.0860m),
            ("Altındağ", 39.9640m, 32.8540m),
            ("Ayaş", 40.0180m, 32.3320m),
            ("Bala", 39.5540m, 33.1230m),
            ("Beypazarı", 40.1670m, 31.9210m),
            ("Çamlıdere", 40.4890m, 32.4740m),
            ("Çankaya", 39.9208m, 32.8541m),
            ("Çubuk", 40.2380m, 33.0320m),
            ("Elmadağ", 39.9200m, 33.2300m),
            ("Etimesgut", 39.9450m, 32.6690m),
            ("Evren", 39.0250m, 33.8060m),
            ("Gölbaşı", 39.7900m, 32.8090m),
            ("Güdül", 40.2110m, 32.2450m),
            ("Haymana", 39.4330m, 32.4980m),
            ("Kahramankazan", 40.2040m, 32.6830m),
            ("Kalecik", 40.1000m, 33.4100m),
            ("Keçiören", 39.9810m, 32.8660m),
            ("Kızılcahamam", 40.4700m, 32.6500m),
            ("Mamak", 39.9300m, 32.9200m),
            ("Nallıhan", 40.1850m, 31.3510m),
            ("Polatlı", 39.5840m, 32.1470m),
            ("Pursaklar", 40.0330m, 32.8950m),
            ("Şereflikoçhisar", 38.9400m, 33.5400m),
            ("Sincan", 39.9660m, 32.5840m),
            ("Yenimahalle", 39.9710m, 32.8110m)
        };

        foreach (var district in districts)
        {
            for (var number = 1; number <= 2; number++)
            {
                var asciiDistrict = ToAsciiKey(district.Name);
                var displayName = BuildAnkaraRestaurantName(district.Name, number);
                var slug = $"ankara-{asciiDistrict}-{number}";
                var email = $"caterer.{asciiDistrict}.{number}@test.com";
                var lat = district.Lat + (number == 1 ? 0.006m : -0.006m);
                var lng = district.Lng + (number == 1 ? 0.006m : -0.006m);

                var caterer = await EnsureCatererAsync(
                    db,
                    displayName,
                    slug,
                    $"{district.Name} district focused Ankara catering service for corporate events, family gatherings, and planned celebrations.",
                    "Ankara",
                    district.Name,
                    $"{district.Name} merkez, Ankara",
                    email,
                    lat,
                    lng);

                await EnsureUserAsync(userManager, $"{displayName} Owner", email, "Test123!", AppRole.RestaurantOwner, caterer.Id, null);
                await EnsureAnkaraMenusAsync(db, caterer, district.Name, number);
            }
        }
    }

    private static string BuildAnkaraRestaurantName(string district, int number)
    {
        if (district == "Çankaya" && number == 2) return "Bahçeli Davet Mutfağı";
        if (district == "Beypazarı" && number == 1) return "Beypazarı Taş Konak Catering";
        if (district == "Gölbaşı" && number == 1) return "Gölbaşı Göl Manzarası Catering";
        if (district == "Polatlı" && number == 1) return "Polatlı Anadolu Sofrası";

        return number == 1
            ? $"{district} Sofrası"
            : $"{district} Davet Mutfağı";
    }

    private static async Task EnsureAnkaraMenusAsync(CateringProjectContext db, CatererProfile caterer, string district, int restaurantNumber)
    {
        var menuSet = ((district.Length + restaurantNumber) % 6) switch
        {
            0 => new[]
            {
                ("Traditional", "Ankara Tava Menü", "Pilav üstü kuzu eti, köz sebze, mevsim salata ve ayran ile servis edilir.", 420m),
                ("Meze", "Etli Yaprak Sarma Menü", "Zeytinyağlı meze, etli yaprak sarma, yoğurt ve ev baklavası içeren davet menüsü.", 360m),
                ("Main Course", "Fırın Tavuk Menü", "Baharatlı fırın tavuk, bulgur pilavı, çoban salata ve tatlı paketi.", 310m)
            },
            1 => new[]
            {
                ("Breakfast", "Serpme Kahvaltı Paketi", "Peynir çeşitleri, zeytin, reçel, menemen, pişi ve sınırsız çay sunumu.", 285m),
                ("Breakfast", "Gözleme & Çay Menü", "Patatesli ve peynirli gözleme, domates-salatalık tabağı ve taze dem çay.", 260m),
                ("Traditional", "Peynirli Börek Menü", "Tepsi böreği, kısır, patates salatası ve mini tatlılarla brunch paketi.", 295m)
            },
            2 => new[]
            {
                ("Event Package", "Davet Kokteyl Paketi", "Mini sandviçler, kanepe çeşitleri, sıcak atıştırmalık ve tatlı lokmaları.", 540m),
                ("Grill", "Izgara Et Menü", "Dana şiş, köfte, közlenmiş sebze, pilav ve özel soslu salata sunumu.", 720m),
                ("Dessert", "Tatlı & Meze Paketi", "Soğuk meze seçkisi, mini tatlılar, meyve tabağı ve içecek desteği.", 390m)
            },
            3 => new[]
            {
                ("Vegetarian", "Sebzeli Anadolu Menü", "İmam bayıldı, mercimek köftesi, bulgur pilavı ve mevsim yeşillikleri.", 330m),
                ("Meze", "Zeytinyağlı Meze Sofrası", "Humus, haydari, ezme, yaprak sarma ve taze lavaş içeren paylaşım menüsü.", 350m),
                ("Main Course", "Mantarlı Güveç Menü", "Mantarlı sebze güveci, pirinç pilavı, cacık ve sütlü tatlı paketi.", 375m)
            },
            4 => new[]
            {
                ("Grill", "Ankara Köfte Menü", "Izgara Ankara köfte, patates, piyaz, lavaş ve ayran ile servis edilir.", 440m),
                ("Main Course", "Kuzu İncik Davet Menü", "Ağır pişmiş kuzu incik, iç pilav, roka salatası ve şerbetli tatlı.", 890m),
                ("Traditional", "Keşkek & Pilav Menü", "Tören usulü keşkek, tavuklu pilav, turşu ve helva sunumu.", 480m)
            },
            _ => new[]
            {
                ("Event Package", "Kurumsal Öğle Yemeği", "Çorba, ana yemek, salata, tatlı ve içecekten oluşan ofis catering paketi.", 325m),
                ("Main Course", "Tavuklu Pilav & Meze Menü", "Tavuklu pilav, yoğurtlu meze, salata ve ev yapımı limonata.", 280m),
                ("Dessert", "Ankara Tatlı Masası", "Baklava, sütlaç, mini tartlar ve mevsim meyveleriyle tatlı büfesi.", 510m)
            }
        };

        foreach (var item in menuSet)
        {
            if (await db.MenuItems.AnyAsync(m => m.CatererId == caterer.Id && m.Name == item.Item2))
            {
                continue;
            }

            var category = await EnsureCategoryAsync(db, item.Item1, Slugify(item.Item1));
            db.MenuItems.Add(new MenuItemEntity
            {
                CatererId = caterer.Id,
                CategoryId = category.Id,
                Name = item.Item2,
                Description = $"{district} bölgesindeki etkinlikler için {item.Item3}",
                BasePrice = item.Item4 + 300,
                ImagePath = "/images/menu/mezze.svg",
                IsActive = true
            });
        }

        await db.SaveChangesAsync();
    }

    private static async Task EnsureTurkeyMapDemoCaterersAsync(CateringProjectContext db)
    {
        var cities = new (string City, decimal Lat, decimal Lng, string[] Districts)[]
        {
            ("Adana", 37.0000m, 35.3213m, ["Seyhan", "Cukurova", "Yuregir", "Saricam"]),
            ("Adiyaman", 37.7648m, 38.2786m, ["Merkez", "Kahta", "Besni", "Golbasi"]),
            ("Afyonkarahisar", 38.7569m, 30.5387m, ["Merkez", "Sandikli", "Bolvadin", "Dinar"]),
            ("Agri", 39.7191m, 43.0503m, ["Merkez", "Dogubayazit", "Patnos", "Eleskirt"]),
            ("Aksaray", 38.3687m, 34.0370m, ["Merkez", "Ortakoy", "Eskil", "Guzelyurt"]),
            ("Amasya", 40.6533m, 35.8331m, ["Merkez", "Merzifon", "Suluova", "Tasova"]),
            ("Antalya", 36.8969m, 30.7133m, ["Muratpasa", "Konyaalti", "Kepez", "Alanya"]),
            ("Ardahan", 41.1105m, 42.7022m, ["Merkez", "Gole", "Cildir", "Hanak"]),
            ("Artvin", 41.1828m, 41.8183m, ["Merkez", "Hopa", "Arhavi", "Borcka"]),
            ("Aydin", 37.8444m, 27.8458m, ["Efeler", "Nazilli", "Kusadasi", "Soke"]),
            ("Balikesir", 39.6484m, 27.8826m, ["Altieylul", "Karesi", "Edremit", "Bandirma"]),
            ("Bartin", 41.6358m, 32.3375m, ["Merkez", "Amasra", "Ulus", "Kurucasile"]),
            ("Batman", 37.8812m, 41.1351m, ["Merkez", "Kozluk", "Besiri", "Sason"]),
            ("Bayburt", 40.2552m, 40.2249m, ["Merkez", "Aydintepe", "Demirozu", "Kop"]),
            ("Bilecik", 40.1501m, 29.9833m, ["Merkez", "Bozuyuk", "Osmaneli", "Sogut"]),
            ("Bingol", 38.8854m, 40.4983m, ["Merkez", "Genc", "Solhan", "Karliova"]),
            ("Bitlis", 38.4006m, 42.1095m, ["Merkez", "Tatvan", "Ahlat", "Guroymak"]),
            ("Bolu", 40.7395m, 31.6116m, ["Merkez", "Gerede", "Mudurnu", "Mengen"]),
            ("Burdur", 37.7203m, 30.2908m, ["Merkez", "Bucak", "Golhisar", "Yesilova"]),
            ("Bursa", 40.1828m, 29.0665m, ["Osmangazi", "Nilufer", "Yildirim", "Mudanya"]),
            ("Canakkale", 40.1553m, 26.4142m, ["Merkez", "Biga", "Gelibolu", "Ayvacik"]),
            ("Cankiri", 40.6013m, 33.6134m, ["Merkez", "Cerkes", "Ilgaz", "Kursunlu"]),
            ("Corum", 40.5506m, 34.9556m, ["Merkez", "Sungurlu", "Osmancik", "Alaca"]),
            ("Denizli", 37.7765m, 29.0864m, ["Pamukkale", "Merkezefendi", "Tavas", "Acipayam"]),
            ("Diyarbakir", 37.9144m, 40.2306m, ["Kayapinar", "Sur", "Yenisehir", "Baglar"]),
            ("Duzce", 40.8438m, 31.1565m, ["Merkez", "Akcakoca", "Golyaka", "Kaynasli"]),
            ("Edirne", 41.6771m, 26.5557m, ["Merkez", "Kesan", "Uzunkopru", "Ipsala"]),
            ("Elazig", 38.6748m, 39.2225m, ["Merkez", "Kovancilar", "Palu", "Sivrice"]),
            ("Erzincan", 39.7500m, 39.5000m, ["Merkez", "Tercan", "Refahiye", "Uzumlu"]),
            ("Erzurum", 39.9043m, 41.2679m, ["Yakutiye", "Palandoken", "Aziziye", "Oltu"]),
            ("Eskisehir", 39.7667m, 30.5256m, ["Odunpazari", "Tepebasi", "Sivrihisar", "Alpu"]),
            ("Gaziantep", 37.0662m, 37.3833m, ["Sahinbey", "Sehitkamil", "Nizip", "Oguzeli"]),
            ("Giresun", 40.9128m, 38.3895m, ["Merkez", "Bulancak", "Espiye", "Tirebolu"]),
            ("Gumushane", 40.4603m, 39.4814m, ["Merkez", "Kelkit", "Torul", "Siran"]),
            ("Hakkari", 37.5744m, 43.7408m, ["Merkez", "Yuksekova", "Semdinli", "Cukurca"]),
            ("Hatay", 36.2021m, 36.1600m, ["Antakya", "Defne", "Iskenderun", "Arsuz"]),
            ("Igdir", 39.9237m, 44.0450m, ["Merkez", "Tuzluca", "Aralik", "Karakoyunlu"]),
            ("Isparta", 37.7648m, 30.5566m, ["Merkez", "Yalvac", "Egirdir", "Sarkikaraagac"]),
            ("Istanbul", 41.0082m, 28.9784m, ["Besiktas", "Kadikoy", "Sisli", "Beyoglu"]),
            ("Izmir", 38.4237m, 27.1428m, ["Konak", "Karsiyaka", "Bornova", "Balcova"]),
            ("Kahramanmaras", 37.5753m, 36.9228m, ["Onikisubat", "Dulkadiroglu", "Elbistan", "Afsin"]),
            ("Karabuk", 41.2061m, 32.6204m, ["Merkez", "Safranbolu", "Eskipazar", "Yenice"]),
            ("Karaman", 37.1811m, 33.2150m, ["Merkez", "Ermenek", "Ayranci", "Kazimkarabekir"]),
            ("Kars", 40.6013m, 43.0975m, ["Merkez", "Sarikamis", "Kagizman", "Selim"]),
            ("Kastamonu", 41.3887m, 33.7827m, ["Merkez", "Tosya", "Taskopru", "Inebolu"]),
            ("Kayseri", 38.7205m, 35.4826m, ["Melikgazi", "Kocasinan", "Talas", "Develi"]),
            ("Kilis", 36.7184m, 37.1212m, ["Merkez", "Elbeyli", "Musabeyli", "Polateli"]),
            ("Kirikkale", 39.8468m, 33.5153m, ["Merkez", "Yahsihan", "Keskin", "Delice"]),
            ("Kirklareli", 41.7351m, 27.2252m, ["Merkez", "Luleburgaz", "Babaeski", "Vize"]),
            ("Kirsehir", 39.1425m, 34.1709m, ["Merkez", "Kaman", "Mucur", "Cicekdagi"]),
            ("Kocaeli", 40.8533m, 29.8815m, ["Izmit", "Gebze", "Darica", "Kartepe"]),
            ("Konya", 37.8714m, 32.4846m, ["Selcuklu", "Meram", "Karatay", "Eregli"]),
            ("Kutahya", 39.4167m, 29.9833m, ["Merkez", "Tavsanli", "Simav", "Gediz"]),
            ("Malatya", 38.3552m, 38.3095m, ["Battalgazi", "Yesilyurt", "Darende", "Akcadag"]),
            ("Manisa", 38.6191m, 27.4289m, ["Yunusemre", "Sehzadeler", "Akhisar", "Turgutlu"]),
            ("Mardin", 37.3122m, 40.7351m, ["Artuklu", "Kiziltepe", "Midyat", "Nusaybin"]),
            ("Mersin", 36.8121m, 34.6415m, ["Akdeniz", "Yenisehir", "Mezitli", "Tarsus"]),
            ("Mugla", 37.2153m, 28.3636m, ["Mentese", "Bodrum", "Marmaris", "Fethiye"]),
            ("Mus", 38.9462m, 41.7539m, ["Merkez", "Malazgirt", "Bulanik", "Varto"]),
            ("Nevsehir", 38.6244m, 34.7239m, ["Merkez", "Urgup", "Avanos", "Goreme"]),
            ("Nigde", 37.9667m, 34.6833m, ["Merkez", "Bor", "Ulukisla", "Ciftlik"]),
            ("Ordu", 40.9847m, 37.8789m, ["Altinordu", "Unye", "Fatsa", "Persembe"]),
            ("Osmaniye", 37.0742m, 36.2478m, ["Merkez", "Kadirli", "Duzici", "Bahce"]),
            ("Rize", 41.0201m, 40.5234m, ["Merkez", "Cayeli", "Ardesen", "Pazar"]),
            ("Sakarya", 40.7569m, 30.3781m, ["Adapazari", "Serdivan", "Erenler", "Sapanca"]),
            ("Samsun", 41.2867m, 36.3300m, ["Ilkadim", "Atakum", "Canik", "Bafra"]),
            ("Sanliurfa", 37.1674m, 38.7955m, ["Haliliye", "Eyyubiye", "Karakopru", "Siverek"]),
            ("Siirt", 37.9274m, 41.9453m, ["Merkez", "Kurtalan", "Baykan", "Pervari"]),
            ("Sinop", 42.0264m, 35.1551m, ["Merkez", "Boyabat", "Gerze", "Ayancik"]),
            ("Sirnak", 37.5164m, 42.4611m, ["Merkez", "Cizre", "Silopi", "Idil"]),
            ("Sivas", 39.7477m, 37.0179m, ["Merkez", "Sarkisla", "Gemerek", "Zara"]),
            ("Tekirdag", 40.9780m, 27.5110m, ["Suleymanpasa", "Corlu", "Cerkezkoy", "Malkara"]),
            ("Tokat", 40.3167m, 36.5500m, ["Merkez", "Erbaa", "Niksar", "Turhal"]),
            ("Trabzon", 41.0027m, 39.7168m, ["Ortahisar", "Akcaabat", "Yomra", "Vakfikebir"]),
            ("Tunceli", 39.1081m, 39.5483m, ["Merkez", "Pertek", "Hozat", "Mazgirt"]),
            ("Usak", 38.6823m, 29.4082m, ["Merkez", "Banaz", "Esme", "Sivasli"]),
            ("Van", 38.5012m, 43.3729m, ["Ipekyolu", "Tusba", "Edremit", "Ercis"]),
            ("Yalova", 40.6500m, 29.2667m, ["Merkez", "Ciftlikkoy", "Cinarcik", "Altinova"]),
            ("Yozgat", 39.8181m, 34.8147m, ["Merkez", "Sorgun", "Akdagmadeni", "Bogazliyan"]),
            ("Zonguldak", 41.4564m, 31.7987m, ["Merkez", "Eregli", "Caycuma", "Devrek"])
        };

        var concepts = new (string Suffix, string Story, decimal LatOffset, decimal LngOffset)[]
        {
            ("Ev Yemekleri", "Home-style meals and daily catering for family events and office lunches.", 0.018m, 0.014m),
            ("Kebap Davet", "Grill-focused event catering with generous service for group dining.", -0.016m, 0.018m),
            ("Kahvalti Sofrasi", "Breakfast tables, brunch service, and light celebration menus.", 0.012m, -0.017m),
            ("Kurumsal Catering", "Practical corporate catering for meetings, launches, and planned events.", -0.014m, -0.013m)
        };

        foreach (var city in cities)
        {
            for (var index = 0; index < concepts.Length; index++)
            {
                var concept = concepts[index];
                var district = city.Districts[index % city.Districts.Length];
                var slug = $"demo-{ToAsciiKey(city.City)}-{index + 1}";
                var email = $"demo.{ToAsciiKey(city.City)}.{index + 1}@vellora.local";

                if (await db.Caterers.AnyAsync(c => c.Slug == slug || c.Email == email))
                {
                    continue;
                }

                db.Caterers.Add(new CatererProfile
                {
                    Name = $"{city.City} {concept.Suffix}",
                    Slug = slug,
                    BrandStory = concept.Story,
                    City = city.City,
                    District = district,
                    AddressLine = $"{district} merkez, {city.City}",
                    Email = email,
                    Phone = "+90 555 000 00 00",
                    Latitude = city.Lat + concept.LatOffset,
                    Longitude = city.Lng + concept.LngOffset,
                    IsApproved = true
                });
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task<CuisineCategory> EnsureCategoryAsync(CateringProjectContext db, string name, string slug)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Slug == slug);
        if (category is not null)
        {
            return category;
        }

        category = new CuisineCategory { Name = name, Slug = slug, IconKey = "plate" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return category;
    }

    private static async Task EnsureOrdersAsync(CateringProjectContext db, int customerId, IReadOnlyList<int> catererIds)
    {
        if (await db.Orders.AnyAsync(o => o.PaymentReference != null && o.PaymentReference.StartsWith("DEMO-2026-")))
        {
            return;
        }

        var statuses = new[] { OrderStatus.Paid, OrderStatus.Preparing, OrderStatus.Delivered, OrderStatus.Completed, OrderStatus.Paid, OrderStatus.Preparing };
        for (var i = 0; i < Math.Min(6, catererIds.Count * 2); i++)
        {
            var catererId = catererIds[i % catererIds.Count];
            var lines = await db.MenuItems.Where(m => m.CatererId == catererId).OrderBy(m => m.Id).Take(2).ToListAsync();
            if (lines.Count == 0) continue;
            var order = new OrderEntity
            {
                CustomerId = customerId,
                CatererId = catererId,
                Status = statuses[i],
                CreatedUtc = DateTime.UtcNow.Date.AddDays(-i),
                CompletedUtc = statuses[i] is OrderStatus.Delivered or OrderStatus.Completed ? DateTime.UtcNow.Date.AddDays(-i).AddHours(4) : null,
                DeliveryAddress = "Demo event venue, Istanbul",
                EventNotes = "Seeded demo order for presentation.",
                PaymentReference = $"DEMO-2026-{i + 1:000}",
                TotalAmount = lines.Sum(m => m.BasePrice)
            };
            foreach (var line in lines)
            {
                order.Lines.Add(new OrderLineEntity { MenuItemId = line.Id, Quantity = 1, UnitPrice = line.BasePrice, CustomizationSummary = "Standard catering setup" });
            }
            db.Orders.Add(order);
        }
        await db.SaveChangesAsync();
    }

    private static async Task EnsureSmartMenuOptionsAsync(CateringProjectContext db)
    {
        var menuItems = await db.MenuItems
            .Include(m => m.Category)
            .Include(m => m.OptionGroups)
            .ThenInclude(g => g.Options)
            .ToListAsync();

        foreach (var item in menuItems)
        {
            var templates = new[] { BuildIncludedTemplate(item) }.Concat(BuildOptionTemplates(item));
            foreach (var template in templates)
            {
                var group = item.OptionGroups.FirstOrDefault(g => g.Title == template.Title);
                if (group is null)
                {
                    group = new MenuItemOptionGroup
                    {
                        MenuItemId = item.Id,
                        Title = template.Title,
                        AllowsMultipleSelections = template.AllowsMultipleSelections,
                        IsRequired = template.IsRequired,
                        MinSelection = template.MinSelection,
                        MaxSelection = template.MaxSelection
                    };
                    db.MenuItemOptionGroups.Add(group);
                    item.OptionGroups.Add(group);
                }
                else
                {
                    group.AllowsMultipleSelections = template.AllowsMultipleSelections;
                    group.IsRequired = template.IsRequired;
                    group.MinSelection = template.MinSelection;
                    group.MaxSelection = template.MaxSelection;
                }

                foreach (var option in template.Options)
                {
                    if (group.Options.Any(o => o.Title == option.Title))
                    {
                        continue;
                    }

                    group.Options.Add(new MenuItemOption
                    {
                        Title = option.Title,
                        QuantityInfo = option.QuantityInfo,
                        OptionType = template.OptionType,
                        PriceDelta = option.PriceDelta,
                        IsDefault = option.IsDefault,
                        IsAvailable = true,
                        IsRemovable = template.OptionType == MenuItemOptionType.Removable
                    });
                }
            }
        }

        await db.SaveChangesAsync();
    }

    private static OptionGroupSeed BuildIncludedTemplate(MenuItemEntity item)
    {
        var text = $"{item.Name} {item.Category?.Name} {item.Description}".ToLowerInvariant();

        if (ContainsAny(text, "pizza", "margherita", "pepperoni", "calzone"))
        {
            return Included(("Pizza portion", "1 tray"), ("House salad", "1 bowl"), ("Table sauce set", "standard"));
        }

        if (ContainsAny(text, "kebap", "kebab", "kofte", "kÃ¶fte", "adana", "grill", "izgara", "lamb", "kuzu"))
        {
            return Included(("Main grill portion", "1 serving"), ("Rice pilaf", "1 portion"), ("Seasonal salad", "1 portion"), ("Lavash", "2 pcs"));
        }

        if (ContainsAny(text, "kahvalt", "breakfast", "brunch", "serpme"))
        {
            return Included(("Cheese selection", "1 plate"), ("Olives", "1 bowl"), ("Fresh bread", "basket"), ("Tea service", "included"));
        }

        if (ContainsAny(text, "burger", "cheeseburger", "hamburger"))
        {
            return Included(("Burger", "1 pc"), ("Sauce set", "standard"), ("Napkin & service kit", "included"));
        }

        if (ContainsAny(text, "dessert", "tatli", "tatlÃ„Â±", "baklava", "kunefe", "tiramisu"))
        {
            return Included(("Dessert selection", "1 plate"), ("Seasonal fruit garnish", "included"), ("Service plates", "included"));
        }

        return Included(("Main dish", "1 portion"), ("Side dish", "1 portion"), ("Seasonal garnish", "included"));
    }

    private static IReadOnlyList<OptionGroupSeed> BuildOptionTemplates(MenuItemEntity item)
    {
        var text = $"{item.Name} {item.Category?.Name} {item.Description}".ToLowerInvariant();

        if (ContainsAny(text, "burger", "cheeseburger", "hamburger"))
        {
            return [
                Multi("Extras", 0, 3, ("Extra patty", 80m), ("Extra cheese", 25m), ("Extra caramelized onion", 20m), ("Extra sauce", 15m)),
                Multi("Remove Ingredients", 0, 4, ("No onion", 0m), ("No pickles", 0m), ("No tomato", 0m), ("No sauce", 0m)),
                Single("Side Choice", false, ("French fries", 60m, false), ("Onion rings", 70m, false), ("Side salad", 50m, false)),
                Single("Drink Choice", false, ("Ayran", 30m, false), ("Cola", 50m, false), ("Lemonade", 45m, false))
            ];
        }

        if (ContainsAny(text, "grill", "izgara", "kebap", "kebab", "et ", "kofte", "köfte", "lamb", "kuzu", "adana", "beyti", "tavuk sis", "tavuk ÅŸiÅŸ"))
        {
            return [
                Single("Cooking Preference", true, ("Medium", 0m, true), ("Well done", 0m, false)),
                Multi("Extras", 0, 4, ("Extra grilled meat", 120m), ("Extra rice pilaf", 45m), ("Extra lavash", 20m), ("Extra grilled vegetables", 55m)),
                Multi("Remove Ingredients", 0, 3, ("No onion", 0m), ("No spicy sauce", 0m), ("No parsley", 0m)),
                Single("Drink Choice", false, ("Ayran", 30m, false), ("Salgam", 35m, false), ("Mineral water", 25m, false))
            ];
        }

        if (ContainsAny(text, "kahvalt", "breakfast", "brunch", "serpme"))
        {
            return [
                Multi("Extras", 0, 4, ("Extra egg", 35m), ("Extra cheese plate", 70m), ("Extra olives", 30m), ("Extra honey & cream", 60m), ("Extra borek", 55m)),
                Multi("Remove Ingredients", 0, 4, ("No olives", 0m), ("No tomato", 0m), ("No cucumber", 0m), ("No jam", 0m)),
                Single("Drink Choice", true, ("Turkish tea", 0m, true), ("Fresh orange juice", 65m, false), ("Turkish coffee", 45m, false))
            ];
        }

        if (ContainsAny(text, "borek", "bÃ¶rek", "gözleme", "gozleme", "pastry"))
        {
            return [
                Single("Filling Choice", true, ("Cheese filling", 0m, true), ("Potato filling", 0m, false), ("Spinach filling", 10m, false), ("Minced meat filling", 45m, false)),
                Multi("Extras", 0, 3, ("Extra cheese", 25m), ("Extra yogurt sauce", 25m), ("Extra tea", 20m)),
                Multi("Remove Ingredients", 0, 2, ("No parsley", 0m), ("No sesame", 0m))
            ];
        }

        if (ContainsAny(text, "tatli", "tatlÄ±", "dessert", "baklava", "kunefe", "kÃ¼nefe", "sutlac", "sÃ¼tlaÃ§", "kazandibi", "tiramisu", "panna", "cannoli"))
        {
            return [
                Multi("Extras", 0, 3, ("Extra pistachio", 65m), ("Extra ice cream", 55m), ("Extra chocolate sauce", 35m), ("Extra fruit topping", 45m)),
                Multi("Remove Ingredients", 0, 2, ("No nuts", 0m), ("No syrup", 0m)),
                Single("Drink Choice", false, ("Turkish coffee", 45m, false), ("Tea", 20m, false), ("Lemonade", 45m, false))
            ];
        }

        if (ContainsAny(text, "meze", "salata", "salad", "hummus", "humus", "ezme", "haydari", "cacik", "cacÄ±k"))
        {
            return [
                Multi("Extras", 0, 4, ("Extra hummus", 45m), ("Extra haydari", 45m), ("Extra ezme", 40m), ("Extra bread", 15m), ("Extra salad", 40m)),
                Multi("Remove Ingredients", 0, 3, ("No garlic", 0m), ("No spicy sauce", 0m), ("No onion", 0m))
            ];
        }

        if (ContainsAny(text, "sebze", "vegetarian", "vegan", "buddha", "quinoa", "tofu", "falafel", "green"))
        {
            return [
                Multi("Extras", 0, 4, ("Extra grilled vegetables", 55m), ("Extra chickpeas", 45m), ("Extra avocado", 70m), ("Extra vegan sauce", 30m)),
                Multi("Remove Ingredients", 0, 3, ("No onion", 0m), ("No garlic", 0m), ("No spicy sauce", 0m))
            ];
        }

        if (ContainsAny(text, "balik", "balÄ±k", "seafood", "fish", "sushi", "salmon", "tuna", "ebi", "maki", "roll"))
        {
            return [
                Multi("Extras", 0, 4, ("Extra grilled fish portion", 130m), ("Extra salad", 40m), ("Extra lemon sauce", 25m), ("Extra roasted vegetables", 55m)),
                Multi("Remove Ingredients", 0, 2, ("No onion", 0m), ("No garlic sauce", 0m)),
                Single("Drink Choice", false, ("Mineral water", 25m, false), ("Lemonade", 45m, false), ("Ayran", 30m, false))
            ];
        }

        if (ContainsAny(text, "davet", "kokteyl", "catering", "event package", "package", "kurumsal", "table presentation"))
        {
            return [
                Single("Package Size", true, ("Standard package", 0m, true), ("Large package", 180m, false), ("Premium package", 350m, false)),
                Multi("Extras", 0, 4, ("Extra appetizer selection", 120m), ("Extra dessert selection", 100m), ("Extra drink service", 90m), ("Extra table presentation", 150m)),
                Multi("Remove Ingredients", 0, 2, ("No spicy items", 0m), ("No nuts", 0m))
            ];
        }

        if (ContainsAny(text, "ayran", "tea", "coffee", "espresso", "cappuccino", "limonata", "lemonade", "juice", "water", "soda", "kombucha", "salgam", "Åalgam"))
        {
            return [
                Single("Drink Size", true, ("Standard", 0m, true), ("Large", 25m, false)),
                Multi("Extras", 0, 2, ("Extra ice", 0m), ("Extra lemon", 10m), ("No sugar", 0m))
            ];
        }

        return [
            Single("Portion Size", true, ("Standard", 0m, true), ("Large portion", 75m, false)),
            Multi("Extras", 0, 4, ("Extra rice pilaf", 45m), ("Extra yogurt", 35m), ("Extra salad", 40m), ("Extra bread", 15m)),
            Multi("Remove Ingredients", 0, 3, ("No onion", 0m), ("No parsley", 0m), ("Less spicy", 0m))
        ];
    }

    private static bool ContainsAny(string text, params string[] needles) =>
        needles.Any(text.Contains);

    private static OptionGroupSeed Included(params (string Title, string QuantityInfo)[] options) =>
        new("Included Menu Items", MenuItemOptionType.Included, false, true, 0, 0, options.Select(o => new OptionSeed(o.Title, 0, true, o.QuantityInfo)).ToArray());

    private static OptionGroupSeed Multi(string title, int min, int max, params (string Title, decimal PriceDelta)[] options) =>
        new(title, GuessOptionType(title), false, true, min, max, options.Select(o => new OptionSeed(o.Title, o.PriceDelta, false, null)).ToArray());

    private static OptionGroupSeed Single(string title, bool required, params (string Title, decimal PriceDelta, bool IsDefault)[] options) =>
        new(title, GuessOptionType(title), required, false, required ? 1 : 0, 1, options.Select(o => new OptionSeed(o.Title, o.PriceDelta, o.IsDefault, null)).ToArray());

    private static string GuessOptionType(string title)
    {
        if (title.Contains("Drink", StringComparison.OrdinalIgnoreCase)) return MenuItemOptionType.Drink;
        if (title.Contains("Remove", StringComparison.OrdinalIgnoreCase)) return MenuItemOptionType.Removable;
        return MenuItemOptionType.Extra;
    }

    private sealed record OptionGroupSeed(string Title, string OptionType, bool IsRequired, bool AllowsMultipleSelections, int MinSelection, int MaxSelection, IReadOnlyList<OptionSeed> Options);
    private sealed record OptionSeed(string Title, decimal PriceDelta, bool IsDefault, string? QuantityInfo);

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, string fullName, string email, string password, string role, int? catererProfileId, int? customerProfileId)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                IsEmailVerified = true,
                EmailVerifiedAt = DateTime.UtcNow,
                CatererProfileId = catererProfileId,
                CustomerProfileId = customerProfileId,
                IsActive = true
            };
            var created = await userManager.CreateAsync(user, password);
            if (!created.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", created.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            user.FullName = fullName;
            user.EmailConfirmed = true;
            user.IsEmailVerified = true;
            user.EmailVerifiedAt ??= DateTime.UtcNow;
            user.EmailVerificationCode = null;
            user.EmailVerificationCodeExpiresAt = null;
            user.CatererProfileId = catererProfileId;
            user.CustomerProfileId = customerProfileId;
            user.IsActive = true;
            await userManager.UpdateAsync(user);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, token, password);
        }

        foreach (var existingRole in await userManager.GetRolesAsync(user))
        {
            if (!string.Equals(existingRole, role, StringComparison.OrdinalIgnoreCase))
            {
                await userManager.RemoveFromRoleAsync(user, existingRole);
            }
        }
        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }

    private static string Slugify(string text) =>
        string.Join("-", text.ToLowerInvariant().Split([' ', '/', '&'], StringSplitOptions.RemoveEmptyEntries)).Trim('-');

    private static string ToAsciiKey(string text)
    {
        var normalized = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(character switch
            {
                'ı' => 'i',
                'ğ' => 'g',
                'ü' => 'u',
                'ş' => 's',
                'ö' => 'o',
                'ç' => 'c',
                _ when char.IsLetterOrDigit(character) => character,
                _ => '-'
            });
        }

        return string.Join("-", builder.ToString().Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
}
