using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuovoPortaleGeo.Models;
using NuovoPortaleGeo.reader.csv;
using NuovoPortaleGeo.Helpers;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Globalization;
using Microsoft.AspNet.Identity;
using LanguageExt.ClassInstances.Pred;
using System.Collections;
using PagedList;
using System.Data;
using System.Web.Script.Serialization;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;
using NuovoPortaleGeo.ViewModels;
using System.Diagnostics;
using PortaleGeoWeb.Models;

namespace NuovoPortaleGeo.Controllers
{
    [Authorize(Roles = "Amministratore,Utente,Consultatore")]

    public class SistemaHereController : Controller
    {
        GeoCodeEntities1 db = new GeoCodeEntities1();
        static int GeoRighe;
        static int GeoNoRighe;
        // GET: SistemaHere
        public ActionResult Index()
        {
            return View();
        }









    }
}
  
            
            
        
        
    
   
 


        
            
 



