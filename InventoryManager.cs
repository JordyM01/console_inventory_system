using System.IO;
using System.Collections.Generic;
using System.Linq;

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

    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    // --- NUEVO MÉTODO DE BÚSQUEDA ---
    /// <summary>
    /// Filtra la lista de productos basada en un término de búsqueda.
    /// Busca coincidencias en el Nombre y SKU del producto.
    /// </summary>
    /// <param name="searchTerm">El texto a buscar.</param>
    /// <returns>Una lista de productos que coinciden con el término.</returns>
    public IEnumerable<Product> SearchProducts(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return _products; // Si la búsqueda está vacía, devuelve todos los productos
        }

        // Búsqueda case-insensitive
        return _products.Where(p =>
            p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            p.Sku.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        );
    }

    public void AddProduct(Product product)
    {
        _products.Add(product);
        SaveToFile();
    }

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

    public void DeleteProduct(string productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        if (product != null)
        {
            _products.Remove(product);
            SaveToFile();
        }
    }

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
        catch (IOException e) { Console.WriteLine($"Error al cargar: {e.Message}"); }
    }

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
        catch (IOException e) { Console.WriteLine($"Error al guardar: {e.Message}"); }
    }
}




