using APMC.DataApp;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.IO;
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
    public partial class AddEditAccounting : Page
    {
        private FasTable _tempfas = new FasTable();
        private ButterTable _tempbet = new ButterTable();

        // Флаг, чтобы определить, с какой таблицей работаем
        private bool _isButterTable = false;
        private bool _isFasTable = false;

        /// Конструктор для ButterTable
        public AddEditAccounting(ButterTable selectedBet)
        {
            InitializeComponent();

            _isButterTable = true;

            // Находим лейблы один раз при инициализации ButterTable
            beted1 = FindName("beted1") as Label;
            bettype1 = FindName("bettype1") as Label;
            betdep1 = FindName("betdep1") as Label;
            betquantity1 = FindName("betquantity1") as Label;
            betdate1 = FindName("betdate1") as Label;
            betstatus1 = FindName("betstatus1") as Label;

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

            // Скрываем элементы для FasTable
            HideFasControls();

            // Если редактируем существующий, заполняем поля
            if (_tempbet.ButterID != 0)
            {
                bettype.SelectedValue = _tempbet.TypeID;
                beted.SelectedValue = _tempbet.UnitOfMeasuresID;
                betdep.SelectedValue = _tempbet.DepartmentID;
                betstatus.SelectedValue = _tempbet.StatuseID;
                betquantity.Text = _tempbet.Quantity;
                betdate.Text = _tempbet.DateProduction;
            }
        }

        /// Конструктор для FasTable
        public AddEditAccounting(FasTable selectedFas)
        {
            InitializeComponent();

            _isFasTable = true;

            // Находим лейблы один раз при инициализации FasTable
            fsed1 = FindName("fsed1") as Label;
            fstype1 = FindName("fstype1") as Label;
            fsdep1 = FindName("fsdep1") as Label;
            fsquantity1 = FindName("fsquantity1") as Label;
            fsdate1 = FindName("fsdate1") as Label;
            fsstatus1 = FindName("fsstatus1") as Label;

            // Передача данных выбранного пользователя во временный объект
            if (selectedFas != null)
            {
                _tempfas = selectedFas;
            }

            // Привязка контента формы ко временному объекту
            DataContext = _tempfas;

            // Загрузка списков в ComboBox
            fstype.ItemsSource = ConnectObject.GetConnect().Typesses.ToList();
            fsed.ItemsSource = ConnectObject.GetConnect().UnitOfMeasures.ToList();
            fsdep.ItemsSource = ConnectObject.GetConnect().Departments.ToList();
            fsstatus.ItemsSource = ConnectObject.GetConnect().Statuses.ToList();

            // Скрываем элементы для ButterTable
            HideButterControls();

            // Если редактируем существующий, заполняем поля
            if (_tempfas.FasID != 0)
            {
                fstype.SelectedValue = _tempfas.TypeID;
                fsed.SelectedValue = _tempfas.UnitOfMeasuresID;
                fsdep.SelectedValue = _tempfas.DepartmentID;
                fsstatus.SelectedValue = _tempfas.StatuseID;
                fsquantity.Text = _tempfas.Quantity;
                fsdate.Text = _tempfas.DateProduction;
            }
        }

        private void HideFasControls()
        {
            // Скрываем все элементы для FasTable
            if (fstype != null) fstype.Visibility = Visibility.Collapsed;
            if (fsed != null) fsed.Visibility = Visibility.Collapsed;
            if (fsstatus != null) fsstatus.Visibility = Visibility.Collapsed;
            if (fsdep != null) fsdep.Visibility = Visibility.Collapsed;
            if (fsquantity != null) fsquantity.Visibility = Visibility.Collapsed;
            if (fsdate != null) fsdate.Visibility = Visibility.Collapsed;
            // Скрываем Label
            if (FindName("fstype1") is Label fsLabelType) fsLabelType.Visibility = Visibility.Collapsed;
            if (FindName("fsed1") is Label fsLabelEd) fsLabelEd.Visibility = Visibility.Collapsed;
            if (FindName("fsdep1") is Label fsLabelDep) fsLabelDep.Visibility = Visibility.Collapsed;
            if (FindName("fsquantity1") is Label fsLabelQuantity) fsLabelQuantity.Visibility = Visibility.Collapsed;
            if (FindName("fsdate1") is Label fsLabelDate) fsLabelDate.Visibility = Visibility.Collapsed;
            if (FindName("fsstatus1") is Label fsLabelStatuse) fsLabelStatuse.Visibility = Visibility.Collapsed;
            if (FindName("fasovka") is TextBlock bettextfasovka) bettextfasovka.Visibility = Visibility.Collapsed;
        }

        private void HideButterControls()
        {
            // Скрываем все элементы для ButterTable
            if (bettype != null) bettype.Visibility = Visibility.Collapsed;
            if (beted != null) beted.Visibility = Visibility.Collapsed;
            if (betdep != null) betdep.Visibility = Visibility.Collapsed;
            if (betquantity != null) betquantity.Visibility = Visibility.Collapsed;
            if (betdate != null) betdate.Visibility = Visibility.Collapsed;
            if (betstatus != null) betstatus.Visibility = Visibility.Collapsed;
            // Скрываем Label
            if (FindName("bettype1") is Label betLabelType) betLabelType.Visibility = Visibility.Collapsed;
            if (FindName("beted1") is Label betLabelEd) betLabelEd.Visibility = Visibility.Collapsed;
            if (FindName("betdep1") is Label betLabelDep) betLabelDep.Visibility = Visibility.Collapsed;
            if (FindName("betquantity1") is Label betLabelQuantity) betLabelQuantity.Visibility = Visibility.Collapsed;
            if (FindName("betdate1") is Label betLabelDate) betLabelDate.Visibility = Visibility.Collapsed;
            if (FindName("betstatus1") is Label betLabelStatuse) betLabelStatuse.Visibility = Visibility.Collapsed;
            if (FindName("maslo") is TextBlock bettextmaslo) bettextmaslo.Visibility = Visibility.Collapsed;
        }

        private void ButtonOKAddEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_isButterTable)
            {
                SaveButterTable();
            }
            else if (_isFasTable)
            {
                SaveFasTable();
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

            if (_tempbet.ButterID == 0)
            {
                context.ButterTables.Add(_tempbet);
            }
            else
            {
                context.ButterTables.AddOrUpdate(_tempbet);
            }

            SaveChanges();
        }

        private void SaveFasTable()
        {
            // Валидация для FasTable
            if (string.IsNullOrWhiteSpace(fsquantity.Text) ||
                fsed.SelectedValue == null ||
                fstype.SelectedValue == null ||
                fsdep.SelectedValue == null ||
                fsstatus.SelectedValue == null ||
                string.IsNullOrWhiteSpace(fsdate.Text))
            {
                MessageBox.Show("Заполните все поля формы для Fas!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавление или обновление данных
            _tempfas.Quantity = fsquantity.Text;
            _tempfas.DateProduction = fsdate.Text;
            _tempfas.UnitOfMeasuresID = Convert.ToInt32(fsed.SelectedValue);
            _tempfas.TypeID = Convert.ToInt32(fstype.SelectedValue);
            _tempfas.DepartmentID = Convert.ToInt32(fsdep.SelectedValue);
            _tempfas.StatuseID = Convert.ToInt32(fsstatus.SelectedValue);

            var context = ConnectObject.GetConnect();

            if (_tempfas.FasID == 0)
            {
                context.FasTables.Add(_tempfas);
            }
            else
            {
                context.FasTables.AddOrUpdate(_tempfas);
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

        private void AccountingRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Обработка изменения выбора для ButterTable
        }

        private void AccountingRole2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Обработка изменения выбора для FasTable
        }
    }
}