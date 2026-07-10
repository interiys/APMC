using APMC.DataApp;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace APMC.Pages
{
    public partial class LogsPage : Page
    {
        public LogsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                // Проверяем коннект
                if (ConnectObject.GetConnect() == null)
                {
                    MessageBox.Show("Нет подключения к базе данных", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                var logs = ConnectObject.GetConnect().UserLogs
                    .OrderByDescending(l => l.ActionTime)
                    .ToList();

                DGridLogs.ItemsSource = logs;

                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки логов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics()
        {
            try
            {
                // Проверяем DGridLogs
                if (DGridLogs == null)
                {
                    TBTotalLogs.Text = "Записей: 0";
                    TBTotalLogsCount.Text = "0";
                    return;
                }

                var logs = DGridLogs.ItemsSource as System.Collections.IEnumerable;
                int total = 0;
                int errors = 0;
                int warnings = 0;
                int info = 0;

                if (logs != null)
                {
                    foreach (object item in logs)
                    {
                        total++;
                        if (item is UserLog log)
                        {
                            if (log.ActionType != null)
                            {
                                if (log.ActionType.Contains("Ошибка"))
                                    errors++;
                                else if (log.ActionType.Contains("Предупреждение"))
                                    warnings++;
                                else
                                    info++;
                            }
                            else
                            {
                                info++;
                            }
                        }
                    }
                }

                if (TBTotalLogs != null)
                    TBTotalLogs.Text = $"Записей: {total}";

                if (TBTotalLogsCount != null)
                    TBTotalLogsCount.Text = total.ToString();

                if (TBErrorLogsCount != null)
                    TBErrorLogsCount.Text = errors.ToString();

                if (TBWarningLogsCount != null)
                    TBWarningLogsCount.Text = warnings.ToString();

                if (TBInfoLogsCount != null)
                    TBInfoLogsCount.Text = info.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка статистики: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadLogs();
        }
        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем DataGrid
                if (DGridLogs == null)
                {
                    MessageBox.Show("Таблица не найдена", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var logs = DGridLogs.ItemsSource as System.Collections.IEnumerable;

                if (logs == null)
                {
                    MessageBox.Show("Нет данных для экспорта", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv|Текстовые файлы (*.txt)|*.txt",
                    DefaultExt = ".csv",
                    FileName = $"logs_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var csv = new StringBuilder();
                    csv.AppendLine("Дата и время;Тип;Пользователь;Описание");

                    foreach (object item in logs)
                    {
                        if (item is UserLog log)
                        {
                            string time = log.ActionTime.HasValue ?
                                log.ActionTime.Value.ToString("dd.MM.yyyy HH:mm") : "";
                            string type = log.ActionType ?? "";
                            string user = log.UserName ?? "";
                            string desc = log.ActionDescription ?? "";

                            csv.AppendLine($"{time};{type};{user};{desc}");
                        }
                    }

                    File.WriteAllText(saveDialog.FileName, csv.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Файл сохранен: {saveDialog.FileName}", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Очистить весь журнал событий?\nЭто действие нельзя отменить.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = ConnectObject.GetConnect())
                    {
                        if (context == null)
                        {
                            MessageBox.Show("Нет подключения к базе данных", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        context.Database.ExecuteSqlCommand("DELETE FROM UserLogs");
                        context.SaveChanges();
                    }

                    LoadLogs();
                    MessageBox.Show("Журнал очищен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка очистки: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}