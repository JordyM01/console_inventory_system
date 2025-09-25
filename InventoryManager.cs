using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

/// <summary>
/// Capa de lógica de negocio.
/// Gestiona todas las operaciones de los productos (CRUD) y la persistencia de datos.
/// No debe contener ninguna referencia a la lógica de la interfaz de usuario (UI).
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

    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    public IEnumerable<Product> SearchProducts(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return _products;
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

    /// <summary>
    /// Actualiza un producto existente en el inventario.
    /// </summary>
    /// <param name="updatedProduct">El producto con los datos modificados.</param>
    public void UpdateProduct(Product updatedProduct)
    {
        var productIndex = _products.FindIndex(p => p.Id == updatedProduct.Id);
        if (productIndex != -1)
        {
            _products[productIndex] = updatedProduct;
            SaveToFile();
        }
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
        _products.RemoveAll(p => p.Id == productId);
        SaveToFile();
    }

    /// <summary>
    /// Carga la lista de productos desde un archivo JSON.
    /// </summary>
    private void LoadFromFile()
    {
        if (!File.Exists(_filePath)) return;

        try
        {
            string jsonString = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                _products = new List<Product>();
                return;
            }
            _products = JsonSerializer.Deserialize<List<Product>>(jsonString) ?? new List<Product>();
        }
        catch (JsonException ex)
        {
            // Escribir errores en la consola es aceptable para la depuración en la capa de negocio.
            Console.WriteLine($"Error al leer el archivo JSON: {ex.Message}");
            _products = new List<Product>();
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error de E/S al cargar el inventario: {e.Message}");
        }
    }

    /// <summary>
    /// Guarda la lista completa de productos en un archivo JSON con formato indentado.
    /// </summary>
    private void SaveToFile()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_products, options);
            File.WriteAllText(_filePath, jsonString);
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error de E/S al guardar el inventario: {e.Message}");
        }
    }
}


