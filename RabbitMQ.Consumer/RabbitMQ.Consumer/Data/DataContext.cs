using Microsoft.EntityFrameworkCore;
using RabbitMQ.Consumer.Dtos.Config;
using RabbitMQ.Consumer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageErrorLog> MessagesErrorLogs { get; set; }
    }
}