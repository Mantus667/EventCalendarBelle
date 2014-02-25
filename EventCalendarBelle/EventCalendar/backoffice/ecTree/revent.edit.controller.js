angular.module("umbraco").controller("EventCalendar.REventEditController",
        function ($scope, $routeParams, reventResource, locationResource, notificationsService, assetsService) {

            $scope.categories = { view: 'tags', label: "Categories", description: 'Specify categories which the event is related to.' };
            var tag_scope = undefined;

            //get a calendar id -> service
            reventResource.getById($routeParams.id.replace("re-", "")).then(function (response) {
                $scope.event = response.data;

                //Get the scope for the tags editor
                $("div.umb-tags").ready(function () {
                    tag_scope = angular.element($("div.umb-tags")).scope();
                    if ($scope.event.categories != null) {
                        tag_scope.currentTags = $scope.event.categories.split(',');
                    }
                });

                //Create the tabs for every language etc
                $scope.tabs = [{ id: "Content", label: "Content" }];
                angular.forEach($scope.event.descriptions, function (value, key) {
                    this.push({ id: key, label: value.culture });
                }, $scope.tabs);

                //Update descriptions with data for rte
                angular.forEach($scope.event.descriptions, function (description) {
                    description.label = '';
                    description.description = '';
                    description.view = 'rte';
                    description.hideLabel = true;
                    description.config = {
                        editor: {
                            toolbar: ["code", "undo", "redo", "cut", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbmacro", "table", "umbembeddialog"],
                            stylesheets: [],
                            dimensions: { height: 400, width: '100%' }
                        }
                    };
                });

                console.log($scope.event);
            }, function (response) {
                notificationsService.error("Error", $scope.currentNode.name + " could not be loaded");
            });

            //Load all locations
            locationResource.getall().then(function (response) {
                $scope.locations = response.data;
            }, function (response) {
                notificationsService.error("Error", "Could not load locations");
            });

            reventResource.getDayOfWeekValues().then(function (response) {
                $scope.DayOfWeekList = response.data;
            });

            reventResource.getFrequencyTypes().then(function (response) {
                $scope.FrequencyTypes = response.data;
            });

            reventResource.getMonthlyIntervalValues().then(function (response) {
                $scope.MonthlyIntervals = response.data;
            });

            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/eventcalendar.custom.css");

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                .then(function () {
                    if ($scope.event) {
                        $('#allday').bootstrapSwitch('setState', $scope.event.allday, true);
                    }
                    $('#allday').on('switch-change', function (e, data) {
                        $scope.event.allday = data.value;
                    });
                });

            $scope.save = function (event) {
                tag_scope = angular.element($("div.umb-tags")).scope();
                event.categories = tag_scope.currentTags.join();
                reventResource.save(event).then(function (response) {
                    //$scope.event = response.data;
                    notificationsService.success("Success", event.title + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", event.title + " could not be saved");
                });
            };

        });