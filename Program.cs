using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static TransportCompany company = new TransportCompany();

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Фирма грузоперевозок ===");
            Console.WriteLine("1. Ввести тариф");
            Console.WriteLine("2. Зарегистрировать клиента");
            Console.WriteLine("3. Создать заказ");
            Console.WriteLine("4. Вывести сумму заказа для клиента");
            Console.WriteLine("5. Подсчитать общую стоимость всех заказов");
            Console.WriteLine("6. Показать все тарифы");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    InputTariff();
                    break;
                case "2":
                    RegisterClient();
                    break;
                case "3":
                    CreateOrder();
                    break;
                case "4":
                    ShowClientTotal();
                    break;
                case "5":
                    ShowTotalRevenue();
                    break;
                case "6":
                    ShowTariffs();
                    break;
                case "0":
                    Console.WriteLine("Выход...");
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                    Console.ReadKey();
                    break;
            }
        }
    }

   static void InputTariff()
{
    Console.Clear();
    Console.WriteLine("=== Ввод тарифа ===");

    // Выбор типа груза
    Console.WriteLine("Выберите тип груза:");
    var types = Enum.GetValues(typeof(CargoType)).Cast<CargoType>().ToList();
    for (int i = 0; i < types.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {types[i]}");
    }

    int typeIndex;
    while (!int.TryParse(Console.ReadLine(), out typeIndex) || typeIndex < 1 || typeIndex > types.Count)
    {
        Console.Write("Неверный ввод. Повторите: ");
    }
    CargoType selectedType = types[typeIndex - 1];

    // Ввод цены с ограничениями
    const decimal MIN_PRICE = 0.01m;
    const decimal MAX_PRICE = 1_000_000m; // 1 миллион — разумный максимум

    decimal price;
    Console.Write($"Введите цену за единицу объема (от {MIN_PRICE} до {MAX_PRICE:C}): ");
    while (true)
    {
        string input = Console.ReadLine();
        if (!decimal.TryParse(input, out price))
        {
            Console.Write("Неверный формат числа. Повторите: ");
            continue;
        }

        if (price < MIN_PRICE)
        {
            Console.Write($"Цена не может быть меньше {MIN_PRICE:C}. Повторите: ");
            continue;
        }

        if (price > MAX_PRICE)
        {
            Console.Write($"Цена не может превышать {MAX_PRICE:C}. Повторите: ");
            continue;
        }

        break;
    }

    company.AddTariff(new Tariff(selectedType, price));
    Console.WriteLine("Тариф добавлен успешно!");
    Console.ReadKey();
}

    static void RegisterClient()
    {
        Console.Clear();
        Console.WriteLine("=== Регистрация клиента ===");
        Console.Write("Введите имя клиента: ");
        string name = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(name))
        {
            Console.Write("Имя не может быть пустым. Повторите: ");
            name = Console.ReadLine();
        }

        Console.Write("Введите телефон клиента: ");
        string phone = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(phone))
        {
            Console.Write("Телефон не может быть пустым. Повторите: ");
            phone = Console.ReadLine();
        }

        company.RegisterClient(new Client(name, phone));
        Console.WriteLine("Клиент зарегистрирован!");
        Console.ReadKey();
    }

    static void CreateOrder()
{
    Console.Clear();
    Console.WriteLine("=== Создание заказа ===");

    var clients = company.GetClients();
    if (clients.Count == 0)
    {
        Console.WriteLine("Нет зарегистрированных клиентов!");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("Выберите клиента:");
    for (int i = 0; i < clients.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {clients[i].Name} ({clients[i].Phone})");
    }

    int clientIndex;
    while (!int.TryParse(Console.ReadLine(), out clientIndex) || clientIndex < 1 || clientIndex > clients.Count)
    {
        Console.Write("Неверный выбор. Повторите: ");
    }
    Client selectedClient = clients[clientIndex - 1];

    var tariffs = company.GetTariffs();
    if (tariffs.Count == 0)
    {
        Console.WriteLine("Нет доступных тарифов! Сначала добавьте тариф.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("Выберите тип груза:");
    var types = tariffs.Select(t => t.Type).Distinct().ToList();
    for (int i = 0; i < types.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {types[i]}");
    }

    int typeIndex;
    while (!int.TryParse(Console.ReadLine(), out typeIndex) || typeIndex < 1 || typeIndex > types.Count)
    {
        Console.Write("Неверный выбор. Повторите: ");
    }
    CargoType selectedType = types[typeIndex - 1];

    // Ввод объёма с ограничениями
    const double MIN_VOLUME = 0.01;
    const double MAX_VOLUME = 10_000.0; // 10 000 м³ — максимум (например, для железнодорожного состава)

    double volume;
    Console.Write($"Введите объем груза в м³ (от {MIN_VOLUME} до {MAX_VOLUME}): ");
    while (true)
    {
        string input = Console.ReadLine();
        if (!double.TryParse(input, out volume))
        {
            Console.Write("Неверный формат числа. Повторите: ");
            continue;
        }

        if (volume < MIN_VOLUME)
        {
            Console.Write($"Объём не может быть меньше {MIN_VOLUME} м³. Повторите: ");
            continue;
        }

        if (volume > MAX_VOLUME)
        {
            Console.Write($"Объём не может превышать {MAX_VOLUME} м³. Повторите: ");
            continue;
        }

        // Дополнительная проверка: не приведёт ли объём * цена к переполнению decimal?
        var tariff = tariffs.First(t => t.Type == selectedType);
        if (volume > (double)(decimal.MaxValue / tariff.PricePerUnit))
        {
            Console.Write($"Слишком большой объём: итоговая стоимость превысит допустимый предел. Макс. объём для этого тарифа: {(double)(decimal.MaxValue / tariff.PricePerUnit):F2} м³. Повторите: ");
            continue;
        }

        break;
    }

    try
    {
        company.CreateOrder(selectedClient, selectedType, volume);
        Console.WriteLine("Заказ создан успешно!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
    Console.ReadKey();
}

    static void ShowClientTotal()
    {
        Console.Clear();
        Console.WriteLine("=== Сумма заказов клиента ===");

        var clients = company.GetClients();
        if (clients.Count == 0)
        {
            Console.WriteLine("Нет клиентов.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Выберите клиента:");
        for (int i = 0; i < clients.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {clients[i].Name}");
        }

        int clientIndex;
        while (!int.TryParse(Console.ReadLine(), out clientIndex) || clientIndex < 1 || clientIndex > clients.Count)
        {
            Console.Write("Неверный выбор. Повторите: ");
        }
        string clientName = clients[clientIndex - 1].Name;

        var orders = company.GetOrdersByClient(clientName);
        if (orders.Count == 0)
        {
            Console.WriteLine($"У клиента '{clientName}' нет заказов.");
        }
        else
        {
            decimal total = orders.Sum(o => o.TotalCost);
            Console.WriteLine($"Общая сумма заказов для '{clientName}': {total:C}");
            Console.WriteLine("\nДетали:");
            foreach (var order in orders)
            {
                Console.WriteLine($" - {order}");
            }
        }
        Console.ReadKey();
    }

    static void ShowTotalRevenue()
    {
        Console.Clear();
        Console.WriteLine("=== Общая выручка ===");
        decimal total = company.GetTotalRevenue();
        Console.WriteLine($"Общая стоимость всех заказов: {total:C}");
        Console.ReadKey();
    }

    static void ShowTariffs()
    {
        Console.Clear();
        Console.WriteLine("=== Список тарифов ===");
        var tariffs = company.GetTariffs();
        if (tariffs.Count == 0)
        {
            Console.WriteLine("Нет тарифов.");
        }
        else
        {
            foreach (var tariff in tariffs)
            {
                Console.WriteLine(tariff);
            }
        }
        Console.ReadKey();
    }
}
