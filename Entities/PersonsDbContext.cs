using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set;}
        public DbSet<Country> Countries { get; set;}

        public PersonsDbContext(DbContextOptions options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            List<Country> countriesJ = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(File.ReadAllText("countries.json"))!;
            List<Person> personsJ = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(File.ReadAllText("persons.json"))!;

            foreach (Country c in countriesJ) 
            {
                modelBuilder.Entity<Country>().HasData(c);
            }
            foreach (Person p in personsJ)
            {
                modelBuilder.Entity<Person>().HasData(p);
            }
        }
    }
}
