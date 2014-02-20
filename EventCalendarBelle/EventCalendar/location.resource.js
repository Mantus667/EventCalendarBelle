angular.module("umbraco.resources")
        .factory("locationResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.locationBaseUrl + "GetById?id=" + id);
                },

                getall: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.locationBaseUrl + "GetAll");
                },

                save: function (location) {
                    //console.log(angular.toJson(calendar));
                    return $http.post(Umbraco.Sys.ServerVariables.eventCalendar.locationBaseUrl + "PostSave", angular.toJson(location));
                },

                deleteById: function (id) {
                    return $http.delete(Umbraco.Sys.ServerVariables.eventCalendar.locationBaseUrl + "DeleteById?id=" + id);
                }
            };
        });