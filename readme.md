# Dokumentacja systemu ERP dla browarni

## Spis treści

1. [Przegląd](#przegląd)
2. [Połączenie z bazą danych](#połączenie-z-bazą-danych)
3. [Modele](#modele)
   - Batch
   - Client
   - Ingredient
   - Keg
   - Order
   - Recipe
   - RecipeIngredient
   - StockEntry
4. [Tworzenie bazy danych](#tworzenie-bazy-danych)
5. [Operacje CRUD](#operacje-crud)
6. [Walidacja formularzy](#walidacja-formularzy)
7. [Konfiguracja i uruchomienie](#konfiguracja-i-uruchomienie)
8. [Instrukcja obsługi](#instrukcja-obsługi)

---

## Przegląd

System **Brewery ERP** jest zaprojektowany, aby pomóc browarniom zarządzać swoimi operacjami w sposób efektywny, w tym obsługiwać zamówienia, beczki, receptury, partie produkcyjne oraz zarządzanie stanem magazynowym. System wykorzystuje bazę danych relacyjną z zoptymalizowanymi modelami, które przechowują i przetwarzają dane niezbędne do procesu warzenia piwa.

## Połączenie z bazą danych

### Opcje:



 **Konfiguracja w pliku z możliwością zmiany połączenia w czasie działania:**
   - Connection string jest zapisany w pliku `appsettings.json` i może być zmieniany dynamicznie podczas działania aplikacji.

---

## Modele

System korzysta z poniższych modeli, które definiują strukturę danych:

### 1. **Batch (Partia)**

```csharp
public class Batch : BaseEntity
{
    [Required]
    public string Code { get; set; }

    [Required]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string Status { get; set; }

    public ICollection<Keg> Kegs { get; set; }
    public ICollection<StockEntry> StockEntries { get; set; }
}
```

### 2. **Client (Klient)**

```csharp
public class Client : BaseEntity
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; }

    [Required]
    public string Address { get; set; }

    public ICollection<Order> Orders { get; set; }
}
```

### 3. **Ingredient (Składnik)**

```csharp
public class Ingredient : BaseEntity
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Unit { get; set; }

    public ICollection<RecipeIngredient> UsedInRecipes { get; set; }
    public ICollection<StockEntry> StockEntries { get; set; }
}
```

### 4. **Keg (Beczka)**

```csharp
public class Keg : BaseEntity, IDistributable
{
    [Required]
    public string Code { get; set; }

    [Required]
    public int BatchId { get; set; }
    public Batch Batch { get; set; }

    [Required]
    public string Size { get; set; }

    [Required]
    public DateTime FilledAt { get; set; }

    public int? OrderId { get; set; }
    public Order? Order { get; set; }
}
```

### 5. **Order (Zamówienie)**

```csharp
public class Order : BaseEntity
{
    [Required]
    public int ClientId { get; set; }
    public Client Client { get; set; }

    [Required]
    public DateTime OrderedAt { get; set; }

    [Required]
    public string Status { get; set; }

    public ICollection<Keg> Kegs { get; set; }
}
```

### 6. **Recipe (Receptura)**

```csharp
public class Recipe : BaseEntity
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Style { get; set; }

    public string? Description { get; set; }

    [AtLeastOneIngredient]
    public ICollection<RecipeIngredient> Ingredients { get; set; }

    public ICollection<Batch> Batches { get; set; }
}
```

### 7. **RecipeIngredient (Składnik w recepturze)**

```csharp
public class RecipeIngredient : BaseEntity
{
    [Required]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }

    [Required]
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }

    [Required]
    public decimal Quantity { get; set; }

    [Required]
    public int SelectedStockEntryId { get; set; }
}
```

### 8. **StockEntry (Wpis magazynowy)**

```csharp
public class StockEntry : BaseEntity
{
    [Required]
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }

    [Required]
    public decimal Quantity { get; set; }

    [Required]
    public DateTime DeliveryDate { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }
}
```

---

## Tworzenie bazy danych

1. **Proces tworzenia bazy danych:**

   - Użyj migracji do wygenerowania schematu bazy danych.
   - Wykonaj `dotnet ef migrations add <MigrationName>`, aby utworzyć pliki migracji.
   - Zastosuj migrację za pomocą `dotnet ef database update`, aby utworzyć strukturę bazy danych.

2. **Seedowanie danych:**

   - Seedery sa używane do automatycznego wypełniania bazy danych danymi testowymi.

---

## Operacje CRUD

Aplikacja zapewnia operacje CRUD, które umożliwiają interakcję z bazą danych. Można:

- **Tworzyć, odczytywać, aktualizować i usuwać** encje takie jak **Zamówienia**, **Beczki**, **Receptury** itd.
- Interfejs umożliwia łatwe filtrowanie rekordów na podstawie określonych kryteriów.

---

## Walidacja formularzy

### Podstawowa walidacja:

- Pola takie jak **Nazwa**, **Email**, **Ilość**, **Data** są wymagane, a w przypadku ich braku lub nieprawidłowych danych wyświetlane są komunikaty o błędach.

### Zaawansowana walidacja:

- Wyrażenia regularne do walidacji **Email**.
- Walidacja zależna od kontekstu, np. różne zasady walidacji dla różnych krajów lub regionów.
- **Asynchroniczna walidacja**, np. sprawdzanie, czy e-mail istnieje już w bazie danych.

### Typy pól formularzy:

- Odpowiednie pola formularzy, takie jak **Data**, **Listy rozwijane* i **Pola tekstowe** są używane w zależności od typu danych.

---

## Konfiguracja i uruchomienie

1. **Konfiguracja połączenia z bazą danych:**

   - Połączenie może być skonfigurowane w pliku `appsettings.json`, a zmiany mogą być dokonywane dynamicznie podczas działania aplikacji lub być zapisane na stałe w kodzie.

2. **Uruchomienie aplikacji:**
   - Zainstaluj zależności: `dotnet restore`
   - Skonfiguruj bazę danych PostgreSQL i uruchom migracje.
   - Uruchom aplikację: `dotnet run`

---

## Instrukcja obsługi

### Instalacja:

1. Pobierz lub sklonuj repozytorium.
2. Upewnij się, że **.NET Core** jest zainstalowane na twoim komputerze.
3. Skonfiguruj serwer PostgreSQL 14.0.

### Konfiguracja:

1. Skonfiguruj połączenie z bazą danych w pliku `appsettings.json`.
2. Uruchom migracje, aby utworzyć odpowiednią strukturę bazy danych i dodać dane testowe.

### Korzystanie z aplikacji:

1. **Przejdź do strony głównej**, aby wybrać moduł, którym chcesz zarządzać, np. **Zamówienia**, **Beczki**, **Receptury** i **Partie**.
2. Korzystaj z filtrów i funkcji wyszukiwania, aby znaleźć konkretne rekordy.
3. Wykonuj operacje CRUD przy użyciu formularzy do tworzenia, edytowania lub usuwania wpisów.

### Scenariusze brzegowe:

- **Błędy walidacji:** Wszystkie formularze wyświetlają komunikaty o błędach dla nieprawidłowych pól.

---

### Podsumowanie

System ERP dla browarni jest zaprojektowany do efektywnego zarządzania procesem warzenia piwa, od surowców (składników) po gotowe produkty (beczki), a także do obsługi zamówień i zarządzania klientami. System wykorzystuje solidną bazę danych z dobrze zdefiniowanymi modelami, regułami walidacji i zoptymalizowanymi operacjami CRUD, zapewniając tym samym lepsze doświadczenie w zarządzaniu browarniczymi operacjami.
