using Models;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DAL
{
    public class FlightRepository : BaseRepository<Flight>, IRepository<Flight>
    {
        public FlightRepository()
        {
            Table = Context.Flights;
        }

        public int Delete(Guid id)
        {
            Context.Entry(new Flight() { FlightID = id }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(Guid id)
        {
            Context.Entry(new Flight() { FlightID = id }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }
    }
}
