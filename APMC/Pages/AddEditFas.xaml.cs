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
    public partial class AddEditFas : Page
    {
        private FasTable _tempbet = new FasTable();

        private bool _isButterTable = false;

        public AddEditFas(FasTable selectedBet)
        {
            InitializeComponent();

            _isButterTable = true;

            // Передача данных выбранного пользователя во временный объект
            if (selectedBet != null)
            {
                _tempbet = selectedBet;
            }

            // Привязка контента формы ко временному объекту
            DataContext = _tempbet;

            // Загрузка списков в ComboBox
            bettype.ItemsSource = ConnectObject.GetConnect().Typesses.ToList();
            beted.ItemsSource = ConnectObject.GetConnect().UnitOfMeasures.ToList();
            betdep.ItemsSource = ConnectObject.GetConnect().Departments.ToList();
            betstatus.ItemsSource = ConnectObject.GetConnect().Statuses.ToList();

            // Если редактируем существующий, заполняем поля
            if (_tempbet.FasID != 0)
            {
                bettype.SelectedValue = _tempbet.TypeID;
                beted.SelectedValue = _tempbet.UnitOfMeasuresID;
                betdep.SelectedValue = _tempbet.DepartmentID;
                betstatus.SelectedValue = _tempbet.StatuseID;
                betquantity.Text = _tempbet.Quantity;
                betdate.Text = _tempbet.DateProduction;
            }
        }

        private void ButtonOKAddEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_isButterTable)
            {
                SaveButterTable();
            }
            else
            {
                MessageBox.Show("Не определен тип таблицы для сохранения!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void SaveButterTable()
        {
            // Валидация для ButterTable
            if (string.IsNullOrWhiteSpace(betquantity.Text) ||
                beted.SelectedValue == null ||
                bettype.SelectedValue == null ||
                betdep.SelectedValue == null ||
                betstatus.SelectedValue == null ||
                string.IsNullOrWhiteSpace(betdate.Text))
            {
                MessageBox.Show("Заполните все поля формы для Butter!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавление или обновление данных
            _tempbet.Quantity = betquantity.Text;
            _tempbet.DateProduction = betdate.Text;
            _tempbet.UnitOfMeasuresID = Convert.ToInt32(beted.SelectedValue);
            _tempbet.TypeID = Convert.ToInt32(bettype.SelectedValue);
            _tempbet.DepartmentID = Convert.ToInt32(betdep.SelectedValue);
            _tempbet.StatuseID = Convert.ToInt32(betstatus.SelectedValue);

            var context = ConnectObject.GetConnect();

            if (_tempbet.FasID == 0)
            {
                context.FasTables.Add(_tempbet);
            }
            else
            {
                context.FasTables.AddOrUpdate(_tempbet);
            }

            SaveChanges();
        }

        private void SaveChanges()
        {
            try
            {
                ConnectObject.GetConnect().SaveChanges();
                MessageBox.Show("Изменения сохранены!", "Сообщение",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                FrameObject.s_frameMain.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void BETRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}