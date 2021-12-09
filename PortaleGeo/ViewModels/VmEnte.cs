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

    public partial class VmEnte
    {
        public VmEnte()
        {
        }

        public int Id { get; set; }
        [Display(Name = "Codice Istat")] [MaxLength(16)] public string CodiceIstat { get; set; }
        [Display(Name = "Codice Belfiore")] [MaxLength(16)] public string CodiceFisco { get; set; }
        [Display(Name = "Sigla Provincia")] [MaxLength(2)] public string SiglaProv { get; set; }
        [Display(Name = "CAP")] [MaxLength(5)] public string Cap { get; set; }
        [Display(Name = "Nome Ente")] [MaxLength(250)] public string Nome { get; set; }
        [Display(Name = "Valido Dal")] public Nullable<System.DateTime> ValidoDal { get; set; }
        [Display(Name = "Valido Al")] public Nullable<System.DateTime> ValidoAl { get; set; }

        public virtual ICollection<Geo_UtenteRuolo> Geo_UtenteRuolo { get; set; }


        [Display(Name = "Aderisce a Digipalm")] public Nullable<int> AderisceDigipalm { get; set; }
        [Display(Name = "OnBoarding Ricevuto")] public Nullable<int> OnBoardingRicevuto { get; set; }
        [Display(Name = "OnBoarding Inviato")] public Nullable<int> OnBoardingInviato { get; set; }
        [MaxLength(16)] [Display(Name = "Codice Fiscale")] public string CodiceFiscale { get; set; }

        [Display(Name = "Ha pratiche USR")] public Nullable<int> PraticheUSR { get; set; }

        [Display(Name = "APIKEY Auth. WS")] [MaxLength(30)]  public string AuthIoWS { get; set; }


        public VmEnte(Geo_Ente model)
        {
            Id = model.Id;
            CodiceIstat = model.CodiceIstat;
            CodiceFisco = model.CodiceFisco;
            SiglaProv = model.SiglaProv;
            Cap = model.Cap;
            Nome = model.Nome;
            ValidoDal = model.ValidoDal;
            ValidoAl = model.ValidoAl;

            AderisceDigipalm = model.AderisceDigipalm;
            OnBoardingRicevuto = model.OnBoardingRicevuto;
            OnBoardingInviato = model.OnBoardingInviato;
            CodiceFiscale = model.CodiceFiscale;
            PraticheUSR = model.PraticheUSR;

           
            Geo_UtenteRuolo = model.Geo_UtenteRuolo;
            AuthIoWS = model.AuthIoWS;


        }


        public static List<int?> Geo_Ente_List(string Userid)
        {
            GeoCodeEntities1 db = new GeoCodeEntities1();

            var EntiAbilitati = db.Geo_UtenteRuolo
                    .Where(t => (t.UserId == Userid))
                    .Select(t => t.IdEnte)
                    .ToList();

            return EntiAbilitati;
        }
    }
}
