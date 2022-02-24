using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuovoPortaleGeo.Models;
using System.ComponentModel.DataAnnotations;

namespace NuovoPortaleGeo.ViewModels
{
    
    using System;
    using System.Collections.Generic;

    public partial class VmUtente
    {
        public VmUtente()
        {
            this.Geo_UtenteRuolo = new HashSet<Geo_UtenteRuolo>();
        }

        public string Id { get; set; }
        [Required] [Display(Name = "Email")] public string Email { get; set; }
        [Display(Name = "Conferma Email")] public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }

        [Required] [Display(Name = "Cognome")] [MaxLength(50)] public string Cognome { get; set; }
        [Required] [Display(Name = "Nome")] [MaxLength(50)] public string Nome { get; set; }
        [Required] [Display(Name = "Codice Fiscale")] [MaxLength(50)] public string CodiceFiscale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Geo_UtenteRuolo> Geo_UtenteRuolo { get; set; }

        [Display(Name = "Password")]  public string Password { get; set; }
        [Display(Name = "Conferma Password")] public string ConfirmPassword { get; set; }

        public VmUtente(Geo_Utente model)
        {
            Id = model.Id;
            Email = model.Email;
            EmailConfirmed = model.EmailConfirmed;
            PasswordHash = model.PasswordHash;
            SecurityStamp = model.SecurityStamp;
            PhoneNumber = model.PhoneNumber;
            PhoneNumberConfirmed = model.PhoneNumberConfirmed;
            TwoFactorEnabled = model.TwoFactorEnabled;
            LockoutEndDateUtc = model.LockoutEndDateUtc;
            LockoutEnabled = model.LockoutEnabled;
            AccessFailedCount = model.AccessFailedCount;
            UserName = model.UserName;
            Geo_UtenteRuolo = model.Geo_UtenteRuolo;

            Cognome = model.Cognome;
            Nome = model.Nome;
            CodiceFiscale = model.CodiceFiscale;
        }
    }    
}
