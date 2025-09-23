using System.Text.Json.Serialization; // Necesario para los atributos de mapeo de JSON

/// <summary>
/// Define la estructura de datos para un producto.
/// Se usan atributos [JsonPropertyName] para mapear los nombres del archivo JSON
/// a las propiedades de la clase en C#, permitiendo que sean diferentes.
/// </summary>
public record Product(
    // El atributo [JsonPropertyName] le dice al serializador:
    [property: JsonPropertyName("ID")] string Id,
    [property: JsonPropertyName("SKU")] string Sku,
    [property: JsonPropertyName("Producto")] string Name,
    [property: JsonPropertyName("cantidad")] int Quantity,
    [property: JsonPropertyName("categoria")] string Category,
    [property: JsonPropertyName("cant_minima")] int MinQuantity,
    [property: JsonPropertyName("descripcion")] string Description,
    [property: JsonPropertyName("precio")] decimal Price
);
