using BrewLogix.Models;

namespace BrewLogix.Services
{
    public class BatchLogService
    {
        private readonly List<BatchLog> _batchLogs;

        public BatchLogService()
        {
            _batchLogs = new List<BatchLog>
            {
                new BatchLog { Id = 1, BatchId = 1, Timestamp = DateTime.Now, Note = "Batch started" },
                new BatchLog { Id = 2, BatchId = 2, Timestamp = DateTime.Now, Note = "Batch started" }
            };
        }

        public IEnumerable<BatchLog> GetAllBatchLogs() => _batchLogs;

        public BatchLog GetBatchLogById(int id) => _batchLogs.FirstOrDefault(b => b.Id == id);

        public void AddBatchLog(BatchLog batchLog)
        {
            batchLog.Id = _batchLogs.Max(b => b.Id) + 1;
            _batchLogs.Add(batchLog);
        }

        public void UpdateBatchLog(BatchLog batchLog)
        {
            var index = _batchLogs.FindIndex(b => b.Id == batchLog.Id);
            if (index != -1)
            {
                _batchLogs[index] = batchLog;
            }
        }

        public void DeleteBatchLog(BatchLog batchLog)
        {
            _batchLogs.Remove(batchLog);
        }
    }
}