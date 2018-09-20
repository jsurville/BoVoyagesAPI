namespace BoVoyagesAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AgenceVoyages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Assurances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Montant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TypeAssurance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DossierReservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumeroUnique = c.Int(nullable: false),
                        NumeroCarteBancaire = c.String(),
                        PrixParPersonne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EtatDossierReservation = c.Int(nullable: false),
                        RaisonAnnulationDossier = c.Int(nullable: false),
                        VoyageId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: false)
                .ForeignKey("dbo.Voyages", t => t.VoyageId, cascadeDelete: false)
                .Index(t => t.VoyageId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Civilite = c.String(),
                        Nom = c.String(),
                        Prenom = c.String(),
                        Adresse = c.String(),
                        Telephone = c.String(),
                        DateNaissance = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumeroUnique = c.Int(nullable: false),
                        DossierReservationId = c.Int(nullable: false),
                        Civilite = c.String(),
                        Nom = c.String(),
                        Prenom = c.String(),
                        Adresse = c.String(),
                        Telephone = c.String(),
                        DateNaissance = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DossierReservations", t => t.DossierReservationId, cascadeDelete: false)
                .Index(t => t.DossierReservationId);
            
            CreateTable(
                "dbo.Voyages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateAller = c.DateTime(nullable: false),
                        DateRetour = c.DateTime(nullable: false),
                        PlacesDisponibles = c.Int(nullable: false),
                        PrixParPersonne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DestinationId = c.Int(nullable: false),
                        AgenceVoyageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AgenceVoyages", t => t.AgenceVoyageId, cascadeDelete: false)
                .ForeignKey("dbo.Destinations", t => t.DestinationId, cascadeDelete: false)
                .Index(t => t.DestinationId)
                .Index(t => t.AgenceVoyageId);
            
            CreateTable(
                "dbo.Destinations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Continent = c.String(),
                        Pays = c.String(),
                        Region = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DossierReservationAssurances",
                c => new
                    {
                        DossierReservation_Id = c.Int(nullable: false),
                        Assurance_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DossierReservation_Id, t.Assurance_Id })
                .ForeignKey("dbo.DossierReservations", t => t.DossierReservation_Id, cascadeDelete: false)
                .ForeignKey("dbo.Assurances", t => t.Assurance_Id, cascadeDelete: false)
                .Index(t => t.DossierReservation_Id)
                .Index(t => t.Assurance_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DossierReservations", "VoyageId", "dbo.Voyages");
            DropForeignKey("dbo.Voyages", "DestinationId", "dbo.Destinations");
            DropForeignKey("dbo.Voyages", "AgenceVoyageId", "dbo.AgenceVoyages");
            DropForeignKey("dbo.Participants", "DossierReservationId", "dbo.DossierReservations");
            DropForeignKey("dbo.DossierReservations", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.DossierReservationAssurances", "Assurance_Id", "dbo.Assurances");
            DropForeignKey("dbo.DossierReservationAssurances", "DossierReservation_Id", "dbo.DossierReservations");
            DropIndex("dbo.DossierReservationAssurances", new[] { "Assurance_Id" });
            DropIndex("dbo.DossierReservationAssurances", new[] { "DossierReservation_Id" });
            DropIndex("dbo.Voyages", new[] { "AgenceVoyageId" });
            DropIndex("dbo.Voyages", new[] { "DestinationId" });
            DropIndex("dbo.Participants", new[] { "DossierReservationId" });
            DropIndex("dbo.DossierReservations", new[] { "ClientId" });
            DropIndex("dbo.DossierReservations", new[] { "VoyageId" });
            DropTable("dbo.DossierReservationAssurances");
            DropTable("dbo.Destinations");
            DropTable("dbo.Voyages");
            DropTable("dbo.Participants");
            DropTable("dbo.Clients");
            DropTable("dbo.DossierReservations");
            DropTable("dbo.Assurances");
            DropTable("dbo.AgenceVoyages");
        }
    }
}
