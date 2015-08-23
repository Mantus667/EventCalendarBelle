angular.module('umbraco')
    .controller('EventCalendar.EventsOverviewController', function ($scope, assetsService, eventResource, $routeParams) {

        assetsService.loadCss("/App_Plugins/EventCalendar/css/DT_bootstrap.css");

        assetsService
            .loadJs("/App_Plugins/EventCalendar/scripts/jquery.dataTables.js")
            .then(function () {

                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/DT_bootstrap.js")
                    .then(function () {

                        var dataSource = [];

                        eventResource.getForCalendar($routeParams.id).then(function (response) {
                            angular.forEach(response.data, function (event) {
                                dataSource.push({ id: event.id, name: event.title });
                            });

                            $('#eventsOverview').dataTable({
                                "aaData": dataSource,
                                "aoColumns": [
                                    { "mData": "name", "sTitle": "1. Name" },
                                    { "mData": "id", "fnCreatedCell": buttonEditEvent }
                                ]
                            });

                        }, function (response) {
                            notificationsService.error("Error", "Could not load calendars");
                        });
                    });
            });
    });

function buttonEditEvent(nTd, sData, oData, iRow, iCol) {
    $(nTd).html('<a class="btn btn-success" href="#/eventCalendar/ecTree/editEvent/' + sData + '"><span class="icon icon-pencil"></span>Edit</a>');
}