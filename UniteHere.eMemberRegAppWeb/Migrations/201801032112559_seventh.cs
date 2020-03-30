namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seventh : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.EMemberRegistrationViewModel", "Deduction_Opt_Out_Flag", c => c.Boolean(nullable: false));
            //AddColumn("dbo.EMemberRegistrationViewModel", "Sms_Opt_In_Flag", c => c.Boolean(nullable: false));
            //AddColumn("dbo.EMemberRegistrationViewModel", "Tip_Contribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            //DropColumn("dbo.EMemberRegistrationViewModel", "Tip_Contribution");
            //DropColumn("dbo.EMemberRegistrationViewModel", "Sms_Opt_In_Flag");
            //DropColumn("dbo.EMemberRegistrationViewModel", "Deduction_Opt_Out_Flag");
        }
    }
}
