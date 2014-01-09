angular.module('umbraco')
    .controller('EventCalendar.CalendarOverviewController', function ($scope, assetsService, calendarResource) {

        assetsService.loadCss("/App_Plugins/EventCalendar/css/DT_bootstrap.css");

        assetsService
            .loadJs("/App_Plugins/EventCalendar/scripts/jquery.dataTables.js")
            .then(function () {

                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/DT_bootstrap.js")
                    .then(function () {

                        var dataSource = [];

                        calendarResource.getall().then(function (response) {
                            angular.forEach(response.data, function (calendar) {
                                dataSource.push({ id: calendar.id, name: calendar.calendarname, gcal: calendar.isGCal, color: calendar.color });
                            });
                            console.log(dataSource);
                            $('#calendarOverview').dataTable({
                                "aaData": dataSource,
                                "aoColumns": [
                                { "mData": "name", "sTitle": "1. Name" },
                                { "mData": "gcal", "sTitle": "2. Uses Google calendar" },
                                { "mData": "color", "sTitle": "3. Color" },
                                ]
                            });

                        }, function (response) {
                            notificationsService.error("Error", "Could not load calendars");
                        });                        

                        //var dataSource = [
                        //    { id: 0, engine: "Trident", browser: "Internet Explorer 4.0", platform: "Win 95+", version: 4, grade: "X" },
                        //    { id: 1, engine: "Trident", browser: "Internet Explorer 5.0", platform: "Win 95+", version: 5, grade: "C" },
                        //    { id: 2, engine: "Trident", browser: "Internet Explorer 5.5", platform: "Win 95+", version: 5.5, grade: "A" },
                        //    { id: 3, engine: "Trident", browser: "Internet Explorer 6.0", platform: "Win 98+", version: 6, grade: "A" },
                        //    { id: 4, engine: "Trident", browser: "Internet Explorer 7.0", platform: "Win XP SP2+", version: 7, grade: "A" },
                        //    { id: 5, engine: "Gecko", browser: "Firefox 1.5", platform: "Win 98+ / OSX.2+", version: 1.8, grade: "A" },
                        //    { id: 6, engine: "Gecko", browser: "Firefox 2", platform: "Win 98+ / OSX.2+", version: 1.8, grade: "A" },
                        //    { id: 7, engine: "Gecko", browser: "Firefox 3", platform: "Win 2k+ / OSX.3+", version: 1.9, grade: "A" },
                        //    { id: 8, engine: "Webkit", browser: "Safari 1.2", platform: "OSX.3", version: 125.5, grade: "A" },
                        //    { id: 9, engine: "Webkit", browser: "Safari 1.3", platform: "OSX.3", version: 312.8, grade: "A" },
                        //    { id: 10, engine: "Webkit", browser: "Safari 2.0", platform: "OSX.4+", version: 419.3, grade: "A" },
                        //    { id: 11, engine: "Webkit", browser: "Safari 3.0", platform: "OSX.4+", version: 522.1, grade: "A" },
                        //    { id: 12, engine: "Trident", browser: "Internet Explorer 4.0", platform: "Win 95+", version: 4, grade: "X" },
                        //    { id: 13, engine: "Trident", browser: "Internet Explorer 5.0", platform: "Win 95+", version: 5, grade: "C" },
                        //    { id: 14, engine: "Trident", browser: "Internet Explorer 5.5", platform: "Win 95+", version: 5.5, grade: "A" },
                        //    { id: 15, engine: "Trident", browser: "Internet Explorer 6.0", platform: "Win 98+", version: 6, grade: "A" },
                        //    { id: 16, engine: "Trident", browser: "Internet Explorer 7.0", platform: "Win XP SP2+", version: 7, grade: "A" },
                        //    { id: 17, engine: "Gecko", browser: "Firefox 1.5", platform: "Win 98+ / OSX.2+", version: 1.8, grade: "A" },
                        //    { id: 18, engine: "Gecko", browser: "Firefox 2", platform: "Win 98+ / OSX.2+", version: 1.8, grade: "A" },
                        //    { id: 19, engine: "Gecko", browser: "Firefox 3", platform: "Win 2k+ / OSX.3+", version: 1.9, grade: "A" },
                        //    { id: 20, engine: "Webkit", browser: "Safari 1.2", platform: "OSX.3", version: 125.5, grade: "A" },
                        //    { id: 21, engine: "Webkit", browser: "Safari 1.3", platform: "OSX.3", version: 312.8, grade: "A" },
                        //    { id: 22, engine: "Webkit", browser: "Safari 2.0", platform: "OSX.4+", version: 419.3, grade: "A" },
                        //    { id: 23, engine: "Webkit", browser: "Safari 3.0", platform: "OSX.4+", version: 522.1, grade: "A" },
                        //    { id: 24, engine: "Trident", browser: "Internet Explorer 4.0", platform: "Win 95+", version: 4, grade: "X" },
                        //    { id: 25, engine: "Trident", browser: "Internet Explorer 5.0", platform: "Win 95+", version: 5, grade: "C" },
                        //    { id: 26, engine: "Trident", browser: "Internet Explorer 5.5", platform: "Win 95+", version: 5.5, grade: "A" },
                        //    { id: 27, engine: "Trident", browser: "Internet Explorer 6.0", platform: "Win 98+", version: 6, grade: "A" },
                        //    { id: 28, engine: "Trident", browser: "Internet Explorer 7.0", platform: "Win XP SP2+", version: 7, grade: "A" },
                        //    { id: 29, engine: "Gecko", browser: "Firefox 1.5", platform: "Win 98+ / OSX.2+", version: 1.8, grade: "A" }
                        //];                        
                    });
        });
});