namespace InfraManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_UpdatedDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Org", "UpdateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Task", "UpdateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Task", "UpdateTime");
            DropColumn("dbo.Org", "UpdateTime");
        }
    }
}
