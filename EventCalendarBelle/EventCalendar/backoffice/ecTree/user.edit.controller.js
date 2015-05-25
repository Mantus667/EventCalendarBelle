angular.module("umbraco").controller("EventCalendar.UserEditController",
        function ($scope, $routeParams, userResource, calendarResource, locationResource, notificationsService, assetsService) {

            //Load all calendar
            calendarResource.getall().then(function (response) {
                $scope.calendars = response.data;

                //Load the settings of the user we want to edit
                userResource.getById($routeParams.id).then(function (response) {
                    var user = response.data;
                    //user.calendar = "";
                    if (user.calendar != null && user.calendar != "") {
                        var cids = user.calendar.split(",");
                        user.calendar = cids.map(function (x) {
                            return parseInt(x, 10);
                        });
                    }
                    if (user.locations != null && user.locations != "") {
                        var cids = user.locations.split(",");
                        user.locations = cids.map(function (x) {
                            return parseInt(x, 10);
                        });
                    }
                    $scope.user = user;

                    userResource.getNameOfUser(user.user_id).then(function (response) {
                        $scope.username = response.data;
                    });
                }, function (response) {
                    notificationsService.error("Error", "Settings for " + $scope.currentNode + " could not be loaded");
                });
            }, function (response) {
                notificationsService.error("Error", "Could not load calendar data");
            });

            locationResource.getall().then(function (response) {
                $scope.locations = response.data;
            }, function (response) {
                notificationsService.error("Error", "Could not load location data");
            });

            $scope.save = function (user) {
                if (user.calendar != null && user.calendar != "") {
                    user.calendar = user.calendar.join(",");
                }
                if (user.locations != null && user.locations != "") {
                    user.locations = user.locations.join(",");
                }
                userResource.save(user).then(function (response) {
                    window.location = "#/eventCalendar/ecTree/editUser/" + response.data.user_id;
                    notificationsService.success("Success", $scope.username + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", $scope.username + " could not be saved");
                });
            };

        });