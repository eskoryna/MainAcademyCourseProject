using System.Data.Entity;

namespace FieldsAndChips
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }
        public DbSet<SavedGame> SavedGames { get; set; }
    }
}
