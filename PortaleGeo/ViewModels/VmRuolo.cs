using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortaleGeoWeb.Models;
using System.ComponentModel.DataAnnotations;


namespace PortaleGeoWeb.ViewModels
{
    public class VmRuolo
    {

        public VmRuolo() {
            this.Geo_Utente = new HashSet<Geo_Utente>();

        }

        public string Id { get; set; }
        [Required] [Display(Name = "Nome Ruolo")] [MaxLength(250)]  public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Geo_Utente> Geo_Utente { get; set; }


        public VmRuolo(Geo_Ruolo model)
        {
            Id = model.Id;
            Name = model.Name;
        }
    }
}