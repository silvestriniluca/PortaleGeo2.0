using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortaleGeoWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace PortaleGeoWeb.ViewModels
{
    
    using System;
    using System.Collections.Generic;

    public partial class VmUtenteRuolo
    {
        public VmUtenteRuolo()
        {
        }

        public int Id { get; set; }
        [Required] [Display(Name = "Utente")] public string UserId { get; set; }
        [Required] [Display(Name = "Ruolo")] public string RoleId { get; set; }
        [Display(Name = "Ente")] public Nullable<int> IdEnte { get; set; }
        [Display(Name = "Fornitore")] public Nullable<int> IdFornitore { get; set; }

        [Display(Name = "Ente")] public virtual Geo_Ente Geo_Ente { get; set; }
        [Display(Name = "Fornitore")] public virtual Geo_Fornitore Geo_Fornitore { get; set; }
        [Display(Name = "Ruolo")]  public virtual Geo_Ruolo Geo_Ruolo { get; set; }
        [Display(Name = "Utente")] public virtual Geo_Utente Geo_Utente { get; set; }

        public VmUtenteRuolo(Geo_UtenteRuolo model)
        {
            Id = model.Id;
            UserId = model.UserId;
            RoleId = model.RoleId;

            IdEnte = model.IdEnte;
            IdFornitore = model.IdFornitore;

            Geo_Ente = model.Geo_Ente;
            Geo_Fornitore = model.Geo_Fornitore;

            Geo_Ruolo = model.Geo_Ruolo;
            Geo_Utente = model.Geo_Utente;

        }
    }
}
