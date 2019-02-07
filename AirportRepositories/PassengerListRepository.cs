using Models;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DAL
{
    public class PassengerListRepository : BaseRepository<Passenger>, IRepository<Passenger>
    {
        public PassengerListRepository()
        {
            Table = Context.PassengerLists;
        }

        public int Delete(Guid id)
        {
            Context.Entry(new Passenger() { PassengerID = id }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(Guid id)
        {
            Context.Entry(new Passenger() { PassengerID = id }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }
    }
}

