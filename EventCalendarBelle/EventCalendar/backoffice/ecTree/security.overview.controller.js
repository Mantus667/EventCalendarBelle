angular.module('umbraco')
    .controller('EventCalendar.SecurityOverviewController', function ($scope, assetsService, userResource) {

        assetsService.loadCss("/App_Plugins/EventCalendar/css/DT_bootstrap.css");

        assetsService
            .loadJs("/App_Plugins/EventCalendar/scripts/jquery.dataTables.js")
            .then(function () {

                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/DT_bootstrap.js")
                    .then(function () {

                        var dataSource = [];

                        userResource.getAllUser().then(function (response) {
                            console.log(response.data);
                            angular.forEach(response.data, function (user) {
                                dataSource.push({ id: user.Id, name: user.Name });
                            });

                            $('#securityOverview').dataTable({
                                "aaData": dataSource,
                                "aoColumns": [
                                    { "mData": "name", "sTitle": "1. Name" },
                                    { "mData": "id", "fnCreatedCell": buttonEditUser }
                                ]
                            });

                        }, function (response) {
                            notificationsService.error("Error", "Could not load calendars");
                        });
                    });
            });
    });

function buttonEditUser(nTd, sData, oData, iRow, iCol) {
    $(nTd).html('<a class="btn btn-success" href="#/eventCalendar/ecTree/editUser/' + sData + '"><span class="icon icon-pencil"></span>Edit</a>');
}