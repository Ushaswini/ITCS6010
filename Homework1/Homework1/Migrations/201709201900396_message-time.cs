namespace Homework1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class messagetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "MessageTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "MessageTime");
        }
    }
}
