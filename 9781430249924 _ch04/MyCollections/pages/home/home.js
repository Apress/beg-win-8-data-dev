(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var listView = element.querySelector(".resultslist").winControl;
            var tapBehavior = listView.tapBehavior;
            listView.tapBehavior = tapBehavior;
            listView.oniteminvoked = this._itemInvoked;
            var collection = new Array();
            var txn = MyCollection.db.transaction(["Movies"], "readonly");
            var movieCursorRequest = txn.objectStore("Movies").openCursor();
            movieCursorRequest.onsuccess = function (e) {              
                var cursor = e.target.result;
                if (cursor) {
                    var data = cursor.value;
                    var movieData = {
                                    movieId: data.id
                                    , title: data.title
                                    , year: data.year
                                    , thumbnail: data.image
                                    , poster: data.poster
                                    , status: data.status
                    };                    
                    var newMovie = MyCollection.Movie.buildMovie(movieData);
                    collection.push(newMovie);
                    cursor.continue();
                   
                }
                else {                         
                    listView.itemDataSource = new WinJS.Binding.List(collection).dataSource;
                  
                }
            }
        },
        _itemInvoked: function (args) {
            args.detail.itemPromise.done(function itemInvoked(item) {
                WinJS.Navigation.navigate("/pages/details/movieDetail.html", { movieDetail: item.data });
            });
        },
        
    });

})();
