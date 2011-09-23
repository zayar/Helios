using System.Data.Entity;
using Helios.Web.Infrastructure;

namespace Helios.Web.Models {
    public class HeliosDbContext : DbContext, IUnitOfWork {
        public DbSet<User> Users { get; set; }

        public int Commit() {
            return this.SaveChanges();
        }
    }
}