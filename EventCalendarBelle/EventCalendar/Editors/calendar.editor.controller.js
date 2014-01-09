angular.module("umbraco").controller("EventCalendar.CalendarEditorController",
        function ($scope, $routeParams, calendarResource, notificationsService, assetsService) {

            $scope.calendars = [{id: '0', calendarname: '-- All calendar --'}];

            //Load all calendar
            calendarResource.getall().then(function (response) {
                //$scope.calendars = response.data;
                angular.forEach(response.data, function (calendar) {
                    $scope.calendars.push({id:calendar.id.toString(), calendarname: calendar.calendarname});
                });
            }, function (response) {
                notificationsService.error("Error", "Could not load calendar");
            });
        });