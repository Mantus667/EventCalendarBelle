angular.module("umbraco").controller("EventCalendar.REventCreateController",
        function ($scope, $routeParams, reventResource, locationResource, notificationsService, assetsService, navigationService) {

            $scope.event = { id: 0, calendarid: $scope.currentNode.id.replace("c-", ""), allDay: false };

            reventResource.getDayOfWeekValues().then(function (response) {
                $scope.DayOfWeekList = response.data;
                $scope.event.day = $scope.DayOfWeekList[0].Key;
            });

            reventResource.getFrequencyTypes().then(function (response) {
                $scope.FrequencyTypes = response.data;
                $scope.event.frequency = $scope.FrequencyTypes[0].Key;
            });

            reventResource.getMonthlyIntervalValues().then(function (response) {
                $scope.MonthlyIntervals = response.data;
                $scope.event.monthly = $scope.MonthlyIntervals[0].Key;
            });

            locationResource.getall().then(function (response) {
                $scope.locations = response.data;
                $scope.event.locationId = $scope.locations[0].id;
            }, function (response) {
                notificationsService.error("Error", "Could not load locations");
            });

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