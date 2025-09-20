/// <summary>
/// Define la estructura de datos para un producto.
/// Se usa un 'record' para obtener inmutabilidad y comparación por valor de forma sencilla.
/// </summary>
public record Product(
    string Id,
    string Sku,
    string Name,
    int Quantity,
    string Category,
    int MinQuantity,
    string Description,
    decimal Price
);

