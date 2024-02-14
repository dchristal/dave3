namespace dave3;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
    #region Windows Form Designer generated code
    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        treeView1 = new MyTreeView();
        searchTreeView1 = new TextBox();
        inventoryDataGridView = new DataGridView();
        treeView2 = new MyTreeView();
        treeView3 = new MyTreeView();
        tv3Tag = new TextBox();
        tv2Tag = new CustomTextBox();
        tv1Tag = new TextBox();
        tvName1 = new TextBox();
        tvName2 = new TextBox();
        tvName3 = new TextBox();
        tvAncestry1 = new TextBox();
        tvAncestry2 = new TextBox();
        tvAncestry3 = new TextBox();
        tvFilter1 = new CheckBox();
        bindingSource1 = new BindingSource(components);
        tvFilter2 = new CheckBox();
        searchTreeView2 = new TextBox();
        tvFilter3 = new CheckBox();
        searchTreeView3 = new TextBox();
        AddNew = new Button();
        tvIncludeChildren1 = new CheckBox();
        tvIncludeChildren2 = new CheckBox();
        tvIncludeChildren3 = new CheckBox();
        SearchInventory = new TextBox();
        tvName2a = new TextBox();
        tv2aTag = new CustomTextBox();
        FilterStatus = new Button();
        ((System.ComponentModel.ISupportInitialize)inventoryDataGridView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
        SuspendLayout();
        // 
        // treeView1
        // 
        treeView1.HideSelection = false;
        treeView1.Location = new Point(15, 57);
        treeView1.Name = "treeView1";
        treeView1.Size = new Size(303, 182);
        treeView1.TabIndex = 0;
        // 
        // searchTreeView1
        // 
        searchTreeView1.Location = new Point(15, 10);
        searchTreeView1.Name = "searchTreeView1";
        searchTreeView1.Size = new Size(207, 23);
        searchTreeView1.TabIndex = 1;
        searchTreeView1.TabStop = false;
        // 
        // inventoryDataGridView
        // 
        inventoryDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        inventoryDataGridView.Location = new Point(17, 377);
        inventoryDataGridView.Name = "inventoryDataGridView";
        inventoryDataGridView.RowTemplate.Height = 25;
        inventoryDataGridView.Size = new Size(1139, 342);
        inventoryDataGridView.TabIndex = 3;
        inventoryDataGridView.TabStop = false;
        inventoryDataGridView.CellClick += InventoryDataGridView_CellClick;
        inventoryDataGridView.DataError += InventoryDataGridView_DataError;
        // 
        // treeView2
        // 
        treeView2.HideSelection = false;
        treeView2.Location = new Point(324, 58);
        treeView2.Name = "treeView2";
        treeView2.Size = new Size(303, 182);
        treeView2.TabIndex = 1;
        // 
        // treeView3
        // 
        treeView3.HideSelection = false;
        treeView3.Location = new Point(633, 57);
        treeView3.Name = "treeView3";
        treeView3.Size = new Size(303, 182);
        treeView3.TabIndex = 2;
        // 
        // tv3Tag
        // 
        tv3Tag.Location = new Point(635, 264);
        tv3Tag.Name = "tv3Tag";
        tv3Tag.Size = new Size(100, 23);
        tv3Tag.TabIndex = 22;
        tv3Tag.TabStop = false;
        // 
        // tv2Tag
        // 
        tv2Tag.Location = new Point(326, 264);
        tv2Tag.Name = "tv2Tag";
        tv2Tag.Size = new Size(100, 23);
        tv2Tag.TabIndex = 21;
        tv2Tag.TabStop = false;
        // 
        // tv1Tag
        // 
        tv1Tag.Location = new Point(17, 264);
        tv1Tag.Name = "tv1Tag";
        tv1Tag.Size = new Size(100, 23);
        tv1Tag.TabIndex = 20;
        tv1Tag.TabStop = false;
        // 
        // tvName1
        // 
        tvName1.Location = new Point(122, 265);
        tvName1.Name = "tvName1";
        tvName1.Size = new Size(198, 23);
        tvName1.TabIndex = 23;
        tvName1.TabStop = false;
        // 
        // tvName2
        // 
        tvName2.Location = new Point(431, 264);
        tvName2.Name = "tvName2";
        tvName2.Size = new Size(198, 23);
        tvName2.TabIndex = 24;
        tvName2.TabStop = false;
        // 
        // tvName3
        // 
        tvName3.Location = new Point(741, 264);
        tvName3.Name = "tvName3";
        tvName3.Size = new Size(198, 23);
        tvName3.TabIndex = 25;
        tvName3.TabStop = false;
        // 
        // tvAncestry1
        // 
        tvAncestry1.Location = new Point(20, 294);
        tvAncestry1.Name = "tvAncestry1";
        tvAncestry1.Size = new Size(917, 23);
        tvAncestry1.TabIndex = 26;
        tvAncestry1.TabStop = false;
        // 
        // tvAncestry2
        // 
        tvAncestry2.Location = new Point(122, 323);
        tvAncestry2.Name = "tvAncestry2";
        tvAncestry2.Size = new Size(917, 23);
        tvAncestry2.TabIndex = 27;
        tvAncestry2.TabStop = false;
        // 
        // tvAncestry3
        // 
        tvAncestry3.Location = new Point(240, 352);
        tvAncestry3.Name = "tvAncestry3";
        tvAncestry3.Size = new Size(917, 23);
        tvAncestry3.TabIndex = 28;
        tvAncestry3.TabStop = false;
        // 
        // tvFilter1
        // 
        tvFilter1.AutoSize = true;
        tvFilter1.Location = new Point(235, 10);
        tvFilter1.Name = "tvFilter1";
        tvFilter1.Size = new Size(52, 19);
        tvFilter1.TabIndex = 29;
        tvFilter1.TabStop = false;
        tvFilter1.Text = "Filter";
        tvFilter1.UseVisualStyleBackColor = true;
        // 
        // tvFilter2
        // 
        tvFilter2.AutoSize = true;
        tvFilter2.Location = new Point(544, 12);
        tvFilter2.Name = "tvFilter2";
        tvFilter2.Size = new Size(52, 19);
        tvFilter2.TabIndex = 31;
        tvFilter2.TabStop = false;
        tvFilter2.Text = "Filter";
        tvFilter2.UseVisualStyleBackColor = true;
        // 
        // searchTreeView2
        // 
        searchTreeView2.Location = new Point(324, 8);
        searchTreeView2.Name = "searchTreeView2";
        searchTreeView2.Size = new Size(207, 23);
        searchTreeView2.TabIndex = 30;
        searchTreeView2.TabStop = false;
        // 
        // tvFilter3
        // 
        tvFilter3.AutoSize = true;
        tvFilter3.Location = new Point(850, 12);
        tvFilter3.Name = "tvFilter3";
        tvFilter3.Size = new Size(52, 19);
        tvFilter3.TabIndex = 33;
        tvFilter3.TabStop = false;
        tvFilter3.Text = "Filter";
        tvFilter3.UseVisualStyleBackColor = true;
        // 
        // searchTreeView3
        // 
        searchTreeView3.Location = new Point(637, 10);
        searchTreeView3.Name = "searchTreeView3";
        searchTreeView3.Size = new Size(207, 23);
        searchTreeView3.TabIndex = 32;
        searchTreeView3.TabStop = false;
        // 
        // AddNew
        // 
        AddNew.Location = new Point(976, 62);
        AddNew.Name = "AddNew";
        AddNew.Size = new Size(75, 23);
        AddNew.TabIndex = 34;
        AddNew.Text = "+";
        AddNew.UseVisualStyleBackColor = true;
        AddNew.Click += AddNew_Click;
        // 
        // tvIncludeChildren1
        // 
        tvIncludeChildren1.AutoSize = true;
        tvIncludeChildren1.Location = new Point(235, 35);
        tvIncludeChildren1.Name = "tvIncludeChildren1";
        tvIncludeChildren1.Size = new Size(93, 19);
        tvIncludeChildren1.TabIndex = 35;
        tvIncludeChildren1.TabStop = false;
        tvIncludeChildren1.Text = "Incl Children";
        tvIncludeChildren1.UseVisualStyleBackColor = true;
        // 
        // tvIncludeChildren2
        // 
        tvIncludeChildren2.AutoSize = true;
        tvIncludeChildren2.Location = new Point(544, 33);
        tvIncludeChildren2.Name = "tvIncludeChildren2";
        tvIncludeChildren2.Size = new Size(93, 19);
        tvIncludeChildren2.TabIndex = 36;
        tvIncludeChildren2.TabStop = false;
        tvIncludeChildren2.Text = "Incl Children";
        tvIncludeChildren2.UseVisualStyleBackColor = true;
        // 
        // tvIncludeChildren3
        // 
        tvIncludeChildren3.AutoSize = true;
        tvIncludeChildren3.Location = new Point(850, 33);
        tvIncludeChildren3.Name = "tvIncludeChildren3";
        tvIncludeChildren3.Size = new Size(93, 19);
        tvIncludeChildren3.TabIndex = 37;
        tvIncludeChildren3.TabStop = false;
        tvIncludeChildren3.Text = "Incl Children";
        tvIncludeChildren3.UseVisualStyleBackColor = true;
        // 
        // SearchInventory
        // 
        SearchInventory.Location = new Point(949, 8);
        SearchInventory.Name = "SearchInventory";
        SearchInventory.Size = new Size(207, 23);
        SearchInventory.TabIndex = 38;
        SearchInventory.TabStop = false;
        SearchInventory.Leave += SearchInventory_Leave;
        // 
        // tvName2a
        // 
        tvName2a.Location = new Point(431, 235);
        tvName2a.Name = "tvName2a";
        tvName2a.Size = new Size(198, 23);
        tvName2a.TabIndex = 41;
        tvName2a.TabStop = false;
        // 
        // tv2aTag
        // 
        tv2aTag.Location = new Point(326, 235);
        tv2aTag.Name = "tv2aTag";
        tv2aTag.Size = new Size(100, 23);
        tv2aTag.TabIndex = 40;
        tv2aTag.TabStop = false;
        // 
        // FilterStatus
        // 
        FilterStatus.Location = new Point(976, 129);
        FilterStatus.Name = "FilterStatus";
        FilterStatus.Size = new Size(75, 23);
        FilterStatus.TabIndex = 42;
        FilterStatus.Text = "button1";
        FilterStatus.UseVisualStyleBackColor = true;
        FilterStatus.Click += FilterStatus_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1178, 800);
        Controls.Add(FilterStatus);
        Controls.Add(tvName2a);
        Controls.Add(tv2aTag);
        Controls.Add(SearchInventory);
        Controls.Add(tvIncludeChildren3);
        Controls.Add(tvIncludeChildren2);
        Controls.Add(tvIncludeChildren1);
        Controls.Add(AddNew);
        Controls.Add(tvFilter3);
        Controls.Add(searchTreeView3);
        Controls.Add(tvFilter2);
        Controls.Add(searchTreeView2);
        Controls.Add(tvFilter1);
        Controls.Add(tvAncestry3);
        Controls.Add(tvAncestry2);
        Controls.Add(tvAncestry1);
        Controls.Add(tvName3);
        Controls.Add(tvName2);
        Controls.Add(tvName1);
        Controls.Add(tv3Tag);
        Controls.Add(tv2Tag);
        Controls.Add(tv1Tag);
        Controls.Add(treeView3);
        Controls.Add(treeView2);
        Controls.Add(inventoryDataGridView);
        Controls.Add(searchTreeView1);
        Controls.Add(treeView1);
        Name = "Form1";
        Text = "Form1";
        FormClosing += Form1_FormClosing;
        Load += Form1_Load;
        ((System.ComponentModel.ISupportInitialize)inventoryDataGridView).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
    #endregion
    private MyTreeView treeView1;
    private TextBox searchTreeView1;
    private DataGridView inventoryDataGridView;
    private MyTreeView treeView2;
    private MyTreeView treeView3;
    private TextBox tv3Tag;
    private CustomTextBox tv2Tag;
    private TextBox tv1Tag;
    private TextBox tvName1;
    private TextBox tvName2;
    private TextBox tvName3;
    private TextBox tvAncestry1;
    private TextBox tvAncestry2;
    private TextBox tvAncestry3;
    private CheckBox tvFilter1;
    private BindingSource bindingSource1;
    private CheckBox tvFilter2;
    private TextBox searchTreeView2;
    private CheckBox tvFilter3;
    private TextBox searchTreeView3;
    private Button AddNew;
    private CheckBox tvIncludeChildren1;
    private CheckBox tvIncludeChildren2;
    private CheckBox tvIncludeChildren3;
    private TextBox SearchInventory;
    private TextBox tvName2a;
    private CustomTextBox tv2aTag;
    private Button FilterStatus;
}
