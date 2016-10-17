(function () {
    "use strict";
    var partiesLV;
    WinJS.UI.Pages.define("/pages/home/home.html", {      
        ready: function (element, options) {           
            partiesLV = document.getElementById('listView').winControl;
            partiesLV.oniteminvoked = this._itemInvoked;
            partiesLV.element.focus();
            document.getElementById("newButton")
               .addEventListener("click", doClickNew, false);
            var createurl = "http://localhost:21962/api/PartyPlanner/";
            WinJS.xhr({
                type: "GET",
                url: createurl,
                headers: { "Content-type": "application/json; charset=utf-8" }
            }).then(success, error);

        }, _itemInvoked: function (args) {
            args.detail.itemPromise.done(function itemInvoked(item) {
                //Navigating to the manageParty.html on ListItem click
                WinJS.Navigation.navigate("/pages/manageparty/manageparty.html", { partyDetail: item.data });
            });
        }
    });

    function success(arg) {
        //HTTP Response binds to the ListView
        var parties =   [];
        parties = JSON.parse(arg.responseText);
        partiesLV.itemDataSource = new WinJS.Binding.List(parties).dataSource;
    }

    function error(arg) {
        //Display Error
    }

    function doClickNew() {
        //App bar button click
        WinJS.Navigation.navigate("/pages/manageparty/manageparty.html", null);
    }
})();
