using System.Data.Entity;

namespace Helios.Web.Models {
#if DEBUG
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<HeliosDbContext> {
#else
    public class DatabaseInitializer : CreateDatabaseIfNotExists<HeliosDbContext> {
#endif
        protected override void Seed(HeliosDbContext context) {
            base.Seed(context);

            var defaultAdmin = new User() { 
                UserName = "admin",
                IsAdmin = true,
                RequiredToChangePassword = true
            };

            defaultAdmin.SetPassword("admin");

            context.Users.Add(defaultAdmin);
            context.Commit();

        }
    }
}