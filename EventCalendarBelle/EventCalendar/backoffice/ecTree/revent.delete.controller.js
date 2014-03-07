angular.module("umbraco").controller("EventCalendar.REventDeleteController",
        function ($scope, $routeParams, reventResource, notificationsService, navigationService, treeService) {

            $scope.performDelete = function () {

                //mark it for deletion (used in the UI)
                $scope.currentNode.loading = true;

                reventResource.deleteById($scope.currentNode.id.replace("re-", "")).then(function () {
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