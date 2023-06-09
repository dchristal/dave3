#nullable enable
namespace dave3.Model;

public partial class TreeNodeEntity
{
    public int Id { get; set; }

    public int TreeId { get; set; }

    public int Order { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }

    public virtual ICollection<TreeNodeEntity> Children { get; set; } = new List<TreeNodeEntity>();

    public virtual TreeNodeEntity? Parent { get; set; }
}
