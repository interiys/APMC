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

namespace APMC
{
    /// <summary>
    /// Главное окно приложения
    /// </summary>
    /// <remarks>
    /// Служит основным контейнером для навигации между страницами приложения.
    /// Управляет отображением навигационных элементов и обеспечивает
    /// базовую функциональность интерфейса.
    /// </remarks>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Инициализирует новый экземпляр главного окна приложения
        /// </summary>
        /// <remarks>
        /// Выполняет начальную настройку интерфейса:
        /// − инициализирует основной навигационный фрейм
        /// − устанавливает минимальные размеры окна для обеспечения
        ///   читаемости интерфейса
        /// − настраивает начальное состояние навигационных элементов
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent();

            // Создание объекта фрейма
            FrameObject.s_frameMain = MainFrame;

            // Ограничение на минимальный размер окна
            this.MinWidth = 800;
            this.MinHeight = 450;
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки возврата на предыдущую страницу
        /// </summary>
        /// <param name="sender">Источник события − кнопка возврата</param>
        /// <param name="e">Данные события маршрутизации</param>
        /// <remarks>
        /// Выполняет навигацию назад по истории страниц основного фрейма.
        /// Доступно только когда в истории навигации есть предыдущие страницы.
        /// </remarks>
        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.s_frameMain.GoBack();
        }

        /// <summary>
        /// Обрабатывает событие завершения навигации по фрейму
        /// </summary>
        /// <param name="sender">Источник события − навигационный фрейм</param>
        /// <param name="e">Данные события навигации</param>
        /// <remarks>
        /// Автоматически управляет видимостью кнопки возврата в зависимости
        /// от текущей страницы.
        /// Скрывает кнопку на странице авторизации для предотвращения
        /// несанкционированного доступа.
        /// </remarks>
        private void FrameOnNavigated(object sender, NavigationEventArgs e)
        {
            var currentPage = e.Content.GetType().Name;

            if (currentPage == "Authorization")
            {
                BTGoBack.Visibility = Visibility.Hidden;
            }
            else
            {
                BTGoBack.Visibility = Visibility.Visible;
            }
        }
    }
}