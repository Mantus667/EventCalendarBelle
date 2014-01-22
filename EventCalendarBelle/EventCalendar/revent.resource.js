angular.module("umbraco.resources")
        .factory("reventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get("backoffice/EventCalendar/REventApi/GetById?id=" + id);
                },

                getall: function () {
                    return $http.get("backoffice/EventCalendar/REventApi/GetAll");
                },

                save: function (event) {
                    return $http.post("backoffice/EventCalendar/REventApi/PostSave", angular.toJson(event));
                },

                deleteById: function (id) {
                    return $http.delete("backoffice/EventCalendar/REventApi/DeleteById?id=" + id);
                },

                getDayOfWeekValues: function () {
                    return $http.get("backoffice/EventCalendar/REventApi/GetDayOfWeekValues");
                },

                getFrequencyTypes: function () {
                    return $http.get("backoffice/EventCalendar/REventApi/GetFrequencyTypes");
                },

                getMonthlyIntervalValues: function () {
                    return $http.get("backoffice/EventCalendar/REventApi/GetMonthlyIntervalValues");
                }
            };
        });