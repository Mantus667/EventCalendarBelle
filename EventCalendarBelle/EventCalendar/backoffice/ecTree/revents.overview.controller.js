angular.module('umbraco')
    .controller('EventCalendar.REventsOverviewController', function ($scope, assetsService, reventResource, $routeParams, notificationsService) {

        assetsService
            .loadJs("/App_Plugins/EventCalendar/scripts/jquery.dataTables.js")
            .then(function () {

                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/DT_bootstrap.js")
                    .then(function () {

                        var dataSource = [];

                        reventResource.getForCalendar($routeParams.id).then(function (response) {
                            angular.forEach(response.data, function (revent) {
                                dataSource.push({ id: revent.id, name: revent.title });
                            });

                            $('#reventsOverview').dataTable({
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
    $(nTd).html('<a class="btn btn-success" href="#/eventCalendar/ecTree/editREvent/' + sData + '"><span class="icon icon-pencil"></span>Edit</a>');
}