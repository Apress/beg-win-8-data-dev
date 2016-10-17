// For an introduction to the Navigation template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232506
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;

    var db = null;


app.addEventListener("activated", function (args) {
    if (args.detail.kind === activation.ActivationKind.launch) {
        if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
            // TODO: This application has been newly launched. Initialize
            // your application here.
        } else {
            // TODO: This application has been reactivated from suspension.
            // Restore application state here.
        }
           
        if (app.sessionState.history) {
            nav.history = app.sessionState.history;
        }
        args.setPromise(WinJS.UI.processAll().then(function () {
            if (nav.location) {
                nav.history.current.initialPlaceholder = true;
                return nav.navigate(nav.location, nav.state);
            } else {
                return nav.navigate(Application.navigator.home);
            }
        }));
        //creating the indexeddb database
        createDB();
    }
});

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. If you need to 
        // complete an asynchronous operation before your application is 
        // suspended, call args.setPromise().
        app.sessionState.history = nav.history;
    };

function createDB() {
    // Create the request to open the database, named CollectionDB. If it doesn't exist, create it and immediately
    // upgrade to version 1.
    var dbRequest = window.indexedDB.open("CollectionDB", 1);        
    dbRequest.onupgradeneeded = function (e) {
        MyCollection.db = e.target.result;
        var txn = e.target.transaction;
        var movieTable = MyCollection.db.createObjectStore(
                                                    "Movies"
                                                    ,{
                                                        keyPath: "id"
                                                        , autoIncrement: true
                                                    });
        movieTable.createIndex("title"
            , "title"
            , { unique: false });
        movieTable.createIndex("year"
            , "year"
            , { unique: false });
        movieTable.createIndex("image"
            , "image"
            , { unique: false });
        movieTable.createIndex("poster"
            , "poster"
            , { unique: false });
        movieTable.createIndex("status"
            , "status"
            , { unique: false });
        txn.onerror = function () {
            WinJS.log && WinJS.log("Database creation failed"
                , "Log"
                , "Status");
        };
        txn.oncomplete = function () {
            WinJS.log && WinJS.log("Database table created"
                , "Log"
                , "Status");
        };
    };
    dbRequest.onsuccess = function (e) {
        MyCollection.db = e.target.result;
    };
}

    WinJS.Namespace.define("MyCollection", {       
        db: db,
    });

    app.start();
})();


