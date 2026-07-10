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
    public partial class AdminPage : Page
    {
        public AdminPage()
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
                DGridUsers.ItemsSource = ConnectObject.GetConnect().Users.ToList();

                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridUsers.ItemsSource = ConnectObject.GetConnect().Roles.ToList();

                UpdateStatistics();

            }
        }

        private void ButtonAddUser_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditUsers(null));
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new Authorization());
        }
        private void ButtonEditUser_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditUsers((sender as Button).DataContext as User));
        }

        private void DGridUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UpdateStatistics()
        {
            try
            {
                var users = DGridUsers.ItemsSource as IEnumerable<User>;

                if (users != null && users.Any())
                {
                    int totalUsers = 0;
                    int activeUsers = 0;
                    int adminUsers = 0;

                    foreach (var user in users)
                    {
                        totalUsers++;

                        // Считаем активных пользователей (предположим, что статус 1 = активен)
                        if (user.UserID == 1)
                        {
                            activeUsers++;
                        }

                        // Считаем администраторов (предположим, что роль 1 = администратор)
                        if (user.UserStatus == 1)
                        {
                            adminUsers++;
                        }
                    }

                    // Обновляем текстовые блоки
                    TBTotalUsers.Text = totalUsers.ToString();
                    TBActiveUsers.Text = activeUsers.ToString();
                    TBAdminUsers.Text = adminUsers.ToString();
                }
                else
                {
                    TBTotalUsers.Text = "0";
                    TBActiveUsers.Text = "0";
                    TBAdminUsers.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при обновлении статистики: {ex.Message}");
                TBTotalUsers.Text = "0";
                TBActiveUsers.Text = "0";
                TBAdminUsers.Text = "0";
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridUsers.ItemsSource = ConnectObject.GetConnect().Users.ToList();

                UpdateStatistics();

                MessageBox.Show("Данные обновлены!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка");
            }
        }
        private void ButtonLogs_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new LogsPage());
        }

        private void ButtonDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную строку из DataGrid
            var selectedItem1 = DGridUsers.SelectedItem as User;

            if (selectedItem1 == null)
            {
                MessageBox.Show("Выберите строку для удаления из первой таблицы!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result1 = MessageBox.Show(
                $"Вы уверены, что хотите удалить запись?\nОтчество: {selectedItem1.Patronymic}\nФамилия: {selectedItem1.LastName}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result1 == MessageBoxResult.Yes)
            {
                try
                {
                    var context = ConnectObject.GetConnect();
                    var itemToDelete1 = context.Users.Find(selectedItem1.UserID);
                    if (itemToDelete1 != null)
                    {
                        context.Users.Remove(itemToDelete1);
                        context.SaveChanges();

                        // Обновляем таблицу
                        RefreshUserTable();

                        MessageBox.Show("Запись успешно удалена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void RefreshUserTable()
        {
            try
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridUsers.ItemsSource = ConnectObject.GetConnect().Users.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении таблицы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}