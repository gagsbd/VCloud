namespace InfraManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Admin_user_to_org : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Org", "AdminName", c => c.String());
            AddColumn("dbo.Org", "AdminPassword", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Org", "AdminPassword");
            DropColumn("dbo.Org", "AdminName");
        }
    }
}
