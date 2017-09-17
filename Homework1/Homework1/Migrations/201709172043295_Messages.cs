namespace Homework1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageBody = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        SenderId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.SenderId, cascadeDelete: false)
              .ForeignKey("dbo.AspNetUsers", t => t.ReceiverId, cascadeDelete: false);

        }
        
        public override void Down()
        {
            DropTable("dbo.Messages");
        }
    }
}
