angular.module("umbraco.resources")
        .factory("eventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "GetById?id=" + id);
                },

                getall: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "GetAll");
                },

                getForCalendar: function(id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "GetForCalendar?id=" + id);
                },

                save: function (calendar) {
                    return $http.post(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "PostSave", angular.toJson(calendar));
                },

                deleteById: function (id) {
                    return $http.delete(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "DeleteById?id=" + id);
                },

                getRTEConfiguration: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.descriptionBaseUrl + "GetRTEConfiguration");
                },

                getPaged: function (type, itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm) {
                    if (sortColumn == undefined)
                        sortColumn = "";
                    if (sortOrder == undefined)
                        sortOrder = "";
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "GetPaged?itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&searchTerm=" + searchTerm);
                }
            };
        });