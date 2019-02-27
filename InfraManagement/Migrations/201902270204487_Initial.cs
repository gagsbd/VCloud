namespace InfraManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Org", "Address_Zip", c => c.String());
            AddColumn("dbo.Org", "Address_Address1", c => c.String());
            AddColumn("dbo.Org", "Address_Address2", c => c.String());
            AddColumn("dbo.Org", "Address_City", c => c.String());
            AddColumn("dbo.Org", "Address_State", c => c.String());
            AddColumn("dbo.Org", "Address_Country", c => c.String());
            DropColumn("dbo.Org", "CompanyAddress_Zip");
            DropColumn("dbo.Org", "CompanyAddress_Address1");
            DropColumn("dbo.Org", "CompanyAddress_Address2");
            DropColumn("dbo.Org", "CompanyAddress_City");
            DropColumn("dbo.Org", "CompanyAddress_State");
            DropColumn("dbo.Org", "CompanyAddress_Country");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Org", "CompanyAddress_Country", c => c.String());
            AddColumn("dbo.Org", "CompanyAddress_State", c => c.String());
            AddColumn("dbo.Org", "CompanyAddress_City", c => c.String());
            AddColumn("dbo.Org", "CompanyAddress_Address2", c => c.String());
            AddColumn("dbo.Org", "CompanyAddress_Address1", c => c.String());
            AddColumn("dbo.Org", "CompanyAddress_Zip", c => c.String());
            DropColumn("dbo.Org", "Address_Country");
            DropColumn("dbo.Org", "Address_State");
            DropColumn("dbo.Org", "Address_City");
            DropColumn("dbo.Org", "Address_Address2");
            DropColumn("dbo.Org", "Address_Address1");
            DropColumn("dbo.Org", "Address_Zip");
        }
    }
}
