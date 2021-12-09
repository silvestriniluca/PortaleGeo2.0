/*
 La classe CohesionSSO fornisce tutti i metodi necessari per poter integrare l'autenticazione Cohesion in siti ASP .NET
 
 Il token XML di autenticazione sarà memorizzato nella variabile di sessione "TOKEN" come stringa.
 L'XML con i dati di sessione sarà memorizzato nella variabile di sessione "AUTH" come stringa.
 
 Impostare le seguenti chiavi nel web.config per utilizzare il login classico:
 <appSettings>
    <add key="sso.check.url" value="http://cohesion2.regione.marche.it/SSO/Check.aspx"/>
    <add key="sso.webCheckSessionSSO" value="https://cohesion2.regione.marche.it/SSO/webCheckSessionSSO.aspx"/>
    <add key="sso.additionalData" value=""/>
    <add key="site.URLLogout" value="../Logout.aspx?ReturnUrl=index.aspx"/> <!-- Inserire l'url della propria pagina di logout relativo alla posizione della pagina protetta; in ReturnUrl impostare l'url della pagina a cui ritornare una volta effettuato il logout-->
    <add key="site.IndexURL" value="index.aspx"/> <!-- Inserire l'url della propria pagina a cui ritornare in caso di errore di autenticazione -->
 </appSettings>
         
 Impostare le seguenti chiavi nel web.config per utilizzare il login federato:
 <appSettings>
    <add key="sso.check.url" value="https://cohesion2.regione.marche.it/SPManager/WAYF.aspx"/>
    <add key="sso.webCheckSessionSSO" value="https://cohesion2.regione.marche.it/SPManager/webCheckSessionSSO.aspx"/>
    <add key="sso.additionalData" value="http://proprio_host_pubblico/prorpio_sito/Logout.aspx"/> <!--Impostare qui l'url pubblico completo della pagina di logout del proprio sito-->
    <!-- <add key="sso.additionalData" value="AuthRestriction=0,1,2,3;http://proprio_host_pubblico/prorpio_sito/Logout.aspx"/> --> <!-- Prima del proprio url di logout è possibile specificiare, separato da ';' i tipi di autenticazioni da visualizzare in Cohesion: 0 = autenticazione con Utente e Password, 1 = autenticazione con Utente Password e PIN, 2 = autenticazione Smart Card, 3 = autenticazione di Dominio (valida solo per utenti interni alla rete regionale) -->
    <add key="site.URLLogout" value="../Logout.aspx?ReturnUrl=https%3A%2F%2Fcohesion2.regione.marche.it%2FSPManager%2FLogout.aspx"/> <!-- Inserire l'url della propria pagina di logout relativo alla posizione della pagina protetta; non modificare il valore di ReturnUrl -->
    <add key="site.IndexURL" value="index.aspx"/> <!-- Inserire l'url della propria pagina a cui ritornare in caso di errore di autenticazione -->
 </appSettings>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CohesionSSO
{
    public string SSOCheckUrl ;
    public string webCheckSessionSSOUrl;
    public string additionalData;
    public string IDSito = "SITE_ID";

    public string indexUrl;

    private HttpRequest request;
    private HttpResponse response;
    private System.Web.SessionState.HttpSessionState session;
        
    //Costruttore minimo obbligatorio
    public CohesionSSO(HttpRequest request, HttpResponse response, System.Web.SessionState.HttpSessionState session) 
    {
        this.request = request;
        this.response = response;
        this.session = session;

        this.SSOCheckUrl = System.Configuration.ConfigurationManager.AppSettings["sso.check.url"];
        this.webCheckSessionSSOUrl = System.Configuration.ConfigurationManager.AppSettings["sso.webCheckSessionSSO"];
        this.additionalData = System.Configuration.ConfigurationManager.AppSettings["sso.additionalData"];
        if (this.additionalData == null)
            this.additionalData = "";
        this.indexUrl = System.Configuration.ConfigurationManager.AppSettings["site.IndexURL"];

        if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("site.ID_SITO"))
            IDSito = System.Configuration.ConfigurationManager.AppSettings["site.ID_SITO"];
   }

    //Costruttore completo: Costruttore con possibiltà di specificare i valori di SSOCheckUrl, webCheckSessionSSOUrl e additionalData
    public CohesionSSO(HttpRequest request, HttpResponse response, System.Web.SessionState.HttpSessionState session, string SSOCheckUrl, string webCheckSessionSSOUrl, string additionalData, string indexUrl)
    {
        this.request = request;
        this.response = response;
        this.session = session;

        this.SSOCheckUrl = SSOCheckUrl;
        this.webCheckSessionSSOUrl = webCheckSessionSSOUrl;
        this.additionalData = additionalData;
        if (this.additionalData == null)
            this.additionalData = "";

        this.indexUrl = indexUrl;

        if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("site.ID_SITO"))
            IDSito = System.Configuration.ConfigurationManager.AppSettings["site.ID_SITO"];
    }

    //Metodo di gestione login. Il metodo va richiamato nel load della pagina che gestisce la forms authentication
    public void ValidateFE()
    {
        if (String.IsNullOrEmpty(this.SSOCheckUrl) || String.IsNullOrEmpty(this.webCheckSessionSSOUrl)) 
        {
            response.Clear();
            response.Write("I valori di SSOCheckUrl o webCheckSessionSSOUrl non sono stati impostati");
            response.End();
        }

        if (request.Params["auth"] == null)
        {
            string urlValidate = request.Url.AbsoluteUri;
            string urlRichiesta = request.Url.Scheme + "://" + request.Url.Host + ((request.Url.Port) == 80 ? "" : ":" + request.Url.Port) + request.Params["ReturnUrl"];
            string auth = "<dsAuth xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://tempuri.org/Auth.xsd\"><auth><user /><id_sa /><id_sito>" + IDSito + "</id_sito><esito_auth_sa /><id_sessione_sa /><id_sessione_aspnet_sa /><url_validate>" + urlValidate.Replace("&", "&amp;") + "</url_validate><url_richiesta>" + urlRichiesta.Replace("&", "&amp;") + "</url_richiesta><esito_auth_sso /><id_sessione_sso /><id_sessione_aspnet_sso /><stilesheet>" + this.additionalData + "</stilesheet></auth></dsAuth>";

            auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(auth));
            response.Redirect(this.SSOCheckUrl + "?auth=" + System.Web.HttpUtility.UrlEncode(auth));
        }
        else
        {
            string auth = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(request.Params["auth"]));
            System.Xml.XmlDocument authXml = new System.Xml.XmlDocument();
            authXml.LoadXml(auth);

            string esitoAuthSSO = authXml.GetElementsByTagName("esito_auth_sso")[0].InnerText;

            if (esitoAuthSSO.Equals("OK"))
            {
                string idsessioneSSO = authXml.GetElementsByTagName("id_sessione_sso")[0].InnerText;
                string idsessioneSSOASPNET = authXml.GetElementsByTagName("id_sessione_aspnet_sso")[0].InnerText;

                string token = webCheckSessionSSO(webCheckSessionSSOUrl, "GetCredential", idsessioneSSO, idsessioneSSOASPNET);

                if (!token.Equals("") && token.IndexOf("<AUTH>NO</AUTH>") == -1)
                {
					token = "<?xml version=\"1.0\"?>" + token;
                    int loginStart = token.IndexOf("<login>") + 7;
                    int loginStop = token.IndexOf("</login>");
                    string login = token.Substring(loginStart, loginStop - loginStart);

                    System.Web.Security.FormsAuthentication.SetAuthCookie(login, false);

                    int cfStart = token.IndexOf("<codice_fiscale>") + 16;
                    int cfStop = token.IndexOf("</codice_fiscale>");
                    string cf = token.Substring(cfStart, cfStop - cfStart);

                    session["TOKEN"] = token;
                    session["AUTH"] = auth;
                    session["REDIRECT"] = authXml.GetElementsByTagName("url_richiesta")[0].InnerText;
                    session["CF"] = cf;
                    return;
                    //response.Redirect(authXml.GetElementsByTagName("url_richiesta")[0].InnerText);
                }
                else {
                    response.Clear();
                    response.Write("Errore durante l'autenticazione.\nInfo token:\n\n<pre>\n" + token + "\n</pre>");
                    response.End();
                }
            }
            if (!String.IsNullOrEmpty(this.indexUrl))
                response.Redirect(this.indexUrl);
            else {
                response.Clear();
                response.Write("Errore durante l'autenticazione.\nInfo auth:\n\n<pre>\n" + auth + "\n</pre>");
                response.End();
            }
        }
    }

    //Metodo di gestione logout. Il metodo va richiamato nel load della pagina che gestisce la logout, ovvero la pagina specificata nella chiave site.URLLogout del web.config e restituita dal metodo getLogoutUrl()
    public void LogoutFE()
    {
        if (session["AUTH"] != null)
        {
            System.Xml.XmlDocument authXml = new System.Xml.XmlDocument();
            authXml.LoadXml(session["AUTH"].ToString());
            String idsessioneSSO = authXml.GetElementsByTagName("id_sessione_sso")[0].InnerText;
            String idsessioneSSOASPNET = authXml.GetElementsByTagName("id_sessione_aspnet_sso")[0].InnerText;

            String ret = webCheckSessionSSO(webCheckSessionSSOUrl, "LogoutSito", idsessioneSSO, idsessioneSSOASPNET);
        }

        System.Web.Security.FormsAuthentication.SignOut();
        session.Abandon();
        session.Clear();

        request.ClientCertificate.Clear();
        if (request.Params["ReturnUrl"] != null)
            response.Redirect(request.Params["ReturnUrl"], true);
    }

    //Il metodo ritorna il valore dell'url della pagina di logout (letto nel web.config alla chiave site.URLLogout). 
    //Tale url va utilizzato nella pagina protetta, mediante un link o un button, per disconnettere l'utente e redirigerlo alla pagina iniziale
    public static string getLogoutUrl() 
    {
        string ret = System.Configuration.ConfigurationManager.AppSettings["site.URLLogout"];
        if (ret == null)
            ret = "";

        return ret;
    }

    private string webCheckSessionSSO(String url, String operation, String idsessioneSSO, String idsessioneSSOASPNET)
    {
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            String param = "Operation=" + operation + "&IdSessioneSSO=" + idsessioneSSO + "&IdSessioneASPNET=" + idsessioneSSOASPNET;
            byte[] dataStream = System.Text.Encoding.UTF8.GetBytes(param);
            Uri urlcheck = new System.Uri(url);
            System.Net.HttpWebRequest client = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(urlcheck);
            client.Method = "POST";
            client.ContentType = "application/x-www-form-urlencoded";
            client.ContentLength = dataStream.Length;
            client.GetRequestStream().Write(dataStream, 0, dataStream.Length);
            client.GetRequestStream().Close();

            System.IO.StreamReader sr = new System.IO.StreamReader(client.GetResponse().GetResponseStream());
            String ret = sr.ReadToEnd();
            sr.Close();

            return ret;
        }
        catch (Exception ex)
        {
            return "<AUTH>NO</AUTH><ERROR>" + ex.Message + "\n" + ex.InnerException + "</ERROR>";
        }
    }
}