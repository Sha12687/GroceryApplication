namespace FoodDeliveryDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removevalidatiion : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Admins", "IX_UniqueAdminEmail");
            DropIndex("dbo.Admins", "IX_UniqueAdminUserName");
            DropIndex("dbo.Customers", "IX_UniqueEmpUserEmail");
            DropIndex("dbo.Customers", "IX_UniqueEmpUserName");
            AlterColumn("dbo.Admins", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Admins", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Admins", "Email", c => c.String(maxLength: 255));
            AlterColumn("dbo.Admins", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Admins", "UserName", c => c.String(maxLength: 255));
            AlterColumn("dbo.Admins", "Password", c => c.String(maxLength: 255));
            AlterColumn("dbo.Customers", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "Email", c => c.String(maxLength: 255));
            AlterColumn("dbo.Customers", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Customers", "UserName", c => c.String(maxLength: 255));
            AlterColumn("dbo.Customers", "Password", c => c.String(maxLength: 255));
            CreateIndex("dbo.Admins", "Email", unique: true, name: "IX_UniqueAdminEmail");
            CreateIndex("dbo.Admins", "UserName", unique: true, name: "IX_UniqueAdminUserName");
            CreateIndex("dbo.Customers", "Email", unique: true, name: "IX_UniqueEmpUserEmail");
            CreateIndex("dbo.Customers", "UserName", unique: true, name: "IX_UniqueEmpUserName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Customers", "IX_UniqueEmpUserName");
            DropIndex("dbo.Customers", "IX_UniqueEmpUserEmail");
            DropIndex("dbo.Admins", "IX_UniqueAdminUserName");
            DropIndex("dbo.Admins", "IX_UniqueAdminEmail");
            AlterColumn("dbo.Customers", "Password", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Customers", "UserName", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Customers", "PhoneNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "Email", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Customers", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Admins", "Password", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Admins", "UserName", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Admins", "PhoneNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Admins", "Email", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Admins", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Admins", "FirstName", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.Customers", "UserName", unique: true, name: "IX_UniqueEmpUserName");
            CreateIndex("dbo.Customers", "Email", unique: true, name: "IX_UniqueEmpUserEmail");
            CreateIndex("dbo.Admins", "UserName", unique: true, name: "IX_UniqueAdminUserName");
            CreateIndex("dbo.Admins", "Email", unique: true, name: "IX_UniqueAdminEmail");
        }
    }
}
