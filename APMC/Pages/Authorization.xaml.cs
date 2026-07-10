using APMC.DataApp;
using APMC.Helpers;
using APMC.Pages;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
    /// <summary>
    /// Страница авторизации пользователей в системе
    /// </summary>
    /// <remarks>
    /// Предоставляет интерфейс для входа в систему с проверкой учетных данных,
    /// управлением блокировкой пользователей и дополнительной проверкой
    /// безопасности через CAPTCHA.
    /// Реализует механизм защиты от подбора пароля с блокировкой после 3
    /// неудачных попыток.
    /// </remarks>
    public partial class Authorization : Page
    {
        /// <summary>
        /// Счетчик неудачных попыток авторизации
        /// </summary>
        private int _numberOfAuthorizations;

        /// <summary>
        /// Инициализирует новый экземпляр страницы авторизации
        /// </summary>
        /// <remarks>
        /// Сбрасывает счетчик неудачных попыток авторизации при создании страницы
        /// </remarks>
        public Authorization()
        {
            InitializeComponent();

            _numberOfAuthorizations = 0;
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки входа в систему
        /// </summary>
        /// <param name="sender">Источник события − кнопка входа</param>
        /// <param name="e">Данные события маршрутизации</param>
        /// <remarks>
        /// Выполняет полный цикл авторизации:
        /// − валидация обязательных полей
        /// − проверка учетных данных в базе данных
        /// − управление блокировкой пользователя при неудачных попытках
        /// − дополнительная проверка CAPTCHA для пользователей со статусом 2
        /// − навигация на соответствующую страницу по роли пользователя
        /// </remarks>
        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            // Установка границ по умолчанию
            TBLogin.BorderBrush = Brushes.Gray;
            TBPassword.BorderBrush = Brushes.Gray;

            // Сбрасываем сообщение об ошибке
            ErrorMessage.Text = string.Empty;

            // Проверяем поля на заполненность
            if (string.IsNullOrEmpty(TBLogin.Text))
            {
                ErrorMessage.Text = "Введите логин";
                TBLogin.Focus(); // Ставим фокус на поле логина
                return;
            }

            if (string.IsNullOrEmpty(TBPassword.Password))
            {
                ErrorMessage.Text = "Введите пароль";
                TBPassword.Focus(); // Ставим фокус на поле пароля
                return;
            }

            // Поиск в БД пользователя с указанным логином
            var user = ConnectObject.GetConnect().Users.AsNoTracking()
                .FirstOrDefault(u => u.UserLogin == TBLogin.Text && u.UserPassword == TBPassword.Password);



                if (user != null)
                {
                // Используем новый метод с описанием
                    SimpleDbLogger.SetUser(user.UserLogin);
                    // Используем новый метод с описанием
                    SimpleDbLogger.LogAction("Вход", $"Пользователь {user.UserLogin} успешно авторизовался");

                    Session.s_userID = user.UserID;
                    Session.s_userID = user.UserID;
                // Используем новый метод с описанием
                    Session.s_userFirstName = user.FirstName;
                    Session.s_userPatronymic = user.Patronymic;

                    Session.CurrentUser = user;

                // Блокировка пользователя в случае х3 неверных вводов пароля
                if (_numberOfAuthorizations <= 3)
                    {
                        switch (user.UserStatus)
                        {
                            case 1:
                                MessageBox.Show("Вы заблокированы. Обратитесь к администратору.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            case 2:
                                // Сброс счетчика при успешном входе
                                _numberOfAuthorizations = 0;

                                // Проверка капчи
                                var captchaWindow = new CaptchaWindow();
                                bool? captchaResult = captchaWindow.ShowDialog();

                                if (captchaResult != true || !captchaWindow.IsCaptchaPassed)
                                {
                                    MessageBox.Show("Вы не прошли проверку безопасности!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                // Отображение окна в зависимости от роли
                                switch (user.UserRole)
                                {
                                    case 1:
                                        FrameObject.s_frameMain.Navigate(new AdminPage());
                                        break;
                                    case 3:
                                        FrameObject.s_frameMain.Navigate(new FasPage());
                                        break;
                                    case 4:
                                        FrameObject.s_frameMain.Navigate(new BetPage());
                                        break;
                                    case 5:
                                        FrameObject.s_frameMain.Navigate(new STRPage());
                                        break;
                                    case 6:
                                        FrameObject.s_frameMain.Navigate(new AccountingPage());
                                        break;
                                case 7:
                                    FrameObject.s_frameMain.Navigate(new SmenaUser());
                                    break;
                            }
                                break;
                        }
                    }
                    else
                    {
                        // При неуспешном ходе увеличение счетчика
                        _numberOfAuthorizations++;

                        user.UserStatus = 1;

                        // Сохранение изменений в БД
                        try
                        {
                            ConnectObject.GetConnect().Users.AddOrUpdate(user);
                            ConnectObject.GetConnect().SaveChanges();
                            MessageBox.Show("Вы заблокированы. Обратитесь к администратору.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Вы ввели неверный логин или пароль. Пожалуйста, проверьте ещё раз введенные данные.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        // Метод для показа сообщения об ошибке
        private void ShowError(string message)
        {
            ErrorMessage.Text = message;

            // Находим родительский Border сообщения об ошибке
            var errorBorder = FindParent<Border>(ErrorMessage);
            if (errorBorder != null)
            {
                errorBorder.Visibility = Visibility.Visible;
            }
        }

        // Метод для скрытия сообщения об ошибке
        private void HideError()
        {
            var errorBorder = FindParent<Border>(ErrorMessage);
            if (errorBorder != null)
            {
                errorBorder.Visibility = Visibility.Collapsed;
            }
        }

        // Вспомогательный метод для поиска родительского элемента
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }

        private void ButtonEnter1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сначала скрываем предыдущую ошибку
                HideError();

                // Проверка логина и пароля
                if (string.IsNullOrWhiteSpace(TBLogin.Text))
                {
                    ShowError("Введите логин");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TBPassword.Password))
                {
                    ShowError("Введите пароль");
                    return;
                }

                // Ваша логика авторизации...

            }
            catch (Exception ex)
            {
                ShowError($"Ошибка авторизации: {ex.Message}");
            }
        }
        private void TB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                if (string.IsNullOrEmpty(passwordBox.Password))
                {
                    passwordBox.BorderBrush = Brushes.Red;
                }
            }
            else if (sender is TextBox textBox)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.BorderBrush = Brushes.Red;
                }
            }
        }
    }
}