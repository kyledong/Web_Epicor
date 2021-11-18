using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Epicor.Data.Mapping;
using Web_Epicor.Entities;

namespace Web_Epicor.Data
{
    public class DbContextSystem : DbContext
    {
        public DbSet<BAQ> BAQs { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<User> Users { get; set; }


        public DbContextSystem(DbContextOptions<DbContextSystem> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BAQMap());
            modelBuilder.ApplyConfiguration(new ErrorMap());
            modelBuilder.ApplyConfiguration(new UserMap());
          
        }
    }
}
