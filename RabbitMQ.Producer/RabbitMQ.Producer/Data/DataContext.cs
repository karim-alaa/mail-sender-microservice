
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Producer.Models;

namespace RabbitMQ.Producer.Data
{
    public class DataContext : DbContext
    {
        // DbSet properties declarations...

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<StuckMessage> StuckMessages { get; set; }
    }
}
