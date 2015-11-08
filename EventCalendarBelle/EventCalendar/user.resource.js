angular.module("umbraco.resources")
        .factory("userResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.userBaseUrl + "GetById?id=" + id);
                },

                getNameOfUser: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.userBaseUrl + "GetNameOfUser?id=" + id);
                },

                getall: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.userBaseUrl + "GetAll");
                },

                save: function (user) {
                    return $http.post(Umbraco.Sys.ServerVariables.eventCalendar.userBaseUrl + "PostSave", angular.toJson(user));
                },

                getAllUser: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.userBaseUrl + "GetAllUser");
                },

                getPaged: function (itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm) {
                    if (sortColumn == undefined)
                        sortColumn = "";
                    if (sortOrder == undefined)
                        sortOrder = "";
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.userBaseUrl + "GetPaged?itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&searchTerm=" + searchTerm);
                }
            };
        });