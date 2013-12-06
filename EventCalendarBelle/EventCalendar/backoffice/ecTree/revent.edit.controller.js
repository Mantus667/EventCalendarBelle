angular.module("umbraco").controller("EventCalendar.REventEditController",
        function ($scope, $routeParams, reventResource, locationResource, notificationsService, assetsService) {

            //get a calendar id -> service
            reventResource.getById($routeParams.id.replace("re-", "")).then(function (response) {
                $scope.event = response.data;
                console.log($scope.event);                

                //Create the tabs for every language etc
                $scope.tabs = [{ id: "Content", label: "Content" }];
                angular.forEach($scope.event.descriptions, function (value, key) {
                    this.push({ id: key, label: value.culture });
                }, $scope.tabs);
            }, function (response) {
                notificationsService.error("Error", $scope.currentNode.name + " could not be loaded");
            });

            //Load all locations
            locationResource.getall().then(function (response) {
                $scope.locations = response.data;
            }, function (response) {
                notificationsService.error("Error", "Could not load locations");
            });

            reventResource.getDayOfWeekValues().then(function (response) {
                $scope.DayOfWeekList = response.data;
            });

            reventResource.getFrequencyTypes().then(function (response) {
                $scope.FrequencyTypes = response.data;
            });

            reventResource.getMonthlyIntervalValues().then(function (response) {
                $scope.MonthlyIntervals = response.data;
            });

            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/eventcalendar.custom.css");

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                .then(function () {
                    if ($scope.event) {
                        $('#allday').bootstrapSwitch('setState', $scope.event.allday, true);
                    }
                    $('#allday').on('switch-change', function (e, data) {
                        $scope.event.allday = data.value;
                    });
                });

            $scope.save = function (event) {
                console.log(event);
                //if (event.$valid) {
                reventResource.save(event).then(function (response) {
                    $scope.event = response.data;
                    notificationsService.success("Success", event.title + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", event.title + " could not be saved");
                });
                //} else {
                //    notificationsService.error("Error", "Form is not valid!");
                //}
            };

        });