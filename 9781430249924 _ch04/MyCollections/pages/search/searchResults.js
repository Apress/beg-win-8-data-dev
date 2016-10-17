// For an introduction to the Search Contract template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232512

// TODO: Add the following script tag to the start page's head to
// subscribe to search contract events.
//  
// <script src="/searchResults.js"></script>

(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var appModel = Windows.ApplicationModel;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var searchPageURI = "/pages/search/searchResults.html";

    ui.Pages.define(searchPageURI, {
        _filters: [],
        _lastSearch: "",

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
          
            var listView = element.querySelector(".resultslist").winControl;
            // listView.itemTemplate = element.querySelector(".itemtemplate");
            listView.oniteminvoked = this._itemInvoked;
            listView.itemTemplate = itemTemplateFunction;          
            this._handleQuery(element, options);
            listView.element.focus();
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            var listView = element.querySelector(".resultslist").winControl;
            if (lastViewState !== viewState) {
                if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
                    var handler = function (e) {
                        listView.removeEventListener("contentanimating", handler, false);
                        e.preventDefault();
                    }
                    listView.addEventListener("contentanimating", handler, false);
                    var firstVisible = listView.indexOfFirstVisible;
                    this._initializeLayout(listView, viewState);
                    if (firstVisible >= 0 && listView.itemDataSource.list.length > 0) {
                        listView.indexOfFirstVisible = firstVisible;
                    }
                }
            }
        },

        // This function filters the search data using the specified filter.
        _applyFilter: function (filter, originalResults) {
            if (filter.results === null) {
                filter.results = originalResults.createFiltered(filter.predicate);
            }
            return filter.results;
        },

        // This function responds to a user selecting a new filter. It updates the
        // selection list and the displayed results.
        _filterChanged: function (element, filterIndex) {
            var filterBar = element.querySelector(".filterbar");
            var listView = element.querySelector(".resultslist").winControl;

            utils.removeClass(filterBar.querySelector(".highlight"), "highlight");
            utils.addClass(filterBar.childNodes[filterIndex], "highlight");

            element.querySelector(".filterselect").selectedIndex = filterIndex;

            listView.itemDataSource = this._filters[filterIndex].results.dataSource;
        },

        _generateFilters: function () {
            this._filters = [];
            this._filters.push(
                {
                    results: null
                    , text: "All"
                    , predicate: function (item) {
                        return true;
                    }
            });
            this._filters.push(
                {
                    results: null
                    , text: "Online"
                    , predicate: function (item) {
                        return item.isInCollection === true;
                    }
                });
            this._filters.push(
                {
                    results: null
                    , text: "In My Collection"
                    , predicate: function (item) {
                        return item.isInCollection !== true;
                    }
                });
        },
        // This function updates the ListView with new layouts
        _initializeLayout: function (listView, viewState) {
            /// <param name="listView" value="WinJS.UI.ListView.prototype" />

            if (viewState === appViewState.snapped) {
                listView.layout = new ui.ListLayout();
                document.querySelector(".titlearea .pagetitle").textContent = '“' + this._lastSearch + '”';
                document.querySelector(".titlearea .pagesubtitle").textContent = "";
            } else {
                listView.layout = new ui.GridLayout();

                // TODO: Change "App Name" to the name of your app.
                document.querySelector(".titlearea .pagetitle").textContent = "My Collections!";
                document.querySelector(".titlearea .pagesubtitle").textContent = "Results for “" + this._lastSearch + '”';
            }
        },
        // This function colors the search term. Referenced in /searchResults.html
        // as part of the ListView item templates.
        _markText: function (text) {
            return ""// text.replace(this._lastSearch, "<mark>" + this._lastSearch + "</mark>");
        },

        // This function generates the filter selection list.
        _populateFilterBar: function (element, originalResults) {
            var filterBar = element.querySelector(".filterbar");
            var listView = element.querySelector(".resultslist").winControl;
            var li, option, filterIndex;

            filterBar.innerHTML = "";
            for (filterIndex = 0; filterIndex < this._filters.length; filterIndex++) {
                this._applyFilter(this._filters[filterIndex], originalResults);

                li = document.createElement("li");
                li.filterIndex = filterIndex;
                li.tabIndex = 0;
                li.textContent = this._filters[filterIndex].text + " (" + this._filters[filterIndex].results.length + ")";
                li.onclick = function (args) { this._filterChanged(element, args.target.filterIndex); }.bind(this);
                li.onkeyup = function (args) {
                    if (args.key === "Enter" || args.key === "Spacebar")
                        this._filterChanged(element, args.target.filterIndex);
                }.bind(this);
                utils.addClass(li, "win-type-interactive");
                utils.addClass(li, "win-type-x-large");
                filterBar.appendChild(li);

                if (filterIndex === 0) {
                    utils.addClass(li, "highlight");
                    listView.itemDataSource = this._filters[filterIndex].results.dataSource;
                }

                option = document.createElement("option");
                option.value = filterIndex;
                option.textContent = this._filters[filterIndex].text + " (" + this._filters[filterIndex].results.length + ")";
                element.querySelector(".filterselect").appendChild(option);
            }

            element.querySelector(".filterselect").onchange = function (args) { this._filterChanged(element, args.currentTarget.value); }.bind(this);
        },

        _itemInvoked: function (args) {
            args.detail.itemPromise.done(function itemInvoked(item) {
                WinJS.Navigation.navigate("/pages/details/movieDetail.html"
                    , {
                        movieDetail: item.data
                    });
            });
        },
        // This function executes each step required to perform a search.
        _handleQuery: function (element, args) {
            var originalResults;
            this._lastSearch = args.queryText;
            WinJS.Namespace.define("searchResults"
                , {
                    markText: WinJS.Binding.converter(this._markText.bind(this))
                });
            this._initializeLayout(element.querySelector(".resultslist").winControl
                , Windows.UI.ViewManagement.ApplicationView.value);
            this._generateFilters();
            this._searchData(args.queryText, element, this);
        },
        // This function populates a WinJS.Binding.List with search results for the
        // provided query.
        _searchData: function (queryText, element, object) {
            var originalResults;               
            originalResults = MyCollection.Movie.loadFromDB(queryText);
            MyCollection.Movie.loadSearchResult(queryText).done(
            function (result) {
                for (var i = 0; i < result.length; i++)  
                {
                    originalResults.push(result[i]);
                }
                originalResults = new WinJS.Binding.List(originalResults);
                    if (originalResults.length === 0) {
                        document.querySelector('.filterarea').style.display = "none";
                    } else {
                        document.querySelector('.resultsmessage').style.display = "none";
                    }
                   
                    
                    object._populateFilterBar(element, originalResults);
                    object._applyFilter(object._filters[0], originalResults);
                    return originalResults;
                });         
            }
        });

    
    function itemTemplateFunction(itemPromise) {
        return itemPromise.then(function (item) {             
            var itemTemplate = document.getElementById("onlineItemtemplate");
            if (item.data.isInCollection) {
                itemTemplate = document.getElementById("dbItemtemplate");
            };
           
            var container = document.createElement("div");
            itemTemplate.winControl.render(item.data, container);
            return container;
        });
    }

    WinJS.Application.addEventListener("activated", function (args) {
        if (args.detail.kind === appModel.Activation.ActivationKind.search) {
            args.setPromise(ui.processAll().then(function () {
                if (!nav.location) {
                    nav.history.current = { location: Application.navigator.home, initialState: {} };
                }
                var searchListView = document.getElementById("searchListView");            
                return nav.navigate(searchPageURI, { queryText: args.detail.queryText });
            }));
        }
    });

    appModel.Search.SearchPane.getForCurrentView().onquerysubmitted = function (args) { nav.navigate(searchPageURI, args); };
})();
