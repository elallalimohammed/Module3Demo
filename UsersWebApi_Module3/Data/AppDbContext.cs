using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UsersWebApi_Module3.Models;

namespace UsersWebApi_Module3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}