angular.module("umbraco").controller("EventCalendar.CalendarEditorController",
        function ($scope, $routeParams, calendarResource, notificationsService, assetsService) {

            $scope.calendars = [];

            //Load all calendar
            calendarResource.getall().then(function (response) {
                $scope.calendars = response.data;
                $scope.calendars.splice(0, 0, { id: 0, calendarname: 'All calendars' });
                console.log($scope.calendars);
            }, function (response) {
                notificationsService.error("Error", "Could not load calendar");
            }).then(function () {
            });

            //$scope.$watch('selected_cal', function () {
            //    $scope.model.value = $scope.selected_cal;
            //    console.log($scope.selected_cal);
            //});
        });