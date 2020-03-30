namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eigth3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMemberRegistration", "Dues_Card_Signed_Date", c => c.DateTime(nullable: true));
            AddColumn("dbo.EMemberRegistration", "TIP_Card_Signed_Date", c => c.DateTime(nullable: true));
            AddColumn("dbo.EMemberRegistration", "Student_Flag", c => c.Boolean(nullable: true));
            AddColumn("dbo.EMemberRegistration", "Beneficiary", c => c.String(maxLength: 100));
            AddColumn("dbo.EMemberRegistration", "Work_Phone", c => c.String(maxLength: 100));
            AddColumn("dbo.EMemberRegistration", "DuesCard_BoxId", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMemberRegistration", "DuesCard_BoxId");
            DropColumn("dbo.EMemberRegistration", "Work_Phone");
            DropColumn("dbo.EMemberRegistration", "Beneficiary");
            DropColumn("dbo.EMemberRegistration", "Student_Flag");
            DropColumn("dbo.EMemberRegistration", "TIP_Card_Signed_Date");
            DropColumn("dbo.EMemberRegistration", "Dues_Card_Signed_Date");
        }
    }
}
