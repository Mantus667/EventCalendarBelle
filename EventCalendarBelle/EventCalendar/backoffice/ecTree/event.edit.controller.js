angular.module("umbraco").controller("EventCalendar.EventEditController",
        function ($scope, $routeParams, eventResource, locationResource, notificationsService, assetsService) {

            //get a calendar id -> service
            eventResource.getById($routeParams.id.replace("e-","")).then(function (response) {
                $scope.event = response.data;

                //Create the tabs for every language etc
                $scope.tabs = [{ id: "Content", label: "Content" }];
                angular.forEach($scope.event.descriptions, function (value, key) {
                    this.push({ id: key, label: value.culture });
                }, $scope.tabs);


                //Load js library add set the date values for starttime/endtime
                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/moment.min.js")
                    .then(function () {
                        $('#datetimepicker1 input').val(moment($scope.event.starttime).format('MM/DD/YYYY HH:mm:ss'));
                        $('#datetimepicker2 input').val(moment($scope.event.endtime).format('MM/DD/YYYY HH:mm:ss'));
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

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                .then(function () {
                    if($scope.event) {
                        $('#allday').bootstrapSwitch('setState', $scope.event.allday, true);
                    }
                    $('#allday').on('switch-change', function (e, data) {
                        $scope.event.allday = data.value;
                    });
                });

            $scope.save = function (event) {
                //console.log(event);
                //if (event.$valid) {
                    eventResource.save(event).then(function (response) {
                        $scope.event = response.data;

                        notificationsService.success("Success", event.title + " has been saved");
                    }, function (response) {
                        notificationsService.error("Error", event.title + " could not be saved");
                    });
                //} else {
                //    notificationsService.error("Error", "Form is not valid!");
                //}
            };

        });