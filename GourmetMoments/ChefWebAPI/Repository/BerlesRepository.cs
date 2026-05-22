using ChefWebAPI.Models;

namespace ChefWebAPI.Repository
{
    public class BerlesRepository
    {
        private readonly List<Berles> _items = new();
        private int _nextId = 1;
        private readonly ReaderWriterLockSlim _lock = new();

        public IEnumerable<Berles> GetAll()
        {
            _lock.EnterReadLock();
            try
            {
                return _items.Select(i => Clone(i)).ToList();
            }
            finally { _lock.ExitReadLock(); }
        }

        public Berles? GetById(int id)
        {
            _lock.EnterReadLock();
            try
            {
                var item = _items.FirstOrDefault(x => x.Id == id);
                return item == null ? null : Clone(item);
            }
            finally { _lock.ExitReadLock(); }
        }

        public Berles Add(Berles b)
        {
            _lock.EnterWriteLock();
            try
            {
                b.Id = _nextId++;
                _items.Add(Clone(b));
                return Clone(b);
            }
            finally { _lock.ExitWriteLock(); }
        }

        public bool Delete(int id)
        {
            _lock.EnterWriteLock();
            try
            {
                var existing = _items.FirstOrDefault(x => x.Id == id);
                if (existing == null) return false;
                _items.Remove(existing);
                return true;
            }
            finally { _lock.ExitWriteLock(); }
        }

        public bool HasOverlapForChef(int chefId, DateTime newStart, DateTime newEnd)
        {
            _lock.EnterReadLock();
            try
            {
                foreach (var ex in _items.Where(x => x.ChefId == chefId))
                {
                    if (ex.StartDate.Date <= newEnd.Date && ex.EndDate.Date >= newStart.Date)
                        return true;
                }
                return false;
            }
            finally { _lock.ExitReadLock(); }
        }

        private static Berles Clone(Berles src)
        {
            return new Berles
            {
                Id = src.Id,
                Uid = src.Uid,
                ChefId = src.ChefId,
                StartDate = src.StartDate,
                EndDate = src.EndDate,
                DailyRate = src.DailyRate,
                BaseFee = src.BaseFee
            };
        }
    }
}