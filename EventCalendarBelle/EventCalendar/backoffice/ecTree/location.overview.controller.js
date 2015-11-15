angular.module('umbraco')
    .controller('EventCalendar.LocationOverviewController', function ($scope, assetsService, locationResource, notificationsService) {

        $scope.selectedIds = [];

        $scope.currentPage = 1;
        $scope.itemsPerPage = 10;
        $scope.totalPages = 1;

        $scope.reverse = false;

        $scope.searchTerm = "";
        $scope.predicate = 'id';

        function fetchData() {
            locationResource.getPaged($scope.itemsPerPage, $scope.currentPage, $scope.predicate, $scope.reverse ? "desc" : "asc", $scope.searchTerm).then(function (response) {
                $scope.locations = response.data.locations;
                $scope.totalPages = response.data.totalPages;
            }, function (response) {
                console.log(response);
                notificationsService.error("Error", "Could not load locations");
            });
        };

        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
            $scope.currentPage = 1;
            fetchData();
        };

        $scope.toggleSelection = function (val) {
            var idx = $scope.selectedIds.indexOf(val);
            if (idx > -1) {
                $scope.selectedIds.splice(idx, 1);
            } else {
                $scope.selectedIds.push(val);
            }
        };

        $scope.isRowSelected = function (id) {
            return $scope.selectedIds.indexOf(id) > -1;
        };

        $scope.isAnythingSelected = function () {
            return $scope.selectedIds.length > 0;
        };

        $scope.prevPage = function () {
            if ($scope.currentPage > 1) {
                $scope.currentPage--;
                fetchData();
            }
        };

        $scope.nextPage = function () {
            if ($scope.currentPage < $scope.totalPages) {
                $scope.currentPage++;
                fetchData();
            }
        };

        $scope.setPage = function (pageNumber) {
            $scope.currentPage = pageNumber;
            fetchData();
        };

        $scope.search = function (searchFilter) {
            $scope.searchTerm = searchFilter;
            $scope.currentPage = 1;
            fetchData();
        };

        $scope.delete = function () {
            if (confirm("Are you sure you want to delete " + $scope.selectedIds.length + " location" + ($scope.selectedIds.length > 1 ? "s" : "") + "?")) {
                $scope.actionInProgress = true;
                _.each($scope.selectedIds, function (id) {
                    locationResource.deleteById(id).then(function () {
                    });
                });
                $scope.locations = _.reject($scope.locations, function (el) { return $scope.selectedIds.indexOf(el.id) > -1; });
                $scope.selectedIds = [];
                $scope.actionInProgress = false;
            }
        };

        fetchData();
    });