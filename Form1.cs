using System.Linq.Expressions;
using System.Text.RegularExpressions;
using dave3.Model;
using Equin.ApplicationFramework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace dave3;

public partial class Form1 : Form
{
    private readonly DelightfulContext _cnx;

    // Define the dictionary at the class level
    private readonly Dictionary<string, string> _columnToIdColumnMap = new()
    {
        { "ProductName", "ProductId" },
        { "LocationName", "LocationId" }
        // Add more mappings if needed
    };

    private readonly Dictionary<TreeNode, TreeNodeEntity> _treeNodeEntityMapping = new();
    private bool _enableEdit = true;
    private bool _filtering;

    private bool _isDirty;

    //   private Control _lastFocusedControl;
    private int _previousRowIndex;

    public TreeView LastFocusedTreeView;


    public Form1()
    {
        InitializeComponent();
        // Subscribe to the Enter event of all controls
        //foreach (Control control in Controls) control.Enter += Control_Enter;
        //treeView2.AfterSelect += TreeView2_AfterSelect;
        KeyPreview = true;
        _cnx = new DelightfulContext();
        LastFocusedTreeView = treeView1;
        Width = _cnx.ControlObjects.Find("Form1Width").ControlInt ?? 800;
        Height = _cnx.ControlObjects.Find("Form1Height").ControlInt ?? 600;
        var filterObject = new FilterClass { FilterStat = true };

        // Set the data source of the binding source to the object
        var bs = new BindingSource();
        bs.DataSource = filterObject;

        // Bind the Enabled property of the button to the boolean property of the object
        FilterStatus.DataBindings.Add("Enabled", bs, "FilterStat", false, DataSourceUpdateMode.OnPropertyChanged);
        UpdateButtonState(filterObject.FilterStat);

        var treeViews = new List<MyTreeView> { treeView1, treeView2, treeView3 };
        // Create the context menu
        var contextMenu = new ContextMenuStrip();
        var moveUpItem = new ToolStripMenuItem("Move Up");
        var moveDownItem = new ToolStripMenuItem("Move Down");

        contextMenu.Items.AddRange(new ToolStripItem[] { moveUpItem, moveDownItem });

        //inventoryDataGridView.DefaultValuesNeeded += InventoryDataGridView_DefaultValuesNeeded;
        LastFocusedTreeView.GotFocus += TreeView_GotFocus;
        moveUpItem.Click += (_, _) => MoveNode(-1);
        moveDownItem.Click += (_, _) => MoveNode(1);
        tvFilter1.CheckedChanged += TvFilter_CheckedChanged;
        tvFilter2.CheckedChanged += TvFilter_CheckedChanged;
        tvFilter3.CheckedChanged += TvFilter_CheckedChanged;
        tvIncludeChildren1.CheckedChanged += TvFilter_CheckedChanged;
        tvIncludeChildren2.CheckedChanged += TvFilter_CheckedChanged;
        tvIncludeChildren3.CheckedChanged += TvFilter_CheckedChanged;
        searchTreeView1.TextChanged += SearchTreeView_TextChanged;
        searchTreeView2.TextChanged += SearchTreeView_TextChanged;
        searchTreeView3.TextChanged += SearchTreeView_TextChanged;
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
            treeView.NodeMouseClick += TreeView_NodeMouseClick;

            BindTreeView(treeView);
            //  treeView.MouseDown += treeView_MouseDown;
            //treeView.SelectedNode.EnsureVisible();  
        }

        BuildInventoryDataGridView();
    }

    private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        var node = e.Node;
        var treeView = (MyTreeView)sender;
        var treeId = treeView!.Name[^1];
        var searchTreeView = Controls["SearchTreeView" + treeId];
        if (searchTreeView.Text.Length > 0)
        {
            searchTreeView.Text = "";
            UpdateTreeViewFilteringState();
        }
        // Execute the code you want here.
    }
    /*
        private int previousRowIndex;
    */

    private void UpdateButtonState(bool isEnabled)
    {
        if (isEnabled)
            FilterStatus.Text = "Filter";
        // FilterStatus.BackColor = Color.Aquamarine;
        else
            FilterStatus.Text = "Update";
        // FilterStatus.BackColor = Color.Pink;
        FilterStatus.Tag = isEnabled; // Store the state in the Tag property
    }

    private void FilterStatus_Click(object sender, EventArgs e)
    {
        // Get the current state from the Tag property
        var currentState = (bool)FilterStatus.Tag;

        // Toggle the state
        var newState = !currentState;

        // Update the button
        UpdateButtonState(newState);
    }

    private void SearchTreeView_TextChanged(object sender, EventArgs e)
    {
        var sb = sender as TextBox;
        var focusedControl = ActiveControl;
        var searchTreeView = Convert.ToInt32(MyRegex2().Match(sb!.Name).Value);
        CheckBox tvFilter = null;

        switch (searchTreeView)
        {
            case 1:
                LastFocusedTreeView = treeView1;
                tvFilter = tvFilter1;
                break;

            case 2:
                LastFocusedTreeView = treeView2;
                tvFilter = tvFilter2;
                break;
            case 3:
                LastFocusedTreeView = treeView3;
                tvFilter = tvFilter3;
                break;
        }

        if (sb.Text.Length > 0)
        {
            tvFilter.Checked = true;
            FindAndSelectNode(sb.Text);
        }
        else
        {
            tvFilter.Checked = false;
        }

        // Restore the focus
        focusedControl.Focus();
    }

    private void TreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
        // Cancel the label edit action, without canceling the editing of other nodes.
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


    private void InventoryDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        var dataGridView = (DataGridView)sender;
        var row = dataGridView.Rows[e.RowIndex];

        if (row.IsNewRow) _isDirty = true;
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
            FilterInventoryByTreeView();
        }
        else if (e.Control && e.KeyCode == Keys.OemQuotes) // Check if Ctrl+' is pressed
        {
            var currentCell = inventoryDataGridView.CurrentCell;
            if (currentCell != null && currentCell.RowIndex > 0) // Check if it's not the first row
            {
                var cellAbove = inventoryDataGridView[currentCell.ColumnIndex, currentCell.RowIndex - 1];
                currentCell.Value = cellAbove.Value;

                // Check if the current column is in the dictionary
                if (_columnToIdColumnMap.TryGetValue(currentCell.OwningColumn.Name, out var idColumnName))
                {
                    var idColumnIndex = inventoryDataGridView.Columns[idColumnName].Index;

                    // Get the cell above in the ID column
                    cellAbove = inventoryDataGridView[idColumnIndex, currentCell.RowIndex - 1];

                    // Update the current cell in the ID column
                    inventoryDataGridView[idColumnIndex, currentCell.RowIndex].Value = cellAbove.Value;
                }

                e.Handled = true; // Mark the event as handled
                try
                {
                    _cnx.SaveChanges();
                }
                catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                   (ex.InnerException as SqlException)?.Number == 2627)
                {
                    // Handle the duplicate key error
                    MessageBox.Show("Cannot insert duplicate item: " + ex.InnerException.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void InventoryDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        var validValues = new[] { 1, 2, 3 };
        if (validValues.Contains(inventoryDataGridView.Columns[e.ColumnIndex].DisplayIndex))
        {
            var rows = inventoryDataGridView.Rows;
            var controlObject = _cnx.ControlObjects.FirstOrDefault(co => co.Name == "AutoBulkUpdateMax");
            var updateMax = controlObject?.ControlInt;
            var proceed = rows.Count < updateMax;
            if (!proceed)
            {
                var result = MessageBox.Show("Do you really want to modify all " + rows + "?",
                    "Confirmation", MessageBoxButtons.YesNo);
                proceed = result == DialogResult.Yes;
            }

            if (proceed)
            {
                foreach (DataGridViewRow r in inventoryDataGridView.Rows)
                    switch (inventoryDataGridView.Columns[e.ColumnIndex].DisplayIndex)
                    {
                        case 1:
                            r.Cells[0].Value = tv1Tag.Text;
                            r.Cells["ProductName"].Value = tvName1.Text;
                            break;
                        case 2:
                            r.Cells[1].Value = tv2Tag.Text;
                            r.Cells["LocationName"].Value = tvName2.Text;
                            break;
                        case 3:
                            r.Cells[2].Value = tv3Tag.Text;
                            r.Cells["CategoryName"].Value = tvName3.Text;
                            break;
                    } // value is 1, 2, or 3

                _cnx.SaveChanges();
            }
        }
    }


    private void BuildInventoryDataGridView()
    {
        inventoryDataGridView.Leave += InventoryDataGridView_Leave;
        inventoryDataGridView.RowLeave += InventoryDataGridView_RowLeave;
        inventoryDataGridView.RowValidated += InventoryDataGridView_RowValidated;
        inventoryDataGridView.UserDeletingRow += InventoryDataGridView_UserDeletingRow;
        inventoryDataGridView.CellBeginEdit += InventoryDataGridView_CellBeginEdit;
        inventoryDataGridView.KeyDown += InventoryDataGridView_KeyDown;
        inventoryDataGridView.CellEnter += InventoryDataGridView_CellEnter;
        inventoryDataGridView.ColumnHeaderMouseClick += InventoryDataGridView_ColumnHeaderMouseClick;


        // Load Inventory entities from the database into memory
        _cnx.Inventories.Load();


        foreach (var item in _cnx.Inventories)
        {
            item.ProductName = _cnx.TreeNodeEntities.Find(item.ProductId)?.Name!;
            item.LocationName = _cnx.TreeNodeEntities.Find(item.LocationId)?.Name!;
            item.CategoryName = _cnx.TreeNodeEntities.Find(item.CategoryId)?.Name!;
        }


        // Populate [NotMapped] Inventory properties


        // Set the DataSource of the BindingSource to the local entities
        var inv = _cnx.Inventories.ToList();
        var view = new BindingListView<Inventory>(inv);
        bindingSource1.DataSource = view;

        // Set the DataSource of the DataGridView to the BindingSource
        inventoryDataGridView.DataSource = bindingSource1;


        // Configure column headers, widths, and display order
        var smallWidth = 30;
        inventoryDataGridView.Columns["ProductId"]!.Width = 20;
        inventoryDataGridView.Columns["ProductName"]!.Width = 100;
        inventoryDataGridView.Columns["LocationName"]!.Width = 100;
        inventoryDataGridView.Columns["LocationId"]!.Width = 100; //.Visible = false; //.Width = 100;
        inventoryDataGridView.Columns["LastUpdate"]!.Width = 70;
        inventoryDataGridView.Columns["Description"]!.Width = _cnx.ControlObjects.Find("DescWidth").ControlInt ?? 150;
        ; //Description
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
        inventoryDataGridView.Columns["LocationId"].DisplayIndex = 3;
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

        //inventoryDataGridView.Columns["PRODUCTID"]!.Visible = false;
        //inventoryDataGridView.Columns["LocationId"]!.Visible = false;
        inventoryDataGridView.Columns["CATEGORYID"]!.Visible = false;
        inventoryDataGridView.Columns["InventoryId"]!.Visible = false;
    }


    private void InventoryDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
        //var lf = _lastFocusedControl;
        var tvTag = tv2Tag.Text;
        var tvName = tvName2.Text;
        var currentCell = inventoryDataGridView.CurrentCell;
        
        if(currentCell.RowIndex > 0){
        if (currentCell != null  ) // Check if it's not the first row
            if (currentCell.OwningColumn.Name == "LocationName")
            {
                // Update the current cell and the Location cell based on the selected node in treeView2
                currentCell.Value = tvName;

                var locationColumnIndex = inventoryDataGridView.Columns["LocationId"].Index;
                inventoryDataGridView[locationColumnIndex, currentCell.RowIndex].Value = tvTag;
                return;
            }

        //  if (!_filtering)
        
            var row = inventoryDataGridView.Rows[e.RowIndex];
            var tn = _cnx.TreeNodeEntities.Find(row.Cells["ProductId"].Value);
            tvAncestry1.Text = Ancestry(tn);
            SelectNode(treeView1, tvAncestry1.Text);
            tn = _cnx.TreeNodeEntities.Find(row.Cells["LocationId"].Value);
            tvAncestry2.Text = Ancestry(tn);
            SelectNode(treeView2, tvAncestry2.Text);
            tn = _cnx.TreeNodeEntities.Find(row.Cells["CategoryId"].Value);
            tvAncestry3.Text = Ancestry(tn);
            SelectNode(treeView3, tvAncestry3.Text);
            _filtering = false;
        }

    }

    private void InventoryDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e)
    {
        var dataGridView = (DataGridView)sender;
        var row = dataGridView.Rows[e.RowIndex];

        // Check if it's a new row
        if (row.IsNewRow)
        {
            // Set the LastUpdate to the current time
            row.Cells["LastUpdate"].Value = DateTime.Now;

            // The row is a new row that the user has just finished entering data for.
            // A new Inventory object has been created and added to the binding source.
            // You can retrieve this object using the DataBoundItem property of the row.

            var objectView = (ObjectView<Inventory>)row.DataBoundItem;
            if (objectView != null)
            {
                var inventory = objectView.Object;

                // Now you can add the new Inventory object to the DbContext.

                _cnx.Inventories.Add(inventory);
            }
        
        }

        _cnx.SaveChanges();
    }


    private void InventoryDataGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
    {
        _previousRowIndex = e.RowIndex;
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
                Id = rootNode.Id
            };
            treeNode.Tag = tagData;
            tree.Nodes.Add(treeNode);

            // Add the TreeNode and its corresponding entity to the dictionary
            _treeNodeEntityMapping[treeNode] = rootNode;

            // Add child nodes
            AddChildNodes(treeNode);
        }
    }

    private async void InventoryDataGridView_Leave(object sender, EventArgs e)
        //private void InventoryDataGridView_Leave(object o, EventArgs e)
    {
        bindingSource1.EndEdit();
        //if (inventoryDataGridView.CurrentRow != null) _previousRowIndex = inventoryDataGridView.CurrentRow.Index;
        await _cnx.SaveChangesAsync();
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
        // Wrapping the code in a try-catch block to handle any runtime exceptions
        try
        {
            var treeId = Convert.ToInt32(MyRegex().Replace(LastFocusedTreeView.Name, ""));
            var treeView = (MyTreeView)sender;
            var node = LastFocusedTreeView.SelectedNode;
            var ancestry = Ancestry(node);
            var tagData = (TreeNodeTagData)node.Tag;

            if (tagData != null)
            {
                // Extracting the Id from the tag data
                var nTag = tagData.Id;

                // Depending on the treeId, different actions are performed
                switch (treeId)
                {
                    case 1:
                        // Setting some text boxes and potentially a row in a DataGridView based on the selected node
                        // The specifics of this will depend on the context of your program
                        // Similar actions are done in cases 2 and 3, but with different text boxes and potentially different DataGridView cells
                        // The DataGridView row updates seem to only happen if a row is selected and some condition regarding FilterStatus.Tag is not met
                        tvName1.Text = LastFocusedTreeView.SelectedNode.Text;
                        tv1Tag.Text = nTag.ToString();
                        tvAncestry1.Text = ancestry;
                        if (inventoryDataGridView.CurrentRow != null && !(bool)FilterStatus.Tag)
                        {
                            var row = inventoryDataGridView.CurrentRow;
                            row.Cells["ProductName"].Value = tvName1.Text;
                            row.Cells["ProductId"].Value = tv1Tag.Text;
                        }

                        break;
                    case 2:
                        // Similar to case 1
                        tvName2.Text = LastFocusedTreeView.SelectedNode.Text;
                        tv2Tag.Text = nTag.ToString();
                        tvAncestry2.Text = ancestry;
                        if (inventoryDataGridView.CurrentRow != null && !(bool)FilterStatus.Tag)
                        {
                            var row = inventoryDataGridView.CurrentRow;
                            row.Cells["LocationName"].Value = tvName2.Text;
                            row.Cells["LocationId"].Value = tv2Tag.Text;
                            UpdateButtonState(true);
                        }

                        break;
                    case 3:
                        // Similar to case 1
                        tvName3.Text = LastFocusedTreeView.SelectedNode.Text;
                        tv3Tag.Text = nTag.ToString();
                        tvAncestry3.Text = ancestry;
                        if (inventoryDataGridView.CurrentRow != null && !(bool)FilterStatus.Tag)
                        {
                            var row = inventoryDataGridView.CurrentRow;
                            row.Cells["CategoryName"].Value = tvName3.Text;
                            row.Cells["CategoryId"].Value = tv3Tag.Text;
                        }

                        break;
                }
            }

            // If the DataGridView has a currently selected row and its index matches _previousRowIndex, 
            // the selected row's cells are updated based on the selected node of the TreeView corresponding to the treeId,
            // and then the database is updated to reflect these changes
            // The specifics of this will depend on the context of your program
            if (inventoryDataGridView.CurrentRow != null && inventoryDataGridView.CurrentRow.Index == _previousRowIndex)
            {
                var inventory = inventoryDataGridView.CurrentRow.DataBoundItem as Inventory;
                if (inventory != null)
                {
                    switch (treeId)
                    {
                        case 1:
                            inventoryDataGridView.CurrentRow.Cells["ProductName"].Value =
                                LastFocusedTreeView.SelectedNode.Text;
                            inventoryDataGridView.CurrentRow.Cells["ProductId"].Value = tv1Tag.Text;
                            inventory.ProductId = Convert.ToInt32(tv1Tag.Text);
                            break;
                        case 2:
                            inventoryDataGridView.CurrentRow.Cells["LocationName"].Value =
                                LastFocusedTreeView.SelectedNode.Text;
                            inventoryDataGridView.CurrentRow.Cells["LocationId"].Value = tv2Tag.Text;
                            inventory.LocationId = Convert.ToInt32(tv2Tag.Text);
                            break;
                        case 3:
                            inventoryDataGridView.CurrentRow.Cells["CategoryName"].Value =
                                LastFocusedTreeView.SelectedNode.Text;
                            inventoryDataGridView.CurrentRow.Cells["CategoryId"].Value = tv3Tag.Text;
                            inventory.CategoryId = Convert.ToInt32(tv3Tag.Text);
                            break;
                    }

                    _cnx.Entry((object)inventory).State = EntityState.Modified;
                    _cnx.SaveChanges();
                }
            }
        }
        catch (Exception)
        {
            // Any exceptions are caught and potentially displayed to the user
            // The actual displaying is currently commented out
            // MessageBox.Show(ex.Message);
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

    private static string Ancestry(TreeNodeEntity entity)
    {
        if (entity != null)
        {
            var ancestry = entity.Name;

            while (entity.Parent != null)
            {
                entity = entity.Parent;
                ancestry = entity.Name + "/" + ancestry;
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
            var treeId = Convert.ToInt32(MyRegex().Replace(LastFocusedTreeView.Name, ""));
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
                    TreeId = treeId,
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

    public void SelectNode(TreeView treeView, string path)
    {
        treeView.CollapseAll();
        var names = path.Split('/');
        var nodes = treeView.Nodes;
        TreeNode node = null;

        foreach (var name in names)
        {
            node = FindNode(nodes, name);
            if (node == null)
                return; // Node not found
            nodes = node.Nodes;
        }

        if (node != null)
        {
            treeView.SelectedNode = node;
            // node.Expand();
            treeView.SelectedNode.EnsureVisible();
        }
    }

    private TreeNode FindNode(TreeNodeCollection nodes, string name)
    {
        foreach (TreeNode node in nodes)
            if (node.Text == name)
                return node;
        return null; // Node not found
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
        AddOrUpdateControlObject("nodePath1", treeView1.SelectedNode.FullPath, null, null);
        AddOrUpdateControlObject("nodePath2", treeView2.SelectedNode.FullPath, null, null);
        AddOrUpdateControlObject("nodePath3", treeView3.SelectedNode.FullPath, null, null);

        try
        {
            _cnx.SaveChanges();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }

        UpdateControlObject("Form1Width", Width);
        UpdateControlObject("Form1Height", Height);
        UpdateControlObject("DescWidth", inventoryDataGridView.Columns[4].Width);
    }

    public void UpdateControlObject(string key, object value)
    {
        var controlObject = _cnx.ControlObjects.Find(key);
        if (controlObject != null)
        {
            switch (value)
            {
                case int intValue:
                    controlObject.ControlInt = intValue;
                    break;
                case float floatValue:
                    controlObject.ControlFloat = floatValue;
                    break;
                case string stringValue:
                    controlObject.ControlString = stringValue;
                    break;
                default:
                    throw new ArgumentException("Unsupported value type.");
            }

            _cnx.ControlObjects.Update(controlObject);
            _cnx.SaveChanges();
        }
    }


    private void Form1_Load(object sender, EventArgs e)
    {
        try
        {
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
                    var ancestry = Ancestry(node3);
                    tvAncestry3.Text = ancestry;
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

    private void UpdateTreeViewFilteringState()
    {
        _filtering = true;
        bindingSource1.EndEdit();
        _cnx.SaveChangesAsync();

        tvIncludeChildren1.Enabled = tvFilter1.Checked;
        if (!tvFilter1.Checked) tvIncludeChildren1.Checked = false;

        tvIncludeChildren2.Enabled = tvFilter2.Checked;
        if (!tvFilter2.Checked) tvIncludeChildren2.Checked = false;

        tvIncludeChildren3.Enabled = tvFilter3.Checked;
        if (!tvFilter3.Checked) tvIncludeChildren3.Checked = false;

        FilterInventoryByTreeView();
        inventoryDataGridView.Refresh();
    }

    private void TvFilter_CheckedChanged(object sender, EventArgs e)
    {
        var focusedControl = ActiveControl;
        var treeId = focusedControl!.Name[^1];
        var textbox = Controls["searchTreeView" + treeId] as TextBox;
        var checkbox = Controls["tvFilter" + treeId] as CheckBox;

        if (!checkbox.Checked && checkbox == focusedControl) textbox!.Clear();

        UpdateTreeViewFilteringState();
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

    private void FilterInventoryByText()
    {
        var searchedDescriptionsNotes = ApplyFilter(SearchInventory.Text);
        var bindingListView = new BindingListView<Inventory>(_cnx.Inventories.Local.ToList());

        if (!searchedDescriptionsNotes.Any())
        {
            bindingListView.RemoveFilter();
            UpdateButtonState(true);
        }
        else
        {
            bindingListView.ApplyFilter(i => searchedDescriptionsNotes.Contains(i.InventoryId))
                ;
            UpdateButtonState(false);
        }

        inventoryDataGridView.DataSource = bindingListView;
    }


    private void FilterInventoryByTreeView()
    {
        var selectedProductIds = ApplyFilter(treeView1, tvFilter1, tvIncludeChildren1, i => i.ProductId);
        var selectedLocations = ApplyFilter(treeView2, tvFilter2, tvIncludeChildren2, i => i.LocationId);
        var selectedCategoryIds = ApplyFilter(treeView3, tvFilter3, tvIncludeChildren3, i => i.CategoryId);
        var bindingListView = new BindingListView<Inventory>(_cnx.Inventories.Local.ToList());

        if (!selectedProductIds.Any() && !selectedLocations.Any() && !selectedCategoryIds.Any())
        {
            bindingListView.RemoveFilter();
            UpdateButtonState(true);
        }
        else
        {
            bindingListView.ApplyFilter(i => selectedProductIds.Contains(i.ProductId)
                                             || selectedLocations.Contains(i.LocationId)
                                             || selectedCategoryIds.Contains(i.CategoryId));
            UpdateButtonState(false);
        }

        bindingSource1.DataSource = bindingListView;

        inventoryDataGridView.Refresh();
    }


    private HashSet<int> ApplyFilter(string searchString)
    {
        var filteredInventories = from i in _cnx.Inventories
            join p in _cnx.TreeNodeEntities on i.ProductId equals p.Id
            join l in _cnx.TreeNodeEntities on i.LocationId equals l.Id
            join c in _cnx.TreeNodeEntities on i.CategoryId equals c.Id
            where (i.Description != null && i.Description.Contains(searchString))
                  || (i.Notes != null && i.Notes.Contains(searchString))
                  || (p.Name != null && p.Name.Contains(searchString))
                  || (l.Name != null && l.Name.Contains(searchString))
                  || (c.Name != null && c.Name.Contains(searchString))
            select i;

        var ids = new HashSet<int>();

        foreach (var inventory in filteredInventories)
        {
            ids.Add(inventory.InventoryId);
        }

        return ids;
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
     FilterInventoryByText();
    }

    public class TreeNodeTagData
    {
        public TreeNodeEntity TreeNodeEntity { get; set; }
        public int Id { get; set; }
    }
}