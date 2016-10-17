using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BOM.Service.Models
{
    public class BOMDataContext : DbContext
    {

        public BOMDataContext()
        {           
           // this.Database.Delete();            
		    Configuration.AutoDetectChangesEnabled = true;
		    Configuration.LazyLoadingEnabled = true;
		    Configuration.ProxyCreationEnabled = true;
		    Configuration.ValidateOnSaveEnabled = true;
	    }

        protected override void Dispose(bool disposing)
        {
            Configuration.LazyLoadingEnabled = false;
            base.Dispose(disposing);
        }

        public DbSet<Part> Parts { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<BillOfMaterial> BillOfMaterials { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Part>()
                .Property(s => s.PartName)
                .IsRequired();

            modelBuilder.Entity<Component>()
                        .Property(s => s.ComponentName)
                        .IsRequired();

            
            modelBuilder.Entity<Part>()
              .HasMany<BillOfMaterial>(e => e.BOMParts);

            modelBuilder.Entity<Component>()
                .HasMany<BillOfMaterial>(e => e.BOMComponents);


            modelBuilder.Entity<BillOfMaterial>()
                       .Property(s => s.ComponentID)
                       .IsRequired();

            modelBuilder.Entity<BillOfMaterial>()
                       .Property(s => s.PartID)
                       .IsRequired();
 
        }

        
    }
}