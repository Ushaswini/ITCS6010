namespace Homework1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class messagelock : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        RegionId = c.String(nullable: false, maxLength: 128),
                        RegionName = c.String(),
                    })
                .PrimaryKey(t => t.RegionId);
            
            AddColumn("dbo.Messages", "RegionId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Messages", "IsUnLocked", c => c.Boolean(nullable: false));
            AddForeignKey("dbo.Messages", "RegionId", "dbo.Regions");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "IsUnLocked");
            DropColumn("dbo.Messages", "RegionId");
            DropTable("dbo.Regions");
        }
    }
}
