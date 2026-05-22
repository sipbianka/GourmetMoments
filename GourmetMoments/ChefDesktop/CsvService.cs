using System.IO;

namespace ChefDesktop
{
    public static class CsvService
    {
        private const string Header = "id;chefname;datum;kategoria;osszeg;megjegyzes";

        public static List<Expense> Load(string filePath)
        {
            var list = new List<Expense>();

            if (!File.Exists(filePath))
                return list;

            var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
            if (lines.Length == 0) return list;

            int start = 0;
            if (lines[0].Trim().StartsWith("id;", StringComparison.OrdinalIgnoreCase))
                start = 1;

            for (int i = start; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var e = Expense.FromCsv(line);
                    list.Add(e);
                }
                catch
                {
                    continue;
                }
            }

            return list;
        }

        public static void Save(string filePath, IEnumerable<Expense> expenses)
        {
            var lines = new List<string> { Header };
            lines.AddRange(expenses.Select(e => e.ToCsvLine()));
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
        }
    }
}