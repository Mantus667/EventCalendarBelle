angular.module("umbraco").controller("EventCalendar.UserEditController",
        function ($scope, $routeParams, userResource, calendarResource, notificationsService, userService, assetsService) {

            //Load the settings of the user we want to edit
            userResource.getById($routeParams.id).then(function (response) {
                $scope.user = response.data;
                console.log($scope.user.calendar_array);
                userService.getCurrentUser().then(function (user) {
                    $scope.username = user.name;
                });

                //Load all calendar
                calendarResource.getall().then(function (response) {
                    $scope.calendars = response.data;
                }, function (response) {
                    notificationsService.error("Error", "Could not load calendar data");
                });
            }, function (response) {
                notificationsService.error("Error", "Settings for " + $scope.currentNode + " could not be loaded");
            });

            $scope.save = function (user) {
                console.log(user);
                userResource.save(user).then(function (response) {
                    $scope.user = response.data;
                    notificationsService.success("Success", $scope.username + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", $scope.username + " could not be saved");
                });
            };

        });