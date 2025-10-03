public class TransportCompany
{
    private List<Tariff> tariffs = new List<Tariff>();
    private List<Client> clients = new List<Client>();
    private List<Order> orders = new List<Order>();

    public void AddTariff(Tariff tariff)
    {
        tariffs.Add(tariff);
    }

    
    public void RegisterClient(Client client)
    {
        clients.Add(client);
    }

    
    public void CreateOrder(Client client, CargoType cargoType, double volume)
    {
        var tariff = tariffs.FirstOrDefault(t => t.Type == cargoType);
        if (tariff == null)
            throw new InvalidOperationException($"Тариф для типа груза '{cargoType}' не найден.");

        var order = new Order(client, tariff, volume);
        orders.Add(order);
    }

    
    public List<Order> GetOrdersByClient(string clientName)
    {
        return orders.Where(o => o.Client.Name == clientName).ToList();
    }

   
    public decimal GetTotalRevenue()
    {
        return orders.Sum(o => o.TotalCost);
    }

    
    public List<Tariff> GetTariffs() => new List<Tariff>(tariffs);

    
    public List<Client> GetClients() => new List<Client>(clients);
}