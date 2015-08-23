angular.module("umbraco").controller("EventCalendar.LocationDeleteController",
        function ($scope, $routeParams, locationResource, notificationsService, navigationService, treeService) {

            $scope.performDelete = function () {

                locationResource.deleteById($scope.currentNode.id.replace("l-","")).then(function (response) {
                    $scope.currentNode.loading = false;
                    if (response.data === true) {
                        //mark it for deletion (used in the UI)
                        $scope.currentNode.loading = true;

                        //get the root node before we remove it
                        var rootNode = treeService.getTreeRoot($scope.currentNode);

                        //TODO: Need to sync tree, etc...
                        treeService.removeNode($scope.currentNode);

                        navigationService.hideMenu();

                        notificationsService.success("Success", $scope.currentNode.name + " has been deleted");
                    } else {
                        $scope.currentNode.loading = false;
                        notificationsService.error("Error", $scope.currentNode.name + " could not be deleted");
                    }

                }, function () {
                    $scope.currentNode.loading = false;
                    notificationsService.error("Error", $scope.currentNode.name + " could not be deleted");
                });
            };

            $scope.cancel = function () {
                navigationService.hideDialog();
            };

        });