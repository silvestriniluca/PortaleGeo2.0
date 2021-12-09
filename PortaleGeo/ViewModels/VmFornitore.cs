using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortaleGeoWeb.Models;
using System.ComponentModel.DataAnnotations;


namespace PortaleGeoWeb.ViewModels
{
    public class VmFornitore
    {

        public int Id { get; set; }
        [Required] [Display(Name = "Ragione Sociale")] [MaxLength(250)]  public string RagioneSociale { get; set; }

        [Required] [Display(Name = "Partita IVA")] [MaxLength(11)]  public string PartitaIva { get; set; }
        [Required] [Display(Name = "Referente Tecnico")] [MaxLength(250)]  public string ContattoTecnico { get; set; }
        [Required] [Display(Name = "E-mail Tecnico")] [MaxLength(250)]  public string EmailTecnico { get; set; }
        [Required] [Display(Name = "Telefono Tecnico")] [MaxLength(250)]  public string TelefonoTecnico { get; set; }

        [Display(Name = "Secondo Referente Tecnico")] [MaxLength(250)]  public string ContattoTecnico1 { get; set; }
        [Display(Name = "E-mail Secondo Tecnico")] [MaxLength(250)]  public string EmailTecnico1 { get; set; }
        [Display(Name = "Telefono Secondo Tecnico")] [MaxLength(250)] public string TelefonoTecnico1 { get; set; }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        

        

        

        public VmFornitore(Geo_Fornitore model)
        {
            Id = model.Id;
            RagioneSociale = model.RagioneSociale;
            PartitaIva = model.PartitaIva;
            ContattoTecnico = model.ContattoTecnico;
            EmailTecnico = model.EmailTecnico;
            TelefonoTecnico = model.TelefonoTecnico;
          

            ContattoTecnico1 = model.ContattoTecnico1;
            EmailTecnico1 = model.EmailTecnico1;
            TelefonoTecnico1 = model.TelefonoTecnico1;
        }


        public static List<int?> Geo_Fornitore_List(string Userid)
        {
            GeoCodeEntities1 db = new GeoCodeEntities1();

            var FornitoriAbilitati = db.Geo_UtenteRuolo
                    .Where(t => (t.UserId == Userid))
                    .Select(t => t.IdFornitore)
                    .ToList();

            return FornitoriAbilitati;
        }


        public static IQueryable<Geo_Fornitore> Geo_Fornitore_Profilo(string Userid)
        {
           GeoCodeEntities1 db = new GeoCodeEntities1();

            var FornitoriAbilitati = db.Geo_UtenteRuolo
                    .Where(t => (t.UserId == Userid))
                    .Select(t => t.IdFornitore)
                    .ToList();

            IQueryable<Geo_Fornitore> geO_Fornitore = db.Geo_Fornitore.Where(t => FornitoriAbilitati.Contains(t.Id));

            return geO_Fornitore;
        }

    }
}