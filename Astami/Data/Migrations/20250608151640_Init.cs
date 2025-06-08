using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Astami.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptNewsletter",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CAP",
                table: "AspNetUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Città",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodiceFiscale",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cognome",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRegistrazione",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Indirizzo",
                table: "AspNetUsers",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provincia",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Abbonamento",
                columns: table => new
                {
                    AbbonamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezzo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Durata = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abbonamento", x => x.AbbonamentoId);
                });

            migrationBuilder.CreateTable(
                name: "Notifica",
                columns: table => new
                {
                    NotificaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titolo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Messaggio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLetta = table.Column<bool>(type: "bit", nullable: false),
                    DataLettura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DatiAggiuntivi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MittenteDaUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifica", x => x.NotificaId);
                    table.ForeignKey(
                        name: "FK_Notifica_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifica_AspNetUsers_MittenteDaUserId",
                        column: x => x.MittenteDaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Agenzia",
                columns: table => new
                {
                    AgenziaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RagioneSociale = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Indirizzo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Città = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Regione = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CAP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SitoWeb = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PartitaIVA = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SDI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    PinAccesso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataRegistrazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AbbonamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agenzia", x => x.AgenziaId);
                    table.ForeignKey(
                        name: "FK_Agenzia_Abbonamento_AbbonamentoId",
                        column: x => x.AbbonamentoId,
                        principalTable: "Abbonamento",
                        principalColumn: "AbbonamentoId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Agenzia_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PianoSelezionato",
                columns: table => new
                {
                    PianoSelezionatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AbbonamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Confermato = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PianoSelezionato", x => x.PianoSelezionatoId);
                    table.ForeignKey(
                        name: "FK_PianoSelezionato_Abbonamento_AbbonamentoId",
                        column: x => x.AbbonamentoId,
                        principalTable: "Abbonamento",
                        principalColumn: "AbbonamentoId");
                    table.ForeignKey(
                        name: "FK_PianoSelezionato_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AgenziaUtente",
                columns: table => new
                {
                    AgenziaUtenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgenziaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ruolo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataAssegnazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgenziaUtente", x => x.AgenziaUtenteId);
                    table.ForeignKey(
                        name: "FK_AgenziaUtente_Agenzia_AgenziaId",
                        column: x => x.AgenziaId,
                        principalTable: "Agenzia",
                        principalColumn: "AgenziaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgenziaUtente_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Immobile",
                columns: table => new
                {
                    ImmobileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titolo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Indirizzo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Città = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CAP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoContratto = table.Column<int>(type: "int", nullable: false),
                    TipoImmobile = table.Column<int>(type: "int", nullable: false),
                    PrezzoBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Superficie = table.Column<int>(type: "int", nullable: true),
                    NumeroLocali = table.Column<int>(type: "int", nullable: true),
                    NumeroBagni = table.Column<int>(type: "int", nullable: true),
                    Piano = table.Column<int>(type: "int", nullable: true),
                    HasAscensore = table.Column<bool>(type: "bit", nullable: false),
                    HasBalcone = table.Column<bool>(type: "bit", nullable: false),
                    HasTerrazza = table.Column<bool>(type: "bit", nullable: false),
                    HasGiardino = table.Column<bool>(type: "bit", nullable: false),
                    HasParcheggio = table.Column<bool>(type: "bit", nullable: false),
                    HasCantina = table.Column<bool>(type: "bit", nullable: false),
                    ClasseEnergetica = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SpeseCondominiali = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCreazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataModifica = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    AgenziaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProprietarioId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Immobile", x => x.ImmobileId);
                    table.ForeignKey(
                        name: "FK_Immobile_Agenzia_AgenziaId",
                        column: x => x.AgenziaId,
                        principalTable: "Agenzia",
                        principalColumn: "AgenziaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Immobile_AspNetUsers_ProprietarioId",
                        column: x => x.ProprietarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Appuntamento",
                columns: table => new
                {
                    AppuntamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titolo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataInizio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    Luogo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LinkVideoCall = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCreazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InviatoGoogleCalendar = table.Column<bool>(type: "bit", nullable: false),
                    GoogleCalendarEventId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImmobileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AgenziaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreadoDaUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClienteUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appuntamento", x => x.AppuntamentoId);
                    table.ForeignKey(
                        name: "FK_Appuntamento_Agenzia_AgenziaId",
                        column: x => x.AgenziaId,
                        principalTable: "Agenzia",
                        principalColumn: "AgenziaId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Appuntamento_AspNetUsers_ClienteUserId",
                        column: x => x.ClienteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Appuntamento_AspNetUsers_CreadoDaUserId",
                        column: x => x.CreadoDaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appuntamento_Immobile_ImmobileId",
                        column: x => x.ImmobileId,
                        principalTable: "Immobile",
                        principalColumn: "ImmobileId");
                });

            migrationBuilder.CreateTable(
                name: "Asta",
                columns: table => new
                {
                    AstaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImmobileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrezzoPartenza = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrezzoCorrente = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PrezzoRiserva = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DataInizio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCreazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadaDaUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AllowRiduci = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asta", x => x.AstaId);
                    table.ForeignKey(
                        name: "FK_Asta_AspNetUsers_CreadaDaUserId",
                        column: x => x.CreadaDaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asta_Immobile_ImmobileId",
                        column: x => x.ImmobileId,
                        principalTable: "Immobile",
                        principalColumn: "ImmobileId");
                });

            migrationBuilder.CreateTable(
                name: "ImmobileDocumento",
                columns: table => new
                {
                    ImmobileDocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImmobileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeFile = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DimensioneFile = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataCaricamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CaricatoDaUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmobileDocumento", x => x.ImmobileDocumentoId);
                    table.ForeignKey(
                        name: "FK_ImmobileDocumento_AspNetUsers_CaricatoDaUserId",
                        column: x => x.CaricatoDaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImmobileDocumento_Immobile_ImmobileId",
                        column: x => x.ImmobileId,
                        principalTable: "Immobile",
                        principalColumn: "ImmobileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImmobileImmagine",
                columns: table => new
                {
                    ImmobileImmagineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImmobileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ordine = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    DataCaricamento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmobileImmagine", x => x.ImmobileImmagineId);
                    table.ForeignKey(
                        name: "FK_ImmobileImmagine_Immobile_ImmobileId",
                        column: x => x.ImmobileId,
                        principalTable: "Immobile",
                        principalColumn: "ImmobileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lead",
                columns: table => new
                {
                    LeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cognome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Messaggio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataCreazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    NoteInterne = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataUltimoContatto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgenziaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImmobileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssegnatoAUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lead", x => x.LeadId);
                    table.ForeignKey(
                        name: "FK_Lead_Agenzia_AgenziaId",
                        column: x => x.AgenziaId,
                        principalTable: "Agenzia",
                        principalColumn: "AgenziaId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Lead_AspNetUsers_AssegnatoAUserId",
                        column: x => x.AssegnatoAUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lead_Immobile_ImmobileId",
                        column: x => x.ImmobileId,
                        principalTable: "Immobile",
                        principalColumn: "ImmobileId");
                });

            migrationBuilder.CreateTable(
                name: "Pagamento",
                columns: table => new
                {
                    PagamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Importo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MetodoPagamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataScadenza = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AbbonamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImmobileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamento", x => x.PagamentoId);
                    table.ForeignKey(
                        name: "FK_Pagamento_Abbonamento_AbbonamentoId",
                        column: x => x.AbbonamentoId,
                        principalTable: "Abbonamento",
                        principalColumn: "AbbonamentoId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pagamento_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagamento_Immobile_ImmobileId",
                        column: x => x.ImmobileId,
                        principalTable: "Immobile",
                        principalColumn: "ImmobileId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AppuntamentoPartecipante",
                columns: table => new
                {
                    AppuntamentoPartecipanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppuntamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ruolo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    DataRisposta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppuntamentoPartecipante", x => x.AppuntamentoPartecipanteId);
                    table.ForeignKey(
                        name: "FK_AppuntamentoPartecipante_Appuntamento_AppuntamentoId",
                        column: x => x.AppuntamentoId,
                        principalTable: "Appuntamento",
                        principalColumn: "AppuntamentoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppuntamentoPartecipante_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AstaPartecipante",
                columns: table => new
                {
                    AstaPartecipanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AstaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataIscrizione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApprovato = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NoteAgenzia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AstaPartecipante", x => x.AstaPartecipanteId);
                    table.ForeignKey(
                        name: "FK_AstaPartecipante_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AstaPartecipante_Asta_AstaId",
                        column: x => x.AstaId,
                        principalTable: "Asta",
                        principalColumn: "AstaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Offerta",
                columns: table => new
                {
                    OffertaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AstaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Importo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataOfferta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsWinning = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offerta", x => x.OffertaId);
                    table.ForeignKey(
                        name: "FK_Offerta_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Offerta_Asta_AstaId",
                        column: x => x.AstaId,
                        principalTable: "Asta",
                        principalColumn: "AstaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Abbonamento",
                columns: new[] { "AbbonamentoId", "Descrizione", "Durata", "Nome", "Prezzo" },
                values: new object[,]
                {
                    { new Guid("49b523f0-8654-41d4-bffb-73a1c92f3438"), "Dashboard gestionale privata, Caricamento e gestione immobili, Accesso clienti con richieste digitali, Notifiche automatiche e report base", 30, "Basic", 99.00m },
                    { new Guid("a05eb488-8980-432d-b9e8-f981530327e9"), "Tutto nel piano Pro, Firma digitale e gestione contratti, Pagamenti e transazioni online, Integrazione con CRM e portali immobiliari, Supporto premium dedicato", 30, "Enterprise", 499.00m },
                    { new Guid("fd4c0cec-956b-4950-b83a-e82bb0eae888"), "Tutto nel piano Basic, Offerte libere digitalizzate, Personalizzazione tempistiche, Notifiche smart per clienti e agenti, Report avanzati sulle performance", 30, "PRO", 149.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CodiceFiscale",
                table: "AspNetUsers",
                column: "CodiceFiscale");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Abbonamento_Nome",
                table: "Abbonamento",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Agenzia_AbbonamentoId",
                table: "Agenzia",
                column: "AbbonamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Agenzia_ApplicationUserId",
                table: "Agenzia",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Agenzia_Città",
                table: "Agenzia",
                column: "Città");

            migrationBuilder.CreateIndex(
                name: "IX_Agenzia_IsActive",
                table: "Agenzia",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Agenzia_IsPublic",
                table: "Agenzia",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Agenzia_RagioneSociale",
                table: "Agenzia",
                column: "RagioneSociale");

            migrationBuilder.CreateIndex(
                name: "IX_AgenziaUtente_AgenziaId_ApplicationUserId",
                table: "AgenziaUtente",
                columns: new[] { "AgenziaId", "ApplicationUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgenziaUtente_ApplicationUserId",
                table: "AgenziaUtente",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AgenziaUtente_IsActive",
                table: "AgenziaUtente",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamento_AgenziaId",
                table: "Appuntamento",
                column: "AgenziaId");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamento_ClienteUserId",
                table: "Appuntamento",
                column: "ClienteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamento_CreadoDaUserId",
                table: "Appuntamento",
                column: "CreadoDaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamento_DataInizio",
                table: "Appuntamento",
                column: "DataInizio");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamento_ImmobileId",
                table: "Appuntamento",
                column: "ImmobileId");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamento_Stato",
                table: "Appuntamento",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamento_Tipo",
                table: "Appuntamento",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_AppuntamentoPartecipante_ApplicationUserId",
                table: "AppuntamentoPartecipante",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppuntamentoPartecipante_AppuntamentoId_ApplicationUserId",
                table: "AppuntamentoPartecipante",
                columns: new[] { "AppuntamentoId", "ApplicationUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppuntamentoPartecipante_Stato",
                table: "AppuntamentoPartecipante",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Asta_CreadaDaUserId",
                table: "Asta",
                column: "CreadaDaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Asta_DataFine",
                table: "Asta",
                column: "DataFine");

            migrationBuilder.CreateIndex(
                name: "IX_Asta_DataInizio",
                table: "Asta",
                column: "DataInizio");

            migrationBuilder.CreateIndex(
                name: "IX_Asta_ImmobileId",
                table: "Asta",
                column: "ImmobileId");

            migrationBuilder.CreateIndex(
                name: "IX_Asta_Stato",
                table: "Asta",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_AstaPartecipante_ApplicationUserId",
                table: "AstaPartecipante",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AstaPartecipante_AstaId_ApplicationUserId",
                table: "AstaPartecipante",
                columns: new[] { "AstaId", "ApplicationUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AstaPartecipante_IsActive",
                table: "AstaPartecipante",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AstaPartecipante_IsApprovato",
                table: "AstaPartecipante",
                column: "IsApprovato");

            migrationBuilder.CreateIndex(
                name: "IX_Immobile_AgenziaId",
                table: "Immobile",
                column: "AgenziaId");

            migrationBuilder.CreateIndex(
                name: "IX_Immobile_Città",
                table: "Immobile",
                column: "Città");

            migrationBuilder.CreateIndex(
                name: "IX_Immobile_IsPublished",
                table: "Immobile",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Immobile_PrezzoBase",
                table: "Immobile",
                column: "PrezzoBase");

            migrationBuilder.CreateIndex(
                name: "IX_Immobile_ProprietarioId",
                table: "Immobile",
                column: "ProprietarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Immobile_Stato",
                table: "Immobile",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Immobile_TipoContratto",
                table: "Immobile",
                column: "TipoContratto");

            migrationBuilder.CreateIndex(
                name: "IX_Immobile_TipoImmobile",
                table: "Immobile",
                column: "TipoImmobile");

            migrationBuilder.CreateIndex(
                name: "IX_ImmobileDocumento_CaricatoDaUserId",
                table: "ImmobileDocumento",
                column: "CaricatoDaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmobileDocumento_ImmobileId",
                table: "ImmobileDocumento",
                column: "ImmobileId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmobileDocumento_TipoDocumento",
                table: "ImmobileDocumento",
                column: "TipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_ImmobileImmagine_ImmobileId",
                table: "ImmobileImmagine",
                column: "ImmobileId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmobileImmagine_IsPrimary",
                table: "ImmobileImmagine",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_ImmobileImmagine_Ordine",
                table: "ImmobileImmagine",
                column: "Ordine");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_AgenziaId",
                table: "Lead",
                column: "AgenziaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_AssegnatoAUserId",
                table: "Lead",
                column: "AssegnatoAUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_DataCreazione",
                table: "Lead",
                column: "DataCreazione");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Email",
                table: "Lead",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_ImmobileId",
                table: "Lead",
                column: "ImmobileId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Stato",
                table: "Lead",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Notifica_ApplicationUserId",
                table: "Notifica",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifica_DataCreazione",
                table: "Notifica",
                column: "DataCreazione");

            migrationBuilder.CreateIndex(
                name: "IX_Notifica_IsLetta",
                table: "Notifica",
                column: "IsLetta");

            migrationBuilder.CreateIndex(
                name: "IX_Notifica_MittenteDaUserId",
                table: "Notifica",
                column: "MittenteDaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifica_Tipo",
                table: "Notifica",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_Offerta_ApplicationUserId",
                table: "Offerta",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Offerta_AstaId",
                table: "Offerta",
                column: "AstaId");

            migrationBuilder.CreateIndex(
                name: "IX_Offerta_DataOfferta",
                table: "Offerta",
                column: "DataOfferta");

            migrationBuilder.CreateIndex(
                name: "IX_Offerta_Importo",
                table: "Offerta",
                column: "Importo");

            migrationBuilder.CreateIndex(
                name: "IX_Offerta_IsWinning",
                table: "Offerta",
                column: "IsWinning");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_AbbonamentoId",
                table: "Pagamento",
                column: "AbbonamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_ApplicationUserId",
                table: "Pagamento",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_DataPagamento",
                table: "Pagamento",
                column: "DataPagamento");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_ImmobileId",
                table: "Pagamento",
                column: "ImmobileId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_Stato",
                table: "Pagamento",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_Tipo",
                table: "Pagamento",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_TransactionId",
                table: "Pagamento",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PianoSelezionato_AbbonamentoId",
                table: "PianoSelezionato",
                column: "AbbonamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_PianoSelezionato_ApplicationUserId",
                table: "PianoSelezionato",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgenziaUtente");

            migrationBuilder.DropTable(
                name: "AppuntamentoPartecipante");

            migrationBuilder.DropTable(
                name: "AstaPartecipante");

            migrationBuilder.DropTable(
                name: "ImmobileDocumento");

            migrationBuilder.DropTable(
                name: "ImmobileImmagine");

            migrationBuilder.DropTable(
                name: "Lead");

            migrationBuilder.DropTable(
                name: "Notifica");

            migrationBuilder.DropTable(
                name: "Offerta");

            migrationBuilder.DropTable(
                name: "Pagamento");

            migrationBuilder.DropTable(
                name: "PianoSelezionato");

            migrationBuilder.DropTable(
                name: "Appuntamento");

            migrationBuilder.DropTable(
                name: "Asta");

            migrationBuilder.DropTable(
                name: "Immobile");

            migrationBuilder.DropTable(
                name: "Agenzia");

            migrationBuilder.DropTable(
                name: "Abbonamento");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CodiceFiscale",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AcceptNewsletter",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CAP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Città",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CodiceFiscale",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Cognome",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DataRegistrazione",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Indirizzo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Provincia",
                table: "AspNetUsers");
        }
    }
}
