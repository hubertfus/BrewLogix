using BrewLogix.Models;

namespace BrewLogix.Services
{
    public class KegService
    {
        private readonly List<Keg> _kegs;

        public KegService()
        {
            _kegs = new List<Keg>
            {
                new Keg { Id = 1, Code = "KEG 001", Size = "5L", IsDistributed = false, FilledAt = DateTime.Now },
                new Keg { Id = 2, Code = "KEG 002", Size = "10L", IsDistributed = false, FilledAt = DateTime.Now }
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
                _kegs[index] = keg;
            }
        }

        public void DeleteKeg(Keg keg)
        {
            _kegs.Remove(keg);
        }
    }
}