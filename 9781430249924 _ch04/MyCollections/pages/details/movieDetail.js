// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var binding = WinJS.Binding;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
   
    var movieDetail;

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

    WinJS.UI.Pages.define("/pages/details/movieDetail.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
       
        ready: function (element, options) {
            // TODO: Initialize the page here.
            movieDetail = options.movieDetail;
            var src = WinJS.Binding.as(movieDetail);
            var form = document.getElementById("divDetail");
            WinJS.Binding.processAll(form, src);
            document.getElementById("saveButton")
                 .addEventListener("click", doClickSave, false);
            document.getElementById("deleteButton")
                .addEventListener("click", doClickDelete, false);

            if (movieDetail.fromSearch == true) {
                document.getElementById("deleteButton").disabled = true;
                document.getElementById("title").innerText = "Add to collection";
            }           
        } 
    });

    function doClickSave() {       
        MyCollection.Movie.saveMovie(movieDetail.id, movieDetail.title, movieDetail.year, movieDetail.image, movieDetail.poster, movieDetail.status);
        WinJS.Navigation.back();        
    }

    function doClickDelete() {
        MyCollection.Movie.deleteMovie(movieDetail.id);
        WinJS.Navigation.back();
    }
})();
