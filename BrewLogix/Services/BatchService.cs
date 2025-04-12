using BrewLogix.Models;

namespace BrewLogix.Services
{
    public class BatchService
    {
        private readonly List<Batch> _batches;

        public BatchService()
        {
            _batches = new List<Batch>
            {
                new Batch
                {
                    Id = 1,
                    Code = "BATCH001",
                    RecipeId = 1,
                    Recipe = new Recipe { Id = 1, Name = "Pale Ale" },
                    StartDate = DateTime.Now,
                    Status = "In Progress",
                    Kegs = new List<Keg>
                    {
                        new Keg { Id = 1, Size = "5L", FilledAt = DateTime.Now, IsDistributed = false }
                    },
                    StockEntries = new List<StockEntry>
                    {
                        new StockEntry { Id = 1, Ingredient = new Ingredient { Id = 1, Name = "Hops", Type = "Bittering", Unit = "g" }, Quantity = 100, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(6) },
                        new StockEntry { Id = 2, Ingredient = new Ingredient { Id = 2, Name = "Barley", Type = "Grain", Unit = "kg" }, Quantity = 150, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(12) }
                    }
                },
                new Batch
                {
                    Id = 2,
                    Code = "BATCH002",
                    RecipeId = 2,
                    Recipe = new Recipe { Id = 2, Name = "IPA" },
                    StartDate = DateTime.Now.AddDays(-5),
                    Status = "Fermentation",
                    Kegs = new List<Keg>
                    {
                        new Keg { Id = 2, Size = "10L", FilledAt = DateTime.Now.AddDays(-2), IsDistributed = false }
                    },
                    StockEntries = new List<StockEntry>
                    {
                        new StockEntry { Id = 3, Ingredient = new Ingredient { Id = 1, Name = "Hops", Type = "Bittering", Unit = "g" }, Quantity = 250, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(6) },
                        new StockEntry { Id = 4, Ingredient = new Ingredient { Id = 3, Name = "Yeast", Type = "Fermentation", Unit = "g" }, Quantity = 30, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(3) }
                    }
                },
                new Batch
                {
                    Id = 3,
                    Code = "BATCH003",
                    RecipeId = 2,
                    Recipe = new Recipe { Id = 2, Name = "IPA" },
                    StartDate = DateTime.Now.AddDays(-10),
                    
                    Status = "Completed",
                    Kegs = new List<Keg>
                    {
                        new Keg { Id = 3, Size = "5L", FilledAt = DateTime.Now.AddDays(-3), IsDistributed = true }
                    },
                    StockEntries = new List<StockEntry>
                    {
                        new StockEntry { Id = 3, Ingredient = new Ingredient { Id = 1, Name = "Hops", Type = "Bittering", Unit = "g" }, Quantity = 250, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(6) },
                    }
                }
            };
        }

        public IEnumerable<Batch> GetAllBatches() => _batches;

        public Batch GetBatchById(int id) => _batches.FirstOrDefault(b => b.Id == id);

        public void AddBatch(Batch batch)
        {
            batch.Id = _batches.Max(b => b.Id) + 1;
            _batches.Add(batch);
        }

        public void UpdateBatch(Batch batch)
        {
            var index = _batches.FindIndex(b => b.Id == batch.Id);
            if (index != -1)
            {
                _batches[index] = batch;
            }
        }

        public void DeleteBatch(Batch batch)
        {
            _batches.Remove(batch);
        }
    }
}
