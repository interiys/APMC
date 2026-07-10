using APMC.DataApp;
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
    /// Страница добавления и редактирования данных пользователя в системе
    /// </summary>
    /// <remarks>
    /// Предоставляет функционал для создания новых пользователей и изменения данных существующих.
    /// Включает валидацию вводимых данных, проверку уникальности логина и сохранение изменений в базе данных.
    /// </remarks>
    public partial class AddEditUsers : Page
    {
        /// <summary>
        /// Временный объект пользователя для хранения данных формы
        /// </summary>
        private User _tempUser = new User();

        /// <summary>
        /// Инициализирует новый экземпляр страницы добавления/редактирования пользователя
        /// </summary>
        /// <param name="selectedUser">Объект пользователя для редактирования.
        /// Если null − создается новый пользователь.</param>
        /// <remarks>
        /// При передаче существующего пользователя происходит заполнение формы его данными.
        /// При создании нового пользователя инициализируется пустая форма.
        /// </remarks>
        public AddEditUsers(User selectedUser)
        {
            InitializeComponent();

            // Передача данных выбранного пользователя во временный объект
            if (selectedUser != null)
            {
                _tempUser = selectedUser;
            }

            // Привязка контента формы ко временному объекту
            DataContext = _tempUser;

            // Загрузка списков в ComboBox
            CBRole.ItemsSource = ConnectObject.GetConnect().Roles.ToList();
            CBUserStatus.ItemsSource = ConnectObject.GetConnect().UserStatuses.ToList();
            CBdep.ItemsSource = ConnectObject.GetConnect().Departments.ToList();


            // Поиск в БД пользователя из сессии
            var user = ConnectObject.GetConnect().Users.AsNoTracking()
                .FirstOrDefault(u => u.UserID == Session.s_userID);
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки сохранения данных пользователя
        /// </summary>
        /// <param name="sender">Источник события − кнопка сохранения</param>
        /// <param name="e">Данные события маршрутизации</param>
        /// <remarks>
        /// Выполняет валидацию обязательных полей, проверку уникальности логина,
        /// сохранение данных в базу и переход обратно к списку пользователей.
        /// При обнаружении ошибок отображает соответствующие сообщения.
        /// </remarks>

        /// <summary>
        /// Обрабатывает потерю фокуса ввода элементами управления формы
        /// </summary>
        /// <param name="sender">Источник события − TextBox или PasswordBox</param>
        /// <param name="e">Данные события фокуса</param>
        /// <remarks>
        /// Валидирует обязательные поля формы, изменяя цвет рамки на красный при пустом значении.
        /// Поддерживает как текстовые поля, так и поля ввода пароля.
        /// </remarks>
        private void TB_LostFocus(object sender, RoutedEventArgs e)
        {
            HideError();

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

        // Обработчик для кнопки "Отмена"
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // Возврат на предыдущую страницу
            FrameObject.s_frameMain.GoBack();
        }

        // Методы для работы с ошибками
        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;
        }

        private void ButtonOKAddEdit1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideError();

                // Валидация полей
                if (string.IsNullOrWhiteSpace(TBLastName.Text))
                {
                    ShowError("Введите фамилию");
                    TBLastName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(TBFirstName.Text))
                {
                    ShowError("Введите имя");
                    TBFirstName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(TBLoginNew.Text))
                {
                    ShowError("Введите логин");
                    TBLoginNew.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(TBPasswordNew.Text))
                {
                    ShowError("Введите пароль");
                    TBPasswordNew.Focus();
                    return;
                }

                // Остальная логика сохранения...
                // Установка границ по умолчанию
                TBLastName.BorderBrush = Brushes.Gray;
                TBFirstName.BorderBrush = Brushes.Gray;
                TBPatronymic.BorderBrush = Brushes.Gray;
                TBLoginNew.BorderBrush = Brushes.Gray;
                TBPasswordNew.BorderBrush = Brushes.Gray;

                // Проверка на пустоту заполнения полей формы
                if (string.IsNullOrWhiteSpace(TBLastName.Text) ||
                    string.IsNullOrWhiteSpace(TBFirstName.Text) ||
                    string.IsNullOrWhiteSpace(TBPatronymic.Text) ||
                    string.IsNullOrWhiteSpace(TBLoginNew.Text) ||
                    string.IsNullOrWhiteSpace(TBPasswordNew.Text) ||
                    string.IsNullOrWhiteSpace(CBRole.SelectedValue.ToString()) ||
                    string.IsNullOrWhiteSpace(CBUserStatus.SelectedValue.ToString()) ||
                    string.IsNullOrWhiteSpace(CBdep.SelectedValue.ToString()))
                {
                    MessageBox.Show("Заполните все поля формы!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Поиск в БД пользователя с указанным логином
                var user = ConnectObject.GetConnect().Users.AsNoTracking()
                    .FirstOrDefault(u => u.UserLogin == TBLoginNew.Text);

                // Добавление нового пользователя в модель данных
                if (_tempUser.UserID == 0)
                {
                    if (user != null)
                    {
                        MessageBox.Show("Пользователь с такими данными уже существует!", "Предупреждение",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        TBLoginNew.BorderBrush = Brushes.Red;
                        return;
                    }

                    // Добавление данных формы во временного пользователя
                    _tempUser.LastName = TBLastName.Text;
                    _tempUser.FirstName = TBFirstName.Text;
                    _tempUser.Patronymic = TBPatronymic.Text;
                    _tempUser.UserLogin = TBLoginNew.Text;
                    _tempUser.UserPassword = TBPasswordNew.Text;
                    _tempUser.UserRole = Convert.ToInt32(CBRole.SelectedValue.ToString());
                    _tempUser.UserStatus = Convert.ToInt32(CBUserStatus.SelectedValue.ToString());
                    _tempUser.DepartmentID = Convert.ToInt32(CBdep.SelectedValue.ToString());

                    ConnectObject.GetConnect().Users.Add(_tempUser);
                }
                else
                {
                    // Обновление существующего пользователя
                    _tempUser.LastName = TBLastName.Text;
                    _tempUser.FirstName = TBFirstName.Text;
                    _tempUser.Patronymic = TBPatronymic.Text;
                    _tempUser.UserLogin = TBLoginNew.Text;
                    _tempUser.UserPassword = TBPasswordNew.Text;
                    _tempUser.UserRole = Convert.ToInt32(CBRole.SelectedValue.ToString());
                    _tempUser.UserStatus = Convert.ToInt32(CBUserStatus.SelectedValue.ToString());
                    _tempUser.DepartmentID = Convert.ToInt32(CBdep.SelectedValue.ToString());

                    ConnectObject.GetConnect().Users.AddOrUpdate(_tempUser);
                }

                // Сохранение изменений в модели данных
                try
                {
                    ConnectObject.GetConnect().SaveChanges();
                    MessageBox.Show("Изменения сохранены!", "Сообщение",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

                FrameObject.s_frameMain.GoBack();

            }
            catch (Exception ex)
            {
                ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CBRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}