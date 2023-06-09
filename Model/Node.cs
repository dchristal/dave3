#nullable enable
namespace dave3.Model;

public partial class Node
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public int? ParentId { get; set; }

    public int? NodeId { get; set; }

    public virtual ICollection<Node> InverseNodeNavigation { get; set; } = new List<Node>();

    public virtual ICollection<Node> Children { get; set; } = new List<Node>();

    public virtual Node? NodeNavigation { get; set; }

    public virtual Node? Parent { get; set; }
}
