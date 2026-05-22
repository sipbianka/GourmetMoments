using System.Globalization;

namespace ChefDesktop
{
    public class Expense
    {
        public int Id { get; set; }
        public string ChefName { get; set; } = string.Empty;
        public DateTime Datum { get; set; }
        public string Kategoria { get; set; } = string.Empty;
        public decimal Osszeg { get; set; }
        public string Megjegyzes { get; set; } = string.Empty;

        public static Expense FromCsv(string line)
        {
            var parts = line.Split(';');
            if (parts.Length < 6) throw new FormatException("Invalid CSV line: " + line);

            var e = new Expense();

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                throw new FormatException("Invalid id: " + parts[0]);
            e.Id = id;

            e.ChefName = parts[1];

            var dateString = parts[2];
            var dateFormats = new[] { "yyyy-MM-dd", "yyyy. MM. dd.", "yyyy.MM.dd.", "yyyy-MM-ddTHH:mm:ss", "yyyy/M/d", "M/d/yyyy", "d.MM.yyyy" };
            if (!DateTime.TryParseExact(dateString, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var datum)
                && !DateTime.TryParse(dateString, out datum))
            {
                datum = DateTime.Now.Date;
            }
            e.Datum = datum;

            e.Kategoria = parts[3];

            if (!decimal.TryParse(parts[4], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
            {
                amount = 0m;
            }
            e.Osszeg = amount;

            e.Megjegyzes = parts[5];

            return e;
        }

        public string ToCsvLine()
        {
            string safe(string s) => (s ?? string.Empty).Replace(";", ",");
            return string.Join(";", Id.ToString(CultureInfo.InvariantCulture),
                                   safe(ChefName),
                                   Datum.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                   safe(Kategoria),
                                   Osszeg.ToString(CultureInfo.InvariantCulture),
                                   safe(Megjegyzes));
        }
    }
}