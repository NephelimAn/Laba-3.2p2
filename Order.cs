public class Order
{
    public Client Client { get; private set; }
    public Tariff Tariff { get; private set; }
    public double Volume { get; private set; }

    public decimal TotalCost => (decimal)Volume * Tariff.PricePerUnit;

    public Order(Client client, Tariff tariff, double volume)
    {
        Client = client;
        Tariff = tariff;
        Volume = volume;
    }

    ~Order()
    {
        // Деструктор
        Console.WriteLine($"Заказ для клиента '{Client.Name}' удалён из памяти.");
    }

    public override string ToString()
    {
        return $"Заказ: {Client.Name}, Тип груза: {Tariff.Type}, Объем: {Volume} м³, Стоимость: {TotalCost:C}";
    }
}