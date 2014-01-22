angular.module("umbraco.resources")
        .factory("calendarResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get("backoffice/EventCalendar/CalendarApi/GetById?id=" + id);
                },

                getall: function () {
                    return $http.get("backoffice/EventCalendar/CalendarApi/GetAll");
                },

                save: function (calendar) {
                    //console.log(angular.toJson(calendar));
                    return $http.post("backoffice/EventCalendar/CalendarApi/PostSave", angular.toJson(calendar));
                },

                deleteById: function (id) {
                    return $http.delete("backoffice/EventCalendar/CalendarApi/DeleteById?id=" + id);
                }
            };
        });