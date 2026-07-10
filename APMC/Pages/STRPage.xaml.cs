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
    public partial class STRPage : Page
    {
        public STRPage()
        {
            InitializeComponent();

        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                try
                {
                    var context = ConnectObject.GetConnect();

                    context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                    DGridSTRTable.ItemsSource = context.FasTables.ToList();
                    DGridSTRTable2.ItemsSource = context.ButterTables.ToList();

                    LoadDataFromDatabase();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new Authorization());
        }

        private void ButtonEditSTR_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditSTR((sender as Button).DataContext as FasTable));
        }

        private void DGridSTRTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonAddSTR_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditSTR((sender as Button).DataContext as FasTable));
        }

        private void UpdateStatistics()
        {
            try
            {
                int vegetablesCount = 0;
                int oilCount = 0;

                if (DGridSTRTable.ItemsSource != null)
                {
                    var enumerator1 = DGridSTRTable.ItemsSource.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        vegetablesCount++;
                    }
                }

                if (DGridSTRTable2.ItemsSource != null)
                {
                    var enumerator2 = DGridSTRTable2.ItemsSource.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        oilCount++;
                    }
                }

                int totalCount = vegetablesCount + oilCount;

                TBVegetablesCount.Text = vegetablesCount.ToString();
                TBOilCount.Text = oilCount.ToString();
                TBTotalCount.Text = totalCount.ToString();

                Debug.WriteLine($"Статистика обновлена: Овощи={vegetablesCount}, Масло={oilCount}, Всего={totalCount}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при обновлении статистики: {ex.Message}");
                TBVegetablesCount.Text = "0";
                TBOilCount.Text = "0";
                TBTotalCount.Text = "0";
            }
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                var vegetablesData = ConnectObject.GetConnect().FasTables.ToList();
                DGridSTRTable.ItemsSource = vegetablesData;

                var oilData = ConnectObject.GetConnect().ButterTables.ToList();
                DGridSTRTable2.ItemsSource = oilData;

                UpdateStatistics();

                Debug.WriteLine("Данные успешно загружены из базы данных");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDataFromDatabase();

                MessageBox.Show("Данные успешно обновлены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void ButtonEditSTR2_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditSTR((sender as Button).DataContext as ButterTable));
        }

        private void DGridSTRTables2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonAddSTR2_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditSTR((sender as Button).DataContext as ButterTable));
        }

        private void ButtonSTRCheckSmena_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new FasCheckSmen());
        }
    }
}