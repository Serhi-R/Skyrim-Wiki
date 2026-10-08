using SkyrimWIKI.Models;

namespace SkyrimWIKI.Data;

// Fills the wiki with starter content: races, locations, quests and a few
// comments. Runs after DbSeeder.Seed on every start-up and does nothing once
// data exists, so it is safe to call repeatedly.
public static class DomainSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Races.Any())
        {
            return;
        }

        db.Races.AddRange(
            new Race
            {
                Name = "Nord",
                Playable = true,
                Description = "Tall, fair-haired warriors from the frozen north of Skyrim. " +
                              "They favour two-handed weapons and resist frost."
            },
            new Race
            {
                Name = "Dunmer",
                Playable = true,
                Description = "Dark elves from Morrowind, skilled with destruction magic, " +
                              "stealth and long blades. They resist fire and poison."
            },
            new Race
            {
                Name = "Khajiit",
                Playable = true,
                Description = "Feline people of Elsweyr with sharp claws and excellent night " +
                              "vision. Natural sneakers and archers."
            },
            new Race
            {
                Name = "Argonian",
                Playable = true,
                Description = "Reptilian folk of Black Marsh. They can breathe underwater, " +
                              "resist disease and are natural lockpicks."
            },
            new Race
            {
                Name = "Orsimer",
                Playable = true,
                Description = "The Orcs of Orsinium. Heavy armour and one-handed weapons suit " +
                              "their proud warrior traditions."
            },
            new Race
            {
                Name = "Altmer",
                Playable = true,
                Description = "The High Elves of the Summerset Isle. The strongest magical " +
                              "affinity of all races, but weaker in combat."
            });

        var locations = new List<Location>
        {
            new()
            {
                Name = "Whiterun",
                Region = "Whiterun Hold",
                Description = "The trading heart of Skyrim, built around the Gildergreen tree " +
                              "and guarded by the Companions in Jorrvaskr."
            },
            new()
            {
                Name = "Windhelm",
                Region = "Eastmarch",
                Description = "The oldest city in Skyrim, seat of the High King and home of " +
                              "the Gray Quarter."
            },
            new()
            {
                Name = "Riften",
                Region = "The Rift",
                Description = "A canal city infamous for the Thieves Guild, the Bee and Barb " +
                              "and more than a few fenced goods."
            },
            new()
            {
                Name = "Solitude",
                Region = "Haafingar",
                Description = "The capital of Skyrim, perched on a natural arch and home to " +
                              "the Blue Palace and the Bard's College."
            },
            new()
            {
                Name = "Markarth",
                Region = "The Reach",
                Description = "A city of ancient Dwemer stonework, forever troubled by the " +
                              "rebels of the Forsworn."
            },
            new()
            {
                Name = "Falkreath",
                Region = "Falkreath Hold",
                Description = "A quiet hold capital surrounded by pine forest and, above all, " +
                              "by cemeteries."
            },
            new()
            {
                Name = "Dawnstar",
                Region = "Winterhold",
                Description = "A fishing port on the northern coast, haunted by nightmares " +
                              "and rich in iron mines."
            },
            new()
            {
                Name = "Ivarstead",
                Region = "The Rift",
                Description = "A small village at the foot of the Seven Thousand Steps, the " +
                              "last stop before High Hrothgar."
            }
        };
        db.Locations.AddRange(locations);
        db.SaveChanges();

        // Locations are saved first so the quests can reference their ids.
        var whiterun = db.Locations.Single(l => l.Name == "Whiterun");
        var windhelm = db.Locations.Single(l => l.Name == "Windhelm");
        var riften = db.Locations.Single(l => l.Name == "Riften");
        var solitude = db.Locations.Single(l => l.Name == "Solitude");
        var markarth = db.Locations.Single(l => l.Name == "Markarth");
        var falkreath = db.Locations.Single(l => l.Name == "Falkreath");
        var dawnstar = db.Locations.Single(l => l.Name == "Dawnstar");
        var ivarstead = db.Locations.Single(l => l.Name == "Ivarstead");

        db.Quests.AddRange(
            new Quest
            {
                Title = "Bleak Falls Barrow",
                Level = 4,
                Reward = "500 gold, Dragonstone",
                LocationId = whiterun.Id,
                Description = "Deliver the Dragonstone to Farengar and clear the ancient barrow " +
                              "on the way. The first real dungeon crawl of the main story."
            },
            new Quest
            {
                Title = "The Man Who Cried Wolf",
                Level = 6,
                Reward = "1,000 gold",
                LocationId = solitude.Id,
                Description = "A retired scholar wants one more look at Wolfskull Cave before " +
                              "the dragons return to Skyrim."
            },
            new Quest
            {
                Title = "Missing In Action",
                Level = 6,
                Reward = "400 gold",
                LocationId = windhelm.Id,
                Description = "A merchant believes his missing son may still be alive with the " +
                              "Thalmor. Track the party down before it is too late."
            },
            new Quest
            {
                Title = "The Pursuit",
                Level = 8,
                Reward = "1,000 gold",
                LocationId = riften.Id,
                Description = "Break into Mercer Frey's house, gather the evidence and report " +
                              "back to Brynjolf."
            },
            new Quest
            {
                Title = "The Forsworn Conspiracy",
                Level = 9,
                Reward = "None",
                LocationId = markarth.Id,
                Description = "A murder in the market reveals a plot that reaches the heart of " +
                              "Markarth's government."
            },
            new Quest
            {
                Title = "Deathbrand",
                Level = 36,
                Reward = "Deathbrand armor set",
                LocationId = dawnstar.Id,
                Description = "A treasure map found in the wilds leads to the helm of the most " +
                              "notorious pirate who ever lived."
            },
            new Quest
            {
                Title = "The Woodsman's Friend",
                Level = 5,
                Reward = "500 gold",
                LocationId = falkreath.Id,
                Description = "A hunter asks you to find his missing friend somewhere in the " +
                              "pine forest south of Falkreath."
            },
            new Quest
            {
                Title = "The Way of the Voice",
                Level = 1,
                Reward = "Fus Ro Dah",
                LocationId = ivarstead.Id,
                Description = "Climb the Seven Thousand Steps to High Hrothgar and learn the " +
                              "first Word of Power from the Greybeards."
            });
        db.SaveChanges();

        db.Comments.AddRange(
            new Comment
            {
                QuestId = db.Quests.Single(q => q.Title == "Bleak Falls Barrow").Id,
                AuthorName = "lore-fan",
                Rating = 5,
                Text = "Classic first dungeon. Bring potions and watch for the frostbite spiders."
            },
            new Comment
            {
                QuestId = db.Quests.Single(q => q.Title == "Bleak Falls Barrow").Id,
                AuthorName = "whiterun-guard",
                Rating = 4,
                Text = "Short and straightforward, but the Golden Claw puzzle is easy to miss."
            },
            new Comment
            {
                QuestId = db.Quests.Single(q => q.Title == "Deathbrand").Id,
                AuthorName = "seadog",
                Rating = 5,
                Text = "Best armor set in the game if you find it early enough."
            });
        db.SaveChanges();
    }
}
