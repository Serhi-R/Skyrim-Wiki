using SkyrimWIKI.Models;

namespace SkyrimWIKI.Data;

// Fills the wiki with starter content: races, locations, quests and a few
// comments. Runs after DbSeeder.Seed on every start-up and does nothing once
// data exists, so it is safe to call repeatedly.
//
// The text is written in my own words from the Skyrim wiki (cross-checked with
// UESP). No game numbers are copied; the level and reward fields are rough
// guides and are listed as unverified in docs/CONTENT-GAPS.md.
public static class DomainSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Races.Any())
        {
            return;
        }

        db.Races.AddRange(
            new Race { Name = "Nord", Playable = true, Description =
                "Tall, fair-haired people of Skyrim, hardened by the cold. They resist " +
                "frost, favour heavy weapons, and can raise a Battle Cry that sends enemies " +
                "running." },
            new Race { Name = "Dunmer", Playable = true, Description =
                "Dark Elves of Morrowind, equally at home with a blade and with fire. They " +
                "resist fire and can wrap themselves in the flames of Ancestor's Wrath." },
            new Race { Name = "Khajiit", Playable = true, Description =
                "Feline people of Elsweyr, quick, agile and natural thieves. Their claws are " +
                "deadly unarmed weapons and their Eyes of the Night see in the dark." },
            new Race { Name = "Argonian", Playable = true, Description =
                "Reptilian folk of Black Marsh, at home in swamp and water. They breathe " +
                "underwater, resist disease and can call on the Histskin to heal quickly." },
            new Race { Name = "Orsimer", Playable = true, Description =
                "The Orcs of the Wrothgarian and Dragontail mountains. Master smiths and " +
                "feared heavy-armour warriors who give in to Berserker Rage." },
            new Race { Name = "Altmer", Playable = true, Description =
                "High Elves of the Summerset Isles and the most gifted mages of Tamriel. " +
                "Their Highborn blood lets magicka return at great speed." },
            new Race { Name = "Bosmer", Playable = true, Description =
                "Wood Elves of Valenwood and the finest archers in Tamriel. They shrug off " +
                "poison and disease and can command animals to fight for them." },
            new Race { Name = "Breton", Playable = true, Description =
                "Natives of High Rock with a quick grasp of spellcraft and a natural " +
                "resistance to magic. Their Dragonskin absorbs hostile spells." },
            new Race { Name = "Imperial", Playable = true, Description =
                "Natives of Cyrodiil, shrewd diplomats and traders who are just as skilled " +
                "with blade and spell. Their Voice of the Emperor can calm a foe." },
            new Race { Name = "Redguard", Playable = true, Description =
                "The most naturally talented warriors of Hammerfell, hardy and resistant to " +
                "poison. Adrenaline Rush gives them a surge of stamina in combat." });

        var locations = new List<Location>
        {
            new() { Name = "Whiterun", Region = "Whiterun Hold", Description =
                "The trading city at the heart of Skyrim, split across three districts " +
                "beneath Dragonsreach. Home of the Companions in Jorrvaskr." },
            new() { Name = "Windhelm", Region = "Eastmarch", Description =
                "The oldest city in Skyrim and the seat of the Stormcloak rebellion under " +
                "Jarl Ulfric. Its Dunmer live in the Gray Quarter while Argonians work the docks." },
            new() { Name = "Riften", Region = "The Rift", Description =
                "A canal city on Lake Honrich, home to the Thieves' Guild in the Ratway and " +
                "to the corrupt Black-Briar family." },
            new() { Name = "Solitude", Region = "Haafingar", Description =
                "The wealthy capital of Skyrim, perched on a stone arch above the sea. Seat " +
                "of the Imperial Legion in Castle Dour and home of the Blue Palace." },
            new() { Name = "Markarth", Region = "The Reach", Description =
                "A city carved into a cliff above an ancient Dwemer ruin, forever troubled by " +
                "the native Forsworn rebels." },
            new() { Name = "Falkreath", Region = "Falkreath Hold", Description =
                "A quiet forest city in south-western Skyrim, close to the borders of " +
                "Cyrodiil and Hammerfell and known for its vast graveyard." },
            new() { Name = "Dawnstar", Region = "The Pale", Description =
                "A northern garrison town and fishing port, one of the oldest holds and one " +
                "of the least populated thanks to its harsh, sunless coast." },
            new() { Name = "Morthal", Region = "Hjaalmarch", Description =
                "A small, misty city in the marshes of Hjaalmarch and the capital of the " +
                "hold, home to the mage Falion." },
            new() { Name = "Winterhold", Region = "Winterhold", Description =
                "A once-great northern city, now largely ruined, dominated by the College of " +
                "Winterhold on its cliff above the sea." },
            new() { Name = "Ivarstead", Region = "The Rift", Description =
                "A small village at the foot of the Seven Thousand Steps, the last stop " +
                "before the long climb to High Hrothgar." }
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
            new Quest { Title = "Bleak Falls Barrow", Level = 4, Reward = "Dragonstone and gold",
                LocationId = whiterun.Id, Description =
                "Farengar, the court wizard of Whiterun, wants a stone tablet from the " +
                "barrow that is said to map the ancient dragon burial sites. The first real " +
                "dungeon crawl of the main story." },
            new Quest { Title = "Dragon Rising", Level = 6, Reward = "Thane of Whiterun",
                LocationId = whiterun.Id, Description =
                "A dragon has been sighted at the watchtower near Whiterun. Join Irileth and " +
                "the city guard to bring it down, then report back to Jarl Balgruuf." },
            new Quest { Title = "The Man Who Cried Wolf", Level = 6, Reward = "Gold from Falk Firebeard",
                LocationId = solitude.Id, Description =
                "A representative from Dragon Bridge asks the court in Solitude for help. " +
                "Clear Wolfskull Cave, where a circle of necromancers is trying to summon the Wolf Queen." },
            new Quest { Title = "The Mind of Madness", Level = 5, Reward = "Wabbajack",
                LocationId = solitude.Id, Description =
                "A ragged beggar named Dervenin asks you to fetch his master back from " +
                "holiday. To reach him you must first gain entry to the Pelagius Wing of the Blue Palace." },
            new Quest { Title = "Missing In Action", Level = 6, Reward = "Gold and a family heirloom",
                LocationId = windhelm.Id, Description =
                "A Windhelm merchant refuses to believe her son is dead. She believes the " +
                "Thalmor are holding him at Northwatch Keep and begs you to find out the truth." },
            new Quest { Title = "The Pursuit", Level = 12, Reward = "Guild advancement",
                LocationId = riften.Id, Description =
                "Brynjolf is sure Mercer Frey will attempt one last heist before leaving " +
                "Skyrim. Break into his manor in Riften and gather the evidence he left behind." },
            new Quest { Title = "A Night To Remember", Level = 14, Reward = "Sanguine Rose",
                LocationId = riften.Id, Description =
                "A friendly drinking contest with a stranger named Sam Guevenne turns into a " +
                "night you cannot remember, and a trail of debts across Skyrim to put right." },
            new Quest { Title = "The Forsworn Conspiracy", Level = 9, Reward = "Varies with the outcome",
                LocationId = markarth.Id, Description =
                "A murder in the Markarth market, in broad daylight and in front of the " +
                "guards, opens a conspiracy that reaches the heart of the city's government." },
            new Quest { Title = "Siege on the Dragon Cult", Level = 12, Reward = "Rahgot, a dragon priest mask",
                LocationId = falkreath.Id, Description =
                "A soldier named Valmir asks you to recover a powerful mask from the ruined " +
                "fort of Forelhost, a place where the old dragon cult made its last stand." },
            new Quest { Title = "Diplomatic Immunity", Level = 14, Reward = "Progress in the main story",
                LocationId = solitude.Id, Description =
                "Delphine believes the Thalmor are behind the returning dragons. Infiltrate " +
                "the Thalmor Embassy near Solitude to find out what their ambassador is hiding." },
            new Quest { Title = "The Way of the Voice", Level = 8, Reward = "Word of Power",
                LocationId = ivarstead.Id, Description =
                "The Greybeards call you to High Hrothgar. Climb the Seven Thousand Steps " +
                "from Ivarstead and learn to use the Voice from the masters on the mountain." },
            new Quest { Title = "Waking Nightmare", Level = 6, Reward = "Skull of Corruption",
                LocationId = dawnstar.Id, Description =
                "The people of Dawnstar are trapped in the same nightmare night after night. " +
                "Follow the trail to Nightcaller Temple and the Daedric prince behind it." });
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
                QuestId = db.Quests.Single(q => q.Title == "Waking Nightmare").Id,
                AuthorName = "dawnstar-sleepless",
                Rating = 5,
                Text = "Two Daedric artefacts and a tower full of history, all in one visit."
            });
        db.SaveChanges();
    }
}
