using CoreWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Data
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions options): base(options) {  }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<DocumentLog> DocumentList { get; set; }
    }
}
