using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace ChefDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _csvPath;
        private readonly ObservableCollection<Expense> _expenses = new ObservableCollection<Expense>();
        private readonly string[] _categories = new[] { "Travel", "Ingredients", "Accommodation", "Equipment", "Other" };

        public MainWindow()
        {
            InitializeComponent();

            _csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chef_koltsegek_2025.csv");

            ExpensesGrid.ItemsSource = _expenses;

            foreach (var c in _categories)
                KategoriaComboBox.Items.Add(c);
            KategoriaComboBox.SelectedIndex = 0;
            DatumPicker.SelectedDate = DateTime.Now.Date;


            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var list = CsvService.Load(_csvPath);
                _expenses.Clear();
                foreach (var e in list.OrderBy(x => x.Id))
                    _expenses.Add(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a CSV beolvasásakor: " + ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var chef = ChefNameTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(chef))
            {
                MessageBox.Show("Adja meg a séf nevét.", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DatumPicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Adjon meg egy dátumot.", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var datum = DatumPicker.SelectedDate.Value.Date;

            var kategoria = (KategoriaComboBox.SelectedItem as string) ?? string.Empty;

            if (!decimal.TryParse(OsszegTextBox.Text?.Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var osszeg))
            {
                if (!decimal.TryParse(OsszegTextBox.Text?.Trim(), out osszeg))
                {
                    MessageBox.Show("Érvénytelen összeg.", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var megjegyzes = MegjegyzesTextBox.Text?.Trim() ?? string.Empty;

            var nextId = _expenses.Any() ? _expenses.Max(x => x.Id) + 1 : 1;

            var newExpense = new Expense
            {
                Id = nextId,
                ChefName = chef,
                Datum = datum,
                Kategoria = kategoria,
                Osszeg = osszeg,
                Megjegyzes = megjegyzes
            };

            _expenses.Add(newExpense);

            try
            {
                CsvService.Save(_csvPath, _expenses);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a CSV mentésekor: " + ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ChefNameTextBox.Clear();
            OsszegTextBox.Clear();
            MegjegyzesTextBox.Clear();
            DatumPicker.SelectedDate = DateTime.Now.Date;
            ChefNameTextBox.Focus();
        }
    }
}
