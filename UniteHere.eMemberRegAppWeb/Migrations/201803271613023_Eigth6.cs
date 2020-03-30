namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eigth6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMemberRegistration", "Country_Of_Origin", c => c.String(maxLength: 5));
            AddColumn("dbo.EMemberRegistration", "Dicard_Reason", c => c.String(maxLength: 100));
            DropColumn("dbo.EMemberRegistration", "CountryOfOrigin");
            DropColumn("dbo.EMemberRegistration", "DicardReason");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EMemberRegistration", "DicardReason", c => c.String(maxLength: 100));
            AddColumn("dbo.EMemberRegistration", "CountryOfOrigin", c => c.String(maxLength: 5));
            DropColumn("dbo.EMemberRegistration", "Dicard_Reason");
            DropColumn("dbo.EMemberRegistration", "Country_Of_Origin");
        }
    }
}
