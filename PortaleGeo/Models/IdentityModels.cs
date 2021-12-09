using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PortaleGeoWeb.Models
{
    // È possibile aggiungere dati del profilo per l'utente aggiungendo altre proprietà alla classe ApplicationUser. Per altre informazioni, vedere https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenere presente che il valore di authenticationType deve corrispondere a quello definito in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Aggiungere qui i reclami utente personalizzati
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //PortaleGeoCode: inserito per rinominare le tabelle di gestione dei Ruoli/Utenti
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            modelBuilder.Entity<ApplicationUser>().ToTable("Geo_Utente");
            modelBuilder.Entity<IdentityRole>().ToTable("Geo_Ruolo");
            modelBuilder.Entity<IdentityUserRole>().ToTable("Geo_UtenteRuolo");
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            //modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}