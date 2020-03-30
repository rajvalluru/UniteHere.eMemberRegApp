namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
/*            CreateTable(
                "dbo.EMemberRegistration",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        First_Name = c.String(maxLength: 100),
                        Last_Name = c.String(maxLength: 100),
                        Middle_Name = c.String(maxLength: 50),
                        Dob = c.String(),
                        Address = c.String(maxLength: 100),
                        Address_2 = c.String(maxLength: 100),
                        City = c.String(maxLength: 100),
                        State = c.String(maxLength: 10),
                        Postal_Code = c.String(maxLength: 20),
                        Deduction_Opt_Out_Flag = c.Boolean(nullable: false),
                        Home_Phone = c.String(maxLength: 100),
                        Mobile_Phone = c.String(maxLength: 100),
                        Email = c.String(maxLength: 100),
                        Employer_Union_Id = c.String(maxLength: 20),
                        Position = c.String(maxLength: 100),
                        Date_Of_Hire = c.String(),
                        Ssn = c.String(maxLength: 12),
                        Sms_Opt_In_Flag = c.Boolean(nullable: false),
                        Race = c.String(maxLength: 20),
                        Gender = c.String(maxLength: 20),
                        Other_Gender = c.String(maxLength: 100),
                        Ethnicity = c.String(maxLength: 20),
                        Country = c.String(maxLength: 20),
                        First_Language = c.String(maxLength: 100),
                        Second_Language = c.String(maxLength: 100),
                        Tip_Opt_In_Flag = c.Boolean(nullable: false),
                        Tip_Frequency = c.String(maxLength: 20),
                        Tip_Contribution = c.String(),
                        LocalNumber = c.String(maxLength: 20),
                        Validated_Flag = c.Boolean(nullable: false),
                        Validation_Status = c.String(maxLength: 256),
                        Validation_Date = c.String(maxLength: 256),
                        Processed_Flag = c.Boolean(nullable: false),
                        Processed_Status = c.String(maxLength: 256),
                        Processed_Date = c.String(maxLength: 256),
                        Success_Flag = c.Boolean(nullable: false),
                        Member_Union_Id = c.String(maxLength: 20),
                        CreatedBy = c.String(nullable: false, maxLength: 20),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 20),
                        ModifiedOn = c.DateTime(),
                        RowVersion = c.Int(nullable: false),
                        Dues_Card_File_Name = c.String(maxLength: 100),
                        Dues_Card_Image = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Error",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ErrorCode = c.Int(nullable: false),
                        Message = c.String(),
                        StackTrace = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportDef",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 512),
                        CreatedBy = c.String(nullable: false, maxLength: 20),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 20),
                        ModifiedOn = c.DateTime(),
                        RowVersion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportParameter",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ReportDefId = c.String(maxLength: 128),
                        ParameterName = c.String(nullable: false, maxLength: 20),
                        DataType = c.String(maxLength: 20),
                        IsRequired = c.Boolean(nullable: false),
                        DefaultValue = c.String(maxLength: 128),
                        Description = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReportDef", t => t.ReportDefId)
                .Index(t => t.ReportDefId);
            
            CreateTable(
                "dbo.ReportSecurity",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ReportDefId = c.String(maxLength: 128),
                        Role = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReportDef", t => t.ReportDefId)
                .Index(t => t.ReportDefId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        LocalNumber = c.String(maxLength: 20),
                        Role = c.String(maxLength: 20),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(maxLength: 20),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
*/            
        }
        
        public override void Down()
        {
/*            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ReportSecurity", "ReportDefId", "dbo.ReportDef");
            DropForeignKey("dbo.ReportParameter", "ReportDefId", "dbo.ReportDef");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ReportSecurity", new[] { "ReportDefId" });
            DropIndex("dbo.ReportParameter", new[] { "ReportDefId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ReportSecurity");
            DropTable("dbo.ReportParameter");
            DropTable("dbo.ReportDef");
            DropTable("dbo.Error");
            DropTable("dbo.EMemberRegistration");  */
        }
    }
}
