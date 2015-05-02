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

                getEvents: function (id, quantity, forward) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.calendarBaseUrl + "GetEvents?id=" + id + "&quantity=" + quantity + "&forward=" + forward);
                },
            };
        });