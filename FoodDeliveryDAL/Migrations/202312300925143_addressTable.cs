namespace FoodDeliveryDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addressTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HouseNo = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        PostalCode = c.String(),
                        Country = c.String(),
                        CustomerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Addresses", new[] { "CustomerId" });
            DropTable("dbo.Addresses");
        }
    }
}
