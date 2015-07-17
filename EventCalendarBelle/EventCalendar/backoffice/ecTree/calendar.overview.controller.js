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

                            $('#calendarOverview').dataTable({
                                "aaData": dataSource,
                                "aoColumns": [
                                    { "mData": "name", "sTitle": "1. Name" },
                                    { "mData": "gcal", "sTitle": "2. Uses Google calendar", "fnCreatedCell": gcal },
                                    { "mData": "color", "sTitle": "3. Color", "fnCreatedCell": color },
                                    { "mData": "id", "fnCreatedCell": buttonEditCalendar }
                                ]
                            });

                        }, function (response) {
                            notificationsService.error("Error", "Could not load calendars");
                        });                      
                    });
        });
    });

function buttonEditCalendar(nTd, sData, oData, iRow, iCol) {
    $(nTd).html('<a class="btn btn-success" href="#/eventCalendar/ecTree/editCalendar/' + sData + '"><span class="icon icon-pencil"></span>Edit</a>');
}

function color(nTd, sData, oData, iRow, iCol) {
    $(nTd).html('<span style="background-color:' + sData + ';display:block;height:25px;width:25px;"></span>');
}

function gcal(nTd, sData, oData, iRow, iCol) {
    if (sData == true) {
        $(nTd).html('<span class="icon icon-check"></span>');
    } else {
        $(nTd).html('<span class="icon icon-delete"></span>');
    }
}