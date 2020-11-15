using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Helpers;

namespace TodoApi.Data
{
    public class SqlServerDbInitializer: IDbInitializer
    {
        private IAuthenticationHelper authenticationHelper;

        public SqlServerDbInitializer(IAuthenticationHelper authHelper)
        {
            authenticationHelper = authHelper;
        }

        // This method will cre<ate and seed the database.
        public void Initialize(TodoContext context)
        {
            // Create the database, if it does not already exists. If the database
            // already exists, no action is taken (and no effort is made to ensure it
            // is compatible with the model for this context).
            context.Database.EnsureCreated();

            //Look for any TodoItems
            if (context.TodoItems.Any() || context.Users.Any())
                {
                // Delete and re-create the database, if it had already been created.
                // You must delete all the tables in the database. We do this, because
                // "context.Database.EnsureDeleted()" doesn't work on an Azure SQL
                // database with our type of subscription.
                // The statements below doesn't work on SqLite. This is the reason why
                // we have two database initializer classes (one for SqLite and one for
                // SQL Server.
                context.Database.ExecuteSqlRaw("DROP TABLE TodoItems");
                context.Database.ExecuteSqlRaw("DROP TABLE Users");
                context.Database.EnsureCreated();
                }

            List<TodoItem> items = new List<TodoItem>
            {
                new TodoItem {IsComplete=true, Name="Use SQL Server"}
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
