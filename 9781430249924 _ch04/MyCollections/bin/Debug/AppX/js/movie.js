/// <reference path="//Microsoft.WinJS.1.0/js/base.js" /> 
/// <reference path="//Microsoft.WinJS.1.0/js/ui.js" /> 


(function () {
    "use strict";

    WinJS.Namespace.define("MyCollection", {
        Movie: WinJS.Class.define(

            function () {
                this.title = "";
                this.year = 0;
                this.image = "";
                this.isInCollection = false;
                this.status = "";
                this.poster = "";
                this.id = 0;
            },
             
            {
                getTitle: function () { return this.title; },
                setTitle: function (newValue) { this.title = newValue; },

                getImage: function () { return this.image; },
                setImage: function (newValue) { this.image = newValue; },

                getIsInCollection: function () { return this.isInCollection; },
                setIsInCollection: function (newValue) { this.isInCollection = newValue; },

                getYear: function () { return this.year; },
                setYear: function (newValue) {
                    this.year = newValue;
                },

                getPoster: function () { return this.poster; },
                setPoster: function (newValue) { this.poster = newValue; },

                getStatus: function () { return this.status; },
                setStatus: function (newValue) { this.status = newValue; },

                getID: function () { return this.id; },
                setID: function (newValue) {
                    this.id = newValue;
                },

                 
            },

           
            {
                buildMovie: function (model) {

                    var newMovie = new MyCollection.Movie();
                    if (model.hasOwnProperty("title")) {
                        newMovie.setTitle(model.title);
                    }
                    if (model.hasOwnProperty("year")) {
                        newMovie.setYear(model.year);
                    }                    
                    if (model.hasOwnProperty("movieId")) {
                        newMovie.setID(model.id);
                        newMovie.setIsInCollection(true);
                    }
                    if (model.hasOwnProperty("status")) {
                        newMovie.setStatus(model.status);
                    }
                    if (model.hasOwnProperty("thumbnail")) {
                        newMovie.setImage(model.thumbnail);
                    }
                    if (model.hasOwnProperty("poster")) {
                        newMovie.setPoster(model.poster);
                    }
                    //only if the request from rottentomatoes 
                    if (model.hasOwnProperty("posters")) {
                        newMovie.setImage(model.posters.thumbnail);
                        newMovie.setPoster(model.posters.detailed);
                    }
                    return new WinJS.Binding.as(newMovie);
                },                
               
                loadSearchResult: function (searchText) {
                    var searchUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies.json?apikey=cdz4j2qsae9bztxhk96km3f8&page_limit=10&q=" + searchText;                     
                    return WinJS.xhr({ url: searchUrl }).then(
                            function (result) {
                                var result = window.JSON.parse(result.responseText).movies;
                                 
                                var collection = new Array();

                                if (result) {
                                    result.forEach(function (newObject) {
                                        var newMovie = MyCollection.Movie.buildMovie(newObject);
                                        collection.push(newMovie);
                                    }); 
                                    var txn = MyCollection.db.transaction(["Movies"], "readonly");
                                    var movieCursorRequest = txn.objectStore("Movies").openCursor();
                                     movieCursorRequest.onsuccess = function (e) {
                                        var cursor = e.target.result;
                                        if (cursor) {
                                            var data = cursor.value;
                                            if (data.title.indexOf(searchText) > -1) {

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
                                            }
                                            cursor.continue();
                                        }
                                    };
                                }

                                return collection;
                            });
                },
               
                loadFromDB: function (searchText) {
                    var collection = new Array();                   
                    var txn = MyCollection.db.transaction(["Movies"], "readonly");
                    var movieCursorRequest = txn.objectStore("Movies").openCursor();
                    movieCursorRequest.onsuccess = function (e) {
                        var cursor = e.target.result;
                        if (cursor) {
                            var data = cursor.value;
                            if (data.title.indexOf(searchText) > -1) {

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
                            }
                            cursor.continue();
                        }
                    };
                    return collection;                            
                },

                deleteMovie: function ( id ) {
                    var txn = MyCollection.db.transaction(["Movies"], "readwrite");
                    var movieTable = txn.objectStore("Movies");                     
                    var deleteRequest = movieTable.delete(id)                   
                    deleteRequest.onsuccess = function () {
                        WinJS.log && WinJS.log("Movie Deleted: " + this + ".", "Log", "Status");

                    };
                    deleteRequest.onerror = function () {
                        WinJS && WinJS.log("Failed to delete Movie: " + this + ".", "Log", "error");
                    };
                },

                saveMovie: function (id, title, year, image, poster, status ) {
                    var txn = MyCollection.db.transaction(["Movies"], "readwrite");
                    var movieTable = txn.objectStore("Movies");
                    var saveRequest;
                    if (id > 0)
                        saveRequest = movieTable.put(
                        {
                            id: id,
                            title: title,
                            year: year,
                            image: image,
                            poster: poster,
                            status: status
                        });
                    else {
                        saveRequest = movieTable.add(
                        {                           
                            title: title,
                            year: year,
                            image: image,
                            poster: poster,
                            status: status
                        });

                    }
                     
                    saveRequest.onsuccess = function () {
                        WinJS.log && WinJS.log("Movie Updated: " + this + ".", "Log", "Status");

                    };
                    saveRequest.onerror = function () {
                        WinJS && WinJS.log("Failed to update Movie: " + this + ".", "Log", "error");
                    };
                },
               
            })//end  WinJS.Class.define
    });
})();