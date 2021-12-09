using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PortaleGeoWeb.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using PortaleGeoWeb.ViewModels;



namespace PortaleGeoWeb.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class UtenteController : Controller
    {
        private GeoCodeEntities1 db = new GeoCodeEntities1();

        // GET: Utente
        public ActionResult Index()
        {
            return View(db.Geo_Utente.ToList());
        }

        // GET: Utente/Details/5
        public ActionResult Dettagli(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Utente geO_Utente = db.Geo_Utente.Find(id);
            if (geO_Utente == null)
            {
                return HttpNotFound();
            }

            var vm = new VmUtente(geO_Utente);
            return View(vm);
        }

        // GET: Utente/Create
        public ActionResult Aggiungi()
        {
            return View();
        }

        /*
        // POST: Utente/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aggiungi([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] IO_Utente iO_Utente)
        {
            if (ModelState.IsValid)
            {
                db.IO_Utente.Add(iO_Utente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(iO_Utente);
        }
        */
        // GET: Utente/Edit/5
        public ActionResult Modifica(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Utente geO_Utente = db.Geo_Utente.Find(id);
            if (geO_Utente == null)
            {
                return HttpNotFound();
            }

            var vm = new VmUtente(geO_Utente);
            return View(vm);

        }

        // POST: Utente/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifica([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] Geo_Utente geO_Utente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geO_Utente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(geO_Utente);
        }

        // GET: Utente/Delete/5
        public ActionResult Cancella(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Utente geO_Utente = db.Geo_Utente.Find(id);
            if (geO_Utente == null)
            {
                return HttpNotFound();
            }

            var vm = new VmUtente(geO_Utente);
            return View(vm);

        }

        // POST: Utente/Delete/5
        [HttpPost, ActionName("Cancella")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Geo_Utente geO_Utente = db.Geo_Utente.Find(id);
            db.Geo_Utente.Remove(geO_Utente);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Registrazione
        //*** REGISTRAZIONE NUOVO UTENTE CON LE SICUREZZE ASPNET (VEDERE AccountController)

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Aggiungi(VmUtente model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //Aggiorno gli altri dati inseriti
                    Geo_Utente geO_Utente = db.Geo_Utente.Find(user.Id);
                    geO_Utente.Cognome = model.Cognome;
                    geO_Utente.Nome = model.Nome;
                    geO_Utente.CodiceFiscale = model.CodiceFiscale;
                    db.Entry(geO_Utente).State = EntityState.Modified;
                    db.SaveChanges();

                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // Per altre informazioni su come abilitare la conferma dell'account e la reimpostazione della password, vedere https://go.microsoft.com/fwlink/?LinkID=320771
                    // Inviare un messaggio di posta elettronica con questo collegamento
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Conferma account", "Per confermare l'account, fare clic <a href=\"" + callbackUrl + "\">qui</a>");

                    return RedirectToAction("Index", "Utente");
                }
                AddErrors(result);
            }

            // Se si è arrivati a questo punto, significa che si è verificato un errore, rivisualizzare il form
            return View(model);
        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        #endregion Registrazione


    }
}
