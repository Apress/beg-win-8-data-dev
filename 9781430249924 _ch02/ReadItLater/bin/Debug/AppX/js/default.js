// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var applicationData = Windows.Storage.ApplicationData.current;
    var consumer_id = "10921-65ff0df65a5aa76bc4f4eaa3";
    var request_code = "";
    var access_token = "";
    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
       
            var localSettings = applicationData.localSettings;
            access_token = localSettings.values["pocket_access_token"]
           if (access_token != null) {
                retriveList(access_token);
             }
             else {
                launchPocketWebAuth();
            }
        }
    }; 

function launchPocketWebAuth() {
    var pocketReqUrl = "https://getpocket.com/v3/oauth/request";    
    var callbackURL = "readitlater123:authorizationFinished";
    var dataString = "consumer_key=" + consumer_id
                        + "&redirect_uri=" + callbackURL;
    try {
        WinJS.xhr({
            type: "post"
            , data: dataString
            , url: pocketReqUrl
            , headers: {
                "Content-type": "application/x-www-form-urlencoded; charset=UTF8"
            }
            }).done(
                function (request) {          
                    request_code = getParameterByName("code", request.responseText); 
                    var pocketAuthUrl = "https://getpocket.com/auth/authorize?request_token=";           
                    var authCallbackURL = "http://www.myweb.com";
                    pocketAuthUrl += request_code
                        + "&redirect_uri=" + encodeURIComponent(authCallbackURL)
                        + "&webauthenticationbroker=1";
                    var startURI = new Windows.Foundation.Uri(pocketAuthUrl);
                    var endURI = new Windows.Foundation.Uri(authCallbackURL);
                    Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
                        Windows.Security.Authentication.Web.WebAuthenticationOptions.useTitle
                        , startURI
                        , endURI).then(callbackPocketWebAuth, callbackPocketWebAuthError);            
                                    },
                                    function error(error) {
                                        //handle error here               
                                    },
                                    function progress(result) {
                                        //Do somehting to show the progress
                                    });

                        }
                        catch (err) {
                            /*Error launching WebAuth"*/
                            return;
                        }
                    }

    function callbackPocketWebAuth(result) {        
        var pocketAuthUrl = "https://getpocket.com/v3/oauth/authorize"; 
        var dataString = "consumer_key=" + consumer_id
            + "&code=" + request_code;

        WinJS.xhr({
            type: "post"
            , data: dataString
            , url: pocketAuthUrl
            , headers: { "Content-type": "application/x-www-form-urlencoded; charset=UTF8" }
        }).done(
                function (request) {
                    var access = request.responseText;
                    access_token = getParameterByName("access_token", access);
                    var username = getParameterByName("username", access);
                    var localSettings = applicationData.localSettings;
                    localSettings.values["pocket_access_token"] = access_token;
                    localSettings.values["pocket_username"] = access_token;
                    retriveList(access_token);
                },
                function error(error) {
                    //handle error here
                },
                function progress(result) {
                    //Do something to show the progress
                });

        if (result.responseStatus == 2) {
            response += "Error returned: " + result.responseErrorDetail + "\r\n";
        }
    }

    function callbackPocketWebAuthError(result) {
       //handle error here
    }

    function OKbuttonClickHandler(eventInfo) {
        var localSettings = applicationData.localSettings;
        access_token = localSettings.values["pocket_access_token"]
        if (access_token != null) {
            retriveList(access_token);
        }
        else {
            launchPocketWebAuth();
        }
    }

    function retriveList(token) {
        var pocketGetUrl = "https://getpocket.com/v3/get";
        var dataString = "detailType=simple&consumer_key=" + consumer_id
            + "&access_token=" + token;
        WinJS.xhr({
            type: "post"
            , data: dataString
            , url: pocketGetUrl
            , headers: { "Content-type": "application/x-www-form-urlencoded; charset=UTF8" }
        })
        .done(
            function (response) {             
                var json = JSON.parse(response.responseText);
                ko.applyBindings(new ArticleViewModel(response.responseText));                  
            },
            function error(error) {
                //handle error here
            },
            function progress(result) {
                //Do something to show the progress
            });
    }

    function Item(item) {
        var self = this;
        self.title = item.given_title;
        self.url = item.given_url;
        self.excerpt = item.excerpt;
    }

    function ListStatus(list) {
        var self = this;        
        self.keys = ko.observableArray(ko.utils.arrayMap(list, function (article) {
            return new Item(article);
        }));
    }
 
    function ArticleViewModel(data) {
        var self = this;
        self.selectedItem = ko.observable();
        self.setItem = function (item) {
            self.selectedItem(item);
        }

        self.ShowContent = function (url) {
            $.get(url, function (data) {
                var szStaticHTML = toStaticHTML(data);
                $('#siteloader').html(szStaticHTML);
            });
        };      

        /* uncomment this section to show load data from pocket bookmark service
        self.model = ko.utils.arrayMap(data, function (jsonData) {
          return new ListStatus(jsonData.list);
            });
      
      */

        /* If you have account with pocket and like to use it then comment the below section and uncomment the above section*/
        var dummyData = [{
            "status": 1,
            "complete": 1,
            "list": [
                {
                    "item_id": "190359798",
                    "resolved_id": "190359798",
                    "given_url": "http:\/\/www.codeguru.com\/csharp\/implementing-mvvm-pattern-in-web-applications-using-knockout.html",
                    "given_title": "Implementing MVVM Pattern in Web Applications Using Knockout",
                     "excerpt": "Data driven web sites rely heavily on JavaScript and JavaScript based libraries such as jQuery. The client side programming tends to become complex as the user interface becomes more and more rich. In such situations features such as data binding and dependency tracking are highly desirable."
                },
                {
                    "item_id": "258150679",
                    "resolved_id": "258150679",
                    "given_url": "http:\/\/devhammer.net\/blog\/building-back-end-data-and-services-for-windows-8-apps-odata---part-2",
                    "given_title": "Devhammer's Den - Building Back-end Data and Services for Windows 8 Apps: O",
                    "excerpt": "In part 1 of this post, I showed how to create a SQL database in Windows Azure, create a schema for adding leaderboard functionality to a game, create an Entity Framework model for the database, and then create and test a WCF Data Service on top of the model that provides a rich REST-style interacti"
                },
                {
                    "item_id": "242786115",
                    "resolved_id": "242786115",
                    "given_url": "http:\/\/blog.stevensanderson.com\/2012\/10\/29\/knockout-2-2-0-released\/",
                    "given_title": "Knockout 2.2.0 released - Steve Sanderson\u2019s blog - As seen on YouTube\u2122",                     
                    "excerpt": "It\u2019s been five months since the last significant Knockout release, so it\u2019s about time for another! The core team and many contributors have been hard at work adding some sweet new features, performance upgrades, architectural improvements, and bug fixes.",                    
                },
               {
              "item_id": "241035211",
                    "resolved_id": "241035211",
                    "given_url": "http:\/\/www.codeproject.com\/Articles\/483700\/RecreatingplustheplusWindowsplus8plusSkyDriveplusA",
                    "given_title": "Recreating the Windows 8 SkyDrive App",                   
                    "excerpt": "The SkyDrive Windows 8 application allows a user to manage their SkyDrive account. In this post I am going to show how to recreate the Folder\/File User Interface in a WinJS (HTML & Javascript) project.",                     
                },
            ],
            "since": 1354335928
        }];
       
         self.model = ko.utils.arrayMap(dummyData, function (jsonData) {
            return new ListStatus(jsonData.list);
         });

        /* dummy data section ends*/
       
     
    }

    function getParameterByName(key, src) {       
        key = key.replace(/[*+?^$.\[\]{}()|\\\/]/g, "\\$&"); // escape RegEx meta chars        
        var match = src.match(new RegExp(  key + "=([^&]+)(&|$)"));
        return match && decodeURIComponent(match[1].replace(/\+/g, " "));
    }

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };
    
    app.start();
     
})();


