angular.module("umbraco.resources")
        .factory("eventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get("EventCalendar/EventApi/GetById?id=" + id);
                },

                getall: function () {
                    return $http.get("EventCalendar/EventApi/GetAll");
                },

                save: function (calendar) {
                    return $http.post("EventCalendar/EventApi/PostSave", angular.toJson(calendar));
                },

                deleteById: function (id) {
                    return $http.delete("EventCalendar/EventApi/DeleteById?id=" + id);
                }
            };
        });