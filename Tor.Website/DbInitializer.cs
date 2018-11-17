using System.Collections.Generic;
using System.Linq;
using Tor.Website.EF;
using Tor.Website.Models.DBO;

namespace Tor.Website
{
    public class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            User user = GetDefaultUser();

            context.Users.Add(user);

            context.SaveChanges();
        }

        private static User GetDefaultUser()
        {
            return new User { Login = "default@default.test", Name = "Default", Password = "1DefaulT1" };
        }
    }
}
