using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace registry.PostgreSQL
{
    public class OperationContext : DbContext
    {
        public OperationContext(DbContextOptions<OperationContext> options) : base(options) { }

        public DbSet<Languages> languages { get; set; }
        public DbSet<Concepts> concepts { get; set; }

        public DbSet<Examples> examples { get; set; }
    }

    public class Languages
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Concepts
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string method { get; set; }
        public int languageId { get; set; }
        public Languages language { get; set; }
    }

    public class Examples
    {
        [Key]
        public int id { get; set; }
        public string example { get; set; }
        public string notes { get; set; }
        public int conceptId { get; set; }
        public Concepts concept { get; set; }
    }
}