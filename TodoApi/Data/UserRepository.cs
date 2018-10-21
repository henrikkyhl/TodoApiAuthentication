using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class UserRepository : IRepository<User>
    {
        private readonly TodoContext db;

        public UserRepository(TodoContext context)
        {
            db = context;
        }

        public IEnumerable<User> GetAll()
        {
            return db.Users.ToList();
        }

        public User Get(long id)
        {
            return db.Users.FirstOrDefault(b => b.Id == id);
        }

        public void Add(User entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
        }

        public void Edit(User entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Remove(long id)
        {
            var item = db.Users.FirstOrDefault(b => b.Id == id);
            db.Users.Remove(item);
            db.SaveChanges();
        }
    }
}
