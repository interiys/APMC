using APMC.DataApp;
using System;
using System.Collections;
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
    public partial class BetPage : Page
    {
        public BetPage()
        {
            InitializeComponent();

            TBHello.Text = $"Добро пожаловать,\n{Session.s_userFirstName} {Session.s_userPatronymic}";
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridBETTable.ItemsSource = ConnectObject.GetConnect().FasTables.ToList();
            }
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new Authorization());
        }
        private void ButtonEditBET_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditBet((sender as Button).DataContext as ButterTable));
        }

        private void DGridBETTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonAddBET_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditBet((sender as Button).DataContext as ButterTable));
        }

        private void ButtonSmenBet_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new FasCheckSmen());
        }

        private void UpdateStatistics()
        {
            try
            {
                var betRecords = DGridBETTable.ItemsSource;

                if (betRecords != null)
                {
                    int totalRecords = 0;

                    var items = betRecords as IEnumerable;

                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            totalRecords++;
                        }
                    }

                    TBTotalRecords.Text = totalRecords.ToString();

                    Debug.WriteLine($"Статистика обновлена: Всего записей={totalRecords}");
                }
                else
                {
                    TBTotalRecords.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при обновлении статистики: {ex.Message}");
                TBTotalRecords.Text = "0";
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridBETTable.ItemsSource = ConnectObject.GetConnect().ButterTables.ToList();

                UpdateStatistics();

                MessageBox.Show("Данные обновлены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    }