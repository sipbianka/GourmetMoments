using ChefConsole;
using System.Globalization;

Console.OutputEncoding = System.Text.Encoding.UTF8;

const string csvFile = "chef_berlesek_2025.csv";
if (!File.Exists(csvFile))
{
    Console.WriteLine($"Nem található a fájl: {csvFile}");
    return;
}

List<Berles> berlesek = new();
var lines = File.ReadAllLines(csvFile);
if (lines.Length <= 1)
{
    Console.WriteLine("A CSV fájl üres vagy csak fejlécet tartalmaz.");
    return;
}

for (int i = 1; i < lines.Length; i++)
{
    if (Berles.TryParseFromCsv(lines[i], out var b) && b != null)
    {
        berlesek.Add(b);
    }
}

const int year = 2025;
var yearStart = new DateTime(year, 1, 1);
var yearEnd = new DateTime(year, 12, 31);

int month = 0;
while (true)
{
    Console.Write("Adjon meg egy hónapot (1-12): ");
    var input = Console.ReadLine();
    if (int.TryParse(input, out month) && month >= 1 && month <= 12) break;
    Console.WriteLine("Érvénytelen hónap. Próbálja újra.");
}

var monthStart = new DateTime(year, month, 1);
var monthEnd = monthStart.AddMonths(1).AddDays(-1);

// Havi bevétel
decimal monthlyRevenue = berlesek.Sum(b => b.DailyRate * b.OverlapDays(monthStart, monthEnd));

// Teljes éves bevétel
decimal yearlyRevenue = berlesek.Sum(b => b.DailyRate * b.OverlapDays(yearStart, yearEnd));

// Legdrágább bérlés
var mostExpensive = berlesek.OrderByDescending(b => b.TotalPrice).FirstOrDefault();

// 5. Number of distinct chefs rented during 2024 (any overlap with 2024)
var chefsInYear = berlesek
    .Where(b => b.OverlapDays(yearStart, yearEnd) > 0)
    .Select(b => b.ChefId)
    .Distinct()
    .Count();

// Bérelt séfek száma
var mostFrequentChefGroup = berlesek
    .Where(b => b.OverlapDays(yearStart, yearEnd) > 0)
    .GroupBy(b => b.Name)
    .Select(g => new { Name = g.Key, Count = g.Count() })
    .OrderByDescending(x => x.Count)
    .ThenBy(x => x.Name)
    .FirstOrDefault();

//  Leggyakrabban bérelt séf
var cuisineGroups = berlesek
    .Where(b => b.OverlapDays(yearStart, yearEnd) > 0)
    .GroupBy(b => b.Cuisine)
    .Select(g => new { Cuisine = g.Key, Count = g.Count() })
    .OrderByDescending(g => g.Count)
    .ThenBy(g => g.Cuisine)
    .ToList();

// Átlagos bérlés
var durations = berlesek
    .Select(b => b.OverlapDays(yearStart, yearEnd))
    .Where(d => d > 0)
    .Select(d => (decimal)d)
    .ToList();

decimal avgDuration = durations.Any() ? durations.Average() : 0m;

Console.WriteLine();
Console.WriteLine($"A(z) {month}. hónap bevétele: {Math.Round(monthlyRevenue):N0} euró");
Console.WriteLine($"A teljes {year}-es éves bevétel: {Math.Round(yearlyRevenue):N0} euró");

if (mostExpensive != null)
{
    Console.WriteLine($"A legdrágább bérlés {mostExpensive.Name} séftől volt, teljes ár: {Math.Round(mostExpensive.TotalPrice):N0} euró");
}
else
{
    Console.WriteLine("Nincs bérlés az adatállományban.");
}

Console.WriteLine($"Összesen {chefsInYear} különböző séfet béreltek ki.");

if (mostFrequentChefGroup != null)
{
    Console.WriteLine($"A legtöbbször bérelt séf: {mostFrequentChefGroup.Name} ({mostFrequentChefGroup.Count} bérlés)");
}
else
{
    Console.WriteLine("Nincs adat a legtöbbször bérelt séfről.");
}

Console.WriteLine("Bérlések száma konyhatípusonként:");
foreach (var g in cuisineGroups)
{
    Console.WriteLine($"{g.Cuisine}: {g.Count} bérlés");
}

var hu = new CultureInfo("hu-HU");
Console.WriteLine($"Átlagos bérlési időtartam: {avgDuration.ToString("F2", hu)} nap");