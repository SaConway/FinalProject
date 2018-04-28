using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server.Tables;
using MsorLiService.DataObjects;

namespace MsorLiService.Models
{
    public class MsorLiContext : DbContext
    {
        private const string connectionStringName = "Name=MS_TableConnectionString";

        public MsorLiContext() : base(connectionStringName)
        {
        } 

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<ItemSubCategory> ItemSubCategories { get; set; }
        public DbSet<SavedItem> SavedItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }
    }
}