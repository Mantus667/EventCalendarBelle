angular.module('umbraco')
    .controller('EventCalendar.LocationOverviewController', function ($scope, assetsService, locationResource) {

        assetsService.loadCss("/App_Plugins/EventCalendar/css/DT_bootstrap.css");

        assetsService
            .loadJs("/App_Plugins/EventCalendar/scripts/jquery.dataTables.js")
            .then(function () {

                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/DT_bootstrap.js")
                    .then(function () {

                        var dataSource = [];

                        locationResource.getall().then(function (response) {
                            angular.forEach(response.data, function (location) {
                                dataSource.push({ id: location.id, name: location.name, country: location.country, city: location.city });
                            });
                            console.log(dataSource);
                            $('#locationOverview').dataTable({
                                "aaData": dataSource,
                                "aoColumns": [
                                    { "mData": "name", "sTitle": "1. Name" },
                                    { "mData": "country", "sTitle": "2. Country" },
                                    { "mData": "city", "sTitle": "3. City" },
                                    { "mData": "id" }
                                ],
                                "aoColumnDefs": [{
                                    "aTargets": [3],
                                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                                        $(nTd).html('<a class="btn btn-success" href="#/eventCalendar/ecTree/editLocation/' + sData + '"><span class="icon icon-pencil"></span>Edit</a>');
                                    }
                                }]
                            });

                        }, function (response) {
                            notificationsService.error("Error", "Could not load locations");
                        });
                    });
            });
    });