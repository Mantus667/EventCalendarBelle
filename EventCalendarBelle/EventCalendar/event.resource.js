angular.module("umbraco.resources")
        .factory("eventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get("backoffice/EventCalendar/EventApi/GetById?id=" + id);
                },

                getall: function () {
                    return $http.get("backoffice/EventCalendar/EventApi/GetAll");
                },

                save: function (calendar) {
                    return $http.post("backoffice/EventCalendar/EventApi/PostSave", angular.toJson(calendar));
                },

                deleteById: function (id) {
                    return $http.delete("backoffice/EventCalendar/EventApi/DeleteById?id=" + id);
                }
            };
        });