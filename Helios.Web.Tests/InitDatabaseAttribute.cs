using System.Data.Entity;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests {
    internal class InitDatabaseAttribute : BeforeAfterTestAttribute {
        public override void Before(System.Reflection.MethodInfo methodUnderTest) {
            base.Before(methodUnderTest);

            InitDatabase();
        }
                
        private static void InitDatabase() {
            Database.SetInitializer<HeliosDbContext>(new DropCreateDatabaseAlways<HeliosDbContext>());

            using (var context = new HeliosDbContext()) {
                context.Database.Initialize(force: true);
            }
        }
    }
}
