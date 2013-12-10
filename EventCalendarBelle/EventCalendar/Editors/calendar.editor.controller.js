angular.module("umbraco").controller("EventCalendar.CalendarEditorController",
        function ($scope, $routeParams, calendarResource, notificationsService, assetsService) {

            //$scope.model.value -= 1;

            //Load all calendar
            calendarResource.getall().then(function (response) {
                $scope.calendars = response.data;
            }, function (response) {
                notificationsService.error("Error", "Could not load calendar");
            });
        });