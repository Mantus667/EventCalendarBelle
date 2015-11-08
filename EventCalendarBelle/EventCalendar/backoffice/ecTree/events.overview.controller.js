angular.module('umbraco')
    .controller('EventCalendar.EventsOverviewController', function ($scope, assetsService, eventResource, $routeParams) {

        $scope.selectedIds = [];

        $scope.currentPage = 1;
        $scope.itemsPerPage = 10;
        $scope.totalPages = 1;

        $scope.reverse = false;

        $scope.searchTerm = "";
        $scope.predicate = 'id';

        function fetchData() {
            eventResource.getPaged($scope.typeName, $scope.itemsPerPage, $scope.currentPage, $scope.predicate, $scope.reverse ? "desc" : "asc", $scope.searchTerm).then(function (response) {
                $scope.events = response.data.events;
                $scope.totalPages = response.data.totalPages;
            }, function (response) {
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

        fetchData();
    });