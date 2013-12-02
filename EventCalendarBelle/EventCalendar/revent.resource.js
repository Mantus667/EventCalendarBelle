angular.module("umbraco.resources")
        .factory("reventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get("EventCalendar/REventApi/GetById?id=" + id);
                },

                getall: function () {
                    return $http.get("EventCalendar/REventApi/GetAll");
                },

                save: function (calendar) {
                    return $http.post("EventCalendar/REventApi/PostSave", angular.toJson(calendar));
                },

                deleteById: function (id) {
                    return $http.delete("EventCalendar/REventApi/DeleteById?id=" + id);
                }
            };
        });