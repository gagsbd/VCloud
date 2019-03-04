namespace InfraManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Tenant_id : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Org", "TenantId", c => c.String());
            DropColumn("dbo.Org", "Url");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Org", "Url", c => c.String());
            DropColumn("dbo.Org", "TenantId");
        }
    }
}
