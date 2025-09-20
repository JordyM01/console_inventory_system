// FilePath: InventoryManager.cs
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Capa de lógica de negocio.
/// Gestiona todas las operaciones de los productos (CRUD) y la persistencia de datos en un archivo binario.
/// No tiene conocimiento de la interfaz de usuario.
/// </summary>
public class InventoryManager
{
    private readonly string _filePath;
    private List<Product> _products;

    public InventoryManager(string filePath)
    {
        _filePath = filePath;
        _products = new List<Product>();
        LoadFromFile();
    }

    /// <summary>
    /// Proporciona acceso de solo lectura a la lista de productos para las vistas.
    /// </summary>
    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    /// <summary>
    /// Filtra la lista de productos por Nombre o SKU (sin distinguir mayúsculas/minúsculas).
    /// </summary>
    public IEnumerable<Product> SearchProducts(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return _products;
        return _products.Where(p =>
            p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            p.Sku.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Agrega un nuevo producto a la lista y guarda en el archivo.
    /// </summary>
    public void AddProduct(Product product)
    {
        _products.Add(product);
        SaveToFile();
    }

    /// <summary>
    /// Actualiza la cantidad de un producto específico y guarda en el archivo.
    /// </summary>
    public void UpdateProductQuantity(string productId, int change)
    {
        var productIndex = _products.FindIndex(p => p.Id == productId);
        if (productIndex != -1)
        {
            var product = _products[productIndex];
            int newQuantity = product.Quantity + change;
            _products[productIndex] = product with { Quantity = Math.Max(0, newQuantity) };
            SaveToFile();
        }
    }

    /// <summary>
    /// Elimina un producto por su ID y guarda en el archivo.
    /// </summary>
    public void DeleteProduct(string productId)
    {
        _products.RemoveAll(p => p.Id == productId);
        SaveToFile();
    }

    /// <summary>
    /// Carga la lista de productos desde un archivo binario.
    /// </summary>
    private void LoadFromFile()
    {
        if (!File.Exists(_filePath)) return;
        try
        {
            using var reader = new BinaryReader(File.Open(_filePath, FileMode.Open));
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                _products.Add(new Product(
                    Id: reader.ReadString(), Sku: reader.ReadString(), Name: reader.ReadString(),
                    Quantity: reader.ReadInt32(), Category: reader.ReadString(), MinQuantity: reader.ReadInt32(),
                    Description: reader.ReadString(), Price: reader.ReadDecimal()
                ));
            }
        }
        catch (IOException e) { Console.WriteLine($"Error al cargar el inventario: {e.Message}"); }
    }

    /// <summary>
    /// Guarda la lista completa de productos en un archivo binario.
    /// </summary>
    private void SaveToFile()
    {
        try
        {
            using var writer = new BinaryWriter(File.Open(_filePath, FileMode.Create));
            writer.Write(_products.Count);
            foreach (var p in _products)
            {
                writer.Write(p.Id); writer.Write(p.Sku); writer.Write(p.Name);
                writer.Write(p.Quantity); writer.Write(p.Category); writer.Write(p.MinQuantity);
                writer.Write(p.Description); writer.Write(p.Price);
            }
        }
        catch (IOException e) { Console.WriteLine($"Error al guardar el inventario: {e.Message}"); }
    }
}

