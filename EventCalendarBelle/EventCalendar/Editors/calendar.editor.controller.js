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
<<<<<<< HEAD
=======
            }).then(function () {
>>>>>>> 0aeab6d97270bdc9ee3c05b43307ae7c288f9a1b
            });
        });