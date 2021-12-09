function hdProjectData(state) {
  const prjData = {
    apikey: "f0cbfb06106a604b0ea1b8c1974ceb47ad87c583"     // chiave dell'utente <-- creato un vostro utente HD va modificata
    , redmine_url: "https://pass.regione.marche.it"           // FISSO
    , project_id: 321 // FISSO - id del progetto IOServiceAssistenza
    , list_url: "/Pass/ElencoTicket" // DA EDITARE: url della pagina in cui si vuole compaia l'elenco dei ticket dell'utente
    , issue_url: "/Pass/DettaglioTicket" // DA EDITARE: url della pagina in cui si vuole che compaia il dettaglio della singola segnalazione dell'utente
    , cfid_segnalatore: 2 // FISSO - custom field id del segnalattore
    , cfname_segnalatore: 'segnalatore'  // FISSO
    , cfid_ultimo_accesso: 9 //FISSO - custom field id della data di ultimo accesso
    , update_url: "https://pass.regione.marche.it/apirm/" // FISSO
    , include_closed: true // EDITABILE: nella lista delle segnalazioni se includere quelle chiuse, o presentare solo quelle aperte
    , status_id: null // EDITABILE - se impostare uno stato predefinito in creazione dei ticket... default "nuovo"
    , custom_fields: null
    , default_category_id: 332 // EDITABILE - categoria assegnata al ticket. 332 = helpdesk
    , default_hd_state: "contracted" // non mi ricordo... :)
    , parent_issue_id: null
    , watcher_user_id: null // EDITABILE - se impostare sempre degli osservatori prestabiliti per questo tipo di ticket
    , is_private: null // se il ticket deve essere privato (non visibile a tutit i partecipanti del progetto)
    , tracker_id: null
    //, categories: [{id:11, name: "aspetti tecnici"}, {id:13,name:"aspetti amministrativati/contabili"}]
    , categories: [] // EDITABILE - se vuoi far scegliere all'utente una serie di possibili categorie
  };
  return Object.assign(state, prjData);
};
