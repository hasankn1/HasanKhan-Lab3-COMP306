using HasanKhan_Lab3_COMP306.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HasanKhan_Lab3_COMP306;

namespace HasanKhan_Lab3_COMP306.DbData
{
    public class EFUserRepository : IUserRepository
    {
        private Lab3Comp306DbContext context;
        public EFUserRepository(Lab3Comp306DbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<User> Users => context.Users;

        public void SaveUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
