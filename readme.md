# Dokumentacja systemu ERP dla browarni – **BrewLogix**


# Wstęp

**Opis aplikacji:**
BrewLogix to nowoczesny, webowy system ERP stworzony specjalnie dla małych i średnich browarów. Umożliwia kompleksowe zarządzanie magazynem surowców, recepturami, partiami produkcyjnymi, beczkami z gotowym piwem oraz obsługę zamówień klientów. Dzięki intuicyjnemu interfejsowi opartemu o Blazor Server oraz silnemu backendowi z Entity Framework Core i PostgreSQL, użytkownicy mogą w prosty sposób:

* **Monitorować stan magazynu** (surowce, terminy przydatności, zamówienia towaru).
* **Układać receptury** (z listą wymaganych składników i optymalizacją zużycia zasobów).
* **Tworzyć i śledzić partie produkcyjne** (łączenie receptury z konkretnymi wpisami magazynowymi, weryfikacja dostępności surowców, zmniejszanie stanów magazynowych).
* **Zarządzać beczkami** (przypisywanie beczek do partii, dystrybucja, śledzenie statusu).
* **Obsługiwać zamówienia klientów** (przypisywanie beczek do zamówień, blokowanie usunięcia partii, jeśli ma przypisane beczki).

BrewLogix działa w przeglądarce (bez instalacji), a dzięki wykorzystaniu ASP.NET Core Blazor Server zapewnia natychmiastową interakcję (SignalR) oraz pełne bezpieczeństwo warstwy serwera.

---

# Spis treści

1. [Technologie](#technologie)
2. [Schemat bazy danych](#schemat-bazy-danych)

    1. [Tabela Clients](#tabela-clients)
    2. [Tabela Ingredients](#tabela-ingredients)
    3. [Tabela StockEntries](#tabela-stockentries)
    4. [Tabela Recipes](#tabela-recipes)
    5. [Tabela RecipeIngredients](#tabela-recipeingredients)
    6. [Tabela Batches](#tabela-batches)
    7. [Tabela Kegs](#tabela-kegs)
    8. [Tabela Orders](#tabela-orders)
3. [Struktura plików projektu](#struktura-plików-projektu)
4. [DbContextProvider i konfiguracja połączenia](#dbcontextprovider-i-konfiguracja-połączenia)
5. [Modele (Encje)](#modele-encje)
6. [Seedery](#seedery)
7. [Serwisy (Logika biznesowa i CRUD)](#serwisy-logika-biznesowa-i-crud)
8. [Komponenty Blazor (Widoki)](#komponenty-blazor-widoki)

    1. [Strona główna (Home)](#strona-główna-home)
    2. [Zarządzanie partiami (Batches)](#zarządzanie-partiami-batches)
    3. [Zarządzanie recepturami (Recipes)](#zarządzanie-recepturami-recipes)
    4. [Zarządzanie magazynem (Ingredients & StockEntries)](#zarządzanie-magazynem-ingredients-stockentries)
    5. [Zarządzanie beczkami (Kegs)](#zarządzanie-beczkami-kegs)
    6. [Zarządzanie zamówieniami (Orders)](#zarządzanie-zamówieniami-orders)
9. [Walidacja formularzy](#walidacja-formularzy)
10. [Konfiguracja i uruchomienie](#konfiguracja-i-uruchomienie)
11. [Instrukcja obsługi](#instrukcja-obsługi)
12. [Architektura aplikacji](#architektura-aplikacji)
13. [Podsumowanie i sugestie produkcyjne](#podsumowanie-i-sugestie-produkcyjne)

---

## 1. Technologie

* **ASP.NET Core 8.0 (Blazor Server)**
  – interaktywny interfejs w przeglądarce, przekazywany za pośrednictwem SignalR, przy jednoczesnym bezpieczeństwie i wykonywaniu logiki po stronie serwera.
* **C# 11**
  – język implementacji modeli, serwisów i komponentów.
* **Entity Framework Core 8.0**
  – ORM do mapowania encji na tabele w PostgreSQL, ułatwiający definiowanie relacji, migracje oraz wykonywanie zapytań LINQ.
* **PostgreSQL 14+**
  – baza danych relacyjna, w której przechowywane są wszystkie tabele (magazyn, receptury, partie, beczki, zamówienia, klienci itd.).
* **DataAnnotations**
  – wbudowane atrybuty walidacyjne (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`), wykorzystywane w modelach oraz w formularzach Blazor (`EditForm`).
* **Własne atrybuty walidacyjne**
  – `AtLeastOneIngredientAttribute`, `UniqueIngredientsAttribute`, wymuszające, by każda receptura miała co najmniej jeden składnik oraz by składniki się nie powtarzały.
* **Razor Components**
  – pliki `.razor` definiujące strony i komponenty Blazor.
* **Dependency Injection**
  – wstrzykiwanie `DbContextProvider` oraz serwisów (`BatchService`, `RecipeService` itd.) poprzez kontener w `Program.cs`.
* **AutoMapper (opcjonalnie)**
  – do mapowania DTO-ów (jeśli w przyszłości pojawią się oddzielne klasy ViewModels).

---

## 2. Schemat bazy danych

Poniżej znajduje się uproszczony opis głównych tabel wykorzystywanych w BrewLogix. Każda tabela posiada klucz główny `Id` typu `INT` (serial w PostgreSQL) oraz odpowiednie kolumny opisane w trzech kolumnach: nazwa, typ, opis.

### 2.1 Tabela Clients

| Nazwa kolumny | Typ danych      | Opis                         |
| ------------- | --------------- | ---------------------------- |
| Id            | INT (serial)    | Klucz główny                 |
| Name          | VARCHAR(100)    | Nazwa klienta                |
| Email         | VARCHAR(100)    | Adres e-mail klienta         |
| Address       | TEXT            | Adres klienta                |
| PhoneNumber   | VARCHAR(20)     | Numer telefonu (opcjonalnie) |
| CreatedAt     | TIMESTAMP (UTC) | Data utworzenia wiersza      |
| UpdatedAt     | TIMESTAMP (UTC) | Data ostatniej modyfikacji   |

### 2.2 Tabela Ingredients

| Nazwa kolumny | Typ danych      | Opis                                      |
| ------------- | --------------- | ----------------------------------------- |
| Id            | INT (serial)    | Klucz główny                              |
| Name          | VARCHAR(100)    | Nazwa składnika (np. „Słód Pilzneński”)   |
| Type          | VARCHAR(50)     | Typ składnika (np. Drożdże, Chmiel, Słód) |
| Unit          | VARCHAR(20)     | Jednostka miary (kg, g, L)                |
| CreatedAt     | TIMESTAMP (UTC) | Data utworzenia                           |
| UpdatedAt     | TIMESTAMP (UTC) | Data ostatniej modyfikacji                |

### 2.3 Tabela StockEntries

| Nazwa kolumny | Typ danych      | Opis                                                 |
| ------------- | --------------- | ---------------------------------------------------- |
| Id            | INT (serial)    | Klucz główny                                         |
| IngredientId  | INT             | Klucz obcy → Ingredients(Id)                         |
| Quantity      | DECIMAL(18,2)   | Ilość dostępna (w jednostce `Unit` danego składnika) |
| DeliveryDate  | DATE            | Data dostawy                                         |
| ExpiryDate    | DATE            | Data przydatności do użycia                          |
| CreatedAt     | TIMESTAMP (UTC) | Data utworzenia rekordu                              |
| UpdatedAt     | TIMESTAMP (UTC) | Data ostatniej modyfikacji                           |

### 2.4 Tabela Recipes

| Nazwa kolumny | Typ danych      | Opis                                   |
| ------------- | --------------- | -------------------------------------- |
| Id            | INT (serial)    | Klucz główny                           |
| Name          | VARCHAR(100)    | Nazwa receptury (np. „IPA West Coast”) |
| Style         | VARCHAR(50)     | Styl piwa (np. IPA, Stout, Lager)      |
| Description   | TEXT            | Opis receptury                         |
| CreatedAt     | TIMESTAMP (UTC) | Data utworzenia                        |
| UpdatedAt     | TIMESTAMP (UTC) | Data ostatniej modyfikacji             |

### 2.5 Tabela RecipeIngredients

| Nazwa kolumny        | Typ danych      | Opis                                                                                            |
| -------------------- | --------------- | ----------------------------------------------------------------------------------------------- |
| Id                   | INT (serial)    | Klucz główny                                                                                    |
| RecipeId             | INT             | Klucz obcy → Recipes(Id)                                                                        |
| IngredientId         | INT             | Klucz obcy → Ingredients(Id)                                                                    |
| Quantity             | DECIMAL(18,2)   | Ilość składnika potrzebna do partii (w jednostce z tabeli Ingredients)                          |
| SelectedStockEntryId | INT?            | Id wpisu magazynowego użytego w tej recepturze (nullable – aktualizowane przy tworzeniu partii) |
| CreatedAt            | TIMESTAMP (UTC) | Data utworzenia                                                                                 |
| UpdatedAt            | TIMESTAMP (UTC) | Data ostatniej modyfikacji                                                                      |

### 2.6 Tabela Batches

| Nazwa kolumny | Typ danych      | Opis                                                          |
| ------------- | --------------- | ------------------------------------------------------------- |
| Id            | INT (serial)    | Klucz główny                                                  |
| Code          | VARCHAR(50)     | Kod partii (unikalny identyfikator, np. „BATCH-20250531-001”) |
| RecipeId      | INT             | Klucz obcy → Recipes(Id)                                      |
| StartDate     | DATE            | Data rozpoczęcia warzenia                                     |
| EndDate       | DATE            | Data zakończenia (nullable, gdy partia w toku)                |
| Status        | VARCHAR(20)     | Status partii (np. „Planowana”, „W toku”, „Zakończona”)       |
| CreatedAt     | TIMESTAMP (UTC) | Data utworzenia rekordu                                       |
| UpdatedAt     | TIMESTAMP (UTC) | Data ostatniej modyfikacji                                    |

### 2.7 Tabela Kegs

| Nazwa kolumny | Typ danych      | Opis                                                                             |
| ------------- | --------------- | -------------------------------------------------------------------------------- |
| Id            | INT (serial)    | Klucz główny                                                                     |
| BatchId       | INT             | Klucz obcy → Batches(Id)                                                         |
| VolumeLiters  | DECIMAL(10,2)   | Pojemność beczki w litrach (np. 50.00)                                           |
| FilledAt      | TIMESTAMP (UTC) | Data, kiedy beczka została napełniona (dziedziczone z `IDistributable`)          |
| IsDistributed | BOOLEAN         | Czy beczka została już dostarczona do klienta? (dziedziczone z `IDistributable`) |
| CreatedAt     | TIMESTAMP (UTC) | Data utworzenia rekordu                                                          |
| UpdatedAt     | TIMESTAMP (UTC) | Data ostatniej modyfikacji                                                       |

### 2.8 Tabela Orders

| Nazwa kolumny | Typ danych      | Opis                                                                        |
| ------------- | --------------- | --------------------------------------------------------------------------- |
| Id            | INT (serial)    | Klucz główny                                                                |
| OrderDate     | TIMESTAMP (UTC) | Data złożenia zamówienia                                                    |
| ClientId      | INT             | Klucz obcy → Clients(Id)                                                    |
| TotalAmount   | DECIMAL(18,2)   | Suma wartości zamówionych beczek (opcjonalnie obliczana po stronie serwisu) |
| CreatedAt     | TIMESTAMP (UTC) | Data utworzenia rekordu                                                     |
| UpdatedAt     | TIMESTAMP (UTC) | Data ostatniej modyfikacji                                                  |

> **Relacje między tabelami:**
>
> * `Clients (1) ←── Orders (N)`
> * `Recipes (1) ←── RecipeIngredients (N)`
> * `Ingredients (1) ←── RecipeIngredients (N)`
> * `Ingredients (1) ←── StockEntries (N)`
> * `Recipes (1) ←── Batches (N)`
> * `RecipeIngredients (1) ←── Batches` (przy tworzeniu partii każdy `RecipeIngredient` aktualizuje `SelectedStockEntryId`)
> * `Batches (1) ←── Kegs (N)`
> * `Orders (1) ←── Kegs (N)`

---

## 3. Struktura plików projektu

Poniżej przedstawiono układ folderów i plików w katalogu głównym projektu **BrewLogix**. Całość umieszczona jest w rozwiązaniu `BrewLogix.sln`.

```
BrewLogix.sln
BrewLogix.sln.DotSettings.user
│
└───BrewLogix
    │   Program.cs
    │   appsettings.json
    │   appsettings.Development.json
    │   BrewLogix.csproj
    │
    ├───Components/           ← Komponenty Blazor (np. częściowy kod współdzielony)
    │       NavMenu.razor
    │       Dashboard.razor
    │       ...
    │
    ├───Pages/                ← Strony Blazor (pliki .razor)
    │       Index.razor       ← Strona główna (Dashboard)
    │       Batches.razor     ← Zarządzanie partiami
    │       Recipes.razor     ← Zarządzanie recepturami
    │       Ingredients.razor ← Zarządzanie składnikami
    │       StockEntries.razor← Zarządzanie wpisami magazynowymi
    │       Kegs.razor        ← Zarządzanie beczkami
    │       Orders.razor      ← Zarządzanie zamówieniami
    │       Clients.razor     ← Zarządzanie klientami
    │
    ├───Models/               ← Definicje encji (klasy dziedziczące po BaseEntity)
    │       BaseEntity.cs
    │       Batch.cs
    │       Client.cs
    │       Ingredient.cs
    │       StockEntry.cs
    │       Recipe.cs
    │       RecipeIngredient.cs
    │       Keg.cs
    │       Order.cs
    │       OrderKeg.cs    ← (opcjonalnie tabela łącząca Order↔Keg, jeżeli wiele-do-wielu)
    │
    ├───Services/             ← Serwisy business logic (CRUD, operacje na encjach)
    │       DbContextProvider.cs
    │       ClientService.cs
    │       IngredientService.cs
    │       StockEntriesService.cs
    │       RecipeService.cs
    │       BatchService.cs
    │       KegService.cs
    │       OrderService.cs
    │
    ├───Seeders/              ← Klasy inicjujące (seed) przykładowe dane
    │       DatabaseInitializer.cs
    │       ClientSeeder.cs
    │       IngredientSeeder.cs
    │       RecipeSeeder.cs
    │       RecipeIngredientSeeder.cs
    │       StockEntrySeeder.cs
    │       BatchSeeder.cs
    │       KegSeeder.cs
    │       OrderSeeder.cs
    │
    ├───Validation/           ← Własne atrybuty walidacyjne
    │       AtLeastOneIngredientAttribute.cs
    │       UniqueIngredientsAttribute.cs
    │
    └───Data/                 ← Migrations i ewentualne pliki bazy (jeśli korzystamy z lokalnej SQLite w Development)
            Migrations/
```

* **Program.cs**
  – konfiguracja hosta, rejestracja serwisów, dodanie `DbContextProvider` oraz serwisów (`AddScoped<ClientService>()` itd.), włączenie Blazor Server.
* **appsettings.json** / **appsettings.Development.json**
  – definicje connection stringów (`DefaultConnection`, `TestConnection`) oraz ewentualne ustawienia logowania czy innych opcji konfiguracyjnych.
* **Pages/**
  – każda strona `.razor` odpowiada za część funkcjonalności: formularze CRUD, wyświetlanie tabel, dynamiczną interakcję.
* **Components/**
  – komponenty wielokrotnego użytku (np. formularze w modalu, kratki statystyk, listy wyboru).
* **Models/**
  – encje dziedziczące po `BaseEntity`, implementujące interfejs `IAuditable` lub inne interfejsy (np. `IDistributable`).
* **Services/**
  – klasa `DbContextProvider` zwraca kontekst EF Core w zależności od wybranego connection string. Pozostałe serwisy (`BatchService`, `RecipeService` itd.) zawierają metody CRUD.
* **Seeders/**
  – każda klasa typu „Seeder” dodaje przykładowe dane do tabel w odpowiedniej kolejności (najpierw klienci, później składniki, następnie receptury itd.).
* **Validation/**
  – atrybuty walidacyjne, które są stosowane w modelach lub w formularzach Blazor, by wymusić unikalność składników w recepturze, co najmniej jeden składnik, poprawność dat, zakresy liczbowe itd.
* **Data/Migrations/**
  – automatycznie generowane pliki migracji EF Core służące do tworzenia i aktualizacji schematu bazy danych.

---

## 4. DbContextProvider i konfiguracja połączenia

Aby umożliwić dynamiczne przełączanie się pomiędzy kilkoma połączeniami (np. `DefaultConnection` i `TestConnection`), wstrzykujemy do serwisów klasę `DbContextProvider`. Dzięki temu każdy serwis, który potrzebuje `AppDbContext`, wywołuje metodę `GetDbContext()` z odpowiednią konfiguracją.

### 4.1 plik `appsettings.json`

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=brewlogix;Username=postgres;Password=secret;",
    "TestConnection": "Host=localhost;Database=test_brewlogix;Username=postgres;Password=secret;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 4.2 Klasa `DbContextProvider.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BrewLogix.Services
{
    public class DbContextProvider
    {
        private readonly IConfiguration _configuration;
        private string _connectionString;

        public DbContextProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            SetDefaultConnection();
        }

        /// <summary>
        /// Ustawia domyślne połączenie (DefaultConnection).
        /// </summary>
        public void SetDefaultConnection()
        {
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Ustawia połączenie o podanej nazwie z appsettings (np. "TestConnection").
        /// </summary>
        public void SetConnection(string connectionName)
        {
            _connectionString = _configuration.GetConnectionString(connectionName);
        }

        /// <summary>
        /// Zwraca świeżo zainicjalizowany kontekst AppDbContext.
        /// </summary>
        public AppDbContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(_connectionString);
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
```

* **`_configuration.GetConnectionString("DefaultConnection")`** – wczytuje domyślny connection string.
* Metoda `SetConnection(string connectionName)` umożliwia zmianę połączenia np. przed migracjami w testach.
* `GetDbContext()` zwraca obiekt `AppDbContext`, korzystający z Npgsql (PostgreSQL).

### 4.3 Rejestracja w `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Dodanie DbContextProvider jako singleton
builder.Services.AddSingleton<DbContextProvider>();

// Dodanie serwisów CRUD
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<StockEntriesService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<BatchService>();
builder.Services.AddScoped<KegService>();
builder.Services.AddScoped<OrderService>();

// Dodanie Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Standardowa konfiguracja pipeline (middleware itp.)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

---

## 5. Modele (Encje)

Wszystkie encje dziedziczą po abstrakcyjnej klasie `BaseEntity`, która definiuje `Id`, `CreatedAt` oraz `UpdatedAt`. Ponadto niektóre encje implementują dodatkowe interfejsy, jak `IAuditable` (śledzenie dat) czy `IDistributable` (szczegóły dotyczące dystrybucji).

### 5.1 `BaseEntity.cs`

```csharp
using System;

namespace BrewLogix.Models
{
    public abstract class BaseEntity : IEntity, IAuditable
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
```

* `IEntity` – marker (w przyszłości może zawierać dodatkowe metody/statyczne właściwości).
* `IAuditable` – interfejs zapewniający pola do śledzenia dat tworzenia i aktualizacji.

### 5.2 `Client.cs`

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public class Client : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        // Nawigacja do zamówień
        public List<Order> Orders { get; set; } = new();
    }
}
```

* Walidacje DataAnnotations: `[Required]`, `[StringLength(100)]`, `[EmailAddress]`, `[Phone]`.
* Kolekcja `Orders` powiązana relacją 1\:N.

### 5.3 `Ingredient.cs`

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public class Ingredient : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } // np. „Słód”, „Chmiel”, „Drożdże”

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } // np. „kg”, „g”, „L”

        // Nawigacja
        public List<StockEntry> StockEntries { get; set; } = new();
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    }
}
```

### 5.4 `StockEntry.cs`

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public class StockEntry : BaseEntity
    {
        [Required]
        public int IngredientId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        // Nawigacja
        public Ingredient Ingredient { get; set; }
    }
}
```

### 5.5 `Recipe.cs`

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public class Recipe : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Style { get; set; }

        public string Description { get; set; }

        // Nawigacja
        [AtLeastOneIngredient]
        public List<RecipeIngredient> Ingredients { get; set; } = new();
    }
}
```

* Atrybut `[AtLeastOneIngredient]` (własny) sprawdza, czy lista `Ingredients` ma co najmniej jeden element.
* `Style` i `Description` mogą być opcjonalne.

### 5.6 `RecipeIngredient.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public class RecipeIngredient : BaseEntity
    {
        [Required]
        public int RecipeId { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        // Wybrany wpis magazynowy (podczas tworzenia partii)
        public int? SelectedStockEntryId { get; set; }

        // Nawigacja
        public Recipe Recipe { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}
```

* Atrybut `[UniqueIngredients]` można dodać nad klasą lub w modelu `Recipe`, by zapobiec powtórzeniom tego samego `IngredientId` w ramach jednej receptury.

### 5.7 `Batch.cs`

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public class Batch : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        public int RecipeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        // Nawigacja
        public Recipe Recipe { get; set; }
        public List<Keg> Kegs { get; set; } = new();
    }
}
```

* `Status` może przyjmować wartości np. „Planowana”, „W toku”, „Zakończona”.
* W nawigacji znajdują się powiązane `Kegs` (lista beczek wyprodukowanych w tej partii).

### 5.8 `Keg.cs`

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public interface IDistributable
    {
        DateTime FilledAt { get; set; }
        bool IsDistributed { get; set; }
    }

    public class Keg : BaseEntity, IDistributable
    {
        [Required]
        public int BatchId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal VolumeLiters { get; set; }

        public DateTime FilledAt { get; set; }

        public bool IsDistributed { get; set; }

        // Nawigacja
        public Batch Batch { get; set; }
        public Order Order { get; set; }      // jeśli jedna beczka przypisana jest do jednego zam.
        public int? OrderId { get; set; }     // nullable, bo beczka może nie być jeszcze sprzedana
    }
}
```

* `IDistributable` zapewnia pola `FilledAt` i `IsDistributed`.
* Opcjonalnie, jeżeli w projekcie jest wiele-do-wielu relacja między `Orders` i `Kegs`, można dodać dodatkową tabelę łączącą, np. `OrderKeg`.

### 5.9 `Order.cs`

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public class Order : BaseEntity
    {
        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int ClientId { get; set; }

        // Podsumowanie wartości (może być wyliczane dynamicznie lub przechowywane)
        public decimal TotalAmount { get; set; }

        // Nawigacja
        public Client Client { get; set; }
        public List<Keg> Kegs { get; set; } = new();
    }
}
```

* `TotalAmount` można obliczać na podstawie cen za litr lub ustalonych stawek (opcjonalnie).
* Jeżeli zachodzi potrzeba wielu-do-wielu między `Orders` i `Kegs`, można wprowadzić pośrednią encję `OrderKeg`, ale w tej wersji jedna beczka ma jedno pole `OrderId`.

---

## 6. Seedery

Aby wypełnić bazę przykładowymi danymi, tworzymy klasę główną `DatabaseInitializer`, która wywołuje kolejne seedy w odpowiedniej kolejności, by zachować integralność FK:

```csharp
using BrewLogix.Models;
using BrewLogix.Services;
using System.Linq;

namespace BrewLogix.Seeders
{
    public static class DatabaseInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Upewniamy się, że baza jest utworzona
            context.Database.Migrate();

            // Uruchamiamy pojedyncze seedery w odpowiedniej kolejności:
            if (!context.Clients.Any())
                ClientSeeder.Seed(context);

            if (!context.Ingredients.Any())
                IngredientSeeder.Seed(context);

            if (!context.Recipes.Any())
                RecipeSeeder.Seed(context);

            if (!context.RecipeIngredients.Any())
                RecipeIngredientSeeder.Seed(context);

            if (!context.StockEntries.Any())
                StockEntrySeeder.Seed(context);

            if (!context.Batches.Any())
                BatchSeeder.Seed(context);

            if (!context.Kegs.Any())
                KegSeeder.Seed(context);

            if (!context.Orders.Any())
                OrderSeeder.Seed(context);
        }
    }
}
```

**Przykład `ClientSeeder.cs`:**

```csharp
using BrewLogix.Models;
using System;

namespace BrewLogix.Seeders
{
    public static class ClientSeeder
    {
        public static void Seed(AppDbContext context)
        {
            var clients = new[]
            {
                new Client { Name = "Browar Podgórski", Email = "kontakt@podgorski.pl", Address = "ul. Browarnicza 5, 35-060 Rzeszów", PhoneNumber = "+48 123 456 789" },
                new Client { Name = "Pub Stara Kamienica", Email = "pub@stara.pl", Address = "ul. Staromiejska 12, 35-001 Rzeszów", PhoneNumber = "+48 987 654 321" },
                new Client { Name = "Sklep Piwny Rzeszów", Email = "sklep@piwny.pl", Address = "ul. Piwna 3, 35-200 Rzeszów" }
            };

            foreach (var client in clients)
            {
                context.Clients.Add(client);
            }
            context.SaveChanges();
        }
    }
}
```

Pozostałe seedery (`IngredientSeeder`, `RecipeSeeder`, `RecipeIngredientSeeder`, `StockEntrySeeder`, `BatchSeeder`, `KegSeeder`, `OrderSeeder`) postępują analogicznie: dodają po kilka wierszy, definiują odpowiednie klucze obce (np. przypisują `IngredientId` do ziarna słodu, chmielu itd.), a na końcu wywołują `context.SaveChanges()`.

---

## 7. Serwisy (Logika biznesowa i CRUD)

Każda encja posiada swój dedykowany serwis, zapewniający metody typu CRUD (GetAll, GetById, Add, Update, Delete). Serwis korzysta z `DbContextProvider` do utworzenia `AppDbContext` i wykonywania operacji na bazie.

### 7.1 `AppDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using BrewLogix.Models;

namespace BrewLogix.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<StockEntry> StockEntries { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Keg> Kegs { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfiguracja relacji wielo-do-wielu lub dodatkowe reguły można umieścić tutaj.
            base.OnModelCreating(modelBuilder);
        }
    }
}
```

### 7.2 Przykład serwisu: `BatchService.cs`

```csharp
using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BrewLogix.Services
{
    public class BatchService
    {
        private readonly DbContextProvider _dbContextProvider;

        public BatchService(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public IEnumerable<Batch> GetAllBatches()
        {
            using var context = _dbContextProvider.GetDbContext();
            // Eager loading receptury i związanych składników
            return context.Batches
                          .Include(b => b.Recipe)
                              .ThenInclude(r => r.Ingredients)
                                  .ThenInclude(ri => ri.Ingredient)
                          .ToList();
        }

        public Batch GetBatch(int id)
        {
            using var context = _dbContextProvider.GetDbContext();
            return context.Batches
                          .Include(b => b.Recipe)
                              .ThenInclude(r => r.Ingredients)
                                  .ThenInclude(ri => ri.Ingredient)
                          .FirstOrDefault(b => b.Id == id);
        }

        public void AddBatch(Batch batch)
        {
            using var context = _dbContextProvider.GetDbContext();
            context.Batches.Add(batch);
            context.SaveChanges();
        }

        public void UpdateBatch(Batch batch)
        {
            using var context = _dbContextProvider.GetDbContext();
            context.Batches.Update(batch);
            context.SaveChanges();
        }

        public void DeleteBatch(Batch batch)
        {
            using var context = _dbContextProvider.GetDbContext();
            context.Batches.Remove(batch);
            context.SaveChanges();
        }
    }
}
```

### 7.3 Inne serwisy (schematycznie)

* **`ClientService.cs`**
  – metody: `GetAllClients()`, `GetClient(int id)`, `AddClient(Client client)`, `UpdateClient(Client client)`, `DeleteClient(Client client)`.
* **`IngredientService.cs`**
  – `GetAllIngredients()`, `GetIngredient(int id)`, `AddIngredient(Ingredient ingredient)`, `UpdateIngredient(Ingredient ingredient)`, `DeleteIngredient(Ingredient ingredient)`.
* **`StockEntriesService.cs`**
  – `GetAllStockEntries()`, `GetStockEntriesForIngredient(int ingredientId)`, `GetStockEntry(int id)`, `AddStockEntry(StockEntry entry)`, `UpdateStockEntry(StockEntry entry)`, `DeleteStockEntry(StockEntry entry)`.
* **`RecipeService.cs`**
  – `GetAllRecipes()`, `GetRecipe(int id)`, `AddRecipe(Recipe recipe)`, `UpdateRecipe(Recipe recipe)`, `DeleteRecipe(Recipe recipe)`.
  – Przy pobieraniu receptury warto włączyć `Include(r => r.Ingredients).ThenInclude(ri => ri.Ingredient)` w celu wczytania szczegółów składników.
* **`KegService.cs`**
  – `GetAllKegs()`, `GetKeg(int id)`, `AddKeg(Keg keg)`, `UpdateKeg(Keg keg)`, `DeleteKeg(Keg keg)`, `IsBatchAssignedToAnyKeg(int batchId)` (zwraca `true`, jeśli dany `batchId` jest użyty przez co najmniej jedną beczkę).
* **`OrderService.cs`**
  – `GetAllOrders()`, `GetOrder(int id)`, `AddOrder(Order order)`, `UpdateOrder(Order order)`, `DeleteOrder(Order order)`.
  – Przy pobieraniu zamówienia należy uwzględnić `Include(o => o.Client)` oraz `Include(o => o.Kegs).ThenInclude(k => k.Batch)` (jeśli chcemy w widoku wyświetlać kod partii wraz z informacjami o beczkach).

---

## 8. Komponenty Blazor (Widoki)

W projekcie używamy Blazor Server, więc każda strona to plik `.razor` znajdujący się w katalogu `Pages/`. W każdej z tych stron następuje wstrzyknięcie potrzebnych serwisów, pobranie danych w metodzie `OnInitialized()`, a następnie wyświetlenie interfejsu z formularzem oraz tabelą/listą.

### 8.1 Strona główna (Index.razor)

```razor
@page "/"
@inject ClientService ClientService
@inject BatchService BatchService

<PageTitle>Dashboard</PageTitle>

<h3>Dashboard BrewLogix</h3>

<p>Witamy w systemie BrewLogix! Poniżej krótki przegląd najważniejszych statystyk:</p>

<div class="row">
    <div class="col-md-3">
        <div class="card text-white bg-primary mb-3">
            <div class="card-body">
                <h5 class="card-title">Klienci</h5>
                <p class="card-text">@clientsCount</p>
            </div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="card text-white bg-success mb-3">
            <div class="card-body">
                <h5 class="card-title">Partie aktywne</h5>
                <p class="card-text">@activeBatchesCount</p>
            </div>
        </div>
    </div>
    <!-- Dalsze karty: stany magazynowe poniżej progu, liczba nieskończonych zamówień itp. -->
</div>

@code {
    private int clientsCount;
    private int activeBatchesCount;

    protected override void OnInitialized()
    {
        clientsCount = ClientService.GetAllClients().Count();
        activeBatchesCount = BatchService.GetAllBatches()
            .Count(b => b.Status != "Zakończona");
    }
}
```

* Strona `Index.razor` pełni rolę kokpitu (dashboard), wyświetlając karty z najważniejszymi danymi (liczba klientów, aktywne partie, etc.).
* W `OnInitialized()` pobieramy listę klientów i listę partii, a następnie obliczamy podstawowe wskaźniki.

---

### 8.2 Zarządzanie partiami (Pages/Batches.razor)

Poniżej znajduje się kluczowy fragment strony `Batches.razor`, odpowiedzialny za CRUD partii produkcyjnych. Ten kod należy wkleić jako podsekcję do dokumentacji, ponieważ demonstruje zaawansowaną walidację stanów magazynowych oraz integrację z serwisami.

```razor
@page "/batches"
@using BrewLogix.Models
@inject BatchService BatchService
@inject RecipeService RecipeService
@inject KegService KegService
@inject StockEntriesService StockEntriesService

<PageTitle>Partie produkcyjne</PageTitle>

<h3 class="mb-4">Partie produkcyjne (Batches)</h3>

<button class="btn btn-primary mb-3" @onclick="() => OpenBatchForm()">
    <i class="bi bi-plus-circle me-2"></i> Dodaj nową partię
</button>

@if (!string.IsNullOrWhiteSpace(errorMessage))
{
    <div class="alert alert-danger" role="alert">
        @errorMessage
    </div>
}

@if (isBatchFormOpen)
{
    <div class="card mb-4">
        <div class="card-body">
            <h5 class="card-title mb-3">@((newBatch.Id == 0) ? "Dodaj partię" : "Edytuj partię")</h5>
            <EditForm Model="@newBatch" OnValidSubmit="HandleAddBatch">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <!-- Pole: Kod partii -->
                <div class="mb-3">
                    <label class="form-label">Kod partii</label>
                    <InputText class="form-control" @bind-Value="newBatch.Code" />
                </div>

                <!-- Wybór receptury (tylko przy nowej partii) -->
                @{
                    if (newBatch.Recipe == null)
                    {
                        <div class="mb-3">
                            <label class="form-label">Receptura</label>
                            <InputSelect class="form-control" @bind-Value="newBatch.RecipeId">
                                <option value="">Wybierz recepturę</option>
                                @foreach (var recipe in recipes)
                                {
                                    <option value="@recipe.Id">@recipe.Name</option>
                                }
                            </InputSelect>
                        </div>

                        @* Po wybraniu receptury: lista składników i stanów magazynowych *@
                        var selectedRecipe = recipes.FirstOrDefault(r => r.Id == newBatch.RecipeId);
                        if (selectedRecipe != null)
                        {
                            <div class="mb-3">
                                <p class="form-label">Składniki:</p>
                                @foreach (var ingredientEntry in selectedRecipe.Ingredients)
                                {
                                    <div class="mb-2">
                                        <label class="form-label">@ingredientEntry.Ingredient.Name</label>
                                        @{
                                            var stockList = StockEntriesService
                                                            .GetStockEntriesForIngredient(ingredientEntry.Ingredient.Id)
                                                            .ToList();

                                            if (stockList.Any())
                                            {
                                                <InputSelect class="form-control"
                                                             @bind-Value="ingredientEntry.SelectedStockEntryId">
                                                    <option value="">-- wybierz wpis magazynowy --</option>
                                                    @foreach (var entry in stockList)
                                                    {
                                                        <option value="@entry.Id">
                                                            @entry.Quantity @ingredientEntry.Ingredient.Unit 
                                                            ( ważne do: @entry.ExpiryDate.ToShortDateString() )
                                                        </option>
                                                    }
                                                </InputSelect>
                                            }
                                            else
                                            {
                                                <p class="text-danger">Brak stanów magazynowych dla  
                                                   @ingredientEntry.Ingredient.Name</p>
                                            }
                                        }
                                    </div>
                                }
                            </div>
                        }
                    }
                    else
                    {
                        @* Edycja: wyświetlamy tylko podsumowanie składników *@
                        <div class="mb-3">
                            <label class="form-label">Receptura:</label>
                            <p>@newBatch.Recipe.Name</p>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Składniki:</label><br />
                            @foreach (var ingredientEntry in newBatch.Recipe.Ingredients)
                            {
                                <p>- @ingredientEntry.Ingredient.Name:   
                                   @ingredientEntry.Quantity  
                                   @ingredientEntry.Ingredient.Unit
                                </p>
                            }
                        </div>
                    }
                }

                <!-- Daty partii -->
                <div class="mb-3">
                    <label class="form-label">Data rozpoczęcia</label>
                    <InputDate class="form-control" 
                               @bind-Value="newBatch.StartDate" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Data zakończenia</label>
                    <InputDate class="form-control" 
                               @bind-Value="newBatch.EndDate" />
                </div>

                <!-- Status partii -->
                <div class="mb-3">
                    <label class="form-label">Status</label>
                    <InputText class="form-control" @bind-Value="newBatch.Status" />
                </div>

                <div class="d-flex gap-2">
                    <button type="submit" class="btn btn-primary">
                        @((newBatch.Id == 0) ? "Dodaj partię" : "Zapisz zmiany")
                    </button>
                    <button type="button" class="btn btn-secondary" 
                            @onclick="CancelBatchForm">Anuluj</button>
                </div>
            </EditForm>
        </div>
    </div>
}

<!-- Pole wyszukiwania partii -->
<div class="mb-3">
    <label class="form-label">Wyszukaj partie</label>
    <InputText class="form-control" @bind-Value="searchTerm" 
               placeholder="Kod, receptura lub status..." />
</div>

@if (filteredBatches?.Any() == true)
{
    <div class="card">
        <ul class="list-group list-group-flush">
            @foreach (var batch in filteredBatches)
            {
                <li class="list-group-item p-3">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <h5 class="mb-2">@batch.Code</h5>
                            <div class="text-muted">
                                <div>
                                    <i class="bi bi-calendar me-2"></i>
                                    @batch.StartDate.ToShortDateString() 
                                    – @(batch.EndDate?.ToShortDateString() ?? "W toku")
                                </div>
                                <div>
                                    <i class="bi bi-gear me-2"></i>
                                    Receptura: @batch.Recipe.Name
                                </div>
                                <div>
                                    <i class="bi bi-gear me-2"></i>
                                    Status: @batch.Status
                                </div>
                            </div>
                        </div>
                        <div>
                            <button class="btn btn-warning btn-sm me-2" 
                                    @onclick="() => EditBatch(batch)">Edytuj</button>
                            <button class="btn btn-danger btn-sm" 
                                    @onclick="() => DeleteBatch(batch)">Usuń</button>
                        </div>
                    </div>
                </li>
            }
        </ul>
    </div>
}
else
{
    <p>Brak partii spełniających kryteria wyszukiwania.</p>
}

@code {
    private List<Batch> batches = new();
    private List<Recipe> recipes = new();
    private Batch newBatch = new();
    private bool isBatchFormOpen = false;
    private string searchTerm = string.Empty;
    private string errorMessage = null;

    protected override void OnInitialized()
    {
        batches = BatchService.GetAllBatches().ToList();
        recipes = RecipeService.GetAllRecipes().ToList();
    }

    private void OpenBatchForm()
    {
        newBatch = new Batch
        {
            StartDate = DateTime.Today,
            EndDate = null,
            Status = "Planowana"
        };
        isBatchFormOpen = true;
        errorMessage = null;
    }

    private void CancelBatchForm()
    {
        isBatchFormOpen = false;
        newBatch = new Batch();
        errorMessage = null;
    }

    private void HandleAddBatch()
    {
        errorMessage = null;
        newBatch.StartDate = DateTime.SpecifyKind(newBatch.StartDate, DateTimeKind.Utc);
        if (newBatch.EndDate.HasValue)
            newBatch.EndDate = DateTime.SpecifyKind(newBatch.EndDate.Value, DateTimeKind.Utc);

        if (newBatch.Id == 0)
        {
            var selectedRecipe = recipes.FirstOrDefault(r => r.Id == newBatch.RecipeId);
            if (selectedRecipe != null)
            {
                // Walidacja: każde SelectedStockEntryId musi być wybrane i mieć wystarczający stan
                foreach (var ingredientEntry in selectedRecipe.Ingredients)
                {
                    var stockEntry = StockEntriesService
                                     .GetStockEntry(ingredientEntry.SelectedStockEntryId ?? 0);
                    if (stockEntry == null)
                    {
                        errorMessage = $"Nie wybrano wpisu magazynowego dla składnika {ingredientEntry.Ingredient.Name}.";
                        return;
                    }
                    if (stockEntry.Quantity < ingredientEntry.Quantity)
                    {
                        errorMessage = $"Za mało składnika: {ingredientEntry.Ingredient.Name}. " +
                                       $"Wymagane: {ingredientEntry.Quantity}, dostępne: {stockEntry.Quantity}.";
                        return;
                    }
                }

                // Pomniejsz stany magazynowe
                foreach (var ingredientEntry in selectedRecipe.Ingredients)
                {
                    var stockEntry = StockEntriesService
                                     .GetStockEntry(ingredientEntry.SelectedStockEntryId.Value);
                    stockEntry.Quantity -= ingredientEntry.Quantity;
                    StockEntriesService.UpdateStockEntry(stockEntry);
                }
            }

            BatchService.AddBatch(newBatch);
        }
        else
        {
            BatchService.UpdateBatch(newBatch);
        }

        // Odśwież listę i zamknij formularz
        batches = BatchService.GetAllBatches().ToList();
        isBatchFormOpen = false;
        newBatch = new Batch();
    }

    private void EditBatch(Batch batch)
    {
        // Załaduj dane partii do formularza (zależnie od akcji trzeba wczytać powiązane dane)
        newBatch = new Batch
        {
            Id = batch.Id,
            Code = batch.Code,
            RecipeId = batch.RecipeId,
            Recipe = recipes.First(r => r.Id == batch.RecipeId),
            StartDate = batch.StartDate,
            EndDate = batch.EndDate,
            Status = batch.Status
        };
        isBatchFormOpen = true;
        errorMessage = null;
    }

    private void DeleteBatch(Batch batch)
    {
        if (KegService.IsBatchAssignedToAnyKeg(batch.Id))
        {
            errorMessage = 
                $"Nie można usunąć partii {batch.Code}, ponieważ jest przypisana do co najmniej jednej beczki.";
            return;
        }
        BatchService.DeleteBatch(batch);
        batches = BatchService.GetAllBatches().ToList();
    }

    private IEnumerable<Batch> filteredBatches =>
        batches.Where(batch =>
            string.IsNullOrWhiteSpace(searchTerm) ||
            batch.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            batch.Status.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            recipes.Any(r => r.Id == batch.RecipeId &&
                             r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
}
```

**Wytłumaczenie najważniejszych fragmentów:**

1. **Dynamiczna lista receptur i stanów magazynowych**

    * Jeśli `newBatch.Recipe == null` (czyli tworzymy nową partię), wyświetlamy `<InputSelect>` z listą `recipes`.
    * Po wybraniu receptury pobieramy wszystkie wpisy magazynowe dla każdego składnika (`StockEntriesService.GetStockEntriesForIngredient(...)`).
    * Użytkownik wybiera konkretny wpis (pole `SelectedStockEntryId`) dla każdego `RecipeIngredient`.

2. **Walidacja stanów magazynowych w `HandleAddBatch()`**

    * Sprawdzamy, czy każde `SelectedStockEntryId` nie jest `null`.
    * Pobieramy `StockEntry` i weryfikujemy, czy `Quantity >= ingredientEntry.Quantity`.
    * Jeśli walidacja nie przejdzie, ustawiamy `errorMessage` i przerywamy wykonanie, co powoduje wyświetlenie komunikatu w `<alert>`.
    * Po pozytywnej weryfikacji dekrementujemy stany magazynowe, wywołując `StockEntriesService.UpdateStockEntry(stockEntry)`.

3. **Zabezpieczenie przed usunięciem partii**

    * Metoda `DeleteBatch()` najpierw wywołuje `KegService.IsBatchAssignedToAnyKeg(batch.Id)`.
    * Jeśli partia ma przypisaną przynajmniej jedną beczkę, wyświetlamy odpowiedni komunikat w `errorMessage` i nie usuwamy.

4. **Filtrowanie listy**

    * `filteredBatches` zwraca partie, które zawierają w kodzie, statusie lub nazwie receptury ciąg wyszukiwania (`searchTerm`).
    * Dzięki temu użytkownik może w locie filtrować partie bez przeładowania strony.

---

### 8.3 Zarządzanie recepturami (Pages/Recipes.razor)

```razor
@page "/recipes"
@using BrewLogix.Models
@inject RecipeService RecipeService
@inject IngredientService IngredientService
@inject RecipeService recipeService

<PageTitle>Receptury</PageTitle>

<h3 class="mb-4">Receptury</h3>

<button class="btn btn-primary mb-3" @onclick="() => OpenRecipeForm()">
    <i class="bi bi-plus-circle me-2"></i> Dodaj recepturę
</button>

@if (isRecipeFormOpen)
{
    <div class="card mb-4">
        <div class="card-body">
            <h5 class="card-title mb-3">@((newRecipe.Id == 0) ? "Nowa receptura" : "Edytuj recepturę")</h5>
            <EditForm Model="@newRecipe" OnValidSubmit="HandleAddRecipe">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <div class="mb-3">
                    <label class="form-label">Nazwa receptury</label>
                    <InputText class="form-control" @bind-Value="newRecipe.Name" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Styl</label>
                    <InputText class="form-control" @bind-Value="newRecipe.Style" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Opis</label>
                    <InputTextArea class="form-control" rows="3" @bind-Value="newRecipe.Description" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Składniki</label>
                    @foreach (var entry in newRecipe.Ingredients)
                    {
                        <div class="row g-2 mb-2 align-items-center">
                            <div class="col-6">
                                <InputSelect class="form-select"
                                             @bind-Value="entry.IngredientId">
                                    <option value="">-- Wybierz składnik --</option>
                                    @foreach (var ing in allIngredients)
                                    {
                                        <option value="@ing.Id">@ing.Name (@ing.Unit)</option>
                                    }
                                </InputSelect>
                            </div>
                            <div class="col-4">
                                <InputNumber class="form-control"
                                             @bind-Value="entry.Quantity" 
                                             Min="0.01" 
                                             Step="0.01" />
                            </div>
                            <div class="col-2">
                                <button type="button" class="btn btn-danger btn-sm"
                                        @onclick="() => RemoveIngredient(entry)">
                                    <i class="bi bi-trash"></i>
                                </button>
                            </div>
                        </div>
                    }
                    <button type="button" class="btn btn-secondary btn-sm"
                            @onclick="AddIngredientField">
                        <i class="bi bi-plus-circle me-1"></i> Dodaj składnik
                    </button>
                </div>

                <div class="d-flex gap-2">
                    <button type="submit" class="btn btn-primary">
                        @((newRecipe.Id == 0) ? "Dodaj recepturę" : "Zapisz zmiany")
                    </button>
                    <button type="button" class="btn btn-secondary" 
                            @onclick="CancelRecipeForm">Anuluj</button>
                </div>
            </EditForm>
        </div>
    </div>
}

@if (filteredRecipes.Any())
{
    <div class="mb-3">
        <label class="form-label">Wyszukaj receptury</label>
        <InputText class="form-control" @bind-Value="searchRecipeTerm" 
                   placeholder="Nazwa lub styl..." />
    </div>

    <div class="card">
        <ul class="list-group list-group-flush">
            @foreach (var recipe in filteredRecipes)
            {
                <li class="list-group-item p-3">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <h5 class="mb-2">@recipe.Name</h5>
                            <div class="text-muted">
                                <div>Styl: @recipe.Style</div>
                                <div>@recipe.Description</div>
                            </div>
                        </div>
                        <div>
                            <button class="btn btn-warning btn-sm me-2" 
                                    @onclick="() => EditRecipe(recipe)">Edytuj</button>
                            <button class="btn btn-danger btn-sm" 
                                    @onclick="() => DeleteRecipe(recipe)">Usuń</button>
                        </div>
                    </div>
                </li>
            }
        </ul>
    </div>
}
else
{
    <p>Brak receptur do wyświetlenia.</p>
}

@code {
    private List<Recipe> recipes = new();
    private IEnumerable<Ingredient> allIngredients = new List<Ingredient>();
    private Recipe newRecipe = new();
    private bool isRecipeFormOpen = false;
    private string searchRecipeTerm = string.Empty;
    private string errorMessage = null;

    protected override void OnInitialized()
    {
        recipes = RecipeService.GetAllRecipes().ToList();
        allIngredients = IngredientService.GetAllIngredients();
    }

    private void OpenRecipeForm()
    {
        newRecipe = new Recipe();
        isRecipeFormOpen = true;
        errorMessage = null;
    }

    private void CancelRecipeForm()
    {
        isRecipeFormOpen = false;
        newRecipe = new Recipe();
        errorMessage = null;
    }

    private void AddIngredientField()
    {
        newRecipe.Ingredients.Add(new RecipeIngredient());
    }

    private void RemoveIngredient(RecipeIngredient entry)
    {
        newRecipe.Ingredients.Remove(entry);
    }

    private void HandleAddRecipe()
    {
        errorMessage = null;

        // Weryfikacja co najmniej jednego składnika
        if (!newRecipe.Ingredients.Any())
        {
            errorMessage = "Receptura musi zawierać przynajmniej jeden składnik.";
            return;
        }

        // Sprawdź unikalność składników
        var duplicates = newRecipe.Ingredients
            .GroupBy(i => i.IngredientId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
        if (duplicates.Any())
        {
            errorMessage = "W recepturze występuje powtórzenie składnika.";
            return;
        }

        if (newRecipe.Id == 0)
        {
            RecipeService.AddRecipe(newRecipe);
        }
        else
        {
            RecipeService.UpdateRecipe(newRecipe);
        }

        recipes = RecipeService.GetAllRecipes().ToList();
        isRecipeFormOpen = false;
        newRecipe = new Recipe();
    }

    private void EditRecipe(Recipe recipe)
    {
        // Wczytaj recepturę i jej składniki (wraz z nazwą składników)
        newRecipe = RecipeService.GetRecipe(recipe.Id);
        isRecipeFormOpen = true;
        errorMessage = null;
    }

    private void DeleteRecipe(Recipe recipe)
    {
        RecipeService.DeleteRecipe(recipe);
        recipes = RecipeService.GetAllRecipes().ToList();
    }

    private IEnumerable<Recipe> filteredRecipes => 
        recipes.Where(r =>
            string.IsNullOrWhiteSpace(searchRecipeTerm) ||
            r.Name.Contains(searchRecipeTerm, StringComparison.OrdinalIgnoreCase) ||
            (r.Style != null && 
             r.Style.Contains(searchRecipeTerm, StringComparison.OrdinalIgnoreCase)));
}
```

**Opis najważniejszych fragmentów:**

1. **Dynamiczne pola „Składniki”**

    * Kliknięcie przycisku „Dodaj składnik” dodaje nowy obiekt `RecipeIngredient` do listy `newRecipe.Ingredients`.
    * W każdej iteracji wyświetlamy `<InputSelect>` z pełną listą składników (`allIngredients`), pobranych z serwisu `IngredientService`.
    * Obok pola wyboru składnika jest `<InputNumber>` do wprowadzenia wymaganej ilości.
    * Przycisk „Usuń składnik” usuwa dany `RecipeIngredient` z listy.

2. **Walidacja formularza**

    * Sprawdzamy, czy lista `Ingredients` jest niepusta (własne sprawdzenie w `HandleAddRecipe()`).
    * Weryfikujemy, czy nie ma powtórzeń identyfikatorów `IngredientId` (grupowanie LINQ).
    * Jeżeli błędy, ustawiamy `errorMessage`, co spowoduje wyświetlenie komunikatu w `<ValidationSummary />` i `<alert>`.

3. **Filtrowanie listy receptur**

    * Dzięki polu `searchRecipeTerm` użytkownik może w locie filtrować receptury po nazwie lub stylu.

---

### 8.4 Zarządzanie magazynem (Pages/Ingredients.razor & Pages/StockEntries.razor)

#### 8.4.1 Składniki (Ingredients.razor)

```razor
@page "/ingredients"
@using BrewLogix.Models
@inject IngredientService IngredientService

<PageTitle>Składniki</PageTitle>

<h3 class="mb-4">Składniki</h3>

<button class="btn btn-primary mb-3" @onclick="() => OpenIngredientForm()">
    <i class="bi bi-plus-circle me-2"></i> Dodaj składnik
</button>

@if (isIngredientFormOpen)
{
    <div class="card mb-4">
        <div class="card-body">
            <h5 class="card-title mb-3">@((newIngredient.Id == 0) ? "Nowy składnik" : "Edytuj składnik")</h5>
            <EditForm Model="@newIngredient" OnValidSubmit="HandleAddIngredient">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <div class="mb-3">
                    <label class="form-label">Nazwa składnika</label>
                    <InputText class="form-control" @bind-Value="newIngredient.Name" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Typ</label>
                    <InputText class="form-control" @bind-Value="newIngredient.Type" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Jednostka (kg, g, L)</label>
                    <InputText class="form-control" @bind-Value="newIngredient.Unit" />
                </div>

                <div class="d-flex gap-2">
                    <button type="submit" class="btn btn-primary">
                        @((newIngredient.Id == 0) ? "Dodaj składnik" : "Zapisz zmiany")
                    </button>
                    <button type="button" class="btn btn-secondary" @onclick="CancelIngredientForm">Anuluj</button>
                </div>
            </EditForm>
        </div>
    </div>
}

@if (ingredients.Any())
{
    <div class="mb-3">
        <label class="form-label">Wyszukaj składniki</label>
        <InputText class="form-control" @bind-Value="searchIngredientTerm" 
                   placeholder="Nazwa lub typ..." />
    </div>
    <table class="table table-striped">
        <thead>
            <tr>
                <th>Nazwa</th>
                <th>Typ</th>
                <th>Jednostka</th>
                <th>Akcje</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var ing in filteredIngredients)
            {
                <tr>
                    <td>@ing.Name</td>
                    <td>@ing.Type</td>
                    <td>@ing.Unit</td>
                    <td>
                        <button class="btn btn-warning btn-sm me-2" @onclick="() => EditIngredient(ing)">Edytuj</button>
                        <button class="btn btn-danger btn-sm" @onclick="() => DeleteIngredient(ing)">Usuń</button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
else
{
    <p>Brak zdefiniowanych składników.</p>
}

@code {
    private List<Ingredient> ingredients = new();
    private Ingredient newIngredient = new();
    private bool isIngredientFormOpen = false;
    private string searchIngredientTerm = string.Empty;

    protected override void OnInitialized()
    {
        ingredients = IngredientService.GetAllIngredients().ToList();
    }

    private void OpenIngredientForm()
    {
        newIngredient = new Ingredient();
        isIngredientFormOpen = true;
    }

    private void CancelIngredientForm()
    {
        isIngredientFormOpen = false;
        newIngredient = new Ingredient();
    }

    private void HandleAddIngredient()
    {
        if (newIngredient.Id == 0)
            IngredientService.AddIngredient(newIngredient);
        else
            IngredientService.UpdateIngredient(newIngredient);

        ingredients = IngredientService.GetAllIngredients().ToList();
        isIngredientFormOpen = false;
        newIngredient = new Ingredient();
    }

    private void EditIngredient(Ingredient ing)
    {
        newIngredient = IngredientService.GetIngredient(ing.Id);
        isIngredientFormOpen = true;
    }

    private void DeleteIngredient(Ingredient ing)
    {
        IngredientService.DeleteIngredient(ing);
        ingredients = IngredientService.GetAllIngredients().ToList();
    }

    private IEnumerable<Ingredient> filteredIngredients =>
        ingredients.Where(i =>
            string.IsNullOrWhiteSpace(searchIngredientTerm) ||
            i.Name.Contains(searchIngredientTerm, StringComparison.OrdinalIgnoreCase) ||
            i.Type.Contains(searchIngredientTerm, StringComparison.OrdinalIgnoreCase));
}
```

#### 8.4.2 Wpisy magazynowe (StockEntries.razor)

```razor
@page "/stockentries"
@using BrewLogix.Models
@inject StockEntriesService StockEntriesService
@inject IngredientService IngredientService

<PageTitle>Wpisy magazynowe</PageTitle>

<h3 class="mb-4">Wpisy magazynowe (Stock Entries)</h3>

<button class="btn btn-primary mb-3" @onclick="() => OpenStockEntryForm()">
    <i class="bi bi-plus-circle me-2"></i> Dodaj wpis magazynowy
</button>

@if (isStockEntryFormOpen)
{
    <div class="card mb-4">
        <div class="card-body">
            <h5 class="card-title mb-3">@((newEntry.Id == 0) ? "Nowy wpis magazynowy" : "Edytuj wpis")</h5>
            <EditForm Model="@newEntry" OnValidSubmit="HandleAddStockEntry">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <div class="mb-3">
                    <label class="form-label">Składnik</label>
                    <InputSelect class="form-control" @bind-Value="newEntry.IngredientId">
                        <option value="">-- wybierz składnik --</option>
                        @foreach (var ing in allIngredients)
                        {
                            <option value="@ing.Id">@ing.Name (@ing.Unit)</option>
                        }
                    </InputSelect>
                </div>
                <div class="mb-3">
                    <label class="form-label">Ilość</label>
                    <InputNumber class="form-control" 
                                 @bind-Value="newEntry.Quantity" 
                                 Min="0.01" Step="0.01"/>
                </div>
                <div class="mb-3">
                    <label class="form-label">Data dostawy</label>
                    <InputDate class="form-control" @bind-Value="newEntry.DeliveryDate" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Data przydatności</label>
                    <InputDate class="form-control" @bind-Value="newEntry.ExpiryDate" />
                </div>

                <div class="d-flex gap-2">
                    <button type="submit" class="btn btn-primary">
                        @((newEntry.Id == 0) ? "Dodaj wpis" : "Zapisz zmiany")
                    </button>
                    <button type="button" class="btn btn-secondary" @onclick="CancelStockEntryForm">Anuluj</button>
                </div>
            </EditForm>
        </div>
    </div>
}

@if (filteredStockEntries.Any())
{
    <div class="mb-3">
        <label class="form-label">Wyszukaj po nazwie składnika</label>
        <InputText class="form-control" @bind-Value="searchStockTerm" 
                   placeholder="Nazwa składnika..." />
    </div>
    <table class="table table-striped">
        <thead>
            <tr>
                <th>Składnik</th>
                <th>Ilość</th>
                <th>Data dostawy</th>
                <th>Data przydatności</th>
                <th>Akcje</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var entry in filteredStockEntries)
            {
                <tr>
                    <td>@entry.Ingredient.Name</td>
                    <td>@entry.Quantity @entry.Ingredient.Unit</td>
                    <td>@entry.DeliveryDate.ToShortDateString()</td>
                    <td>@entry.ExpiryDate.ToShortDateString()</td>
                    <td>
                        <button class="btn btn-warning btn-sm me-2" 
                                @onclick="() => EditStockEntry(entry)">Edytuj</button>
                        <button class="btn btn-danger btn-sm" 
                                @onclick="() => DeleteStockEntry(entry)">Usuń</button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
else
{
    <p>Brak wpisów magazynowych.</p>
}

@code {
    private List<StockEntry> stockEntries = new();
    private IEnumerable<Ingredient> allIngredients = new List<Ingredient>();
    private StockEntry newEntry = new();
    private bool isStockEntryFormOpen = false;
    private string searchStockTerm = string.Empty;

    protected override void OnInitialized()
    {
        stockEntries = StockEntriesService.GetAllStockEntries().ToList();
        allIngredients = IngredientService.GetAllIngredients();
    }

    private void OpenStockEntryForm()
    {
        newEntry = new StockEntry { DeliveryDate = DateTime.Today, ExpiryDate = DateTime.Today.AddMonths(6) };
        isStockEntryFormOpen = true;
    }

    private void CancelStockEntryForm()
    {
        isStockEntryFormOpen = false;
        newEntry = new StockEntry();
    }

    private void HandleAddStockEntry()
    {
        if (newEntry.Id == 0)
            StockEntriesService.AddStockEntry(newEntry);
        else
            StockEntriesService.UpdateStockEntry(newEntry);

        stockEntries = StockEntriesService.GetAllStockEntries().ToList();
        isStockEntryFormOpen = false;
        newEntry = new StockEntry();
    }

    private void EditStockEntry(StockEntry entry)
    {
        newEntry = StockEntriesService.GetStockEntry(entry.Id);
        isStockEntryFormOpen = true;
    }

    private void DeleteStockEntry(StockEntry entry)
    {
        StockEntriesService.DeleteStockEntry(entry);
        stockEntries = StockEntriesService.GetAllStockEntries().ToList();
    }

    private IEnumerable<StockEntry> filteredStockEntries => 
        stockEntries.Where(e => 
            string.IsNullOrWhiteSpace(searchStockTerm) ||
            e.Ingredient.Name.Contains(searchStockTerm, StringComparison.OrdinalIgnoreCase));
}
```

**Uwagi:**

* Formularz tworzenia/edycji wpisu magazynowego umożliwia wybór składnika z listy (`allIngredients`) oraz ustalenie ilości, daty dostawy i daty przydatności.
* W widoku tabeli wyświetlamy nazwę składnika (`entry.Ingredient.Name`), ilość (razem z jednostką) oraz daty.
* Filtrowanie po częściowym ciągu w nazwie składnika (scrolling w razie długiej listy).

---

### 8.5 Zarządzanie beczkami (Pages/Kegs.razor)

```razor
@page "/kegs"
@using BrewLogix.Models
@inject KegService KegService
@inject BatchService BatchService

<PageTitle>Beczki</PageTitle>

<h3 class="mb-4">Beczki</h3>

<button class="btn btn-primary mb-3" @onclick="() => OpenKegForm()">
    <i class="bi bi-plus-circle me-2"></i> Dodaj beczkę
</button>

@if (isKegFormOpen)
{
    <div class="card mb-4">
        <div class="card-body">
            <h5 class="card-title mb-3">@((newKeg.Id == 0) ? "Nowa beczka" : "Edytuj beczkę")</h5>
            <EditForm Model="@newKeg" OnValidSubmit="HandleAddKeg">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <div class="mb-3">
                    <label class="form-label">Partia</label>
                    <InputSelect class="form-control" @bind-Value="newKeg.BatchId">
                        <option value="">-- wybierz partię --</option>
                        @foreach (var batch in allBatches)
                        {
                            <option value="@batch.Id">@batch.Code</option>
                        }
                    </InputSelect>
                </div>
                <div class="mb-3">
                    <label class="form-label">Pojemność (litry)</label>
                    <InputNumber class="form-control" 
                                 @bind-Value="newKeg.VolumeLiters" 
                                 Min="0.1" Step="0.1"/>
                </div>
                <div class="mb-3 form-check">
                    <InputCheckbox class="form-check-input" @bind-Value="newKeg.IsDistributed" />
                    <label class="form-check-label">Czy dystrybuowana?</label>
                </div>

                <div class="d-flex gap-2">
                    <button type="submit" class="btn btn-primary">
                        @((newKeg.Id == 0) ? "Dodaj beczkę" : "Zapisz zmiany")
                    </button>
                    <button type="button" class="btn btn-secondary" @onclick="CancelKegForm">Anuluj</button>
                </div>
            </EditForm>
        </div>
    </div>
}

@if (filteredKegs.Any())
{
    <div class="mb-3">
        <label class="form-label">Wyszukaj beczki</label>
        <InputText class="form-control" @bind-Value="searchKegTerm" 
                   placeholder="Kod partii lub status..." />
    </div>
    <table class="table table-striped">
        <thead>
            <tr>
                <th>Kod partii</th>
                <th>Pojemność</th>
                <th>Data napełnienia</th>
                <th>Dystrybuowana</th>
                <th>Akcje</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var keg in filteredKegs)
            {
                <tr>
                    <td>@keg.Batch.Code</td>
                    <td>@keg.VolumeLiters L</td>
                    <td>@keg.FilledAt.ToShortDateString()</td>
                    <td>@(keg.IsDistributed ? "Tak" : "Nie")</td>
                    <td>
                        <button class="btn btn-warning btn-sm me-2" 
                                @onclick="() => EditKeg(keg)">Edytuj</button>
                        <button class="btn btn-danger btn-sm" 
                                @onclick="() => DeleteKeg(keg)">Usuń</button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
else
{
    <p>Brak zdefiniowanych beczek.</p>
}

@code {
    private List<Keg> kegs = new();
    private IEnumerable<Batch> allBatches = new List<Batch>();
    private Keg newKeg = new();
    private bool isKegFormOpen = false;
    private string searchKegTerm = string.Empty;

    protected override void OnInitialized()
    {
        kegs = KegService.GetAllKegs().ToList();
        allBatches = BatchService.GetAllBatches();
    }

    private void OpenKegForm()
    {
        newKeg = new Keg
        {
            FilledAt = DateTime.UtcNow,
            IsDistributed = false
        };
        isKegFormOpen = true;
    }

    private void CancelKegForm()
    {
        isKegFormOpen = false;
        newKeg = new Keg();
    }

    private void HandleAddKeg()
    {
        if (newKeg.Id == 0)
            KegService.AddKeg(newKeg);
        else
            KegService.UpdateKeg(newKeg);

        kegs = KegService.GetAllKegs().ToList();
        isKegFormOpen = false;
        newKeg = new Keg();
    }

    private void EditKeg(Keg keg)
    {
        newKeg = KegService.GetKeg(keg.Id);
        isKegFormOpen = true;
    }

    private void DeleteKeg(Keg keg)
    {
        KegService.DeleteKeg(keg);
        kegs = KegService.GetAllKegs().ToList();
    }

    private IEnumerable<Keg> filteredKegs => 
        kegs.Where(k =>
            string.IsNullOrWhiteSpace(searchKegTerm) ||
            k.Batch.Code.Contains(searchKegTerm, StringComparison.OrdinalIgnoreCase) ||
            (k.IsDistributed ? "Tak" : "Nie").Contains(
                searchKegTerm, StringComparison.OrdinalIgnoreCase));
}
```

**Uwagi:**

* Formularz umożliwia wybór `BatchId` z listy `allBatches` (wyświetlamy kod partii), podanie pojemności, ustawienie `FilledAt` (domyślnie `DateTime.UtcNow`) oraz checklistę `IsDistributed`.
* W tabeli wyświetlamy powiązany kod partii (`keg.Batch.Code`), pojemność, datę napełnienia i informację, czy beczka została już dystrybuowana.

---

### 8.6 Zarządzanie zamówieniami (Pages/Orders.razor)

```razor
@page "/orders"
@using BrewLogix.Models
@inject OrderService OrderService
@inject ClientService ClientService
@inject KegService KegService

<PageTitle>Zamówienia</PageTitle>

<h3 class="mb-4">Zamówienia</h3>

<button class="btn btn-primary mb-3" @onclick="() => OpenOrderForm()">
    <i class="bi bi-plus-circle me-2"></i> Nowe zamówienie
</button>

@if (isOrderFormOpen)
{
    <div class="card mb-4">
        <div class="card-body">
            <h5 class="card-title mb-3">@((newOrder.Id == 0) ? "Nowe zamówienie" : "Edytuj zamówienie")</h5>
            <EditForm Model="@newOrder" OnValidSubmit="HandleAddOrder">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <div class="mb-3">
                    <label class="form-label">Klient</label>
                    <InputSelect class="form-control" @bind-Value="newOrder.ClientId">
                        <option value="">-- wybierz klienta --</option>
                        @foreach (var client in allClients)
                        {
                            <option value="@client.Id">@client.Name</option>
                        }
                    </InputSelect>
                </div>

                <div class="mb-3">
                    <label class="form-label">Data zamówienia</label>
                    <InputDate class="form-control" 
                               @bind-Value="newOrder.OrderDate" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Wybór beczek</label>
                    <select multiple class="form-select" 
                            @onchange="OnKegsSelected">
                        @foreach (var keg in unassignedKegs)
                        {
                            <option value="@keg.Id">
                                @keg.Batch.Code – @keg.VolumeLiters L
                            </option>
                        }
                    </select>
                    <small class="form-text text-muted">
                        Przytrzymaj Ctrl, aby wybrać wiele.
                    </small>
                </div>

                <div class="d-flex gap-2">
                    <button type="submit" class="btn btn-primary">
                        @((newOrder.Id == 0) ? "Dodaj zamówienie" : "Zapisz zmiany")
                    </button>
                    <button type="button" class="btn btn-secondary" 
                            @onclick="CancelOrderForm">Anuluj</button>
                </div>
            </EditForm>
        </div>
    </div>
}

@if (filteredOrders.Any())
{
    <div class="mb-3">
        <label class="form-label">Wyszukaj zamówienia</label>
        <InputText class="form-control" @bind-Value="searchOrderTerm" 
                   placeholder="Nazwa klienta lub kod partii..." />
    </div>
    <table class="table table-striped">
        <thead>
            <tr>
                <th>Klient</th>
                <th>Data zamówienia</th>
                <th>Łączna wartość</th>
                <th>Beczki</th>
                <th>Akcje</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var order in filteredOrders)
            {
                <tr>
                    <td>@order.Client.Name</td>
                    <td>@order.OrderDate.ToShortDateString()</td>
                    <td>@order.TotalAmount zł</td>
                    <td>
                        @foreach (var keg in order.Kegs)
                        {
                            <span class="badge bg-secondary me-1">@keg.Batch.Code</span>
                        }
                    </td>
                    <td>
                        <button class="btn btn-warning btn-sm me-2" 
                                @onclick="() => EditOrder(order)">Edytuj</button>
                        <button class="btn btn-danger btn-sm" 
                                @onclick="() => DeleteOrder(order)">Usuń</button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
else
{
    <p>Brak zamówień.</p>
}

@code {
    private List<Order> orders = new();
    private IEnumerable<Client> allClients = new List<Client>();
    private List<Keg> unassignedKegs = new();
    private Order newOrder = new();
    private bool isOrderFormOpen = false;
    private string searchOrderTerm = string.Empty;
    private List<int> selectedKegIds = new();

    protected override void OnInitialized()
    {
        orders = OrderService.GetAllOrders()
                 .ToList();
        allClients = ClientService.GetAllClients();
        unassignedKegs = KegService.GetUnassignedKegs().ToList();
    }

    private void OpenOrderForm()
    {
        newOrder = new Order
        {
            OrderDate = DateTime.Today
        };
        selectedKegIds.Clear();
        isOrderFormOpen = true;
    }

    private void CancelOrderForm()
    {
        isOrderFormOpen = false;
        newOrder = new Order();
        selectedKegIds.Clear();
    }

    private void OnKegsSelected(ChangeEventArgs e)
    {
        selectedKegIds = ((IEnumerable<string>)e.Value)
                         .Select(int.Parse)
                         .ToList();
    }

    private void HandleAddOrder()
    {
        // Przypisz Kegs do zamówienia i oblicz TotalAmount (np. cena stała za litr)
        newOrder.Kegs = unassignedKegs
                        .Where(k => selectedKegIds.Contains(k.Id))
                        .ToList();

        decimal sumAmount = 0;
        foreach (var keg in newOrder.Kegs)
        {
            // Przykładowe obliczenie: 50 zł za litr
            sumAmount += keg.VolumeLiters * 50;
            keg.OrderId = newOrder.Id; // lub w przyszłości przeniesienie to SaveChanges
        }
        newOrder.TotalAmount = sumAmount;

        if (newOrder.Id == 0)
        {
            OrderService.AddOrder(newOrder);
        }
        else
        {
            OrderService.UpdateOrder(newOrder);
        }

        orders = OrderService.GetAllOrders().ToList();
        unassignedKegs = KegService.GetUnassignedKegs().ToList();
        isOrderFormOpen = false;
        newOrder = new Order();
        selectedKegIds.Clear();
    }

    private void EditOrder(Order order)
    {
        newOrder = OrderService.GetOrder(order.Id);
        selectedKegIds = newOrder.Kegs.Select(k => k.Id).ToList();
        isOrderFormOpen = true;
    }

    private void DeleteOrder(Order order)
    {
        OrderService.DeleteOrder(order);
        orders = OrderService.GetAllOrders().ToList();
        unassignedKegs = KegService.GetUnassignedKegs().ToList();
    }

    private IEnumerable<Order> filteredOrders => 
        orders.Where(o =>
            string.IsNullOrWhiteSpace(searchOrderTerm) ||
            o.Client.Name.Contains(searchOrderTerm, StringComparison.OrdinalIgnoreCase) ||
            o.Kegs.Any(k => 
                k.Batch.Code.Contains(searchOrderTerm, StringComparison.OrdinalIgnoreCase))
        );
}
```

**Wyjaśnienie:**

1. **Wybór klienta i beczek**

    * Pole `<InputSelect>` pozwala na wybór `ClientId`.
    * Wiele-do-wielu wybór beczek: kontrolka `<select multiple>` wyświetla wszystkie niezadysponowane beczki (`GetUnassignedKegs()`), a użytkownik wybiera dowolną liczbę (przytrzymując Ctrl).
    * `selectedKegIds` przechowuje wybrane identyfikatory beczek.

2. **Przypisywanie beczek do zamówienia i obliczanie `TotalAmount`**

    * Po zatwierdzeniu formularza w `HandleAddOrder()` filtrujemy `unassignedKegs` według `selectedKegIds` i przypisujemy je do `newOrder.Kegs`.
    * Przykładowo obliczamy cenę 50 zł za każdy litr (można to zastąpić wyższą logiką biznesową).
    * Następnie wywołujemy `OrderService.AddOrder(newOrder)` lub `UpdateOrder(newOrder)`.
    * Odświeżamy listę `orders` oraz `unassignedKegs`.

3. **Filtrowanie zamówień**

    * Pole `searchOrderTerm` pozwala wyszukać zamówienia po nazwie klienta lub kodzie partii przypisanej do którejkolwiek z beczek w zamówieniu.

---

## 9. Walidacja formularzy

### 9.1 Atrybuty DataAnnotations

Najczęściej stosowane w modelach:

* `[Required]`
* `[StringLength(100)]`
* `[Range(0.01, double.MaxValue)]`
* `[EmailAddress]`
* `[Phone]`
* `[DataType(DataType.Date)]`

Dzięki włączeniu `<DataAnnotationsValidator />` w formularzu Blazor, błędy wyświetlane są w `<ValidationSummary />`.

### 9.2 Własne atrybuty walidacyjne

#### 9.2.1 `AtLeastOneIngredientAttribute.cs`

```csharp
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BrewLogix.Validation
{
    public class AtLeastOneIngredientAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var list = value as IList;
            if (list != null && list.Count > 0)
                return ValidationResult.Success;

            return new ValidationResult("Receptura musi zawierać co najmniej jeden składnik.");
        }
    }
}
```

* Stosowany nad właściwością `public List<RecipeIngredient> Ingredients` w klasie `Recipe`.

#### 9.2.2 `UniqueIngredientsAttribute.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BrewLogix.Validation
{
    public class UniqueIngredientsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var ingredients = value as List<RecipeIngredient>;
            if (ingredients != null)
            {
                var duplicateIds = ingredients
                    .Where(i => i.IngredientId != 0)
                    .GroupBy(i => i.IngredientId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicateIds.Any())
                {
                    return new ValidationResult("Lista składników nie może zawierać duplikatów.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
```

* Można stosować nad kolekcją `Ingredients` w `Recipe`, by wymusić unikalność `IngredientId` w ramach jednej receptury.

---

## 10. Konfiguracja i uruchomienie

### 10.1 Wymagania

* **.NET 8.0 SDK**
* **PostgreSQL 14+**
* **EF Core Tools** (`dotnet tool install --global dotnet-ef`)
* (opcjonalnie) `psql` lub dowolny klient do manualnej weryfikacji bazy danych.

### 10.2 Kroki

1. **Skopiuj repozytorium**

   ```bash
   git clone https://github.com/twoje-konto/BrewLogix.git
   cd BrewLogix
   ```

2. **Skonfiguruj connection string**

    * Otwórz `appsettings.json` i upewnij się, że w sekcji `ConnectionStrings` masz poprawne dane do Twojego serwera PostgreSQL:

      ```json
      "DefaultConnection": "Host=localhost;Database=brewlogix;Username=postgres;Password=twoje_haslo;"
      ```

3. **Zainstaluj pakiety NuGet**

   ```bash
   dotnet restore
   ```

4. **Utwórz/migracje do bazy**

    * Dodaj migrację (jeśli nie ma jeszcze):

      ```bash
      dotnet ef migrations add InitialCreate
      ```
    * Zastosuj migracje w bazie:

      ```bash
      dotnet ef database update
      ```
    * Możesz też włączyć automatyczne migracje przy starcie aplikacji, wywołując `DatabaseInitializer.Initialize(context)` w `Program.cs`.

5. **(Opcjonalnie) Seedowanie przykładowych danych**

    * W `Program.cs` (przed `app.Run()`) dodaj:

      ```csharp
      using (var scope = app.Services.CreateScope())
      {
          var context = scope.ServiceProvider.GetRequiredService<DbContextProvider>().GetDbContext();
          DatabaseInitializer.Initialize(context);
      }
      ```

6. **Uruchom aplikację**

   ```bash
   dotnet run
   ```

    * Aplikacja dostępna będzie pod adresem **[https://localhost:5001](https://localhost:5001)** (lub inny port wygenerowany dynamicznie).

---

## 11. Instrukcja obsługi

Poniżej zebrano typowe scenariusze użytkowe oraz opisy użycia poszczególnych modułów w BrewLogix.

### 11.1 Strona główna (Dashboard)

1. **Logowanie**
   – w tej wersji nie ma uwierzytelniania; aplikacja domyślnie od razu przechodzi do panelu. W przyszłości można dodać Identity.
2. **Przegląd statystyk**
   – liczba klientów, liczba aktywnych partii, ilość surowców poniżej progu minimalnego (rozszerzenie), liczba nieskończonych zamówień.

### 11.2 Zarządzanie klientami (Pages/Clients.razor)

1. **Dodawanie klienta**

    * Przycisk „Dodaj klienta” otwiera formularz.
    * Wypełnij pola `Name`, `Email`, `Address`, opcjonalnie `PhoneNumber`.
    * Kliknij „Dodaj klienta”.
    * Klient pojawi się na liście.

2. **Edycja klienta**

    * Kliknij „Edytuj” przy wybranym kliencie.
    * Zmienione dane zostaną zapisane po kliknięciu „Zapisz zmiany”.

3. **Usuwanie klienta**

    * Kliknij „Usuń”.
    * Jeśli klient ma przypisane zamówienia, można dodać zabezpieczenie, aby nie usuwać klienta z aktywnymi zamówieniami.

### 11.3 Zarządzanie składnikami (Pages/Ingredients.razor)

1. **Dodawanie składnika**

    * Przycisk „Dodaj składnik” otwiera formularz z polami `Name`, `Type`, `Unit`.
    * Kliknij „Dodaj składnik”.
    * Nowy składnik pojawi się na liście.

2. **Edycja składnika**

    * Kliknij „Edytuj” przy wybranym składniku.
    * Po zmianie danych kliknij „Zapisz zmiany”.

3. **Usuwanie składnika**

    * Kliknij „Usuń”.
    * Jeżeli składnik jest używany w jakiejś recepturze lub w `StockEntries`, należy dodać zabezpieczenie w `IngredientService.DeleteIngredient`, aby nie usunąć składnika wykorzystywanego.

### 11.4 Zarządzanie wpisami magazynowymi (Pages/StockEntries.razor)

1. **Dodawanie wpisu**

    * Przycisk „Dodaj wpis” otwiera formularz.
    * Wybierz składnik z listy, uzupełnij `Quantity`, `DeliveryDate`, `ExpiryDate`.
    * Kliknij „Dodaj wpis”.
    * Wpis zostanie zapisany, a stan magazynowy będzie widoczny w `Recipe` → „Dodaj partię” (jeśli składnik występuje w recepturze).

2. **Edycja wpisu**

    * Kliknij „Edytuj” przy wybranym wpisie.
    * Zmodyfikuj pola i kliknij „Zapisz zmiany”.

3. **Usuwanie wpisu**

    * Kliknij „Usuń”.
    * Jeżeli wpis ma zarezerwowaną ilość w aktywnej partii, należy dodać walidację w `StockEntriesService.DeleteStockEntry`, aby nie usuwać wpisu w użyciu.

### 11.5 Zarządzanie recepturami (Pages/Recipes.razor)

1. **Dodawanie receptury**

    * Przycisk „Dodaj recepturę” otwiera formularz.
    * Wypełnij `Name`, `Style`, `Description`.
    * Kliknij „Dodaj składnik” tyle razy, ile jest potrzebnych składników.
    * Dla każdego wpisu wybierz `IngredientId` z listy i podaj `Quantity`.
    * Dopilnuj, by lista `Ingredients` nie zawierała duplikatów (walidowane w `HandleAddRecipe`).
    * Kliknij „Dodaj recepturę”.

2. **Edycja receptury**

    * Kliknij „Edytuj” przy wybranej recepturze.
    * Zmodyfikuj dane lub składniki.
    * Kliknij „Zapisz zmiany”.

3. **Usuwanie receptury**

    * Kliknij „Usuń”.
    * Jeśli receptura jest używana w aktywnych partiach, dodaj zabezpieczenie w `RecipeService.DeleteRecipe`.

### 11.6 Zarządzanie partiami (Pages/Batches.razor)

1. **Dodawanie partii**

    * Przycisk „Dodaj nową partię” otwiera formularz:

        * Podaj `Code` (unikalny kod partii).
        * Wybierz `Recipe` z listy.
        * Po wybraniu receptury zobaczysz listę składników i dostępne stany magazynowe (zbliżając się do terminu ważności).
        * Dla każdego składnika musisz wybrać konkretny `StockEntryId` (dostępne wpisy).
        * Podaj `StartDate` i (opcjonalnie) `EndDate`.
        * Podaj `Status` (domyślnie „Planowana”).
        * Kliknij „Dodaj partię”.

    * **Walidacja w `HandleAddBatch`:**

        * Sprawdza, czy dla każdego `RecipeIngredient` zostało wybrane `SelectedStockEntryId`.
        * Sprawdza, czy `StockEntry.Quantity >= RecipeIngredient.Quantity`.
        * Jeśli wszystko w porządku, pomniejsza stany magazynowe (`StockEntriesService.UpdateStockEntry`).
        * Na końcu zapisuje `BatchService.AddBatch(newBatch)`.

2. **Edycja partii**

    * Kliknij „Edytuj” przy wybranej partii.
    * Formularz wczyta `Code`, `RecipeId`, `StartDate`, `EndDate`, `Status`, ale nie pozwoli na zmianę `RecipeId` (pokazuje tylko podsumowanie składników).
    * Kliknij „Zapisz zmiany”.

3. **Usuwanie partii**

    * Kliknij „Usuń”.
    * Metoda `DeleteBatch` sprawdza, czy partia nie jest przypisana do żadnej beczki (`KegService.IsBatchAssignedToAnyKeg`).
    * Jeśli jest przypisana, wyświetla komunikat „Nie można usunąć partii …”.

4. **Filtrowanie listy**

    * Pole wyszukiwania umożliwia filtrowanie po kodzie partii, statusie lub nazwie receptury.

### 11.7 Zarządzanie beczkami (Pages/Kegs.razor)

1. **Dodawanie beczki**

    * Kliknij „Dodaj beczkę”.
    * Wybierz `BatchId` (kod partii).
    * Podaj `VolumeLiters`.
    * Zaznacz opcjonalnie `IsDistributed`.
    * Kliknij „Dodaj beczkę”.

2. **Edycja beczki**

    * Kliknij „Edytuj”.
    * Zmodyfikuj `BatchId`, `VolumeLiters`, `IsDistributed`.
    * Kliknij „Zapisz zmiany”.

3. **Usuwanie beczki**

    * Kliknij „Usuń”.

4. **Filtrowanie**

    * Pole wyszukiwania pozwala odnaleźć beczki po kodzie partii lub statusie „Tak/Nie” (dystrybuowana).

### 11.8 Zarządzanie zamówieniami (Pages/Orders.razor)

1. **Dodawanie zamówienia**

    * Kliknij „Nowe zamówienie”.
    * Wybierz `ClientId`.
    * Ustaw `OrderDate`.
    * Wybierz z listy `unassignedKegs` (wszystkie beczki, które mają `OrderId == null`).
      – Kontrolka `<select multiple>` pozwala wybrać dowolną liczbę beczek.
    * Kliknij „Dodaj zamówienie”.
    * `HandleAddOrder()` obliczy `TotalAmount` (na podstawie `VolumeLiters ⋅ cena_za_litr`).

2. **Edycja zamówienia**

    * Kliknij „Edytuj”.
    * Zmodyfikuj listę beczek (dodanie/usunięcie), `ClientId`, `OrderDate`.
    * Kliknij „Zapisz zmiany”.

3. **Usuwanie zamówienia**

    * Kliknij „Usuń”.

4. **Filtrowanie**

    * Pole wyszukiwania pozwala filtrować zamówienia po nazwie klienta lub kodzie partii dowolnej z przypisanych beczek.

---

## 12. Architektura aplikacji

```
📁 BrewLogix
 ├───Components/       ← Komponenty wielokrotnego użytku (np. NavMenu, DashboardCard)
 ├───Pages/            ← Strony Blazor (razor) odpowiadające za poszczególne moduły
 ├───Models/           ← Encje EF Core (dziedziczą po BaseEntity)
 ├───Services/         ← Warstwa logiki biznesowej (CRUD, walidacje, operacje między-encjami)
 │       ├ DbContextProvider.cs
 │       ├ AppDbContext.cs
 │       ├ ClientService.cs
 │       ├ IngredientService.cs
 │       ├ StockEntriesService.cs
 │       ├ RecipeService.cs
 │       ├ BatchService.cs
 │       ├ KegService.cs
 │       └ OrderService.cs
 ├───Seeders/          ← Dodawanie przykładowych danych przy starcie aplikacji
 ├───Validation/       ← Atrybuty walidacyjne (AtLeastOneIngredient, UniqueIngredients)
 ├───Data/Migrations/  ← Pliki generowane przez `dotnet ef migrations`
 ├───Program.cs        ← Konfiguracja DI, EF Core, Blazor Server, pipeline HTTP
 └───appsettings.json  ← Connection strings i ustawienia aplikacji
```

* **Komponenty** (`Components/`) pozwalają na wydzielanie powtarzalnych fragmentów UI (karty, formularze modalne, komunikaty).
* **Pages/** to konkretne strony, dostępne pod adresami routingu (np. `/batches`, `/recipes`, `/orders`).
* **Serwisy** separują logikę CRUD od komponentów Blazor – ułatwia to testowanie (można podmienić serwis podczas testów).
* **Seedery** inicjują bazę przykładowymi danymi, co przyśpiesza testowanie i prezentację projektu.
* **Validation/** to własne atrybuty, które wzbogacają walidację nad DataAnnotations.
* **Program.cs** – punkt wejściowy, rejestracja serwisów w kontenerze, dodanie Blazor Server, migracje, seedowanie.

---

## 13. Podsumowanie i sugestie produkcyjne

**BrewLogix** to gotowy do wdrożenia system ERP dla browarów, który:

* Umożliwia pełne zarządzanie procesem produkcyjnym – od przyjęcia surowców, przez receptury, partie, beczki, aż po sprzedaż.
* Działa w przeglądarce dzięki Blazor Server, zapewniając szybkość i bezpieczeństwo (logika po stronie serwera).
* Korzysta z solidnego ORM (EF Core) i sprawdzonego silnika bazodanowego (PostgreSQL).
* Pozwala na łatwe rozszerzenie, np. dodanie uwierzytelniania (ASP.NET Core Identity), generowanie raportów (Excel, PDF), API (kontrolery WebAPI) do komunikacji z systemami zewnętrznymi.

> **Końcowa myśl:**
> BrewLogix stanowi kompletną bazę do dalszego rozwoju w kierunku pełnoprawnego, skalowalnego ERP dla branży browarniczej. Dzięki modularnej budowie (z podziałem na serwisy, komponenty, walidacje) oraz wykorzystaniu najnowszych technologii .NET i Blazor, aplikacja może rosnąć wraz z potrzebami użytkowników – od małego lokalnego browaru aż po większe sieci dystrybucyjne.
