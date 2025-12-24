using System;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Data.SQLite;

namespace RailwayApp
{
    public partial class MainForm : Form
    {
        private readonly IDataRepository _repository;
        private readonly BindingSource _bindingSource = new BindingSource();
        private string _sortColumn = nameof(Tariff.Direction);
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        public MainForm()
        {
            InitializeComponent();
            SetupDataGridView();

            _repository = AppConfig.UseDatabase
                ? (IDataRepository)new DatabaseRepository()
                : new LocalRepository();

            Text += AppConfig.UseDatabase
                ? " (Режим: База данных)"
                : " (Режим: Локальный)";

            InitializeSampleData();
        }

        private void SetupDataGridView()
        {
            dataGridView.AutoGenerateColumns = false;
            dataGridView.DataSource = _bindingSource;

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Tariff.Direction),
                HeaderText = "Направление",
                Name = "colDirection"
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Tariff.BaseCost),
                HeaderText = "Базовая стоимость",
                Name = "colBaseCost",
                DefaultCellStyle = { Format = "C" },
                SortMode = DataGridViewColumnSortMode.Automatic
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Tariff.FinalCost),
                HeaderText = "Итоговая стоимость",
                Name = "colFinalCost",
                DefaultCellStyle = { Format = "C" },
                SortMode = DataGridViewColumnSortMode.Automatic
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Tariff.DiscountType),
                HeaderText = "Тип скидки",
                Name = "colDiscountType"
            });

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            dataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_ColumnHeaderMouseClick);
        }

        private void ApplySorting()
        {
            var tariffs = _repository.GetAllTariffs().AsQueryable();
            switch (_sortColumn)
            {
                case nameof(Tariff.Direction):
                    tariffs = _sortDirection == ListSortDirection.Ascending
                        ? tariffs.OrderBy(t => t.Direction, StringComparer.CurrentCulture)
                        : tariffs.OrderByDescending(t => t.Direction, StringComparer.CurrentCulture);
                    break;

                case nameof(Tariff.BaseCost):
                    tariffs = _sortDirection == ListSortDirection.Ascending
                        ? tariffs.OrderBy(t => t.BaseCost)
                        : tariffs.OrderByDescending(t => t.BaseCost);
                    break;

                case nameof(Tariff.FinalCost):
                    tariffs = _sortDirection == ListSortDirection.Ascending
                        ? tariffs.OrderBy(t => t.FinalCost)
                        : tariffs.OrderByDescending(t => t.FinalCost);
                    break;

                case nameof(Tariff.DiscountType):
                    tariffs = _sortDirection == ListSortDirection.Ascending
                        ? tariffs.OrderBy(t => t.GetDiscountPercentForSorting())
                        : tariffs.OrderByDescending(t => t.GetDiscountPercentForSorting());
                    break;
            }

            _bindingSource.DataSource = tariffs.ToList();
            UpdateSortIndicators();
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = dataGridView.Columns[e.ColumnIndex];
            string propertyName = column.DataPropertyName;

            if (_sortColumn == propertyName)
            {
                _sortDirection = _sortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                _sortColumn = propertyName;
                _sortDirection = ListSortDirection.Ascending;
            }
            ApplySorting();
            UpdateSortIndicators();
        }

        private void UpdateSortIndicators()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            var sortedColumn = dataGridView.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c => c.DataPropertyName == _sortColumn);

            if (sortedColumn != null)
            {
                sortedColumn.HeaderCell.SortGlyphDirection =
                    _sortDirection == ListSortDirection.Ascending
                        ? SortOrder.Ascending
                        : SortOrder.Descending;
            }
        }

        private void InitializeSampleData()
        {
            if (_repository.GetAllTariffs().Count == 0)
            {
                _repository.AddTariff(new Tariff("Москва", 5000, new NoDiscount()));
                _repository.AddTariff(new Tariff("Санкт-Петербург", 3000, new PercentageDiscount(10)));
                _repository.AddTariff(new Tariff("Казань", 2000, new PercentageDiscount(5)));
            }
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            ApplySorting();
            dataGridView.ClearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new EditForm(null))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _repository.AddTariff(form.CreatedTariff);
                        RefreshGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите тариф для редактирования", "Упс!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedTariff = (Tariff)dataGridView.SelectedRows[0].DataBoundItem;
            Tariff originalTariff = new Tariff(
                selectedTariff.Direction,
                selectedTariff.BaseCost,
                selectedTariff.Strategy
            );
            using (var form = new EditForm(selectedTariff))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _repository.UpdateTariff(originalTariff.Direction, form.CreatedTariff);
                        RefreshGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите тариф для удаления", "Упс!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Удалить выбранный тариф?", "Уточним",
       MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var selectedTariff = (Tariff)dataGridView.SelectedRows[0].DataBoundItem;
                _repository.RemoveTariff(selectedTariff.Direction);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFindMin_Click(object sender, EventArgs e)
        {
            try
            {
                var minDirections = _repository.GetAllTariffs()
                    .GroupBy(t => t.FinalCost)
                    .OrderBy(g => g.Key)
                    .First()
                    .Select(t => t.Direction)
                    .ToList();

                string message;
                if (minDirections.Count == 1)
                {
                    message = $"Направление с минимальной стоимостью:\n{minDirections[0]}";
                }
                else
                {
                    message = $"Направления с минимальной стоимостью ({minDirections.Count} штук):\n" +
                              string.Join("\n", minDirections);
                }
                MessageBox.Show(message, "Результат",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnOpenDataFolder_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", AppConfig.AppDataPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия папки: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                dialog.Title = "Сохранить тарифы";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileService.SaveToFile(_repository.GetAllTariffs(), dialog.FileName);
                        MessageBox.Show("Данные успешно сохранены!", "Получилось!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                dialog.Title = "Загрузить тарифы";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _repository.Clear();
                        var station = new Station();
                        FileService.LoadFromFile(station, dialog.FileName);

                        foreach (var tariff in station.GetAllTariffs())
                        {
                            _repository.AddTariff(tariff);
                        }

                    RefreshGrid();
                        MessageBox.Show("Данные успешно загружены!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            string arch = Environment.Is64BitProcess ? "x64" : "x86";
            string sqlitePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, arch, "sqlite3.dll");

            string debugInfo = $"Архитектура приложения: {arch}\n" +
                              $"Путь к SQLite: {sqlitePath}\n" +
                              $"Файл SQLite существует: {File.Exists(sqlitePath)}\n" +
                              $"Путь к БД: {AppConfig.DatabasePath}\n" +
                              $"Файл БД существует: {File.Exists(AppConfig.DatabasePath)}\n" +
                              $"Режим БД: {AppConfig.UseDatabase}\n\n";

            MessageBox.Show(debugInfo, "Отладочная информация");

            System.Diagnostics.Process.Start("explorer.exe", AppConfig.AppDataPath);
        }
    }
}