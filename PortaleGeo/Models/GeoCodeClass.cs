using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PortaleGeoWeb.Models
{

    public class GeoCode
    {
        public string Indirizzo { get; set; }
        public string Comune { get; set; }
        public string Provincia { get; set; }
        public string Cap { get; set; }
        public string Denominazione { get; set; }
        public string AltroIndirizzo { get; set; }
        public string TipoGeocodifica { get; set; }

        public static GeoCode CreateFrom(
            Dictionary<string, object> dati,
            Dictionary<string, object> corrispondenze
            )
        {
            GeoCode geo = new GeoCode();
            string[] campi = new string[] { "Indirizzo", "Comune", "Provincia", "Cap", "Denominazione", "AltroIndirizzo" };

            foreach ( string campo in campi)
            {
                string nomeCampo = campo;
                if (corrispondenze != null && corrispondenze.Keys.Contains(campo))
                    nomeCampo = corrispondenze[campo].ToString();
                if (dati != null && dati.Keys.Contains(nomeCampo))
                {
                    PropertyInfo propertyInfo = geo.GetType().GetProperty(campo);
                    propertyInfo.SetValue(geo, dati[nomeCampo].ToString());
                }
                else
                {
                    PropertyInfo propertyInfo = geo.GetType().GetProperty(campo);
                    propertyInfo.SetValue(geo, "".ToString());
                }
                    


            }

            /*
            string nomeCampo_Indirizzo = "Indirizzo";
            if (corrispondenze != null && corrispondenze.Keys.Contains("Indirizzo"))
                nomeCampo_Indirizzo = corrispondenze["Indirizzo"];
            if (dati != null && dati.Keys.Contains(nomeCampo_Indirizzo))
                geo.Indirizzo = dati[nomeCampo_Indirizzo].ToString();

            string nomeCampo_Comune = "Comune";
            if (corrispondenze != null && corrispondenze.Keys.Contains("Comune"))
                nomeCampo_Comune = corrispondenze["Comune"];
            geo.Comune = dati[nomeCampo_Comune].ToString();

            string nomeCampo_Provincia = "Provincia";
            if (corrispondenze != null && corrispondenze.Keys.Contains("Provincia"))
                nomeCampo_Provincia = corrispondenze["Provincia"];
            geo.Provincia = dati[nomeCampo_Provincia].ToString();

            string nomeCampo_Cap = "Cap";
            if (corrispondenze != null && corrispondenze.Keys.Contains("Cap"))
                nomeCampo_Cap = corrispondenze["Cap"];
            geo.Cap = dati[nomeCampo_Cap].ToString();

            string nomeCampo_Denominazione = "Denominazione";
            if (corrispondenze != null && corrispondenze.Keys.Contains("Denominazione"))
                nomeCampo_Denominazione = corrispondenze["Denominazione"];
            geo.Denominazione = dati[nomeCampo_Denominazione].ToString();

            string nomeCampo_AltroIndirizzo = "AltroIndirizzo";
            if (corrispondenze != null && corrispondenze.Keys.Contains("AltroIndirizzo"))
                nomeCampo_AltroIndirizzo = corrispondenze["AltroIndirizzo"];
            geo.AltroIndirizzo = dati[nomeCampo_AltroIndirizzo].ToString();
            */

            return geo;
        }

        public double Lat { get; set; }
        public double Lon { get; set; }

        public object Approx01 { get; set; }
        public object Approx02 { get; set; }
        public string Here_MatchLevel { get; set; }

        public string Here_Relevance { get; set; }
        public string Here_MatchType { get; set; }
        public string Here_Error { get; set; }
        
        public object Village { get; set; }




        protected GeoCode()
        {
            //
        }

        public GeoCode(double Lat, double Lon, object Approx01, object Approx02, object Cap, object Village)
        {

            this.Lat = Lat;
            this.Lon = Lon;
            this.Approx01 = Approx01;
            this.Approx02 = Approx02;
            this.Cap = Cap.ToString();
            this.Village = Village;
        }




    }
}
