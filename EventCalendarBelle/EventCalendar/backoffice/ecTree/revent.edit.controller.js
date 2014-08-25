angular.module("umbraco").controller("EventCalendar.REventEditController",
        function ($scope, $routeParams, reventResource, locationResource, notificationsService, navigationService, assetsService) {

            $scope.event = { id: 0, calendarid: 0, allDay: false };

            var initAssets = function () {
                assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.css");
                assetsService.loadCss("/App_Plugins/EventCalendar/css/eventcalendar.custom.css");
                assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-tagsinput.css");

                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-tagsinput.min.js")
                    .then(function () {
                        $('input#tags').on('itemAdded', function (event) {
                            // event.item: contains the item
                            if ($scope.event.categories === "") {
                                $scope.event.categories += event.item;
                            } else {
                                $scope.event.categories += "," + event.item;
                            }
                        });
                    });

                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                    .then(function () {
                        $('#allday').bootstrapSwitch('setState', $scope.event.allday, true);
                        $('#allday').on('switch-change', function (e, data) {
                            $scope.event.allday = data.value;
                        });
                    });
            };

            var initRTE = function () {                
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
            };

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

            if ($routeParams.create == "true") {
                $scope.event.calendarid = $routeParams.id.replace("c-", "");
                initAssets();
            } else {
                //get a calendar id -> service
                reventResource.getById($routeParams.id.replace("re-", "")).then(function (response) {
                    $scope.event = response.data;                    

                    initRTE();

                    initAssets();

                }, function (response) {
                    notificationsService.error("Error", $scope.currentNode.name + " could not be loaded");
                });
            }

            $scope.save = function (event) {
                reventResource.save(event).then(function (response) {
                    if ($routeParams.create == "true") {
                        window.location = "#/eventCalendar/ecTree/editREvent/" + response.data.id;
                    }
                    navigationService.syncTree({ tree: 'ecTree', path: ["-1", "calendarTree", "c-" + $scope.event.calendarid], forceReload: true });
                    notificationsService.success("Success", event.title + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", event.title + " could not be saved");
                });
            };

        });