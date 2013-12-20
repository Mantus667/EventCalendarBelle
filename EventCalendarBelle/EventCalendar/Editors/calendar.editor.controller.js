angular.module("umbraco").controller("EventCalendar.CalendarEditorController",
        function ($scope, $routeParams, calendarResource, notificationsService, assetsService) {

            $scope.calendars = [];

            //Load all calendar
            calendarResource.getall().then(function (response) {
                $scope.calendars = response.data;
            }, function (response) {
                notificationsService.error("Error", "Could not load calendar");
            }).then(function () {
                console.log('#calendar_picker option[value="' + $scope.model.value + '"]');
                //alert($('#calendar_picker option[value="' + $scope.model.value + '"]').val());
                $('#calendar_picker option[value="' + $scope.model.value + '"]').attr('selected', 'selected');
            });

            //$scope.$watch('selected_cal', function () {
            //    $scope.model.value = $scope.selected_cal;
            //    console.log($scope.selected_cal);
            //});
        });