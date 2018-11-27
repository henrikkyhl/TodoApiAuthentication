using System.Collections.Generic;
using System.Linq;
using TodoApi.Helpers;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        private IAuthenticationHelper authenticationHelper;

        public DbInitializer(IAuthenticationHelper authHelper)
        {
            authenticationHelper = authHelper;
        }

        // This method will create and seed the database.
        public void Initialize(TodoContext context)
        {
            // Create the database, if it does not already exists. If the database
            // already exists, no action is taken (and no effort is made to ensure it
            // is compatible with the model for this context).
            context.Database.EnsureCreated();

            // Look for any TodoItems
            if (context.TodoItems.Any())
            {
                return;   // DB has been seeded
            }

            List<TodoItem> items = new List<TodoItem>
            {
                new TodoItem { IsComplete=true, Name="Make homework"},
                new TodoItem { IsComplete=false, Name="Sleep"}
            };

            // Create two users with hashed and salted passwords
            string password = "1234";
            byte[] passwordHashJoe, passwordSaltJoe, passwordHashAnn, passwordSaltAnn;
            authenticationHelper.CreatePasswordHash(password, out passwordHashJoe, out passwordSaltJoe);
            authenticationHelper.CreatePasswordHash(password, out passwordHashAnn, out passwordSaltAnn);

            List<User> users = new List<User>
            {
                new User {
                    Username = "UserJoe",
                    PasswordHash = passwordHashJoe,
                    PasswordSalt = passwordSaltJoe,
                    IsAdmin = false
                },
                new User {
                    Username = "AdminAnn",
                    PasswordHash = passwordHashAnn,
                    PasswordSalt = passwordSaltAnn,
                    IsAdmin = true
                }
            };

            context.TodoItems.AddRange(items);
            context.Users.AddRange(users);
            context.SaveChanges();

        }


    }
}
