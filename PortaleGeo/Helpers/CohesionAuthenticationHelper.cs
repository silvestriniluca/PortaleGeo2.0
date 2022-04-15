using NuovoPortaleGeo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using NuovoPortaleGeo.Controllers;

namespace NuovoPortaleGeo.Helpers
{
    
    public static class CohesionAuthenticationHelper
    {
        public static async Task<bool> AuthenticateCohesionUser(System.Web.SessionState.HttpSessionState session, ApplicationSignInManager signInManager)
        {
            if (session["TOKEN"] != null
                && session["AUTH"] != null
                && session["CF"] != null)
            {

                UploadController.cf = session["CF"].ToString();
                var cf = UploadController.cf;
                //var redirectUrl = session["REDIRECT"]? returnUrl
            
            
                using (GeoCodeEntities1 db = new GeoCodeEntities1()) { 

                    var Geo_Utente = db.Geo_Utente
                        .Where(x => x.CodiceFiscale == cf).FirstOrDefault();

                        if (Geo_Utente != null)
                        {
                            var email = Geo_Utente.Email;
                            var id = Geo_Utente.Id;
                            var user = new ApplicationUser
                            {
                                UserName = email,
                                Email = email,
                                Id = id

                            };
                   
                        await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return true;
                    }

                }
            }

            return false;
        }
    }
}