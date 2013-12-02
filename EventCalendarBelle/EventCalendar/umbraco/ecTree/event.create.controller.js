angular.module("umbraco").controller("EventCalendar.EventCreateController",
        function ($scope, $routeParams, eventResource, locationResource, notificationsService, assetsService, navigationService) {

            $scope.event = { id: 0, calendarid: $scope.currentNode.id.replace("c-", "") };

            locationResource.getall().then(function (response) {
                $scope.locations = response.data;
                console.log(response.data);
            }, function (response) {
                notificationsService.error("Error", "Could not load locations");
            });

            assetsService.loadCss("/App_Plugins/EventCalendar/css/eventcalendar.custom.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-datetimepicker.min.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.css");

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/moment.min.js")
                .then(function () {
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
                });

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                .then(function () {
                    $('#allday').on('switch-change', function (e, data) {
                        $scope.event.allday = data.value;
                    });
                });

            $scope.save = function (event) {
                //console.log(event);
                if (event.$valid) {
                    eventResource.save(event).then(function (response) {
                        $scope.event = response.data;

                        notificationsService.success("Success", event.title + " has been created");
                        navigationService.reloadNode($scope.currentNode.parent());
                        navigationService.hideNavigation();
                    }, function (response) {
                        notificationsService.error("Error", event.title + " could not be created");
                    });
                } else {
                    notificationsService.error("Error", "Form is not valid!");
                }
            };

        });