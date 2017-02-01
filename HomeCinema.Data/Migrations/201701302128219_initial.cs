using System.Data.Entity.Migrations;

namespace HomeCinema.Data.Migrations
{
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.Claim",
                c =>
                    new
                        {
                            ID = c.Int(nullable: false, identity: true),
                            ClaimType = c.String(nullable: false, maxLength: 50),
                            ClaimValue = c.String(nullable: false, maxLength: 50),
                            Description = c.String(maxLength: 200),
                            IsSystemDefault = c.Boolean(nullable: false),
                        }).PrimaryKey(t => t.ID);

            this.CreateTable(
                "dbo.Customer",
                c =>
                    new
                        {
                            ID = c.Int(nullable: false, identity: true),
                            FirstName = c.String(nullable: false, maxLength: 100),
                            LastName = c.String(nullable: false, maxLength: 100),
                            Email = c.String(nullable: false, maxLength: 200),
                            IdentityCard = c.String(nullable: false, maxLength: 50),
                            UniqueKey = c.Guid(nullable: false),
                            DateOfBirth = c.DateTime(nullable: false),
                            Mobile = c.String(maxLength: 10),
                            RegistrationDate = c.DateTime(nullable: false),
                        }).PrimaryKey(t => t.ID);

            this.CreateTable("dbo.Error", c => new { ID = c.Int(nullable: false, identity: true), Message = c.String(), StackTrace = c.String(), DateCreated = c.DateTime(nullable: false), })
                .PrimaryKey(t => t.ID);

            this.CreateTable("dbo.Genre", c => new { ID = c.Int(nullable: false, identity: true), Name = c.String(nullable: false, maxLength: 50), }).PrimaryKey(t => t.ID);

            this.CreateTable(
                "dbo.Movie",
                c =>
                    new
                        {
                            ID = c.Int(nullable: false, identity: true),
                            Title = c.String(nullable: false, maxLength: 100),
                            Description = c.String(nullable: false, maxLength: 2000),
                            Image = c.String(),
                            GenreId = c.Int(nullable: false),
                            Director = c.String(nullable: false, maxLength: 100),
                            Writer = c.String(nullable: false, maxLength: 50),
                            Producer = c.String(nullable: false, maxLength: 50),
                            ReleaseDate = c.DateTime(nullable: false),
                            Rating = c.Byte(nullable: false),
                            TrailerURI = c.String(maxLength: 200),
                        }).PrimaryKey(t => t.ID).ForeignKey("dbo.Genre", t => t.GenreId, cascadeDelete: true).Index(t => t.GenreId);

            this.CreateTable(
                    "dbo.Stock",
                    c => new { ID = c.Int(nullable: false, identity: true), MovieId = c.Int(nullable: false), UniqueKey = c.Guid(nullable: false), IsAvailable = c.Boolean(nullable: false), })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Movie", t => t.MovieId, cascadeDelete: true)
                .Index(t => t.MovieId);

            this.CreateTable(
                "dbo.Rental",
                c =>
                    new
                        {
                            ID = c.Int(nullable: false, identity: true),
                            CustomerId = c.Int(nullable: false),
                            StockId = c.Int(nullable: false),
                            RentalDate = c.DateTime(nullable: false),
                            ReturnedDate = c.DateTime(),
                            Status = c.String(nullable: false, maxLength: 10),
                        }).PrimaryKey(t => t.ID).ForeignKey("dbo.Stock", t => t.StockId, cascadeDelete: true).Index(t => t.StockId);

            this.CreateTable("dbo.RoleClaim", c => new { ID = c.Int(nullable: false, identity: true), RoleId = c.Int(nullable: false), ClaimId = c.Int(nullable: false), })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Claim", t => t.ClaimId, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.ClaimId);

            this.CreateTable(
                "dbo.Role",
                c =>
                    new
                        {
                            ID = c.Int(nullable: false, identity: true),
                            Name = c.String(nullable: false, maxLength: 50),
                            Description = c.String(maxLength: 200),
                            IsSystemDefault = c.Boolean(nullable: false),
                        }).PrimaryKey(t => t.ID);

            this.CreateTable("dbo.UserRole", c => new { ID = c.Int(nullable: false, identity: true), UserId = c.Int(nullable: false), RoleId = c.Int(nullable: false), })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            this.CreateTable(
                "dbo.User",
                c =>
                    new
                        {
                            ID = c.Int(nullable: false, identity: true),
                            Username = c.String(nullable: false, maxLength: 100),
                            Firstname = c.String(maxLength: 200),
                            Lastname = c.String(maxLength: 200),
                            Email = c.String(nullable: false, maxLength: 200),
                            HashedPassword = c.String(nullable: false, maxLength: 200),
                            Salt = c.String(nullable: false, maxLength: 200),
                            IsLocked = c.Boolean(nullable: false),
                            DateCreated = c.DateTime(nullable: false),
                            IsSystemDefault = c.Boolean(nullable: false),
                        }).PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            this.DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            this.DropForeignKey("dbo.RoleClaim", "RoleId", "dbo.Role");
            this.DropForeignKey("dbo.RoleClaim", "ClaimId", "dbo.Claim");
            this.DropForeignKey("dbo.Rental", "StockId", "dbo.Stock");
            this.DropForeignKey("dbo.Stock", "MovieId", "dbo.Movie");
            this.DropForeignKey("dbo.Movie", "GenreId", "dbo.Genre");
            this.DropIndex("dbo.UserRole", new[] { "RoleId" });
            this.DropIndex("dbo.UserRole", new[] { "UserId" });
            this.DropIndex("dbo.RoleClaim", new[] { "ClaimId" });
            this.DropIndex("dbo.RoleClaim", new[] { "RoleId" });
            this.DropIndex("dbo.Rental", new[] { "StockId" });
            this.DropIndex("dbo.Stock", new[] { "MovieId" });
            this.DropIndex("dbo.Movie", new[] { "GenreId" });
            this.DropTable("dbo.User");
            this.DropTable("dbo.UserRole");
            this.DropTable("dbo.Role");
            this.DropTable("dbo.RoleClaim");
            this.DropTable("dbo.Rental");
            this.DropTable("dbo.Stock");
            this.DropTable("dbo.Movie");
            this.DropTable("dbo.Genre");
            this.DropTable("dbo.Error");
            this.DropTable("dbo.Customer");
            this.DropTable("dbo.Claim");
        }
    }
}