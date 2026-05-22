using System.Globalization;

namespace ChefConsole
{
    public class Berles
    {
        public int Uid { get; init; }
        public int ChefId { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public decimal DailyRate { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Cuisine { get; init; } = string.Empty;
        public decimal TotalPrice => (DaysIncluded * DailyRate);
        public int DaysIncluded => (EndDate - StartDate).Days + 1;

        public int OverlapDays(DateTime rangeStart, DateTime rangeEnd)
        {
            var s = StartDate < rangeStart ? rangeStart : StartDate;
            var e = EndDate > rangeEnd ? rangeEnd : EndDate;
            if (e < s) return 0;
            return (e - s).Days + 1;
        }

        public static bool TryParseFromCsv(string line, out Berles? b)
        {
            b = null;
            if (string.IsNullOrWhiteSpace(line)) return false;

            var parts = line.Split(',');
            if (parts.Length < 7) return false;

            try
            {
                var uid = int.Parse(parts[0].Trim());
                var chefid = int.Parse(parts[1].Trim());
                var start = DateTime.ParseExact(parts[2].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var end = DateTime.ParseExact(parts[3].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var daily = decimal.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);
                var name = parts[5].Trim();
                var cuisine = parts[6].Trim();

                b = new Berles
                {
                    Uid = uid,
                    ChefId = chefid,
                    StartDate = start,
                    EndDate = end,
                    DailyRate = daily,
                    Name = name,
                    Cuisine = cuisine
                };
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
