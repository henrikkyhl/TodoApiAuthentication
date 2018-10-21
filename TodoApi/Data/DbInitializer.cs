using System.Collections.Generic;
using System.Linq;
using TodoApi.Models;

namespace TodoApi.Data
{
    public static class DbInitializer
    {
        // This method will create and seed the database.
        public static void Initialize(TodoContext context)
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

            // Create two users
            string password = "1234";

            List<User> users = new List<User>
            {
                new User {
                    Username = "UserJoe",
                    Password = password,
                    IsAdmin = false
                },
                new User {
                    Username = "AdminAnn",
                    Password = password,
                    IsAdmin = true
                }
            };

            context.TodoItems.AddRange(items);
            context.Users.AddRange(users);
            context.SaveChanges();

        }

    }
}
