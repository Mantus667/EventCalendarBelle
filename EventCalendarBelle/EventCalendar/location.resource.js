angular.module("umbraco.resources")
        .factory("locationResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get("EventCalendar/LocationApi/GetById?id=" + id);
                },

                getall: function () {
                    return $http.get("EventCalendar/LocationApi/GetAll");
                },

                save: function (location) {
                    //console.log(angular.toJson(calendar));
                    return $http.post("EventCalendar/LocationApi/PostSave", angular.toJson(location));
                },

                deleteById: function (id) {
                    return $http.delete("EventCalendar/LocationApi/DeleteById?id=" + id);
                }
            };
        });