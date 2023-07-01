using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace dave3.Model;

public partial class DelightfulContext : DbContext
{
 
        private readonly string _connectionString;

        //public DelightfulContext(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        // rest of the class implementation
     



  
    public DelightfulContext()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }

    //public DelightfulContext(DbContextOptions<DelightfulContext> options)
    //    : base(options)
    //{
    //}

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryView> InventoryViews { get; set; }

    public virtual DbSet<Node> Nodes { get; set; }

    public virtual DbSet<TreeNodeEntity> TreeNodeEntities { get; set; }

    public virtual DbSet<Attribute> Attributes { get; set; }

    public virtual DbSet<ControlObject> ControlObjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.Location, e.CategoryId, e.Description },
                "IX_Inventories_ProductId_Location_Category_Desc").IsUnique();
        });
        modelBuilder.Entity<ControlObject>(entity =>
        {
            entity.HasKey(e => e.Name);
        });


        // modelBuilder.Entity<InventoryView>(entity =>
        // {
        //     entity
        //         .HasNoKey()
        //         .ToView("InventoryView");
        // });

        modelBuilder.Entity<Attribute>(entity =>
        {
            entity.HasIndex(e => e.AttributeID, "IX_Attributes_AttributeID").IsUnique();
        });

        modelBuilder.Entity<InventoryView>(entity =>
        {
            entity
                .ToView("InventoryView");

            entity
                .HasKey(e => new { e.ProductId, e.Location });
        });


        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasIndex(e => e.NodeId, "IX_Nodes_NodeId");

            entity.HasIndex(e => e.ParentId, "IX_Nodes_ParentId");

            entity.HasOne(d => d.NodeNavigation).WithMany(p => p.InverseNodeNavigation).HasForeignKey(d => d.NodeId);

            entity.HasOne(d => d.Parent).WithMany(p => p.Children).HasForeignKey(d => d.ParentId);
        });

        modelBuilder.Entity<TreeNodeEntity>(entity =>
        {
            entity.HasIndex(e => e.ParentId, "IX_TreeNodeEntities_ParentId");

            entity.HasOne(d => d.Parent).WithMany(p => p.Children).HasForeignKey(d => d.ParentId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}