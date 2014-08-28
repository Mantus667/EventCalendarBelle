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
                }
            };
        });