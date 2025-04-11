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
                    Logs = new List<BatchLog>
                    {
                        new BatchLog { Id = 1, BatchId = 1, Timestamp = DateTime.Now, Note = "Batch started" }
                    },
                    Kegs = new List<Keg>
                    {
                        new Keg { Id = 1, Size = "5L", FilledAt = DateTime.Now, IsDistributed = false }
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
                    Logs = new List<BatchLog>
                    {
                        new BatchLog { Id = 2, BatchId = 2, Timestamp = DateTime.Now, Note = "Fermentation started" }
                    },
                    Kegs = new List<Keg>
                    {
                        new Keg { Id = 2, Size = "10L", FilledAt = DateTime.Now.AddDays(-2), IsDistributed = false }
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
