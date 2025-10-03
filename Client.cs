public class Client
{
    public string Name { get; private set; }
    public string Phone { get; private set; }

    public Client(string name, string phone)
    {
        Name = name;
        Phone = phone;
    }

    ~Client()
    {
        // Деструктор
        Console.WriteLine($"Клиент '{Name}' удалён из памяти.");
    }

    public override string ToString()
    {
        return $"Клиент: {Name}, Телефон: {Phone}";
    }
}