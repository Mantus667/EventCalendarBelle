angular.module("umbraco.resources")
        .factory("eventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "GetById?id=" + id);
                },

                getall: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "GetAll");
                },                

                save: function (calendar) {
                    return $http.post(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "PostSave", angular.toJson(calendar));
                },

                deleteById: function (id) {
                    return $http.delete(Umbraco.Sys.ServerVariables.eventCalendar.eventBaseUrl + "DeleteById?id=" + id);
                }
            };
        });