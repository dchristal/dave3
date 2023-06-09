using Microsoft.EntityFrameworkCore;

namespace dave3.Model;

public partial class DelightfulContext : DbContext
{
    public DelightfulContext()
    {
    }

    public DelightfulContext(DbContextOptions<DelightfulContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryView> InventoryViews { get; set; }

    public virtual DbSet<Node> Nodes { get; set; }

    public virtual DbSet<TreeNodeEntity> TreeNodeEntities { get; set; }

    public virtual DbSet<Attribute> Attributes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.68.67;Database=delightful;User Id=dave;Password=asdf;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.Location,e.CategoryId, e.Description }, "IX_Inventories_ProductId_Location_Category_Desc").IsUnique();
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
