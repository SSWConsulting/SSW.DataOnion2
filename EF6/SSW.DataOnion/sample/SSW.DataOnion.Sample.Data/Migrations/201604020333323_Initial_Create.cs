namespace SSW.DataOnion.Sample.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        Suburb = c.String(),
                        State = c.String(),
                        Postcode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Schools",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Address_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.Address_Id, cascadeDelete: true)
                .Index(t => t.Address_Id);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        School_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Schools", t => t.School_Id, cascadeDelete: true)
                .Index(t => t.School_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "School_Id", "dbo.Schools");
            DropForeignKey("dbo.Schools", "Address_Id", "dbo.Addresses");
            DropIndex("dbo.Students", new[] { "School_Id" });
            DropIndex("dbo.Schools", new[] { "Address_Id" });
            DropTable("dbo.Students");
            DropTable("dbo.Schools");
            DropTable("dbo.Addresses");
        }
    }
}
