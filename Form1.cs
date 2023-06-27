using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using dave3.Model;
using Equin.ApplicationFramework;
using Microsoft.EntityFrameworkCore;

namespace dave3;

public partial class Form1 : Form
{
    private readonly DelightfulContext _cnx;
    private readonly Dictionary<TreeNode, TreeNodeEntity> _treeNodeEntityMapping = new();
    private bool _enableEdit = true;
    private int _previousRowIndex;
    private bool _isDirty;

    public TreeView LastFocusedTreeView;
    /*
        private int previousRowIndex;
    */

    public Form1()
    {
        InitializeComponent();
        KeyPreview = true;
        _cnx = new DelightfulContext();
        LastFocusedTreeView = treeView1;

        var treeViews = new List<TreeView> { treeView1, treeView2, treeView3 };
        // Create the context menu
        var contextMenu = new ContextMenuStrip();
        var moveUpItem = new ToolStripMenuItem("Move Up");
        var moveDownItem = new ToolStripMenuItem("Move Down");

        contextMenu.Items.AddRange(new ToolStripItem[] { moveUpItem, moveDownItem });

        inventoryDataGridView.DefaultValuesNeeded += InventoryDataGridView_DefaultValuesNeeded;
        LastFocusedTreeView.GotFocus += TreeView_GotFocus;
        moveUpItem.Click += (_, _) => MoveNode(-1);
        moveDownItem.Click += (_, _) => MoveNode(1);
        searchTreeView1.Leave += SearchTreeView_Leave;
        searchTreeView2.Leave += SearchTreeView_Leave;
        searchTreeView3.Leave += SearchTreeView_Leave;
        tvFilter1.CheckedChanged += TvFilter_CheckedChanged;
        tvFilter2.CheckedChanged += TvFilter_CheckedChanged;
        tvFilter3.CheckedChanged += TvFilter_CheckedChanged;
        tvIncludeChildren1.CheckedChanged += TvFilter_CheckedChanged;
        tvIncludeChildren2.CheckedChanged += TvFilter_CheckedChanged;
        tvIncludeChildren3.CheckedChanged += TvFilter_CheckedChanged;


        foreach (var treeView in treeViews)
        {
            treeView.AllowDrop = true;
            treeView.LabelEdit = true;
            treeView.ItemDrag += TreeView_ItemDrag;
            treeView.DragEnter += TreeView_DragEnter;
            treeView.DragOver += TreeView_DragOver;
            treeView.DragDrop += TreeView_DragDrop;
            treeView.AfterSelect += TreeView_AfterSelect;
            treeView.KeyDown += TreeView_KeyDown;
            treeView.BeforeLabelEdit += TreeView_BeforeLabelEdit;

            treeView.AfterLabelEdit += TreeView_AfterLabelEdit;
            treeView.ContextMenuStrip = contextMenu;
            treeView.GotFocus += TreeView_GotFocus;

            BindTreeView(treeView);
            //  treeView.MouseDown += treeView_MouseDown;
            //treeView.SelectedNode.EnsureVisible();  
        }

        BuildInventoryDataGridView();
    }

    private void TreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
        //// Cancel the label edit action, without canceling the editing of other nodes.
        //if ((sender as Control)?.Name[..3] == "tre")
        //    e.CancelEdit = true;

        //else
        //    e.CancelEdit = false;

        //// e.CancelEdit = true;
        /// 
        e.CancelEdit = !_enableEdit;
    }

    private void InventoryDataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
    {
        e.Row.Cells["LastUpdate"].Value = DateTime.Now;
    }

    private TreeNode FindNodeByProductId(TreeNodeCollection nodes, int productId)
    {
        foreach (TreeNode node in nodes)
        {
            if (node != null && ((TreeNodeTagData)node.Tag).Id == productId) return node;

            if (node != null)
            {
                var result = FindNodeByProductId(node.Nodes, productId);
                if (result != null) return result;
            }
        }

        return null;
    }

    private void InventoryDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
    {
        return;
        var inventoryView = (ObjectView<Inventory>)e.Row.DataBoundItem;
        var inventory = inventoryView.Object;
        _cnx.Inventories.Remove(inventory);
        _cnx.SaveChanges();
        inventoryDataGridView.Refresh();   
    }

    private void InventoryDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
    {
        return;
        var dataGridView = (DataGridView)sender;
        var row = dataGridView.Rows[e.RowIndex];

        if (row.Cells["Description"].Value == null)
        {
            // Cancel the event and select the cell to be edited by the user.
            e.Cancel = true;
            dataGridView.CurrentCell = row.Cells["Description"];
            MessageBox.Show("Description cannot be empty.");
        }
    }
    private void inventoryDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        var dataGridView = (DataGridView)sender;
        var row = dataGridView.Rows[e.RowIndex];

        if (row.IsNewRow)
        {
            _isDirty = true;
        }
    }
    private void InventoryDataGridView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            foreach (DataGridViewRow row in inventoryDataGridView.SelectedRows)
            {
                var inventoryView = (ObjectView<Inventory>)row.DataBoundItem;
                var inventory = inventoryView.Object;
                _cnx.Inventories.Remove(inventory);
            }
            _cnx.SaveChanges();
            FilterInventoryList();
        }
        else if (e.Control && e.KeyCode == Keys.OemQuotes) // Check if Ctrl+' is pressed
        {
            var currentCell = inventoryDataGridView.CurrentCell;
            if (currentCell != null && currentCell.RowIndex > 0) // Check if it's not the first row
            {
                var cellAbove = inventoryDataGridView[currentCell.ColumnIndex, currentCell.RowIndex - 1];
                currentCell.Value = cellAbove.Value;
                e.Handled = true; // Mark the event as handled
                _cnx.SaveChanges();
            }
        }
    }


    private void BuildInventoryDataGridView()
    {
        inventoryDataGridView.Leave += InventoryDataGridView_Leave;
        inventoryDataGridView.RowLeave += InventoryDataGridView_RowLeave;
        inventoryDataGridView.RowValidating += InventoryDataGridView_RowValidating;
        inventoryDataGridView.RowValidated += InventoryDataGridView_RowValidated;
        inventoryDataGridView.UserDeletingRow += InventoryDataGridView_UserDeletingRow;
        inventoryDataGridView.CellBeginEdit += inventoryDataGridView_CellBeginEdit;
        inventoryDataGridView.KeyDown += InventoryDataGridView_KeyDown;
        inventoryDataGridView.CellEnter += InventoryDataGridView_CellEnter;


        // Load Inventory entities from the database into memory
        _cnx.Inventories.Load();


        foreach (var item in _cnx.Inventories)
        {
            item.ProductName = _cnx.TreeNodeEntities.Find(item.ProductId)?.Name!;
            item.LocationName = _cnx.TreeNodeEntities.Find(item.Location)?.Name!;
            item.CategoryName = _cnx.TreeNodeEntities.Find(item.CategoryId)?.Name!;
        }


        // Populate [NotMapped] Inventory properties

        foreach (var inventory in _cnx.Inventories.Local)
        {
            var correspondingNode = FindNodeByProductId(treeView1.Nodes, inventory.ProductId);

            if (correspondingNode != null) inventory.ProductName = correspondingNode.Text;
        }

        // Create a new BindingSource
        //var bindingSource = new BindingSource();

        // Set the DataSource of the BindingSource to the local entities
        var inv = _cnx.Inventories.ToList();
        var view = new BindingListView<Inventory>(inv);
        bindingSource1.DataSource = view;
        //dataGridView1.DataSource = view;

        //bindingSource1.DataSource = _cnx.Inventories.Local.ToBindingList();

        // Set the DataSource of the DataGridView to the BindingSource
        inventoryDataGridView.DataSource = bindingSource1;


        // Configure column headers, widths, and display order
        var smallWidth = 30;
        inventoryDataGridView.Columns["ProductId"]!.Width = 20;
        inventoryDataGridView.Columns["ProductName"]!.Width = 100;
        inventoryDataGridView.Columns["LocationName"]!.Width = 100;
        inventoryDataGridView.Columns["CategoryName"]!.Width = 100;
        inventoryDataGridView.Columns["LastUpdate"]!.Width = 70;
        inventoryDataGridView.Columns["Description"]!.Width = 150; //Description
        inventoryDataGridView.Columns["Quantity"]!.Width = smallWidth; //qty
        inventoryDataGridView.Columns["Material"]!.Width = 80;
        inventoryDataGridView.Columns["Diameter"]!.Width = smallWidth;
        inventoryDataGridView.Columns["Length"]!.Width = smallWidth;
        inventoryDataGridView.Columns["Width"]!.Width = smallWidth;
        inventoryDataGridView.Columns["Height"]!.Width = smallWidth;
        inventoryDataGridView.Columns["Weight"]!.Width = smallWidth;
        inventoryDataGridView.Columns["Pitch"]!.Width = smallWidth;
        inventoryDataGridView.Columns["Volts"]!.Width = smallWidth;
        inventoryDataGridView.Columns["Amps"]!.Width = smallWidth;
        inventoryDataGridView.Columns["Watts"]!.Width = smallWidth;
        inventoryDataGridView.Columns["UoM"]!.Width = 40;
        inventoryDataGridView.Columns["Notes"]!.Width = 240;

        inventoryDataGridView.Columns["LastUpdate"].DefaultCellStyle.Format = "MM/dd/yyyy";

        inventoryDataGridView.Columns["ProductId"].DisplayIndex = 0;
        inventoryDataGridView.Columns["ProductName"].DisplayIndex = 1;
        inventoryDataGridView.Columns["LocationName"].DisplayIndex = 2;
        inventoryDataGridView.Columns["CategoryName"].DisplayIndex = 3;
        inventoryDataGridView.Columns["LastUpdate"].DisplayIndex = 4;
        inventoryDataGridView.Columns["Description"].DisplayIndex = 5;
        inventoryDataGridView.Columns["Quantity"].DisplayIndex = 6;
        inventoryDataGridView.Columns["Material"].DisplayIndex = 7;
        inventoryDataGridView.Columns["Diameter"].DisplayIndex = 8;
        inventoryDataGridView.Columns["Length"].DisplayIndex = 9;
        inventoryDataGridView.Columns["Width"].DisplayIndex = 10;
        inventoryDataGridView.Columns["Height"].DisplayIndex = 11;
        inventoryDataGridView.Columns["Weight"].DisplayIndex = 12;
        inventoryDataGridView.Columns["Pitch"].DisplayIndex = 13;
        inventoryDataGridView.Columns["Volts"].DisplayIndex = 14;
        inventoryDataGridView.Columns["Amps"].DisplayIndex = 15;
        inventoryDataGridView.Columns["Watts"].DisplayIndex = 16;


        inventoryDataGridView.Columns["Quantity"].HeaderText = @"Qty";
        inventoryDataGridView.Columns["ProductId"].HeaderText = @"ID";
        inventoryDataGridView.Columns["ProductName"].HeaderText = @"Name";
        inventoryDataGridView.Columns["LocationName"].HeaderText = @"Loc";
        inventoryDataGridView.Columns["CategoryName"].HeaderText = @"Cat";
        inventoryDataGridView.Columns["LastUpdate"].HeaderText = @"Last";
        inventoryDataGridView.Columns["Description"].HeaderText = @"Desc";
        //inventoryDataGridView.Columns["Material"].HeaderText = "Mat";
        inventoryDataGridView.Columns["Length"].HeaderText = @"L";
        inventoryDataGridView.Columns["Diameter"].HeaderText = @"D";
        inventoryDataGridView.Columns["Width"].HeaderText = @"W";
        inventoryDataGridView.Columns["Height"].HeaderText = @"H";
        inventoryDataGridView.Columns["Weight"].HeaderText = @"Wt";
        inventoryDataGridView.Columns["Pitch"].HeaderText = @"P";
        inventoryDataGridView.Columns["Volts"].HeaderText = @"V";
        inventoryDataGridView.Columns["Amps"].HeaderText = @"A";
        inventoryDataGridView.Columns["Watts"].HeaderText = @"W";

        inventoryDataGridView.Columns["PRODUCTID"]!.Visible = false;
        inventoryDataGridView.Columns["LOCATION"]!.Visible = false;
        inventoryDataGridView.Columns["CATEGORYID"]!.Visible = false;
        inventoryDataGridView.Columns["InventoryId"]!.Visible = false;

   
    }

    private void InventoryDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
        // Check if the cell is in the column you want to skip
        if (inventoryDataGridView.CurrentCell.ColumnIndex > 17 && inventoryDataGridView.SelectedRows.Count == 0)  //Columns[e.ColumnIndex].Name == "ProductName")
        {
            // Check if there are more rows below
            if (e.RowIndex + 1 < inventoryDataGridView.RowCount)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    try { 
                     inventoryDataGridView.CurrentCell = inventoryDataGridView[4, e.RowIndex + 1];
                    }
                    catch
                    { 
                    }
                }));
            }
        }
    }

  

    private void InventoryDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e)
    {
        var dataGridView = (DataGridView)sender;
        var row = dataGridView.Rows[e.RowIndex];

        if (_isDirty)
        {
            // The row is a new row that the user has just finished entering data for.
            // A new Inventory object has been created and added to the binding source.
            // You can retrieve this object using the DataBoundItem property of the row.

            var objectView = (ObjectView<Inventory>)row.DataBoundItem;
            var inventory = objectView.Object;

            // Now you can add the new Inventory object to the DbContext.

            _cnx.Inventories.Add(inventory);
            _isDirty = false;
        }

        _cnx.SaveChanges();
    }



    private void InventoryDataGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
    {
        _previousRowIndex = e.RowIndex;
    }


    private void SearchTreeView_Leave(object sender, EventArgs e)
    {
        var sb = sender as TextBox;

        var searchTreeView = Convert.ToInt32(MyRegex2().Match(sb!.Name).Value);
        switch (searchTreeView)
        {
            case 1:
                LastFocusedTreeView = treeView1;
                break;

            case 2:
                LastFocusedTreeView = treeView2;
                break;
            case 3:
                LastFocusedTreeView = treeView3;
                break;
        }

        FindAndSelectNode(sb.Text);
    }

    private void BindTreeView(TreeView tree)
    {
        var treeId = Convert.ToInt32(MyRegex().Replace(tree.Name, ""));

        // Clear the existing nodes
        tree.Nodes.Clear();
        // Load data from the database into the local context
        _cnx.TreeNodeEntities.Load();
        // Get the root nodes (i.e., nodes with no parent) and order them
        var rootNodes = _cnx.TreeNodeEntities.Local
            .Where(entity => entity.ParentId == null && entity.TreeId == treeId)
            .OrderBy(entity => entity.Order); // Add this line to sort by Order

        // Add the root nodes to the TreeView
        foreach (var rootNode in rootNodes)
        {
            var treeNode = new TreeNode(rootNode.Name);
            var tagData = new TreeNodeTagData
            {
                TreeNodeEntity = rootNode,
                Id = rootNode.Id // replace with your actual Id
            };
            treeNode.Tag = tagData;
            tree.Nodes.Add(treeNode);

            // Add the TreeNode and its corresponding entity to the dictionary
            _treeNodeEntityMapping[treeNode] = rootNode;

            // Add child nodes
            AddChildNodes(treeNode);
        }
    }

    //private async void inventoryDataGridView_Leave(object sender, EventArgs e)
    private void InventoryDataGridView_Leave(object o, EventArgs e)
    {
        bindingSource1.EndEdit();
        //if (inventoryDataGridView.CurrentRow != null) _previousRowIndex = inventoryDataGridView.CurrentRow.Index;
        _cnx.SaveChangesAsync();
    }

    private void AddChildNodes(TreeNode parentTreeNode)
    {
        var parentEntity = (TreeNodeTagData)parentTreeNode.Tag;

        var childEntities = _cnx.TreeNodeEntities.Local
            .Where(entity => entity.ParentId == parentEntity.Id)
            .OrderBy(entity => entity.Order); // Add this line to sort by Order

        foreach (var childEntity in childEntities)
        {
            var childTreeNode = new TreeNode(childEntity.Name);
            var childTagData = new TreeNodeTagData
            {
                Id = childEntity.Id
            };
            childTreeNode.Tag = childTagData; // Store the TreeNodeTagData in the Tag property for later use
            parentTreeNode.Nodes.Add(childTreeNode);

            // Add the TreeNode and its corresponding entity to the dictionary
            _treeNodeEntityMapping[childTreeNode] = childEntity;

            // Recursively add grandchild nodes
            AddChildNodes(childTreeNode);
        }
    }

    private void UpdateProductNamesInInventory(int productId, string newName)
    {
        foreach (DataGridViewRow row in inventoryDataGridView.Rows)
            if (row.DataBoundItem is Inventory inventoryItem && inventoryItem.ProductId == productId)
                inventoryItem.ProductName = newName;

        inventoryDataGridView.Refresh();
    }

    private void TreeView_DragOver(object sender, DragEventArgs e)
    {
        // Retrieve the client coordinates of the mouse position.  
        var targetPoint = LastFocusedTreeView.PointToClient(new Point(e.X, e.Y));

        // Select the node at the mouse position.  
        LastFocusedTreeView.SelectedNode = LastFocusedTreeView.GetNodeAt(targetPoint);
    }

    private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
        try
        {
            // lastSelectedNodes[lastFocusedTreeView] = e.Node;
            var node = LastFocusedTreeView.SelectedNode;

            var ancestry = Ancestry(node);

            var tagData = (TreeNodeTagData)node.Tag;

            if (tagData != null)
            {
                var nTag = tagData.Id;

                // Set the appropriate TextBox values based on the TreeView that raised the event
                switch (LastFocusedTreeView.Name)
                {
                    case "treeView1":
                        //tv1Key.Text = nKey;
                        tvName1.Text = LastFocusedTreeView.SelectedNode.Text;
                        tv1Tag.Text = nTag.ToString();
                        tvAncestry1.Text = ancestry;
                        break;
                    case "treeView2":
                        tvName2.Text = LastFocusedTreeView.SelectedNode.Text;
                        tv2Tag.Text = nTag.ToString();
                        tvAncestry2.Text = ancestry;
                        break;
                    case "treeView3":
                        tvName3.Text = LastFocusedTreeView.SelectedNode.Text;
                        tv3Tag.Text = nTag.ToString();
                        tvAncestry3.Text = ancestry;
                        break;
                }
            }

            // Set the default value for new rows in the DataGridView
            inventoryDataGridView.DefaultValuesNeeded -=
                InventoryDataGridView_DefaultValuesNeeded; // Unsubscribe first to avoid stacking up event handlers
            inventoryDataGridView.DefaultValuesNeeded += (_, args) =>
            {
                args.Row.Cells[0].Value = tv1Tag.Text;
                args.Row.Cells[1].Value = tv2Tag.Text;
                args.Row.Cells[2].Value = tv3Tag.Text;
                args.Row.Cells["ProductName"].Value = tvName1.Text;
                args.Row.Cells["LocationName"].Value = tvName2.Text;
                args.Row.Cells["CategoryName"].Value = tvName3.Text;
                args.Row.Cells["LastUpdate"].Value = DateTime.Now;
                args.Row.Cells["Quantity"].Value = 1;
            };
            if (inventoryDataGridView.CurrentRow != null && inventoryDataGridView.CurrentRow.Index == _previousRowIndex)

            {
                var treeid =
                    Convert.ToInt32(MyRegex().Replace(LastFocusedTreeView.Name, ""));
                var inventory = inventoryDataGridView.CurrentRow.DataBoundItem as Inventory;
                switch (treeid)
                {
                    case 1:

                        inventoryDataGridView.CurrentRow.Cells["ProductName"].Value =
                            LastFocusedTreeView.SelectedNode.Text;
                        inventoryDataGridView.CurrentRow.Cells["ProductId"].Value = tv1Tag.Text;
                        if (inventory != null) inventory.ProductId = Convert.ToInt32(tv1Tag.Text);
                        break;
                    case 2:
                        inventoryDataGridView.CurrentRow.Cells["LocationName"].Value =
                            LastFocusedTreeView.SelectedNode.Text;
                        inventoryDataGridView.CurrentRow.Cells["Location"].Value = tv2Tag.Text;
                        if (inventory != null) inventory.Location = Convert.ToInt32(tv2Tag.Text);
                        //save inventory changes

                        break;
                    case 3:
                        inventoryDataGridView.CurrentRow.Cells["CategoryName"].Value =
                            LastFocusedTreeView.SelectedNode.Text;
                        inventoryDataGridView.CurrentRow.Cells["CategoryId"].Value = tv3Tag.Text;
                        if (inventory != null) inventory.CategoryId = Convert.ToInt32(tv3Tag.Text);
                        break;
                }

                if (inventory != null) _cnx.Entry((object)inventory).State = EntityState.Modified;

                _cnx.SaveChanges();
            }
        }
        catch (Exception)
        {
            //  MessageBox.Show(ex.Message);
        }
    }

    private static string Ancestry(TreeNode node)
    {
        if (node != null)
        {
            var ancestry = node.Text;

            while (node.Parent != null)
            {
                node = node.Parent;
                ancestry = node.Text + "/" + ancestry;
            }

            return ancestry;
        }

        return "";
    }

    //private bool ContainsNode(TreeNode node1, TreeNode node2)
    //{
    //    // Check the parent node of the second node.  
    //    if (node2.Parent == null) return false;
    //    if (node2.Parent.Equals(node1)) return true;

    //    // If the parent node is not null or equal to the first node,   
    //    // call the ContainsNode method recursively using the parent of   
    //    // the second node.  
    //    return ContainsNode(node1, node2.Parent);
    //}

    private void TreeView_GotFocus(object sender, EventArgs e)
    {
        LastFocusedTreeView = sender as TreeView ?? treeView1;
    }

    private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
    {
        if (e.Item != null) DoDragDrop(e.Item, DragDropEffects.Move);
    }

    private static void TreeView_DragEnter(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;
    }

    private void TreeView_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetData(typeof(TreeNode)) is TreeNode movingNode)
        {
            var targetPoint = LastFocusedTreeView.PointToClient(new Point(e.X, e.Y));
            var targetNode = LastFocusedTreeView.GetNodeAt(targetPoint);

            // Remove the node from its current position
            movingNode.Remove();

            // Insert the node at the new location
            if (targetNode == null)
                // The node is dropped on the background, add it to the root
                LastFocusedTreeView.Nodes.Add(movingNode);
            else
                // The node is dropped on another node, make it a child of that node
                targetNode.Nodes.Add(movingNode);

            // Get the TreeNodeEntity corresponding to the moved TreeNode
            var movingEntity = _treeNodeEntityMapping[movingNode];

            // Modify the parent of the moved entity based on where the TreeNode was moved in the TreeView
            if (targetNode == null)
            {
                movingEntity.ParentId = null; // Or whatever signifies a root node in your model
            }
            else
            {
                var parentEntity = _treeNodeEntityMapping[targetNode];
                movingEntity.ParentId = parentEntity.Id;
            }

            // Save changes to the DbContext
            _cnx.SaveChanges();
        }
    }

    private void TreeView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F2)
        {
            // Start editing the selected node when the F2 key is pressed
            // e.CancelEdit = false;
            if (LastFocusedTreeView.SelectedNode == null) return;
            //LastFocusedTreeView.CancelEdit = false;
            _enableEdit = true;
            LastFocusedTreeView.SelectedNode.BeginEdit();
        }
        else if (e.KeyCode == Keys.Insert)
        {
            // Insert a new node as a child of the selected node when the Insert key is pressed
            // (or as a root node if no node is selected)
            // (or do nothing if the TreeView has no nodes)
            // handle tagData on insert

            if (LastFocusedTreeView.SelectedNode != null)
            {
                var newNode = new TreeNode("New node");
                LastFocusedTreeView.SelectedNode.Nodes.Add(newNode);

                // Create a new entity and add it to the context
                var parentEntity = _treeNodeEntityMapping[LastFocusedTreeView.SelectedNode];

                // Get the highest Order value of the existing child entities
                var maxOrder = _cnx.TreeNodeEntities.Local
                    .Where(entity => entity.ParentId == parentEntity.Id)
                    .Max(entity => (int?)entity.Order) ?? 0;

                var newEntity = new TreeNodeEntity
                {
                    Name = newNode.Text,
                    ParentId = parentEntity.Id,
                    Order = maxOrder +
                            1 // Set the Order property to be one greater than the highest existing Order value
                    // Set other properties as necessary
                };
                _cnx.TreeNodeEntities.Add(newEntity);

                // Add the new node and its corresponding entity to the dictionary
                _treeNodeEntityMapping[newNode] = newEntity;

                // Save changes to the database
                LastFocusedTreeView.SelectedNode = newNode;
                LastFocusedTreeView.SelectedNode.EnsureVisible();
                if (LastFocusedTreeView.SelectedNode == null)
                {
                }
                else
                {
                    LastFocusedTreeView.SelectedNode.BeginEdit();
                }

                _cnx.SaveChanges();
            }
        }

        else if (e.KeyCode == Keys.Delete)
        {
            // Store a reference to the selected node before deleting it
            var selectedNode = LastFocusedTreeView.SelectedNode;
            if (selectedNode != null)
            {
                // Show a confirmation dialog before deleting
                var result = MessageBox.Show(@"Are you sure you want to delete this node?", @"Confirm Delete",
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Remove the node from the TreeView
                    selectedNode.Remove();

                    // Remove the corresponding entity from the context
                    var entity = _treeNodeEntityMapping[selectedNode];
                    _cnx.TreeNodeEntities.Remove(entity);

                    // Remove the node and its corresponding entity from the dictionary
                    _treeNodeEntityMapping.Remove(selectedNode);

                    // Save changes to the database
                    _cnx.SaveChanges();
                }
            }
        }
    }

    private void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
        // Update the entity when a node's text is edited
        if (e.Node != null && e.Label != null)
        {
            var entity = _treeNodeEntityMapping[e.Node];
            entity.Name = e.Label;
            var tagData = new TreeNodeTagData
            {
                Id = entity.Id
            };
            e.Node.Tag = tagData;
            // Save changes to the database
            _cnx.SaveChanges();
            // Now update the DataGridView
            UpdateProductNamesInInventory(entity.Id, e.Label);
        }

        _enableEdit = false;
    }

    private void MoveNode(int direction)
    {
        var selectedNode = LastFocusedTreeView.SelectedNode;
        if (selectedNode == null)
            return;

        TreeNodeCollection parentNodes;
        if (selectedNode.Parent == null)
            parentNodes = LastFocusedTreeView.Nodes; // Root level node
        else
            parentNodes = selectedNode.Parent.Nodes; // Child node

        var index = parentNodes.IndexOf(selectedNode);
        if (index + direction < 0 || index + direction >= parentNodes.Count)
            return;

        // Swap the Order properties of the selected node and the node in the direction
        var selectedEntity = _treeNodeEntityMapping[selectedNode];
        var otherEntity = _treeNodeEntityMapping[parentNodes[index + direction]];
        if (selectedEntity.Order == otherEntity.Order)
            switch (direction)
            {
                case 1:
                    otherEntity.Order++;
                    break;
                case -1:
                    selectedEntity.Order++;
                    break;
            }

        (selectedEntity.Order, otherEntity.Order) = (otherEntity.Order, selectedEntity.Order);

        // Save changes to the database
        _cnx.SaveChanges();

        // Move the node in the TreeView
        parentNodes.RemoveAt(index);
        parentNodes.Insert(index + direction, selectedNode);

        LastFocusedTreeView.SelectedNode = selectedNode;
    }

    private void FindAndSelectNode(string searchTerm)
    {
        foreach (TreeNode node in LastFocusedTreeView.Nodes)
            if (FindAndSelectNode(node, searchTerm))
                return;
    }

    private bool FindAndSelectNode(TreeNode node, string searchTerm)
    {
        if (node.Text.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))

        {
            LastFocusedTreeView.SelectedNode = node;
            node.Expand();
            LastFocusedTreeView.Focus();
            return true;
        }

        foreach (TreeNode child in node.Nodes)
            if (FindAndSelectNode(child, searchTerm))
                return true;

        return false;
    }

    public void AddOrUpdateControlObject(string name, string myString, int? myInt, float? myFloat)
    {
        // Get the existing ControlObject with the specified name
        var controlObject = _cnx.ControlObjects.FirstOrDefault(co => co.Name == name);

        if (controlObject == null)
        {
            // If the ControlObject doesn't exist, create a new one
            controlObject = new ControlObject
            {
                Name = name,
                ControlString = myString,
                ControlInt = myInt,
                ControlFloat = myFloat
            };

            // Add the new ControlObject to the DbSet
            _cnx.ControlObjects.Add(controlObject);
        }
        else
        {
            // If the ControlObject does exist, update its properties
            controlObject.ControlString = myString;
            controlObject.ControlInt = myInt;
            controlObject.ControlFloat = myFloat;
        }

        // Save changes to the database
        _cnx.SaveChanges();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        //var selectedNodePath1B4 = treeView1.SelectedNode.FullPath;
        AddOrUpdateControlObject("nodePath1", treeView1.SelectedNode.FullPath, null, null);
        AddOrUpdateControlObject("nodePath2", treeView2.SelectedNode.FullPath, null, null);
        AddOrUpdateControlObject("nodePath3", treeView3.SelectedNode.FullPath, null, null);

        // File.WriteAllText("nodePath1.txt", selectedNodePath1B4);
        //var selectedNodePath2B4 = treeView2.SelectedNode.FullPath;
        //File.WriteAllText("nodePath2.txt", selectedNodePath2B4);
        //var selectedNodePath3B4 = treeView3.SelectedNode.FullPath;
        //File.WriteAllText("nodePath3.txt", selectedNodePath3B4);
        try
        {
            _cnx.SaveChanges();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }

        //    _cnx.Dispose();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        //if (tbs.DataSource is TreeNode treeNode) 
        //    treeView2.Nodes.Add(treeNode);
        try
        {
            //var selectedNodePath1 = File.ReadAllText("nodePath1.txt");
            // var selectedNodePath2 = File.ReadAllText("nodePath2.txt");
            // var selectedNodePath3 = File.ReadAllText("nodePath3.txt");

            var selectedNodePath1 = _cnx.ControlObjects.FirstOrDefault(co => co.Name == "nodePath1")?.ControlString;
            if (selectedNodePath1 != null)
            {
                var pathParts1 = selectedNodePath1.Split('\\');
                var node1 = FindNodeByPath(treeView1.Nodes, pathParts1, 0);
                treeView1.SelectedNode = node1;
            }

            var selectedNodePath2 = _cnx.ControlObjects.FirstOrDefault(co => co.Name == "nodePath2")?.ControlString;
            if (selectedNodePath2 != null)
            {
                var pathParts2 = selectedNodePath2.Split('\\');
                var node2 = FindNodeByPath(treeView2.Nodes, pathParts2, 0);
                treeView2.SelectedNode = node2;
                ExpandParentNodes(node2);

                var ancestry = Ancestry(node2);
                tvAncestry2.Text = ancestry;
                tvName2.Text = node2?.Text;
                if (node2 != null) tv2Tag.Text = ((TreeNodeTagData)node2.Tag).Id.ToString();
            }

            var selectedNodePath3 = _cnx.ControlObjects.FirstOrDefault(co => co.Name == "nodePath3")?.ControlString;

            if (selectedNodePath3 != null)
            {
                var pathParts3 = selectedNodePath3.Split('\\');
                var node3 = FindNodeByPath(treeView3.Nodes, pathParts3, 0);
                ExpandParentNodes(node3);
                treeView3.SelectedNode = node3;
                if (node3 != null)
                {
                    var ancestry3 = Ancestry(node3);
                    tvAncestry3.Text = ancestry3;
                    tvName3.Text = node3.Text;
                    tv3Tag.Text = ((TreeNodeTagData)node3.Tag).Id.ToString();
                }
            }
        }

        catch (Exception ex2)
        {
            MessageBox.Show(ex2.Message);
        }

        //   AddNewRecord();
    }

    private void ExpandParentNodes(TreeNode node)
    {
        if (node == null)
            return;

        if (node.Parent != null)
        {
            node.Parent.Expand();
            ExpandParentNodes(node.Parent);
        }
    }

    private TreeNode FindNodeByPath(TreeNodeCollection nodes, string[] pathParts, int index)
    {
        foreach (TreeNode node in nodes)
            if (node != null && node.Text == pathParts[index])
            {
                if (index == pathParts.Length - 1)
                    // This is the node we're looking for
                    return node;

                // We need to search the child nodes
                var childNode = FindNodeByPath(node.Nodes, pathParts, index + 1);
                if (childNode != null)
                    // We found the node in the child nodes
                    return childNode;
            }

        // We didn't find the node
        return null;
    }

    private void TvFilter_CheckedChanged(object sender, EventArgs e)
    {
        bindingSource1.EndEdit();
        _cnx.SaveChangesAsync();
        // _cnx.SaveChanges();
        tvIncludeChildren1.Enabled = tvFilter1.Checked;
        if (!tvFilter1.Checked) tvIncludeChildren1.Checked = false;
        tvIncludeChildren2.Enabled = tvFilter2.Checked;
        if (!tvFilter2.Checked) tvIncludeChildren2.Checked = false;
        tvIncludeChildren3.Enabled = tvFilter3.Checked;
        if (!tvFilter3.Checked) tvIncludeChildren3.Checked = false;

        FilterInventoryList();
        inventoryDataGridView.Refresh();
    }

    private Expression<Func<Inventory, bool>> GetFilterCondition(int id, bool includeChildren,
        Func<Inventory, int> propertySelector)
    {
        if (includeChildren)
        {
            var childIds = GetAllChildIds(treeView2.SelectedNode);
            childIds.Add(id); // Include the Id of the selected node itself

            return i => childIds.Contains(propertySelector(i));
        }

        return i => propertySelector(i) == id;
    }

    //private IQueryable<Inventory> ApplyFilter(IQueryable<Inventory> query, TreeView treeView, CheckBox filterCheckBox,
    //    CheckBox includeChildrenCheckBox, Func<Inventory, int> propertySelector)
    //{
    //    if (filterCheckBox.Checked)
    //    {
    //        var tagData = (TreeNodeTagData)treeView.SelectedNode.Tag;
    //        var id = tagData.Id;

    //        var filterCondition = GetFilterCondition(id, includeChildrenCheckBox.Checked, propertySelector);

    //        query = query.Where(filterCondition);
    //    }

    //    return query;
    //}
    //private IQueryable<Inventory> ApplyFilter(IQueryable<Inventory> query, TreeView treeView, CheckBox filterCheckBox,
    //    CheckBox includeChildrenCheckBox, string propertyName)
    //{
    //    if (filterCheckBox.Checked)
    //    {
    //        var tagData = (TreeNodeTagData)treeView.SelectedNode.Tag;
    //        var id = tagData.Id;

    //        switch (propertyName)
    //        {
    //            case "ProductId":
    //                query = query.Where(i => i.ProductId == id);
    //                break;
    //            case "Location":
    //                query = query.Where(i => i.Location == id);
    //                break;
    //            case "CategoryId":
    //                query = query.Where(i => i.CategoryId == id);
    //                break;
    //        }

    //        if (includeChildrenCheckBox.Checked)
    //        {
    //            var childIds = GetAllChildIds(treeView.SelectedNode);
    //            childIds.Add(id); // Include the Id of the selected node itself

    //            switch (propertyName)
    //            {
    //                case "ProductId":
    //                    query = query.Where(i => childIds.Contains(i.ProductId));
    //                    break;
    //                case "Location":
    //                    query = query.Where(i => childIds.Contains(i.Location));
    //                    break;
    //                case "CategoryId":
    //                    query = query.Where(i => childIds.Contains(i.CategoryId));
    //                    break;
    //            }
    //        }
    //    }

    //    return query;
    //}

    private HashSet<int> ApplyFilter(TreeView treeView, CheckBox filterCheckBox,
        CheckBox includeChildrenCheckBox, Func<Inventory, int> propertySelector)
    {
        var ids = new HashSet<int>();

        if (filterCheckBox.Checked)
        {
            var tagData = (TreeNodeTagData)treeView.SelectedNode.Tag;
            var id = tagData.Id;

            // Add the ID of the selected node itself
            ids.Add(id);

            if (includeChildrenCheckBox.Checked)
            {
                // Add the IDs of all child nodes
                var childIds = GetAllChildIds(treeView.SelectedNode);
                ids.UnionWith(childIds);
            }
        }

        return ids;
    }


    //private void FilterInventoryList()
    //{
    //    // End any pending edits
    //    bindingSource1.EndEdit();
    //    _cnx.SaveChanges();
    //    // Apply filters directly on the DbSet
    //    var filteredQuery = _cnx.Inventories.AsQueryable();

    //    var filteredList = inventoryList.AsQueryable();


    //    //filteredQuery = ApplyFilter(filteredQuery, treeView1, tvFilter1, tvIncludeChildren1, i => i.ProductId);
    //    //filteredQuery = ApplyFilter(filteredQuery, treeView2, tvFilter2, tvIncludeChildren2, i => i.Location);
    //    //filteredQuery = ApplyFilter(filteredQuery, treeView3, tvFilter3, tvIncludeChildren3, i => i.CategoryId);

    //    filteredList = ApplyFilter(filteredList, treeView1, tvFilter1, tvIncludeChildren1, "ProductId");
    //    filteredList = ApplyFilter(filteredList, treeView2, tvFilter2, tvIncludeChildren2, "Location");
    //    filteredList = ApplyFilter(filteredList, treeView3, tvFilter3, tvIncludeChildren3, "CategoryId");


    //    // Load the filtered entities into the local context
    //    filteredQuery.Load();

    //    // Update the BindingSource
    //    bindingSource1.DataSource = _cnx.Inventories.Local.ToBindingList();

    //    // Set the DataSource of the DataGridView to the BindingSource
    //    inventoryDataGridView.DataSource = bindingSource1;
    //}

    private void FilterInventoryList()
    {
        //bindingSource1.EndEdit();
        //_cnx.SaveChanges();
        //var inventoryList = _cnx.Inventories.Local.ToBindingList();

        //// Start with the full list
        //var filteredList = inventoryList.AsQueryable();

        //// Apply filters sequentially
        //filteredList = ApplyFilter(filteredList, treeView1, tvFilter1, tvIncludeChildren1, "ProductId");
        //filteredList = ApplyFilter(filteredList, treeView2, tvFilter2, tvIncludeChildren2, "Location");
        //filteredList = ApplyFilter(filteredList, treeView3, tvFilter3, tvIncludeChildren3, "CategoryId");

        //// Update the BindingSource
        //bindingSource1.DataSource = new BindingList<Inventory>(filteredList.ToList());

        //// Set the DataSource of the DataGridView to the BindingSource
        //inventoryDataGridView.DataSource = bindingSource1;
        var selectedProductIds = ApplyFilter(treeView1, tvFilter1, tvIncludeChildren1, i => i.ProductId);
        var selectedLocations = ApplyFilter(treeView2, tvFilter2, tvIncludeChildren2, i => i.Location);
        var selectedCategoryIds = ApplyFilter(treeView3, tvFilter3, tvIncludeChildren3, i => i.CategoryId);
        var bindingListView = new BindingListView<Inventory>(_cnx.Inventories.Local.ToList());

        if (!selectedProductIds.Any() && !selectedLocations.Any() && !selectedCategoryIds.Any())
            bindingListView.RemoveFilter();
        else
            bindingListView.ApplyFilter(i => selectedProductIds.Contains(i.ProductId)
                                             || selectedLocations.Contains(i.Location)
                                             || selectedCategoryIds.Contains(i.CategoryId));


        // Assign to BindingSource
        bindingSource1.DataSource = bindingListView;


        //bindingSource1.Filter = $"ProductId IN ({string.Join(", ", selectedProductIds)}) OR " +
        //                        $"Location IN ({string.Join(", ", selectedLocations)}) OR " +
        //                        $"CategoryId IN ({string.Join(", ", selectedCategoryIds)})";

        inventoryDataGridView.Refresh();
    }


    private void ApplyFilters()
    {
        var originalList = _cnx.Inventories.Local.ToBindingList();

        // Create a HashSet to store all child IDs
        var childIds = new HashSet<int>();

        // Add child IDs from treeView1
        if (tvFilter1.Checked)
        {
            var tagData = (TreeNodeTagData)treeView1.SelectedNode.Tag;
            var id = tagData.Id;
            childIds.Add(id);

            if (tvIncludeChildren1.Checked) childIds.UnionWith(GetAllChildIds(treeView1.SelectedNode));
        }

        // Add child IDs from treeView2
        if (tvFilter2.Checked)
        {
            var tagData = (TreeNodeTagData)treeView2.SelectedNode.Tag;
            var id = tagData.Id;
            childIds.Add(id);

            if (tvIncludeChildren2.Checked) childIds.UnionWith(GetAllChildIds(treeView2.SelectedNode));
        }

        // Add child IDs from treeView3
        if (tvFilter3.Checked)
        {
            var tagData = (TreeNodeTagData)treeView3.SelectedNode.Tag;
            var id = tagData.Id;
            childIds.Add(id);

            if (tvIncludeChildren3.Checked) childIds.UnionWith(GetAllChildIds(treeView3.SelectedNode));
        }

        // Now filter the original list using the child IDs
        var filteredList = originalList.Where(i =>
                childIds.Contains(i.ProductId) || childIds.Contains(i.Location) || childIds.Contains(i.CategoryId))
            .ToList();

        // Set the DataSource of the BindingSource to the filtered list
        _cnx.SaveChanges();
        bindingSource1.DataSource = filteredList.Count > 0
            ? new BindingList<Inventory>(filteredList)
            : _cnx.Inventories.Local.ToBindingList();
    }

    private void ApplyFilters(string searchString)
    {
        var originalList = _cnx.Inventories.Local.ToBindingList();

        // Create a HashSet to store all child IDs
        var childIds = new HashSet<int>();

        // Now filter the original list using the child IDs
        var filteredList = originalList.Where(i =>
#pragma warning disable CA2249
                i.Description != null && i.Description.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
#pragma warning restore CA2249
            .ToList();

        // Set the DataSource of the BindingSource to the filtered list
        if (filteredList.Count > 0)
            bindingSource1.DataSource = new BindingList<Inventory>(filteredList);
        else
            bindingSource1.DataSource = _cnx.Inventories.Local.ToBindingList();
    }

    private List<int> GetAllChildIds(TreeNode node)
    {
        var childIds = new List<int>();

        foreach (TreeNode childNode in node.Nodes)
        {
            var childTagData = (TreeNodeTagData)childNode.Tag;
            childIds.Add(childTagData.Id);

            // Recursively get the Ids of the grandchildren
            childIds.AddRange(GetAllChildIds(childNode));
        }

        return childIds;
    }

    private void InventoryDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        // switch owningcolumn.name
        // case "ProductId":
        // tvAncestry1.Text = getancestrybynodeid(e.value)
        // case "Location":
        // tvAncestry2.Text = getancestrybynodeid(e.value)
        // case "CategoryId":
        // tvAncestry3.Text = getancestrybynodeid(e.value)
        // end switch
        return;
        switch (inventoryDataGridView.CurrentCell.OwningColumn.Name)
        {
            case "ProductName":
                tvAncestry1.Text = Ancestry(treeView1.SelectedNode);
                break;
            case "LocationName":
                tvAncestry2.Text = Ancestry(treeView2.SelectedNode);
                break;
            case "CategoryName":
                tvAncestry3.Text = Ancestry(treeView3.SelectedNode);
                break;
        }
    }

    private void AddNewRecord()
    {
        // Navigate to the last row (the new record row)
        inventoryDataGridView.CurrentCell = inventoryDataGridView.Rows[^1].Cells[4];

        inventoryDataGridView.Focus();
    }

    private void AddNew_Click(object sender, EventArgs e)
    {
        AddNewRecord();
    }


    private void InventoryDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
    }

    [GeneratedRegex("TreeView", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex();

    [GeneratedRegex("SearchTree", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MyRegex1();

    [GeneratedRegex("\\d+")]
    private static partial Regex MyRegex2();

    private void SearchInventory_Leave(object sender, EventArgs e)
    {
        ApplyFilters(SearchInventory.Text);
    }

    public class TreeNodeTagData
    {
        public TreeNodeEntity TreeNodeEntity { get; set; }
        public int Id { get; set; }
    }
}