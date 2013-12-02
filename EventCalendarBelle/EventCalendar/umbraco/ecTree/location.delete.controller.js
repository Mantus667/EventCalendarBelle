angular.module("umbraco").controller("EventCalendar.LocationDeleteController",
        function ($scope, $routeParams, locationResource, notificationsService, navigationService, treeService) {

            //$scope.delete = function (id) {
            //    locationResource.deleteById(id).then(function (response) {
            //        notificationsService.success("Success", $scope.currentNode.name + " has been deleted");
            //        navigationService.reloadNode($scope.currentNode.parent);
            //        navigationService.hideNavigation();
            //    }, function (response) {
            //        notificationsService.error("Error", $scope.currentNode.name + " could not be deleted");
            //    });
            //};

            $scope.performDelete = function () {

                //mark it for deletion (used in the UI)
                $scope.currentNode.loading = true;

                locationResource.deleteById($scope.currentNode.id.replace("l-","")).then(function () {
                    $scope.currentNode.loading = false;

                    //get the root node before we remove it
                    var rootNode = treeService.getTreeRoot($scope.currentNode);

                    //TODO: Need to sync tree, etc...
                    treeService.removeNode($scope.currentNode);

                    navigationService.hideMenu();

                    notificationsService.success("Success", $scope.currentNode.name + " has been deleted");

                }, function () {
                    $scope.currentNode.loading = false;
                    notificationsService.error("Error", $scope.currentNode.name + " could not be deleted");
                });
            };

            $scope.cancel = function () {
                navigationService.hideDialog();
            };

        });