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

        // Ввод цены
        decimal price;
        Console.Write("Введите цену за единицу объема (например, за м³): ");
        while (!decimal.TryParse(Console.ReadLine(), out price) || price < 0)
        {
            Console.Write("Цена должна быть неотрицательным числом. Повторите: ");
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

        // Выбор типа груза
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

        // Ввод объема
        double volume;
        Console.Write("Введите объем груза (в м³): ");
        while (!double.TryParse(Console.ReadLine(), out volume) || volume <= 0)
        {
            Console.Write("Объем должен быть положительным числом. Повторите: ");
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