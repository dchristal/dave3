﻿namespace dave3;

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
        treeView1 = new TreeView();
        searchTreeView1 = new TextBox();
        inventoryDataGridView = new DataGridView();
        treeView2 = new TreeView();
        treeView3 = new TreeView();
        tv3Tag = new TextBox();
        tv2Tag = new TextBox();
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
        ((System.ComponentModel.ISupportInitialize)inventoryDataGridView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
        SuspendLayout();
        // 
        // treeView1
        // 
        treeView1.Location = new Point(15, 57);
        treeView1.Name = "treeView1";
        treeView1.Size = new Size(303, 292);
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
        inventoryDataGridView.Location = new Point(15, 385);
        inventoryDataGridView.Name = "inventoryDataGridView";
        inventoryDataGridView.RowTemplate.Height = 25;
        inventoryDataGridView.Size = new Size(1139, 168);
        inventoryDataGridView.TabIndex = 3;
        inventoryDataGridView.TabStop = false;
        inventoryDataGridView.CellClick += InventoryDataGridView_CellClick;
        inventoryDataGridView.DataError += InventoryDataGridView_DataError;
        inventoryDataGridView.RowLeave += InventoryDataGridView_RowLeave;
        // 
        // treeView2
        // 
        treeView2.Location = new Point(324, 58);
        treeView2.Name = "treeView2";
        treeView2.Size = new Size(303, 292);
        treeView2.TabIndex = 1;
        // 
        // treeView3
        // 
        treeView3.Location = new Point(633, 57);
        treeView3.Name = "treeView3";
        treeView3.Size = new Size(303, 292);
        treeView3.TabIndex = 2;
        // 
        // tv3Tag
        // 
        tv3Tag.Location = new Point(630, 326);
        tv3Tag.Name = "tv3Tag";
        tv3Tag.Size = new Size(100, 23);
        tv3Tag.TabIndex = 22;
        tv3Tag.TabStop = false;
        tv3Tag.Visible = false;
        // 
        // tv2Tag
        // 
        tv2Tag.Location = new Point(321, 326);
        tv2Tag.Name = "tv2Tag";
        tv2Tag.Size = new Size(100, 23);
        tv2Tag.TabIndex = 21;
        tv2Tag.TabStop = false;
        tv2Tag.Visible = false;
        // 
        // tv1Tag
        // 
        tv1Tag.Location = new Point(12, 326);
        tv1Tag.Name = "tv1Tag";
        tv1Tag.Size = new Size(100, 23);
        tv1Tag.TabIndex = 20;
        tv1Tag.TabStop = false;
        tv1Tag.Visible = false;
        // 
        // tvName1
        // 
        tvName1.Location = new Point(117, 327);
        tvName1.Name = "tvName1";
        tvName1.Size = new Size(198, 23);
        tvName1.TabIndex = 23;
        tvName1.TabStop = false;
        tvName1.Visible = false;
        // 
        // tvName2
        // 
        tvName2.Location = new Point(426, 326);
        tvName2.Name = "tvName2";
        tvName2.Size = new Size(198, 23);
        tvName2.TabIndex = 24;
        tvName2.TabStop = false;
        tvName2.Visible = false;
        // 
        // tvName3
        // 
        tvName3.Location = new Point(736, 326);
        tvName3.Name = "tvName3";
        tvName3.Size = new Size(198, 23);
        tvName3.TabIndex = 25;
        tvName3.TabStop = false;
        tvName3.Visible = false;
        // 
        // tvAncestry1
        // 
        tvAncestry1.Location = new Point(15, 356);
        tvAncestry1.Name = "tvAncestry1";
        tvAncestry1.Size = new Size(303, 23);
        tvAncestry1.TabIndex = 26;
        tvAncestry1.TabStop = false;
        // 
        // tvAncestry2
        // 
        tvAncestry2.Location = new Point(324, 356);
        tvAncestry2.Name = "tvAncestry2";
        tvAncestry2.Size = new Size(303, 23);
        tvAncestry2.TabIndex = 27;
        tvAncestry2.TabStop = false;
        // 
        // tvAncestry3
        // 
        tvAncestry3.Location = new Point(633, 355);
        tvAncestry3.Name = "tvAncestry3";
        tvAncestry3.Size = new Size(303, 23);
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
        searchTreeView2.Location = new Point(324, 12);
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
        searchTreeView3.Location = new Point(637, 14);
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
        tvIncludeChildren1.Location = new Point(235, 33);
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
        SearchInventory.Location = new Point(947, 12);
        SearchInventory.Name = "SearchInventory";
        SearchInventory.Size = new Size(207, 23);
        SearchInventory.TabIndex = 38;
        SearchInventory.TabStop = false;
        SearchInventory.Leave += SearchInventory_Leave;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1166, 540);
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
    private TreeView treeView1;
    private TextBox searchTreeView1;
    private DataGridView inventoryDataGridView;
    private TreeView treeView2;
    private TreeView treeView3;
    private TextBox tv3Tag;
    private TextBox tv2Tag;
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
}
