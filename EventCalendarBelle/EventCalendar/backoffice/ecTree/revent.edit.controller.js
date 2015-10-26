angular.module("umbraco").controller("EventCalendar.REventEditController",
        function ($scope, $routeParams, reventResource, locationResource,
            notificationsService, navigationService, assetsService,
            userService, entityResource, dialogService) {

            $scope.event = { id: 0, calendarid: 0, allDay: false, organiser: {} };
            var exceptionDate;
            var locale = 'en-US';
            var dateformat = 'MM/DD/YYYY';
            var rteDefaultConfiguration = {
                editor: {
                    toolbar: ["code", "undo", "redo", "cut", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbmacro", "table", "umbembeddialog"],
                    stylesheets: [],
                    dimensions: { height: 400, width: '100%' }
                }
            };
            $scope.images = [];

            var initSwitch = function () {
                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                    .then(function () {
                        $('#allday').bootstrapSwitch({
                            onColor: "success",
                            onText: "<i class='icon-check icon-white'></i>",
                            offText: "<i class='icon-delete'></i>",
                            onSwitchChange: function (event, state) {
                                $scope.event.allday = !$scope.event.allday;
                            }
                        });
                        $('#allday').bootstrapSwitch('state', $scope.event.allday, false);
                    });
            };

            var initTagsInput = function () {
                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-tagsinput.min.js")
                    .then(function () {
                        $('input#tags').tagsinput();
                        if ($scope.event.categories != "" && $scope.event.categories != null && $scope.event.categories !== undefined) {
                            var tags = $scope.event.categories.split(",");
                            angular.forEach(tags, function (value) {
                                $('input#tags').tagsinput('add', value);
                            });
                        }
                        $('input#tags').on('itemAdded', function (event) {
                            // event.item: contains the item
                            $scope.event.categories = $("input#tags").val();
                        });
                        $('input#tags').on('itemRemoved', function (event) {
                            // event.item: contains the item
                            $scope.event.categories = $("input#tags").val();
                        });
                    });
            };

            var initDatePicker = function () {
                //Get the current user locale
                userService.getCurrentUser().then(function (user) {
                    locale = user.locale;

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
                                       language: locale,
                                       pickDate: false
                                   });
                                   $('#datetimepicker1 input').val(moment.utc($scope.event.starttime).format('LT'));

                                   $('#datetimepicker2').datetimepicker({
                                       language: locale,
                                       pickDate: false
                                   });
                                   $('#datetimepicker2 input').val(moment.utc($scope.event.endtime).format('LT'));

                                   $('#datetimepicker3').datetimepicker({
                                       language: locale,
                                       pickTime: false
                                   });

                                   $('#datetimepicker1').on('dp.change', function (e) {
                                       var d = moment(e.date);
                                       $scope.event.starttime = d.format('HH:mm:ss');
                                   });
                                   $('#datetimepicker2').on('dp.change', function (e) {
                                       var d = moment(e.date);
                                       $scope.event.endtime = d.format('HH:mm:ss');
                                   });
                                   $('#datetimepicker3').on('dp.change', function (e) {
                                       var d = moment(e.date);
                                       exceptionDate = d.format(dateformat);
                                   });
                               });
                        });
                });
            };

            var initAssets = function () {
                initSwitch();
                initTagsInput();
                initDatePicker();
            };

            var initRTE = function () {                
                reventResource.getRTEConfiguration().then(function (response) {
                    var tmp = response.data;
                    if (typeof tmp !== null && typeof tmp === 'object') {
                        rteDefaultConfiguration.editor.toolbar = tmp.toolbar;
                    }

                    //Update descriptions with data for rte
                    angular.forEach($scope.event.descriptions, function (description) {
                        description.label = '';
                        description.description = '';
                        description.view = 'rte';
                        description.hideLabel = true;
                        description.config = rteDefaultConfiguration;
                    });

                    //Create the tabs for every language etc | length
                    $scope.tabs = [{ id: "Content", label: "Content" }, { id: "Exceptions", label: "DateExceptions" }];
                    angular.forEach($scope.event.descriptions, function (value, key) {
                        this.push({ id: key, label: value.culture });
                    }, $scope.tabs);
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

            reventResource.getMonths().then(function (response) {
                $scope.MonthRange = response.data;
            });

            $scope.populate = function (data) {
                $scope.event.organiser_id = data.id;
                $scope.event.organiser = { name: data.name, id: data.id, icon: data.icon };
            };

            $scope.openMemberPicker = function () {
                dialogService.memberPicker({
                    multiPicker: false,
                    callback: $scope.populate
                });
            };

            $scope.openMediaPicker = function () {
                dialogService.mediaPicker({ onlyImages: true, callback: populateFile });
            };

            function populateFile(item) {
                if ($scope.event.mediaItems !== null) {
                    $scope.event.mediaItems.push(item.id);
                } else {
                    $scope.event.mediaItems = [];
                    $scope.event.mediaItems.push(item.id);
                }
                $scope.images.push({ name: item.name, path: item.image });
            };

            $scope.isPicture = function (path) {
                if (/\.(jpg|png|gif|jpeg)$/.test(path)) {
                    return true;
                }
                return false;
            };

            $scope.deleteOrganiser = function () {
                $scope.event.organiser = {};
            };

            $scope.addException = function () {
                $scope.event.exceptions.push({ id: 0, event: $scope.event.id, date: exceptionDate });
                $('#datetimepicker3').val('')
            };

            $scope.deleteException = function (index) {
                $scope.event.exceptions.splice(index, 1);
            };

            $scope.openCreateLocationDialog = function () {
                dialogService.open({
                    template: '/App_Plugins/EventCalendar/backoffice/ecTree/createLocation.html',
                    dialogData: {
                        isDialog: true
                    },
                    show: true,
                    callback: function (data) {
                        $scope.locations.push(data.location);
                        $scope.event.locationid = data.location.id;
                    }
                });
            };

            if ($routeParams.create == "true") {
                $scope.event.calendarid = $routeParams.id.replace("c-", "");
                initAssets();
            } else {
                //get a calendar id -> service
                reventResource.getById($routeParams.id.replace("re-", "")).then(function (response) {
                    $scope.event = response.data;
                    $scope.event.organiser = {};

                    initRTE();

                    initAssets();

                    if ($scope.event.organiser_id != 0) {
                        entityResource.getById($scope.event.organiser_id, "Member")
                           .then(function (data) {
                               $scope.event.organiser = { name: data.name, id: data.id, icon: data.icon };
                           });
                    }

                    if ($scope.event.mediaItems != null) {
                        entityResource.getByIds($scope.event.mediaItems, "Media")
                           .then(function (mediaArray) {
                               _.forEach(mediaArray, function (item) {
                                   $scope.images.push({ name: item.name, path: item.metaData.umbracoFile.Value });
                               });
                           });
                    }

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