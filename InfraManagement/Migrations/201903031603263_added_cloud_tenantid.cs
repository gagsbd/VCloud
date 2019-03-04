namespace InfraManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_cloud_tenantid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Org", "Cloud_TenantId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Org", "Cloud_TenantId");
        }
    }
}
