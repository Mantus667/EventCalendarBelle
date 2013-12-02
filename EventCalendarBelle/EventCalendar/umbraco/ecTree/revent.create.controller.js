angular.module("umbraco").controller("EventCalendar.EventCreateController",
        function ($scope, $routeParams, reventResource, notificationsService, assetsService, navigationService) {

            $scope.event = { id: 0, calendarid: $scope.currentNode.id.replace("c-", "") };

            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.css");

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                .then(function () {
                    $('#allday').on('switch-change', function (e, data) {
                        $scope.event.allday = data.value;
                    });
                });

            $scope.save = function (event) {
                console.log(event);
                reventResource.save(event).then(function (response) {
                    $scope.event = response.data;

                    notificationsService.success("Success", event.title + " has been created");
                    navigationService.reloadNode($scope.currentNode.parent());
                    navigationService.hideNavigation();
                }, function (response) {
                    notificationsService.error("Error", event.title + " could not be created");
                });
            };

        });