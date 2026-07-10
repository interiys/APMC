using APMC.DataApp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace APMC.Pages
{
    public partial class SmenaUser : Page
    {
        private User _currentUser;

        public SmenaUser()
        {
            InitializeComponent();
            LoadCurrentUser();
        }

        private void LoadCurrentUser()
        {
            try
            {
                var userId = Session.s_userID;

                if (userId > 0)
                {
                    var context = ConnectObject.GetConnect();

                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    _currentUser = context.Users
                        .AsNoTracking()
                        .Where(u => u.UserID == userId)
                        .Select(u => new
                        {
                            u.UserID,
                            u.LastName,
                            u.FirstName,
                            u.Patronymic,
                            u.IsShiftActive
                        })
                        .AsEnumerable()
                        .Select(u => new User
                        {
                            UserID = u.UserID,
                            LastName = u.LastName,
                            FirstName = u.FirstName,
                            Patronymic = u.Patronymic,
                            IsShiftActive = u.IsShiftActive
                        })
                        .FirstOrDefault();

                    context.Configuration.AutoDetectChangesEnabled = true;
                    context.Configuration.LazyLoadingEnabled = true;

                    if (_currentUser != null)
                    {
                        UserNameText.Text = $"{_currentUser.LastName} {_currentUser.FirstName}";

                        ShiftCheckBox.IsChecked = _currentUser.IsShiftActive ?? false;

                        UpdateStatusText();

                        Debug.WriteLine($"Загружен статус: {_currentUser.IsShiftActive}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки пользователя: {ex.Message}");

                UserNameText.Text = "Сотрудник";
                ShiftCheckBox.IsChecked = false;
                UpdateStatusText();
            }
        }

        private void UpdateStatusText()
        {
            if (ShiftCheckBox.IsChecked == true)
            {
                StatusText.Text = "+ Вы на смене";
                StatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                StatusText.Text = " - Вы не на смене";
                StatusText.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveShiftStatus();
        }

        private void SaveShiftStatus()
        {
            try
            {
                var userId = Session.s_userID;
                if (userId <= 0) return;

                var context = ConnectObject.GetConnect();
                var dbUser = context.Users.Find(userId);

                if (dbUser != null)
                {
                    // Сохраняем состояние чекбокса
                    bool? newStatus = ShiftCheckBox.IsChecked;
                    dbUser.IsShiftActive = newStatus;

                    // Сохраняем в БД
                    int changes = context.SaveChanges();

                    // ПРОВЕРКА: Сразу читаем обратно из БД
                    context.Entry(dbUser).Reload();
                    bool? savedStatus = dbUser.IsShiftActive;

                    Debug.WriteLine($"Попытка сохранения:");
                    Debug.WriteLine($"  Чекбокс: {newStatus}");
                    Debug.WriteLine($"  Сохранено в БД: {savedStatus}");
                    Debug.WriteLine($"  Изменений: {changes}");

                    if (changes > 0)
                    {
                        MessageBox.Show($"Статус сохранен в БД: {savedStatus}", "Успех");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка");
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new Authorization());
        }
    }
}