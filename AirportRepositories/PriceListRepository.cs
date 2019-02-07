using Models;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DAL
{
    public class PriceListRepository : BaseRepository<PriceItem>, IRepository<PriceItem>
    {
        public PriceListRepository()
        {
            Table = Context.PriceLists;
        }

        public int Delete(Guid id)
        {
            Context.Entry(new PriceItem() { PriceItemID = id }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(Guid id)
        {
            Context.Entry(new PriceItem() { PriceItemID = id }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }
    }
}
