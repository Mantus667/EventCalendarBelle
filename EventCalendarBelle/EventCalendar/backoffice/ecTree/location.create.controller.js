angular.module("umbraco").controller("EventCalendar.LocationCreateController",
        function ($scope, $routeParams, locationResource, notificationsService, assetsService, navigationService) {

            $scope.location = { id: 0, lat: 50.11, lon: 8.68 };

            var isDialog = (typeof $scope.dialogData === 'object' && $scope.dialogData.isDialog) || (typeof $scope.$parent.dialogData === 'object' && $scope.$parent.dialogData.isDialog);

            $scope.save = function (location) {
                locationResource.save(location).then(function (response) {
                    $scope.location = response.data;

                    notificationsService.success("Success", location.name + " has been created");

                    if (!isDialog) {
                        navigationService.reloadNode($scope.currentNode.parent());
                        navigationService.hideNavigation();
                    } else {
                        $scope.submit({ success: true, location: response.data });
                    }
                }, function (response) {
                    notificationsService.error("Error", location.name + " could not be created");
                });
            };

        });