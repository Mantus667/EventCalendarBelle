angular.module("umbraco").controller("EventCalendar.EventEditController",
        function ($scope, $routeParams, eventResource, locationResource, notificationsService, assetsService, tinyMceService, $timeout, dialogService, navigationService, userService, eventCalendarLocalizationService, angularHelper) {           

            $scope.event = { id: 0, calendarid: 0, allday: false, descriptions: {} };
            var locale = 'en-US';
            var dateformat = 'MM/DD/YYYY HH:mm:ss';
            
            var initAssets = function () {
                assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-datetimepicker.min.css");
                assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.css");
                assetsService.loadCss("/App_Plugins/EventCalendar/css/eventcalendar.custom.css");
                assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-tagsinput.css");

                //Get the current user locale
                userService.getCurrentUser().then(function (user) {
                    locale = user.locale;

                    assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-tagsinput.min.js")
                    .then(function () {
                        $('input#tags').tagsinput();
                        $('input#tags').on('itemAdded', function (event) {
                            // event.item: contains the item
                            if ($scope.event.categories === "") {
                                $scope.event.categories += event.item;
                            } else {
                                $scope.event.categories += "," + event.item;
                            }
                        });
                    });

                    //Load js library add set the date values for starttime/endtime
                    assetsService
                        .loadJs("/App_Plugins/EventCalendar/scripts/moment-with-locales.js")
                        .then(function () {
                            //Set the right local of the current user in moment
                            moment.locale([locale, 'en']);

                            if ($routeParams.create == "true") {
                                $scope.event.starttime = moment();
                                $scope.event.endtime = moment();
                            }

                            assetsService
                               .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-datetimepicker.js")
                               .then(function () {
                                   //this function will execute when all dependencies have loaded
                                   $('#datetimepicker1').datetimepicker({
                                       language: locale
                                   });
                                   $('#datetimepicker1 input').val(moment.utc($scope.event.starttime).format('l LT'));

                                   $('#datetimepicker2').datetimepicker({
                                       language: locale
                                   });
                                   $('#datetimepicker2 input').val(moment.utc($scope.event.endtime).format('l LT'));

                                   $('#datetimepicker1').on('dp.change', function (e) {
                                       var d = moment(e.date); //.format('MM/DD/YYYY HH:mm:ss');
                                       //$('#datetimepicker1 input').val(d.format('l LT'));
                                       $scope.event.starttime = d.format('MM/DD/YYYY HH:mm:ss');
                                   });
                                   $('#datetimepicker2').on('dp.change', function (e) {
                                       var d = moment(e.date);
                                       //$('#datetimepicker2 input').val(d.format('l LT'));
                                       $scope.event.endtime = d.format('MM/DD/YYYY HH:mm:ss');
                                   });
                               });
                        });

                    assetsService
                   .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                   .then(function () {
                       $('#allday').bootstrapSwitch();
                       $('#allday').bootstrapSwitch('setState', $scope.event.allday, true);
                       $('#allday').on('switch-change', function (e, data) {
                           $scope.event.allday = data.value;
                       });
                   });
                });
            };

            var initRTE = function () {
                //Create the tabs for every language etc | length
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

            if ($routeParams.create == "true") {
                $scope.event.calendarid = $routeParams.id.replace("c-", "");
                initAssets();
            } else {
                //get a calendar id -> service
                eventResource.getById($routeParams.id.replace("e-","")).then(function (response) {
                    $scope.event = response.data;                

                    initRTE();

                    initAssets();

                }, function (response) {
                    notificationsService.error("Error", $scope.currentNode.name + " could not be loaded");
                });
            }                                 

            $scope.save = function (event) {
                //console.log(event);
                eventResource.save(event).then(function (response) {
                    if ($routeParams.create == "true") {
                        window.location = "#/eventCalendar/ecTree/editEvent/e-" + response.data.id;
                    }
                    navigationService.syncTree({ tree: 'ecTree', path: ["-1", "calendarTree", "c-" + $scope.event.calendarid], forceReload: true });
                    notificationsService.success("Success", event.title + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", event.title + " could not be saved");
                });
            };
        });