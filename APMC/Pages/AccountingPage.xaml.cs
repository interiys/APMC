using APMC.DataApp;
using System;
using System.Collections.Generic;
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace APMC.Pages
{
    public partial class AccountingPage : Page
    {
        public AccountingPage()
        {
            InitializeComponent();

        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridAccountingTable.ItemsSource = ConnectObject.GetConnect().FasTables.ToList();

                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridAccountingTable2.ItemsSource = ConnectObject.GetConnect().ButterTables.ToList();
            }
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new Authorization());
        }

        private void ButtonEditAccounting_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditAccounting((sender as Button).DataContext as FasTable));
        }

        private void DGridAccountingTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonAddAccounting_Click(object sender, RoutedEventArgs e)
        {
            // Создаем ПУСТОЙ объект FasTable для новой записи
            FrameObject.s_frameMain.Navigate(new AddEditAccounting(new FasTable()));
        }

        private void ButtonAddAccounting2_Click(object sender, RoutedEventArgs e)
        {
            // Создаем ПУСТОЙ объект ButterTable для новой записи
            FrameObject.s_frameMain.Navigate(new AddEditAccounting(new ButterTable()));
        }




        private void ButtonEditAccounting2_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new AddEditAccounting((sender as Button).DataContext as ButterTable));
        }

        private void DGridAccountingTables2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonAddSTR1_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную строку из DataGrid
            var selectedItem = DGridAccountingTable.SelectedItem as FasTable;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите строку для удаления из первой таблицы!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить запись?\nДата: {selectedItem.DateProduction}\nКоличество: {selectedItem.Quantity}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var context = ConnectObject.GetConnect();
                    var itemToDelete = context.FasTables.Find(selectedItem.FasID);
                    if (itemToDelete != null)
                    {
                        context.FasTables.Remove(itemToDelete);
                        context.SaveChanges();

                        // Обновляем таблицу
                        RefreshFasTable();

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

        private void ButtonAddSTR2_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную строку из DataGrid
            var selectedItem = DGridAccountingTable2.SelectedItem as ButterTable;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите строку для удаления из второй таблицы!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить запись?\nДата: {selectedItem.DateProduction}\nКоличество: {selectedItem.Quantity}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var context = ConnectObject.GetConnect();
                    var itemToDelete = context.ButterTables.Find(selectedItem.ButterID);
                    if (itemToDelete != null)
                    {
                        context.ButterTables.Remove(itemToDelete);
                        context.SaveChanges();

                        // Обновляем таблицу
                        RefreshButterTable();

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
        private void RefreshFasTable()
        {
            try
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridAccountingTable.ItemsSource = ConnectObject.GetConnect().FasTables.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении таблицы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButterTable()
        {
            try
            {
                ConnectObject.GetConnect().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridAccountingTable2.ItemsSource = ConnectObject.GetConnect().ButterTables.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении таблицы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonAddAccountingOtchet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Диалог сохранения файла
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|Все файлы (*.*)|*.*";
                saveFileDialog.Title = "Сохранить PDF отчет";
                saveFileDialog.FileName = $"Отчет_производства_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Получаем данные из базы
                    var fasData = ConnectObject.GetConnect().FasTables.ToList();
                    var butterData = ConnectObject.GetConnect().ButterTables.ToList();

                    // Создаем PDF документ
                    CreatePDFReport(saveFileDialog.FileName, fasData, butterData);

                    // Показываем сообщение
                    var result = MessageBox.Show($"PDF отчет успешно создан!\nФайл: {saveFileDialog.FileName}\n\nХотите открыть файл?",
                        "Успех", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Открываем PDF файл
                        Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreatePDFReport(string filePath, List<FasTable> fasData, List<ButterTable> butterData)
        {
            // Создаем документ
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 50, 50, 50, 50);

            // Создаем writer
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            // Открываем документ
            document.Open();

            // Устанавливаем кодировку для русского текста
            var baseFont = iTextSharp.text.pdf.BaseFont.CreateFont("c:/windows/fonts/arial.ttf",
                iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
            var titleFont = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
            var headerFont = new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.BOLD);
            var normalFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);
            var smallFont = new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL);

            // Заголовок
            var title = new iTextSharp.text.Paragraph("ОТЧЕТ ПРОИЗВОДСТВА", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            title.SpacingAfter = 20;
            document.Add(title);

            // Дата создания
            var date = new iTextSharp.text.Paragraph($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}", normalFont);
            date.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
            date.SpacingAfter = 20;
            document.Add(date);

            // Получаем текущего пользователя из сессии
            var currentUser = Session.CurrentUser;

            if (currentUser != null)
            {
                // Формируем ФИО пользователя
                string userFullName = $"{currentUser.LastName} {currentUser.FirstName}";
                if (!string.IsNullOrEmpty(currentUser.Patronymic))
                {
                    userFullName += $" {currentUser.Patronymic}";
                }

                var userInfo = new iTextSharp.text.Paragraph($"Ответственный: {userFullName}", normalFont);
                userInfo.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                userInfo.SpacingAfter = 10;
                document.Add(userInfo);
            }
            else
            {
                // Если пользователь не найден, выводим общее сообщение
                var userInfo = new iTextSharp.text.Paragraph("Отчет сформирован системой", normalFont);
                userInfo.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                userInfo.SpacingAfter = 10;
                document.Add(userInfo);
            }

            // Общая статистика
            var stats = new iTextSharp.text.Paragraph($"Общее количество записей: {fasData.Count + butterData.Count}\n" +
                                                       $"Записей овощей: {fasData.Count}\n" +
                                                       $"Записей масла: {butterData.Count}", normalFont);
            stats.SpacingAfter = 20;
            document.Add(stats);

            // Таблица 1: Овощи
            if (fasData.Count > 0)
            {
                var fasTitle = new iTextSharp.text.Paragraph("Таблица Цеха по Фасовке овощей", headerFont);
                fasTitle.SpacingAfter = 10;
                document.Add(fasTitle);

                // Создаем таблицу
                var fasTable = new iTextSharp.text.pdf.PdfPTable(5);
                fasTable.WidthPercentage = 100;
                fasTable.SpacingBefore = 10;
                fasTable.SpacingAfter = 20;

                // Заголовки таблицы
                fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Дата производства", normalFont)));
                fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Цех", normalFont)));
                fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Единица измерения", normalFont)));
                fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Тип продукции", normalFont)));
                fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Количество", normalFont)));

                // Данные таблицы
                foreach (var item in fasData)
                {
                    fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.DateProduction ?? "", smallFont)));
                    fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.DepartmentID.ToString(), smallFont)));
                    fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.UnitOfMeasuresID.ToString(), smallFont)));
                    fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.TypeID.ToString(), smallFont)));
                    fasTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.Quantity ?? "", smallFont)));
                }

                document.Add(fasTable);
            }
            else
            {
                var noFasData = new iTextSharp.text.Paragraph("Таблица Цеха по Фасовке овощей: Нет данных", normalFont);
                noFasData.SpacingAfter = 20;
                document.Add(noFasData);
            }

            // Таблица 2: Масло
            if (butterData.Count > 0)
            {
                var butterTitle = new iTextSharp.text.Paragraph("Таблица Цеха по Разливки масла", headerFont);
                butterTitle.SpacingAfter = 10;
                document.Add(butterTitle);

                // Создаем таблицу
                var butterTable = new iTextSharp.text.pdf.PdfPTable(5);
                butterTable.WidthPercentage = 100;
                butterTable.SpacingBefore = 10;
                butterTable.SpacingAfter = 20;

                // Заголовки таблицы
                butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Дата производства", normalFont)));
                butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Цех", normalFont)));
                butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Единица измерения", normalFont)));
                butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Тип продукции", normalFont)));
                butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Количество", normalFont)));

                // Данные таблицы
                foreach (var item in butterData)
                {
                    butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.DateProduction ?? "", smallFont)));
                    butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.DepartmentID.ToString(), smallFont)));
                    butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.UnitOfMeasuresID.ToString(), smallFont)));
                    butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.TypeID.ToString(), smallFont)));
                    butterTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.Quantity ?? "", smallFont)));
                }

                document.Add(butterTable);
            }
            else
            {
                var noButterData = new iTextSharp.text.Paragraph("Таблица Цеха по Разливки масла: Нет данных", normalFont);
                noButterData.SpacingAfter = 20;
                document.Add(noButterData);
            }

            // Подвал
            var footer = new iTextSharp.text.Paragraph($"Отчет сформирован системой учета производства.\n" +
                                                      $"Всего записей: {fasData.Count + butterData.Count}", smallFont);
            footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            footer.SpacingBefore = 30;
            document.Add(footer);

            // Закрываем документ
            document.Close();
        }

        private void ButtonAddAccountingSmena_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.Navigate(new FasCheckSmen());
        }
    }
}