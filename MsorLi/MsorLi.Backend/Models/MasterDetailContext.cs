using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server.Tables;
using MsorLi.DataObjects;

namespace MsorLi.Backend.Models
{
    public class MasterDetailContext : DbContext
    {
        private const string connectionStringName = "Name=MS_TableConnectionString";

        public MasterDetailContext() : base(connectionStringName)
        {
        }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }
    }
}