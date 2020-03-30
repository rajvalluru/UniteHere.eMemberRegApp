namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eigth4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EMemberRegistration", "Dues_Card_Signed_Date", c => c.DateTime());
            AlterColumn("dbo.EMemberRegistration", "TIP_Card_Signed_Date", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EMemberRegistration", "TIP_Card_Signed_Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EMemberRegistration", "Dues_Card_Signed_Date", c => c.DateTime(nullable: false));
        }
    }
}
