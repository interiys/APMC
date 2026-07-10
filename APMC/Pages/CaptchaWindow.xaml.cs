using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace APMC.Pages
{
    /// <summary>
    /// Окно проверки CAPTCHA с использованием механизма сборки пазла
    /// </summary>
    /// <remarks>
    /// Реализует интерактивную CAPTCHA, где пользователь должен собрать
    /// пазл из 4 фрагментов, перетаскивая их в правильные позиции на сетке 2x2
    /// </remarks>
    public partial class CaptchaWindow : Window
    {
        /// <summary>
        /// Коллекция фрагментов пазла
        /// </summary>
        private List<Image> _puzzlePieces = new List<Image>();

        /// <summary>
        /// Флаг, указывающий на активность перетаскивания элемента
        /// </summary>
        private bool _isDragging = false;

        /// <summary>
        /// Текущий перетаскиваемый элемент изображения
        /// </summary>
        private Image _draggedImage;

        /// <summary>
        /// Смещение курсора мыши относительно элемента при начале перетаскивания
        /// </summary>
        private Point _offset;

        /// <summary>
        /// Флаг, указывающий на успешное прохождение CAPTCHA проверки
        /// </summary>
        private bool _isCaptchaPassed = false;

        // Свойство IsCaptchaPassed только для чтения, гарантирует отсутствие возможности обойти капчу
        /// <summary>
        /// Получает значение, указывающее успешность прохождения CAPTCHA проверки
        /// </summary>
        /// <value>true если пользователь успешно собрал пазл, иначе false</value>
        public bool IsCaptchaPassed
        {
            get { return _isCaptchaPassed; }
        }

        /// <summary>
        /// Инициализирует новый экземпляр окна CAPTCHA и загружает фрагменты пазла
        /// </summary>
        public CaptchaWindow()
        {
            InitializeComponent();
            InitializePuzzle();
        }

        /// <summary>
        /// Инициализирует и размещает фрагменты пазла на холсте
        /// </summary>
        /// <remarks>
        /// Загружает 4 изображения из папки CaptchaResource, устанавливает их размеры 80x80 пикселей,
        /// назначает порядковые номера и случайным образом распределяет в области 120x120 пикселей
        /// </remarks>
        private void InitializePuzzle()
        {
            // Очистка пазла
            PuzzleCanvas.Children.Clear();
            _puzzlePieces.Clear();

            // Путь к папке с картинками
            string captchaPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\DataApp\CaptchaResource");

            // Создание 4 фрагментов пазла
            for (int i = 1; i <= 4; i++)
            {
                string imagePath = System.IO.Path.Combine(captchaPath, $"{i}.png");

                if (!File.Exists(imagePath))
                {
                    MessageBox.Show($"Не найден файл: {imagePath}");
                    continue;
                }

                // Создание изображения с указанными характеристиками
                Image image = new Image
                {
                    Width = 80,
                    Height = 80,
                    Tag = i // правильная позиция
                };

                try
                {
                    // Загрузка картинки из файла
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.EndInit();
                    image.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                    continue;
                }

                // Случайная позиция
                Random rand = new Random();
                Canvas.SetLeft(image, rand.Next(0, 120));
                Canvas.SetTop(image, rand.Next(0, 120));

                // Включаем перетаскивание
                image.MouseDown += Image_MouseDown;
                image.MouseMove += Image_MouseMove;
                image.MouseUp += Image_MouseUp;

                // Отображение элемента пазла
                PuzzleCanvas.Children.Add(image);
                _puzzlePieces.Add(image);
            }

            ResultText.Text = "";
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки мыши на фрагменте пазла
        /// </summary>
        /// <param name="sender">Источник события − элемент Image</param>
        /// <param name="e">Данные события мыши</param>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Получение перетаскиваемой картинки
            _draggedImage = sender as Image;

            if (_draggedImage != null)
            {
                _isDragging = true;
                _offset = e.GetPosition(_draggedImage);
                _draggedImage.CaptureMouse(); // захват мыши для гарантии отслеживания перемещения
            }
        }

        /// <summary>
        /// Обрабатывает перемещение мыши при перетаскивании фрагмента пазла
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события мыши</param>
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            // Гарантирует плавное перетаскивание
            if (_isDragging && _draggedImage != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(PuzzleCanvas);
                Canvas.SetLeft(_draggedImage, currentPosition.X - _offset.X);
                Canvas.SetTop(_draggedImage, currentPosition.Y - _offset.Y);
            }
        }

        /// <summary>
        /// Обрабатывает событие отпускания кнопки мыши после перетаскивания
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события мыши</param>
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _draggedImage.ReleaseMouseCapture();
                _draggedImage = null;
            }
        }

        /// <summary>
        /// Проверяет корректность сборки пазла
        /// </summary>
        /// <param name="sender">Источник события − кнопка проверки</param>
        /// <param name="e">Данные события маршрутизации</param>
        /// <remarks>
        /// Сравнивает текущее положение фрагментов с целевыми позициями сетки 2x2.
        /// Допускается погрешность расположения до 7 пикселей. При успешной проверке
        /// автоматически закрывает окно через 1 секунду с установкой DialogResult в true.
        /// </remarks>
        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            // Целевые позиции для сетки 2x2
            Point[] targetPositions = new[]
            {
                new Point(0, 0),   // фрагмент 1: верхний левый
                new Point(80, 0),  // фрагмент 2: верхний правый
                new Point(0, 80),  // фрагмент 3: нижний левый
                new Point(80, 80)  // фрагмент 4: нижний правый
            };

            bool isCorrect = true;

            for (int i = 0; i < _puzzlePieces.Count; i++)
            {
                double left = Canvas.GetLeft(_puzzlePieces[i]);
                double top = Canvas.GetTop(_puzzlePieces[i]);

                Point target = targetPositions[i];

                // погрешность расположения картинки
                byte tolerance = 7;

                if (Math.Abs(left - target.X) > tolerance || Math.Abs(top - target.Y) > tolerance)
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                _isCaptchaPassed = true;
                ResultText.Text = "Проверка пройдена!";
                ResultText.Foreground = Brushes.Green;

                // Таймер для автоматического закрытия окна после успешной сборки
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    this.DialogResult = true; // закрытие окна с капчей
                };
                timer.Start();
            }
            else
            {
                ResultText.Text = "Пазл собран неверно! Попробуйте еще раз";
                ResultText.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Обновляет CAPTCHA, перемешивая фрагменты пазла
        /// </summary>
        /// <param name="sender">Источник события − кнопка обновления</param>
        /// <param name="e">Данные события маршрутизации</param>
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Перемешиваем фрагменты
            InitializePuzzle();
        }
    }
}