namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eigth2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EMemberRegistration", "CountryOfOrigin", c => c.String(maxLength: 5));
            AlterColumn("dbo.EMemberRegistration", "DicardReason", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EMemberRegistration", "DicardReason", c => c.String());
            AlterColumn("dbo.EMemberRegistration", "CountryOfOrigin", c => c.String());
        }
    }
}
