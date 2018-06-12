namespace ProductAPI_Server.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ProductAPI_Server.Models;
    internal sealed class Configuration : DbMigrationsConfiguration<ProductAPI_Server.Models.ProductServiceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProductAPI_Server.Models.ProductServiceContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Products.AddOrUpdate(x => x.ProductID,
                new Product() { ProductID = 1, ProductName = "Caphe", Quantity = 10, UnitPrice = 20 },
                new Product() { ProductID = 2, ProductName = "Coca", Quantity = 20, UnitPrice = 30 },
                new Product() { ProductID = 3, ProductName = "A", Quantity = 20, UnitPrice = 30 },
                new Product() { ProductID = 4, ProductName = "B", Quantity = 20, UnitPrice = 30 }
            );
            context.TKs.AddOrUpdate(x => x.Id,
                new TK() { Id = "aaa", Password = "123"},
                new TK() { Id = "abc", Password = "123" },
                new TK() { Id = "a123", Password = "123" }
            );
        }
    }
}
