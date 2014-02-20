angular.module("umbraco.resources")
        .factory("reventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetById?id=" + id);
                },

                getall: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetAll");
                },

                save: function (event) {
                    return $http.post(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "PostSave", angular.toJson(event));
                },

                deleteById: function (id) {
                    return $http.delete(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "DeleteById?id=" + id);
                },

                getDayOfWeekValues: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetDayOfWeekValues");
                },

                getFrequencyTypes: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetFrequencyTypes");
                },

                getMonthlyIntervalValues: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetMonthlyIntervalValues");
                }
            };
        });