angular.module("umbraco").controller("EventCalendar.EventEditController",
        function ($scope, $routeParams, eventResource, locationResource, notificationsService, assetsService, tinyMceService, $timeout, dialogService, angularHelper) {           

            $scope.tags_test = { view: 'tags', label: "Tags Test", description: 'funzt auch'};
            console.log($scope.tags_test);
            var tag_scope = undefined;

            //get a calendar id -> service
            eventResource.getById($routeParams.id.replace("e-","")).then(function (response) {
                $scope.event = response.data;
                $scope.event.currentTags = ['Test', 'unko'];
                
                tag_scope = angular.element($("div.umb-tags")).scope();
                console.log(tag_scope);
                tag_scope.currentTags = $scope.event.currentTags;

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

                //Load js library add set the date values for starttime/endtime
                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/moment.min.js")
                    .then(function () {
                        $('#datetimepicker1 input').val(moment($scope.event.starttime).format('MM/DD/YYYY HH:mm:ss'));
                        $('#datetimepicker2 input').val(moment($scope.event.endtime).format('MM/DD/YYYY HH:mm:ss'));
                    });

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
            }, function (response) {
                notificationsService.error("Error", $scope.currentNode.name + " could not be loaded");
            });

            //Load all locations
            locationResource.getall().then(function (response) {
                $scope.locations = response.data;
            }, function (response) {
                notificationsService.error("Error", "Could not load locations");
            });

            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-datetimepicker.min.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/eventcalendar.custom.css");            

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-datetimepicker.min.js")
                .then(function () {
                    //this function will execute when all dependencies have loaded
                    $('#datetimepicker1').datetimepicker({
                        language: 'en'
                    });

                    $('#datetimepicker2').datetimepicker({
                        language: 'en'
                    });

                    $('#datetimepicker1').on('changeDate', function (e) {
                        var d = moment(e.date).format('MM/DD/YYYY HH:mm:ss');
                        $scope.event.starttime = d;
                    });
                    $('#datetimepicker2').on('changeDate', function (e) {
                        var d = moment(e.date).format('MM/DD/YYYY HH:mm:ss');
                        $scope.event.endtime = d;
                    });
                });            

            $scope.save = function (event) {
                console.log($scope.tags_test);
                console.log(tag_scope.currentTags);
                //eventResource.save(event).then(function (response) {
                //    //$scope.event = response.data;

                //    notificationsService.success("Success", event.title + " has been saved");
                //}, function (response) {
                //    notificationsService.error("Error", event.title + " could not be saved");
                //});
            };
        });