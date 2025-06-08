using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Astami.Models;

namespace Astami.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// DbSets per tutte le entità
		public DbSet<Abbonamento> Abbonamento { get; set; }
		public DbSet<PianoSelezionato> PianoSelezionato { get; set; }
		public DbSet<Agenzia> Agenzia { get; set; }
		public DbSet<AgenziaUtente> AgenziaUtente { get; set; }
		public DbSet<Immobile> Immobile { get; set; }
		public DbSet<ImmobileImmagine> ImmobileImmagine { get; set; }
		public DbSet<ImmobileDocumento> ImmobileDocumento { get; set; }
		public DbSet<Asta> Asta { get; set; }
		public DbSet<AstaPartecipante> AstaPartecipante { get; set; }
		public DbSet<Offerta> Offerta { get; set; }
		public DbSet<Lead> Lead { get; set; }
		public DbSet<Appuntamento> Appuntamento { get; set; }
		public DbSet<AppuntamentoPartecipante> AppuntamentoPartecipante { get; set; }
		public DbSet<Pagamento> Pagamento { get; set; }
		public DbSet<Notifica> Notifica { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configurazione ApplicationUser
			builder.Entity<ApplicationUser>(entity =>
			{
				entity.HasIndex(e => e.Email).IsUnique();
				entity.HasIndex(e => e.CodiceFiscale);

				entity.Property(e => e.Nome).HasMaxLength(100);
				entity.Property(e => e.Cognome).HasMaxLength(100);
				entity.Property(e => e.Indirizzo).HasMaxLength(300);
				entity.Property(e => e.CAP).HasMaxLength(10);
				entity.Property(e => e.Città).HasMaxLength(100);
				entity.Property(e => e.Provincia).HasMaxLength(100);
				entity.Property(e => e.CodiceFiscale).HasMaxLength(20);
			});

			// Configurazione Abbonamento
			builder.Entity<Abbonamento>(entity =>
			{
				entity.HasKey(e => e.AbbonamentoId);
				entity.Property(e => e.Prezzo).HasColumnType("decimal(18,2)");
				entity.HasIndex(e => e.Nome);
			});

			// Configurazione PianoSelezionato
			builder.Entity<PianoSelezionato>(entity =>
			{
				entity.HasKey(e => e.PianoSelezionatoId);

				// Relazione con ApplicationUser
				entity.HasOne<ApplicationUser>()
					.WithMany()
					.HasForeignKey(e => e.ApplicationUserId)
					.OnDelete(DeleteBehavior.NoAction);

				// Relazione con Abbonamento
				entity.HasOne<Abbonamento>()
					.WithMany()
					.HasForeignKey(e => e.AbbonamentoId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			// Configurazione Agenzia
			builder.Entity<Agenzia>(entity =>
			{
				entity.HasKey(e => e.AgenziaId);

				entity.Property(e => e.RagioneSociale).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Descrizione).HasMaxLength(1000);
				entity.Property(e => e.Indirizzo).IsRequired().HasMaxLength(300);
				entity.Property(e => e.Città).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Provincia).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Regione).IsRequired().HasMaxLength(100);
				entity.Property(e => e.CAP).IsRequired().IsRequired().HasMaxLength(10);
				entity.Property(e => e.Telefono).HasMaxLength(20);
				entity.Property(e => e.Email).HasMaxLength(200);
				entity.Property(e => e.SitoWeb).HasMaxLength(200);
				entity.Property(e => e.PartitaIVA).HasMaxLength(20);
				entity.Property(e => e.SDI).HasMaxLength(20);
				entity.Property(e => e.PinAccesso).HasMaxLength(100);
				entity.Property(e => e.LogoUrl).HasMaxLength(500);
				entity.Property(e => e.ImageUrl).HasMaxLength(500);

				entity.HasIndex(e => e.RagioneSociale);
				entity.HasIndex(e => e.Città);
				entity.HasIndex(e => e.IsPublic);
				entity.HasIndex(e => e.IsActive);

				// Relazione con ApplicationUser (Owner)
				entity.HasOne(e => e.ApplicationUser)
					.WithMany()
					.HasForeignKey(e => e.ApplicationUserId)
					.OnDelete(DeleteBehavior.NoAction);

				// Relazione con Abbonamento
				entity.HasOne(e => e.Abbonamento)
					.WithMany()
					.HasForeignKey(e => e.AbbonamentoId)
					.OnDelete(DeleteBehavior.SetNull);
			});

			// Configurazione AgenziaUtente (Many-to-Many tra Agenzia e ApplicationUser)
			builder.Entity<AgenziaUtente>(entity =>
			{
				entity.HasKey(e => e.AgenziaUtenteId);

				entity.Property(e => e.Ruolo).IsRequired().HasMaxLength(50);

				entity.HasIndex(e => new { e.AgenziaId, e.ApplicationUserId }).IsUnique();
				entity.HasIndex(e => e.IsActive);

				// Relazioni
				entity.HasOne(e => e.Agenzia)
					.WithMany(a => a.AgenziaUtenti)
					.HasForeignKey(e => e.AgenziaId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.ApplicationUser)
					.WithMany()
					.HasForeignKey(e => e.ApplicationUserId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			// Configurazione Immobile
			builder.Entity<Immobile>(entity =>
			{
				entity.HasKey(e => e.ImmobileId);

				entity.Property(e => e.Titolo).IsRequired().HasMaxLength(300);
				entity.Property(e => e.Descrizione).HasMaxLength(2000);
				entity.Property(e => e.Indirizzo).IsRequired().HasMaxLength(300);
				entity.Property(e => e.Città).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Provincia).HasMaxLength(100);
				entity.Property(e => e.CAP).IsRequired().HasMaxLength(10);
				entity.Property(e => e.PrezzoBase).HasColumnType("decimal(18,2)");
				entity.Property(e => e.SpeseCondominiali).HasColumnType("decimal(18,2)");
				entity.Property(e => e.ClasseEnergetica).HasMaxLength(50);
				entity.Property(e => e.Note).HasMaxLength(500);

				entity.HasIndex(e => e.Città);
				entity.HasIndex(e => e.TipoContratto);
				entity.HasIndex(e => e.TipoImmobile);
				entity.HasIndex(e => e.Stato);
				entity.HasIndex(e => e.IsPublished);
				entity.HasIndex(e => e.PrezzoBase);

				// Relazione con Agenzia
				entity.HasOne(e => e.Agenzia)
					.WithMany(a => a.Immobili)
					.HasForeignKey(e => e.AgenziaId)
					.OnDelete(DeleteBehavior.Cascade);

				// Relazione con Proprietario
				entity.HasOne(e => e.Proprietario)
					.WithMany()
					.HasForeignKey(e => e.ProprietarioId)
					.OnDelete(DeleteBehavior.SetNull);
			});

			// Configurazione ImmobileImmagine
			builder.Entity<ImmobileImmagine>(entity =>
			{
				entity.HasKey(e => e.ImmobileImmagineId);

				entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
				entity.Property(e => e.Descrizione).HasMaxLength(200);

				entity.HasIndex(e => e.Ordine);
				entity.HasIndex(e => e.IsPrimary);

				// Relazione con Immobile
				entity.HasOne(e => e.Immobile)
					.WithMany(i => i.Immagini)
					.HasForeignKey(e => e.ImmobileId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// Configurazione ImmobileDocumento
			builder.Entity<ImmobileDocumento>(entity =>
			{
				entity.HasKey(e => e.ImmobileDocumentoId);

				entity.Property(e => e.NomeFile).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
				entity.Property(e => e.TipoDocumento).HasMaxLength(100);
				entity.Property(e => e.MimeType).HasMaxLength(50);

				entity.HasIndex(e => e.TipoDocumento);

				// Relazione con Immobile
				entity.HasOne(e => e.Immobile)
					.WithMany(i => i.Documenti)
					.HasForeignKey(e => e.ImmobileId)
					.OnDelete(DeleteBehavior.Cascade);

				// Relazione con CaricatoDaUser
				entity.HasOne(e => e.CaricatoDaUser)
					.WithMany()
					.HasForeignKey(e => e.CaricatoDaUserId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			// Configurazione Asta
			builder.Entity<Asta>(entity =>
			{
				entity.HasKey(e => e.AstaId);

				entity.Property(e => e.PrezzoPartenza).HasColumnType("decimal(18,2)");
				entity.Property(e => e.PrezzoCorrente).HasColumnType("decimal(18,2)");
				entity.Property(e => e.PrezzoRiserva).HasColumnType("decimal(18,2)");
				entity.Property(e => e.Note).HasMaxLength(500);

				entity.HasIndex(e => e.Stato);
				entity.HasIndex(e => e.DataInizio);
				entity.HasIndex(e => e.DataFine);

				// Relazione con Immobile
				entity.HasOne(e => e.Immobile)
					.WithMany(i => i.Aste)
					.HasForeignKey(e => e.ImmobileId)
					.OnDelete(DeleteBehavior.NoAction);

				// Relazione con CreadaDaUser
				entity.HasOne(e => e.CreadaDaUser)
					.WithMany()
					.HasForeignKey(e => e.CreadaDaUserId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			// Configurazione AstaPartecipante
			builder.Entity<AstaPartecipante>(entity =>
			{
				entity.HasKey(e => e.AstaPartecipanteId);

				entity.Property(e => e.NoteAgenzia).HasMaxLength(500);

				entity.HasIndex(e => new { e.AstaId, e.ApplicationUserId }).IsUnique();
				entity.HasIndex(e => e.IsApprovato);
				entity.HasIndex(e => e.IsActive);

				// Relazione con Asta
				entity.HasOne(e => e.Asta)
					.WithMany(a => a.Partecipanti)
					.HasForeignKey(e => e.AstaId)
					.OnDelete(DeleteBehavior.Cascade);

				// Relazione con ApplicationUser
				entity.HasOne(e => e.ApplicationUser)
					.WithMany()
					.HasForeignKey(e => e.ApplicationUserId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			// Configurazione Offerta
			builder.Entity<Offerta>(entity =>
			{
				entity.HasKey(e => e.OffertaId);

				entity.Property(e => e.Importo).HasColumnType("decimal(18,2)");
				entity.Property(e => e.Note).HasMaxLength(200);

				entity.HasIndex(e => e.DataOfferta);
				entity.HasIndex(e => e.Importo);
				entity.HasIndex(e => e.IsWinning);

				// Relazione con Asta
				entity.HasOne(e => e.Asta)
					.WithMany(a => a.Offerte)
					.HasForeignKey(e => e.AstaId)
					.OnDelete(DeleteBehavior.Cascade);

				// Relazione con ApplicationUser
				entity.HasOne(e => e.ApplicationUser)
					.WithMany()
					.HasForeignKey(e => e.ApplicationUserId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			// Configurazione Lead
			builder.Entity<Lead>(entity =>
			{
				entity.HasKey(e => e.LeadId);

				entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Cognome).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Telefono).HasMaxLength(20);
				entity.Property(e => e.Messaggio).HasMaxLength(1000);
				entity.Property(e => e.NoteInterne).HasMaxLength(500);

				entity.HasIndex(e => e.Email);
				entity.HasIndex(e => e.Stato);
				entity.HasIndex(e => e.DataCreazione);

				// Relazione con Agenzia
				entity.HasOne(e => e.Agenzia)
					.WithMany(a => a.Leads)
					.HasForeignKey(e => e.AgenziaId)
					.OnDelete(DeleteBehavior.SetNull);

				// Relazione con Immobile
				entity.HasOne(e => e.Immobile)
					.WithMany(i => i.Leads)
					.HasForeignKey(e => e.ImmobileId)
					.OnDelete(DeleteBehavior.NoAction);

				// Relazione con AssegnatoAUser
				entity.HasOne(e => e.AssegnatoAUser)
					.WithMany()
					.HasForeignKey(e => e.AssegnatoAUserId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			// Configurazione Appuntamento
			builder.Entity<Appuntamento>(entity =>
			{
				entity.HasKey(e => e.AppuntamentoId);

				entity.Property(e => e.Titolo).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Descrizione).HasMaxLength(1000);
				entity.Property(e => e.Luogo).HasMaxLength(300);
				entity.Property(e => e.LinkVideoCall).HasMaxLength(500);
				entity.Property(e => e.Note).HasMaxLength(500);
				entity.Property(e => e.GoogleCalendarEventId).HasMaxLength(200);

				entity.HasIndex(e => e.DataInizio);
				entity.HasIndex(e => e.Tipo);
				entity.HasIndex(e => e.Stato);

				// Relazione con Immobile
				entity.HasOne(e => e.Immobile)
					.WithMany(i => i.Appuntamenti)
					.HasForeignKey(e => e.ImmobileId)
					.OnDelete(DeleteBehavior.NoAction);

				// Relazione con Agenzia
				entity.HasOne(e => e.Agenzia)
					.WithMany()
					.HasForeignKey(e => e.AgenziaId)
					.OnDelete(DeleteBehavior.SetNull);

				// Relazione con CreadoDaUser
				entity.HasOne(e => e.CreadoDaUser)
					.WithMany()
					.HasForeignKey(e => e.CreadoDaUserId)
					.OnDelete(DeleteBehavior.NoAction);

				// Relazione con ClienteUser
				entity.HasOne(e => e.ClienteUser)
					.WithMany()
					.HasForeignKey(e => e.ClienteUserId)
					.OnDelete(DeleteBehavior.SetNull);
			});

			// Configurazione AppuntamentoPartecipante
			builder.Entity<AppuntamentoPartecipante>(entity =>
			{
				entity.HasKey(e => e.AppuntamentoPartecipanteId);

				entity.Property(e => e.Ruolo).IsRequired().HasMaxLength(50);
				entity.Property(e => e.Note).HasMaxLength(500);

				entity.HasIndex(e => new { e.AppuntamentoId, e.ApplicationUserId }).IsUnique();
				entity.HasIndex(e => e.Stato);

				// Relazione con Appuntamento
				entity.HasOne(e => e.Appuntamento)
					.WithMany(a => a.Partecipanti)
					.HasForeignKey(e => e.AppuntamentoId)
					.OnDelete(DeleteBehavior.Cascade);

				// Relazione con ApplicationUser
				entity.HasOne(e => e.ApplicationUser)
					.WithMany()
					.HasForeignKey(e => e.ApplicationUserId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			// Configurazione Pagamento
			builder.Entity<Pagamento>(entity =>
			{
				entity.HasKey(e => e.PagamentoId);

				entity.Property(e => e.Importo).HasColumnType("decimal(18,2)");
				entity.Property(e => e.TransactionId).HasMaxLength(200);
				entity.Property(e => e.MetodoPagamento).HasMaxLength(100);
				entity.Property(e => e.Note).HasMaxLength(1000);

				entity.HasIndex(e => e.Stato);
				entity.HasIndex(e => e.Tipo);
				entity.HasIndex(e => e.DataPagamento);
				entity.HasIndex(e => e.TransactionId);

				// Relazione con ApplicationUser
				entity.HasOne(e => e.ApplicationUser)
					.WithMany()
					.HasForeignKey(e => e.ApplicationUserId)
					.OnDelete(DeleteBehavior.NoAction);

				// Relazione con Abbonamento
				entity.HasOne(e => e.Abbonamento)
					.WithMany()
					.HasForeignKey(e => e.AbbonamentoId)
					.OnDelete(DeleteBehavior.SetNull);

				// Relazione con Immobile
				entity.HasOne(e => e.Immobile)
					.WithMany()
					.HasForeignKey(e => e.ImmobileId)
					.OnDelete(DeleteBehavior.SetNull);
			});

			// Configurazione Notifica
			builder.Entity<Notifica>(entity =>
			{
				entity.HasKey(e => e.NotificaId);

				entity.Property(e => e.Titolo).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Messaggio).IsRequired().HasMaxLength(1000);
				entity.Property(e => e.Url).HasMaxLength(500);
				entity.Property(e => e.DatiAggiuntivi).HasMaxLength(1000);

				entity.HasIndex(e => e.IsLetta);
				entity.HasIndex(e => e.Tipo);
				entity.HasIndex(e => e.DataCreazione);

				// Relazione con ApplicationUser (destinatario)
				entity.HasOne(e => e.ApplicationUser)
					.WithMany()
					.HasForeignKey(e => e.ApplicationUserId)
					.OnDelete(DeleteBehavior.NoAction);

				// Relazione con MittenteDaUser
				entity.HasOne(e => e.MittenteDaUser)
					.WithMany()
					.HasForeignKey(e => e.MittenteDaUserId)
					.OnDelete(DeleteBehavior.SetNull);
			});

			// Configurazioni di Seed Data per i ruoli e abbonamenti
			SeedData(builder);
		}

		private void SeedData(ModelBuilder builder)
		{
			// Seed Abbonamenti con IDs fissi
			builder.Entity<Abbonamento>().HasData(
				new Abbonamento
				{
					AbbonamentoId = Utilities.Constants.SubscriptionsConstants.Basic,
					Nome = "Basic",
					Descrizione = "Dashboard gestionale privata, Caricamento e gestione immobili, Accesso clienti con richieste digitali, Notifiche automatiche e report base",
					Prezzo = 99.00m,
					Durata = 30
				},
				new Abbonamento
				{
					AbbonamentoId = Utilities.Constants.SubscriptionsConstants.Pro,
					Nome = "PRO",
					Descrizione = "Tutto nel piano Basic, Offerte libere digitalizzate, Personalizzazione tempistiche, Notifiche smart per clienti e agenti, Report avanzati sulle performance",
					Prezzo = 149.00m,
					Durata = 30
				},
				new Abbonamento
				{
					AbbonamentoId = Utilities.Constants.SubscriptionsConstants.Enterprice,
					Nome = "Enterprise",
					Descrizione = "Tutto nel piano Pro, Firma digitale e gestione contratti, Pagamenti e transazioni online, Integrazione con CRM e portali immobiliari, Supporto premium dedicato",
					Prezzo = 499.00m,
					Durata = 30
				}
			);
		}
	}
}