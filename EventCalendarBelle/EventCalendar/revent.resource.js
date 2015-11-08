﻿angular.module("umbraco.resources")
        .factory("reventResource", function ($http) {
            return {
                getById: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetById?id=" + id);
                },

                getall: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetAll");
                },

                getForCalendar: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetForCalendar?id=" + id);
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
                },

                getMonths: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetMonths");
                },

                getRTEConfiguration: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.descriptionBaseUrl + "GetRTEConfiguration");
                },

                getPaged: function (calendar, itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm) {
                    if (sortColumn == undefined)
                        sortColumn = "";
                    if (sortOrder == undefined)
                        sortOrder = "";
                    return $http.get(Umbraco.Sys.ServerVariables.eventCalendar.reventBaseUrl + "GetPaged?calendar=" + calendar + "&itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&searchTerm=" + searchTerm);
                }
            };
        });