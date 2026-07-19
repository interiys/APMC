# 🐧Разработка приложения на С#

[![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-5C2D91?logo=.net&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Status](https://img.shields.io/badge/status-graduated-brightgreen.svg)]()

##  Что умеет система

| Функция | Описание |
|:---|:---|
|  **Авторизация** | Вход по логину/паролю с капчей и блокировкой после 3 неудачных попыток |
|  **Управление пользователями** | Добавление, редактирование, удаление (только для администратора) |
|  **Управление таблицами** | CRUD-операции для таблиц FasTable и ButterTable |
|  **Смены сотрудников** | Отметка «Я на смене», управление статусами |
|  **Генерация отчётов** | Экспорт в PDF с данными по цехам |
|  **Логирование** | Запись всех действий пользователей в таблицу UserLogs |
|  **Капча** | Пазл-капча для дополнительной безопасности |

---

##  Технологический стек

| Компонент | Технология |
|:---|:---|
| **Язык** | C# |
| **Фреймворк** | .NET Framework (WPF) |
| **СУБД** | Microsoft SQL Server |
| **ORM** | Entity Framework (Database First) |
| **Отчёты** | iTextSharp (PDF) |
| **IDE** | Visual Studio 2022 |

---

##  Структура базы данных

База данных `dairyq` содержит 8 основных таблиц:

- `User` — пользователи системы
- `Roles` — роли (администратор, руководитель цеха, начальник склада, работник)
- `UserStatuses` — статусы пользователей (активен/заблокирован)
- `Department` — цеха (фасовочный, масленый, склад)
- `Types` — типы продукции
- `UnitOfMeasures` — единицы измерения
- `Statuses` — статусы выполнения задач
- `FasTable` / `ButterTable` — таблицы учёта товара
- `UserLogs` — журнал действий пользователей

---

## Как запустить

### 1. Клонируй репозиторий
```bash
git clone https://github.com/interiys/APMC.git
cd APMC
```

### 2. Восстанови базу данных
- Открой **SQL Server Management Studio (SSMS)**
- Выполни скрипт из файла `script_bd.sql`
- Либо присоедини файлы `Diplom123.mdf` и `dairyq_trz.bak`

### 3. Настрой строку подключения
В файле `App.config` или в `ConnectObject.cs` укажи свои параметры:
```xml
<connectionStrings>
  <add name="dairyqEntities" 
       connectionString="metadata=...;provider=System.Data.SqlClient;provider connection string='data source=localhost;initial catalog=dairyq;integrated security=True'" />
</connectionStrings>
```

### 4. Открой проект в Visual Studio
- Запусти `APMC.sln`
- Собери решение (Ctrl+Shift+B)
- Запусти приложение (F5)

### 5. Войди в систему
| Роль | Логин | Пароль |
|:---|:---|:---|
| Администратор | `admin` | `admin123` |
| Руководитель фасовочного цеха | `rucfas` | `rucfas123` |
| Руководитель масленого цеха | `rucbutter` | `rucbutter123` |
| Начальник склада | `bossstorage` | `bossstorage123` |
| Сотрудник | `worker` | `worker123` |

---

##  Структура проекта

```
APMC/
├── 📁 DataApp/               # Подключение к БД, модель Entity Framework
├── 📁 Helper/                # Вспомогательные классы (логирование)
├── 📁 Pages/                 # Все страницы приложения
│   ├── AdminPage.xaml        # Страница администратора
│   ├── Authorization.xaml    # Авторизация
│   ├── FasPage.xaml          # Руководитель фасовочного цеха
│   ├── BetPage.xaml          # Руководитель масленого цеха
│   ├── STRPage.xaml          # Руководитель склада
│   ├── AccountingPage.xaml   # Начальник склада
│   ├── SmenaUser.xaml        # Сотрудник (отметка смены)
│   ├── LogsPage.xaml         # Журнал действий
│   └── AddEdit*.xaml         # Формы добавления/редактирования
├── 📁 Properties/            # Настройки проекта
├── 📄 App.xaml               # Словарь ресурсов (стили)
├── 📄 MainWindow.xaml        # Главное окно
└── 📄 script_bd.sql          # SQL-скрипт для создания БД
```

---

## 📌 О проекте / About the Project

**Author**: The author loves ferrets ;3

<p align="center">
  <img src="https://github.com/user-attachments/assets/b02d24de-8dd5-41c5-9229-2060730c03e2" width="400" alt="ferret" />
</p>
