namespace InfraManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_task_code : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Task", "TaskCode", c => c.String());
            AddColumn("dbo.Task", "Predecessor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Task", "Predecessor");
            DropColumn("dbo.Task", "TaskCode");
        }
    }
}
