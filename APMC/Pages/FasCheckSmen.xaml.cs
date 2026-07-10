using APMC.DataApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APMC.Pages
{
    public partial class FasCheckSmen : Page
    {
        public FasCheckSmen()
        {
            InitializeComponent();

            // Вывод приветствия
            TBHello.Text = $"Добро пожаловать,\n{Session.s_userFirstName} {Session.s_userPatronymic}";
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridFasCheckSmenTable.ItemsSource = ConnectObject.GetConnect().FasTables.ToList();

                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridFasCheckSmenTable.ItemsSource = ConnectObject.GetConnect().ButterTables.ToList();

                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridFasCheckSmenTable.ItemsSource = ConnectObject.GetConnect().Departments.ToList();

                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridFasCheckSmenTable.ItemsSource = ConnectObject.GetConnect().Users.ToList();

                UpdateStatistics();
            }
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new Authorization());
        }

        // Метод для обновления статистики
        private void UpdateStatistics()
        {
            try
            {
                var employees = DGridFasCheckSmenTable.ItemsSource as IEnumerable<dynamic>;

                if (employees != null && employees.Any())
                {
                    int totalEmployees = 0;
                    int activeShifts = 0;
                    int fasDepartment = 0;

                    foreach (var employee in employees)
                    {
                        totalEmployees++;

                        // Считаем сотрудников на смене
                        if (employee.IsShiftActive == true)
                        {
                            activeShifts++;
                        }

                        // Считаем сотрудников цеха фасовки
                        // Предположим, что DepartmentID = 1 это цех фасовки
                        if (employee.DepartmentID != null && employee.DepartmentID.ToString() == "1")
                        {
                            fasDepartment++;
                        }
                    }

                    // Обновляем текстовые блоки
                    TBActiveShifts.Text = activeShifts.ToString();
                    TBTotalEmployees.Text = totalEmployees.ToString();
                }
                else
                {
                    TBActiveShifts.Text = "0";
                    TBTotalEmployees.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при обновлении статистики: {ex.Message}");
                TBActiveShifts.Text = "0";
                TBTotalEmployees.Text = "0";
            }
        }

        // Обработчик для кнопки "Обновить"
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обновляем данные из БД
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridFasCheckSmenTable.ItemsSource = ConnectObject.GetConnect().Users.ToList();

                // Обновляем статистику
                UpdateStatistics();

                MessageBox.Show("Данные обновлены!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка");
            }
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.GoBack();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = ConnectObject.GetConnect();
                int changesCount = context.ChangeTracker.Entries<User>()
                    .Where(entry => entry.State == System.Data.Entity.EntityState.Modified)
                    .Count();

                if (changesCount > 0)
                {
                    int savedCount = context.SaveChanges();

                    MessageBox.Show($"Успешно сохранено {savedCount} изменений!",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);

                    // Обновляем отображение
                    ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                    DGridFasCheckSmenTable.ItemsSource = ConnectObject.GetConnect().Users.ToList();

                    // Обновляем статистику
                    UpdateStatistics();
                }
                else
                {
                    MessageBox.Show("Нет изменений для сохранения.",
                                  "Информация",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void DGridFasCheckSmenTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}