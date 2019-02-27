namespace InfraManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Org",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        CompanyAddress_Zip = c.String(),
                        CompanyAddress_Address1 = c.String(),
                        CompanyAddress_Address2 = c.String(),
                        CompanyAddress_City = c.String(),
                        CompanyAddress_State = c.String(),
                        CompanyAddress_Country = c.String(),
                        CustomerPaymentProfileId = c.String(),
                        CustomerProfileId = c.String(),
                        CompanyShortName = c.String(),
                        CompanyFullName = c.String(),
                        EmailAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ServiceTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StatusUrl = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Task",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrgId = c.Int(nullable: false),
                        Name = c.String(),
                        StatusUrl = c.String(),
                        Status = c.String(),
                        Notes = c.String(),
                        TaskType = c.Int(nullable: false),
                        IsLRP = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Vdc",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrgId = c.Int(nullable: false),
                        OrdVdc = c.String(),
                        OrgType = c.String(),
                        Href = c.String(),
                        OrdEdgeGateWay = c.String(),
                        OrgAdminRole = c.String(),
                        AdminUserHref = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Vdc");
            DropTable("dbo.Task");
            DropTable("dbo.ServiceTasks");
            DropTable("dbo.Org");
        }
    }
}
