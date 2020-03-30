namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eigth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMemberRegistration", "CountryOfOrigin", c => c.String());
            AddColumn("dbo.EMemberRegistration", "DicardReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMemberRegistration", "DicardReason");
            DropColumn("dbo.EMemberRegistration", "CountryOfOrigin");
        }
    }
}
