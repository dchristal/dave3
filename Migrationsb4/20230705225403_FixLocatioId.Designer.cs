﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dave3.Model;

#nullable disable

namespace dave3.Migrations
{
    [DbContext(typeof(DelightfulContext))]
    [Migration("20230705225403_FixLocatioId")]
    partial class FixLocatioId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("dave3.Model.Attribute", b =>
                {
                    b.Property<int>("AttributeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttributeID"));

                    b.Property<string>("AttributeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("AttributeValue")
                        .HasColumnType("real");

                    b.HasKey("AttributeID");

                    b.HasIndex(new[] { "AttributeID" }, "IX_Attributes_AttributeID")
                        .IsUnique();

                    b.ToTable("Attributes");
                });

            modelBuilder.Entity("dave3.Model.ControlObject", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float?>("ControlFloat")
                        .HasColumnType("real");

                    b.Property<int?>("ControlInt")
                        .HasColumnType("int");

                    b.Property<string>("ControlString")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Name");

                    b.ToTable("ControlObjects");
                });

            modelBuilder.Entity("dave3.Model.Inventory", b =>
                {
                    b.Property<int>("InventoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InventoryId"));

                    b.Property<float?>("Amps")
                        .HasColumnType("real");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float?>("Diameter")
                        .HasColumnType("real");

                    b.Property<float?>("Height")
                        .HasColumnType("real");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime2");

                    b.Property<float?>("Length")
                        .HasColumnType("real");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("Material")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("Pitch")
                        .HasColumnType("real");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("UoM")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("Volts")
                        .HasColumnType("real");

                    b.Property<float?>("Watts")
                        .HasColumnType("real");

                    b.Property<float?>("Weight")
                        .HasColumnType("real");

                    b.Property<float?>("Width")
                        .HasColumnType("real");

                    b.HasKey("InventoryId");

                    b.HasIndex(new[] { "ProductId", "LocationId", "CategoryId", "Description" }, "IX_Inventories_ProductId_Location_Category_Desc")
                        .IsUnique()
                        .HasFilter("[Description] IS NOT NULL");

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("dave3.Model.InventoryView", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Location")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "Location");

                    b.ToTable((string)null);

                    b.ToView("InventoryView", (string)null);
                });

            modelBuilder.Entity("dave3.Model.Node", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("NodeId")
                        .HasColumnType("int");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "NodeId" }, "IX_Nodes_NodeId");

                    b.HasIndex(new[] { "ParentId" }, "IX_Nodes_ParentId");

                    b.ToTable("Nodes");
                });

            modelBuilder.Entity("dave3.Model.TreeNodeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<int>("TreeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "ParentId" }, "IX_TreeNodeEntities_ParentId");

                    b.ToTable("TreeNodeEntities");
                });

            modelBuilder.Entity("dave3.Model.Node", b =>
                {
                    b.HasOne("dave3.Model.Node", "NodeNavigation")
                        .WithMany("InverseNodeNavigation")
                        .HasForeignKey("NodeId");

                    b.HasOne("dave3.Model.Node", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("NodeNavigation");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("dave3.Model.TreeNodeEntity", b =>
                {
                    b.HasOne("dave3.Model.TreeNodeEntity", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("dave3.Model.Node", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("InverseNodeNavigation");
                });

            modelBuilder.Entity("dave3.Model.TreeNodeEntity", b =>
                {
                    b.Navigation("Children");
                });
#pragma warning restore 612, 618
        }
    }
}
