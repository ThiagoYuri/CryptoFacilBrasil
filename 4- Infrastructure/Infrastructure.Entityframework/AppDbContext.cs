using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using CryptoFacilBrasil.Domain.Models;

namespace Infrastructure.Entityframework
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                modelBuilder.Entity<OrderDetail>()
                    .Property(e => e.Network)
                    .HasConversion<int>();

                modelBuilder.Entity<OrderDetail>()
                    .Property(e => e.TypeTransferNetwork)
                    .HasConversion<int>();

                modelBuilder.Entity<OrderDetail>()
                    .Property(e => e.MethodPay)
                    .HasConversion<int>();

                modelBuilder.Entity<OrderDetail>()
                    .Property(e => e.StatusOrder)
                    .HasConversion<int>();
            });

            // Additional model configurations can go here
        }

    }

}
