angular.module("umbraco").controller("EventCalendar.ImportController",
        function ($scope, $http, calendarResource, notificationsService) {

            $scope.calendars = [{ id: '0', calendarname: '-- Create new calendar --' }];
            $scope.data = { calendarID: 0 };

            //Load all calendar
            calendarResource.getall().then(function (response) {
                //$scope.calendars = response.data;
                angular.forEach(response.data, function (calendar) {
                    $scope.calendars.push({ id: calendar.id.toString(), calendarname: calendar.calendarname });
                });
            }, function (response) {
                notificationsService.error("Error", "Could not load calendar");
            });

            $scope.filesChanged = function (elm){
                $scope.file = elm.files[0];
                $scope.$apply();
            };

            $scope.upload = function (file) {
                var formData = new FormData();
                formData.append('file', file);
                formData.append('calendar', $scope.data.calendarID);
                $http.post(Umbraco.Sys.ServerVariables.eventCalendar.importBaseUrl + "Import", formData,
                    {
                        transformRequest: angular.identity,
                        headers: {'Content-Type' : undefined}
                    }).success(function (data, status, headers, config) {
                        notificationsService.success("Success", "Data import completed");
                        $scope.data.calendarID = 0;
                    }).error(function (data, status, headers, config) {
                        notificationsService.error("Error", "Data import failed");
                    });
            };
        });