angular.module("umbraco").controller("EventCalendar.CalendarDeleteController",
        function ($scope, $routeParams, calendarResource, notificationsService, navigationService, treeService) {

            //$scope.delete = function (id) {
            //    calendarResource.deleteById(id).then(function (response) {
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

                calendarResource.deleteById($scope.currentNode.id.replace("c-","")).then(function () {
                    $scope.currentNode.loading = false;

                    //get the root node before we remove it
                    var rootNode = treeService.getTreeRoot($scope.currentNode);

                    //TODO: Need to sync tree, etc...
                    treeService.removeNode($scope.currentNode);

                    navigationService.hideMenu();

                }, function () {
                    $scope.currentNode.loading = false;
                });
            };

            $scope.cancel = function () {
                navigationService.hideDialog();
            };

        });