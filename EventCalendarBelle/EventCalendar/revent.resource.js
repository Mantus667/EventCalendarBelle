angular.module("umbraco.resources")
        .factory("reventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get("EventCalendar/REventApi/GetById?id=" + id);
                },

                getall: function () {
                    return $http.get("EventCalendar/REventApi/GetAll");
                },

                save: function (event) {
                    return $http.post("EventCalendar/REventApi/PostSave", angular.toJson(event));
                },

                deleteById: function (id) {
                    return $http.delete("EventCalendar/REventApi/DeleteById?id=" + id);
                },

                getDayOfWeekValues: function () {
                    return $http.get("EventCalendar/REventApi/GetDayOfWeekValues");
                },

                getFrequencyTypes: function () {
                    return $http.get("EventCalendar/REventApi/GetFrequencyTypes");
                },

                getMonthlyIntervalValues: function () {
                    return $http.get("EventCalendar/REventApi/GetMonthlyIntervalValues");
                }
            };
        });