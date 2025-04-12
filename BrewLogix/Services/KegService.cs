using BrewLogix.Models;

namespace BrewLogix.Services
{
    public class KegService
    {
        private readonly List<Keg> _kegs;
        private readonly List<Batch> _batches;

        public KegService(BatchService batchService)
        {
            _batches = batchService.GetAllBatches().ToList();
            _kegs = new List<Keg>
            {
                new Keg { Id = 1, Code = "KEG 001", Size = "5", IsDistributed = false, FilledAt = DateTime.Now, Batch = _batches[0]},
                new Keg { Id = 2, Code = "KEG 002", Size = "10", IsDistributed = false, FilledAt = DateTime.Now, Batch = _batches[1] }
            };
        }

        public IEnumerable<Keg> GetAllKegs() => _kegs;

        public Keg GetKegById(int id) => _kegs.FirstOrDefault(k => k.Id == id);

        public void AddKeg(Keg keg)
        {
            keg.Id = _kegs.Max(k => k.Id) + 1;
            _kegs.Add(keg);
        }

        public void UpdateKeg(Keg keg)
        {
            var index = _kegs.FindIndex(k => k.Id == keg.Id);
            if (index != -1)
            {
                keg.Batch = _batches.Find(b => b.Id == keg.BatchId);
                _kegs[index] = keg;
            }
        }

        public void DeleteKeg(Keg keg)
        {
            _kegs.Remove(keg);
        }
    }
}