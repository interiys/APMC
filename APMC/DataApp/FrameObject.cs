using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace APMC.DataApp
{
    /// <summary>
    /// Статический класс для управления навигацией между страницами приложения
    /// </summary>
    /// <remarks>
    /// Предоставляет глобальный доступ к основному навигационному фрейму,
    /// позволяя осуществлять переходы между страницами из любой части приложения.
    /// </remarks>
    public static class FrameObject
    {
        /// <summary>
        /// Основной навигационный фрейм приложения
        /// </surmary>
        public static Frame s_frameMain;
    }
}
