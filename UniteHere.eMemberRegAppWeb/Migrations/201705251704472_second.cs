namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
    /*        CreateTable(
                "dbo.EMemberRegistrationViewModel",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        First_Name = c.String(),
                        Last_Name = c.String(),
                        Middle_Name = c.String(),
                        Ssn = c.String(),
                        Date_Of_Hire = c.DateTime(nullable: false),
                        Mobile_Phone = c.String(),
                        Employer_Union_Id = c.String(),
                        Position = c.String(),
                        Member_Union_Id = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        LocalNumber = c.String(),
                        Dues_Card_File_Name = c.String(),
                        Processed_Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.EMemberRegistration", "Mbr_Level1", c => c.String(maxLength: 20));
            AddColumn("dbo.EMemberRegistration", "Mbr_Level2", c => c.String(maxLength: 20));
            AddColumn("dbo.EMemberRegistration", "Mbr_Level3", c => c.String(maxLength: 20));
            AddColumn("dbo.EMemberRegistration", "Mbr_Type", c => c.String(maxLength: 20));
            AddColumn("dbo.EMemberRegistration", "Employer_Name", c => c.String(maxLength: 100));
            AddColumn("dbo.EMemberRegistration", "UpdateName", c => c.Boolean(nullable: false));
            AddColumn("dbo.EMemberRegistration", "UpdateAddress", c => c.Boolean(nullable: false));
            AddColumn("dbo.EMemberRegistration", "UpdateHouse", c => c.Boolean(nullable: false));
            AddColumn("dbo.EMemberRegistration", "UpdatePhone", c => c.Boolean(nullable: false));
            AddColumn("dbo.EMemberRegistration", "UpdateEmail", c => c.Boolean(nullable: false));
            AlterColumn("dbo.EMemberRegistration", "Dob", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EMemberRegistration", "Date_Of_Hire", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EMemberRegistration", "Tip_Contribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EMemberRegistration", "Validation_Date", c => c.DateTime());
            AlterColumn("dbo.EMemberRegistration", "Processed_Date", c => c.DateTime());  */
        }
        
        public override void Down()
        {
/*            AlterColumn("dbo.EMemberRegistration", "Processed_Date", c => c.String(maxLength: 256));
            AlterColumn("dbo.EMemberRegistration", "Validation_Date", c => c.String(maxLength: 256));
            AlterColumn("dbo.EMemberRegistration", "Tip_Contribution", c => c.String());
            AlterColumn("dbo.EMemberRegistration", "Date_Of_Hire", c => c.String());
            AlterColumn("dbo.EMemberRegistration", "Dob", c => c.String());
            DropColumn("dbo.EMemberRegistration", "UpdateEmail");
            DropColumn("dbo.EMemberRegistration", "UpdatePhone");
            DropColumn("dbo.EMemberRegistration", "UpdateHouse");
            DropColumn("dbo.EMemberRegistration", "UpdateAddress");
            DropColumn("dbo.EMemberRegistration", "UpdateName");
            DropColumn("dbo.EMemberRegistration", "Employer_Name");
            DropColumn("dbo.EMemberRegistration", "Mbr_Type");
            DropColumn("dbo.EMemberRegistration", "Mbr_Level3");
            DropColumn("dbo.EMemberRegistration", "Mbr_Level2");
            DropColumn("dbo.EMemberRegistration", "Mbr_Level1");
            DropTable("dbo.EMemberRegistrationViewModel"); */
        }
    }
}
