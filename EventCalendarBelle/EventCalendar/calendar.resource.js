angular.module("umbraco.resources")
        .factory("calendarResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.calendarBaseUrl + "GetById?id=" + id);
                },

                getall: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.calendarBaseUrl + "GetAll");
                },

                save: function (calendar) {
                    console.log(angular.toJson(calendar));
                    return $http.post(Umbraco.Sys.ServerVariables.eventCalendar.calendarBaseUrl + "PostSave", angular.toJson(calendar));
                },

                deleteById: function (id) {
                    return $http.delete(Umbraco.Sys.ServerVariables.eventCalendar.calendarBaseUrl + "DeleteById?id=" + id);
                },

                getPaged: function (itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm) {
                    if (sortColumn == undefined)
                        sortColumn = "";
                    if (sortOrder == undefined)
                        sortOrder = "";
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.calendarBaseUrl + "GetPaged?itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&searchTerm=" + searchTerm);
                }
            };
        });