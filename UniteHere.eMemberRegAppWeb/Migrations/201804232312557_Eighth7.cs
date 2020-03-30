namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eighth7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMemberRegistration", "Discard_Reason", c => c.String(maxLength: 100));
            DropColumn("dbo.EMemberRegistration", "Dicard_Reason");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EMemberRegistration", "Dicard_Reason", c => c.String(maxLength: 100));
            DropColumn("dbo.EMemberRegistration", "Discard_Reason");
        }
    }
}
