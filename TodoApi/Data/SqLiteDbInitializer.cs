using System.Collections.Generic;
using TodoApi.Models;
using TodoApi.Helpers;

namespace TodoApi.Data
{
    public class SqLiteDbInitializer : IDbInitializer
    {
        private IAuthenticationHelper authenticationHelper;

        public SqLiteDbInitializer(IAuthenticationHelper authHelper)
        {
            authenticationHelper = authHelper;
        }

        // This method will cre<ate and seed the database.
        public void Initialize(TodoContext context)
        {
            // Delete the database, if it already exists. You need to clean and build
            // the solution for this to take effect.
            context.Database.EnsureDeleted();

            // Create the database, if it does not already exists. If the database
            // already exists, no action is taken (and no effort is made to ensure it
            // is compatible with the model for this context).
            context.Database.EnsureCreated();

            List<TodoItem> items = new List<TodoItem>
            {
                new TodoItem {IsComplete=true, Name="Use SqLite"},
                new TodoItem {IsComplete=false, Name="Exam project"}
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
