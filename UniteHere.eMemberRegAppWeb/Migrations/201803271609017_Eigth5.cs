namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eigth5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMemberRegistration", "Dues_Card_BoxId", c => c.String(maxLength: 256));
            DropColumn("dbo.EMemberRegistration", "DuesCard_BoxId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EMemberRegistration", "DuesCard_BoxId", c => c.String(maxLength: 256));
            DropColumn("dbo.EMemberRegistration", "Dues_Card_BoxId");
        }
    }
}
