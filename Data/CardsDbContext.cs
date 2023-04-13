using Cards.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Cards.API.Data
{
    public class CardsDbContext : DbContext
    {
        public CardsDbContext(DbContextOptions options) : base(options)
        {

        }


        //DbSet replica of Cards table create in SQL
        //DbSet

        public DbSet<Card> Cards { get; set; }

    }
}
