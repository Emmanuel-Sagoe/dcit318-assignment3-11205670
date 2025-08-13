using System;
using System.Collections.Generic;

namespace WarehouseInventory
{
    
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"[Electronic] ID: {Id}, Name: {Name}, Brand: {Brand}, Qty: {Quantity}, Warranty: {WarrantyMonths} months";
    }

    
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"[Grocery] ID: {Id}, Name: {Name}, Qty: {Quantity}, Expiry: {ExpiryDate:yyyy-MM-dd}";
    }

    
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");

            _items[id].Quantity = newQuantity;
        }
    }

    
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));

            
            _groceries.AddItem(new GroceryItem(101, "Rice", 50, DateTime.Now.AddMonths(6)));
            _groceries.AddItem(new GroceryItem(102, "Milk", 20, DateTime.Now.AddDays(10)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock updated for {item.Name}. New Qty: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;
    }

    // Main Application
    public class Program
    {
        public static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("=== Grocery Items ===");
            manager.PrintAllItems(manager.Groceries);

            Console.WriteLine("=== Electronic Items ===");
            manager.PrintAllItems(manager.Electronics);

            // Test exceptions
            try
            {
                manager.Electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12)); // Duplicate
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            manager.RemoveItemById(manager.Groceries, 999); // Non-existent item
            try
            {
                manager.Electronics.UpdateQuantity(2, -5); // Invalid quantity
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
