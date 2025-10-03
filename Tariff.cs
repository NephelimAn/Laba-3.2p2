public class Tariff
{
    public CargoType Type { get; private set; }
    public decimal PricePerUnit { get; private set; }

    public Tariff(CargoType type, decimal pricePerUnit)
    {
        Type = type;
        PricePerUnit = pricePerUnit;
    }

    public override string ToString()
    {
        return $"Тип: {Type}, Цена за единицу: {PricePerUnit:C}";
    }
}