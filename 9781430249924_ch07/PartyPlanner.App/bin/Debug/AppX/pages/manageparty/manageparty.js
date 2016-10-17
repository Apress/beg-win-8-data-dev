// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var party = { PartyID: 0, PartyName: "", DateTime: "", Completed: "false" };
    var shoppingLV;
    var guestLV;
    WinJS.Namespace.define("Binding.Mode", {
        twoway: WinJS.Binding.initializer(function (source, sourceProps, dest, destProps) {
            WinJS.Binding.defaultBind(source, sourceProps, dest, destProps);
            dest.onchange = function () {
                var d = dest[destProps[0]];
                var s = source[sourceProps[0]];
                if (s !== d) source[sourceProps[0]] = d;
            }
        })
    });

    WinJS.UI.Pages.define("/pages/manageparty/manageparty.html", {      
        ready: function (element, options) {
            //Hide right column on page ready
            $('.rightColumn').hide();
            //assign the event handler for the buttons
            document.getElementById("saveParty").addEventListener("click", onSaveParty, false);
            document.getElementById("showShopping").addEventListener("click", onShowShopping, false);
            document.getElementById("showGuest").addEventListener("click", onShowGuest, false);
            document.getElementById("addToShoppingList").addEventListener("click", onAddToShoppingList, false);
            document.getElementById("addToGuestList").addEventListener("click", onAddToGuestList, false);
            
            //ListView control to local variable
            shoppingLV = document.getElementById('shoppingListView').winControl;
            guestLV = document.getElementById('guestListView').winControl;

            //Get the Party object as parameter and update the listview
            if (options != null && options.partyDetail != null) {
                party = options.partyDetail;
                UpdateUI();
            }
            else {
                party = { PartyID: 0, PartyName: "", DateTime: "", Completed: "false" };               
            }
            var src = WinJS.Binding.as(party);
            var form = document.getElementById("divDetail");
            WinJS.Binding.processAll(form, src);
        },      
    });

    function UpdateUI() {
        //Check to see if Party is already created
        if (party.PartyID > 0) {
            $('.rightColumn').show();
            if (party.Guests == null) {
                $("#lblGuestMessage").text('No guest is invited to this party!')
            } else {
                $("#lblGuestMessage").text('');
            }
            if (party.ShoppingList == null) {
                $("#lblItemMessage").text('No item to the shoppinglist is added.')
            } else {
                $("#lblItemMessage").text('');
            }
        }
        //binding Guests and ShoppingItem to ListViews
        shoppingLV.itemDataSource = new WinJS.Binding.List(party.ShoppingList).dataSource;
        guestLV.itemDataSource = new WinJS.Binding.List(party.Guests).dataSource;
    }


    function onShowShopping() {
        var btnShopping = document.getElementById("showShopping");
        document.getElementById("shoppingItemFlyout").winControl.show(btnShopping);
    }

    function onShowGuest() {
        var btnGuest = document.getElementById("showGuest");
        document.getElementById("guestFlyout").winControl.show(btnGuest);
    }

    /*
    function onShowGuest() {

        var btnGuest = document.getElementById("showGuest");
        document.getElementById("guestFlyout").winControl.show(btnGuest);
    }*/

    

    function onAddToShoppingList() {
        if (party.ShoppingList == null) {
            party.ShoppingList = [];
        }
        party.ShoppingList.push(
            {ShoppingItemID:"0", ItemName:  $("#shopppingItem").val() , Quantity:    $("#quantity").val()}
        );
        sendPartyToService("PUT");
        document.getElementById("shoppingItemFlyout").winControl.hide();    
    }

    function onAddToGuestList() {
        if (party.Guests == null) {
            party.Guests = [];
        }
        party.Guests.push(
            {GuestID:"0", FamilyName:  $("#guestName").val() , NumberOfPeople:    $("#noofguest").val()}
        );       
        sendPartyToService("PUT");
        document.getElementById("guestFlyout").winControl.hide();    
    }
    
    function onSaveParty() {
        var createurl = "http://localhost:21962/api/PartyPlanner/";
        sendPartyToService("POST")
    }

    function sendPartyToService(method) {
        var createurl = "http://localhost:21962/api/PartyPlanner/" + party.PartyID;
        WinJS.xhr({
            type: method,
            url: createurl,
            headers: { "Content-type": "application/json; charset=utf-8" },
            data: JSON.stringify(party)
        }).then(success, error);
    }

    function success(arg) {
        party = JSON.parse(arg.responseText);
        UpdateUI();
    }

    function error(arg) {
        //Display error
    }



})();
