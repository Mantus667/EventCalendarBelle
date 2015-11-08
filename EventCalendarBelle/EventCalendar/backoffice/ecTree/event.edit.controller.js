angular.module("umbraco").controller("EventCalendar.EventEditController",
        function ($scope, $routeParams, eventResource, locationResource, notificationsService, assetsService,
            tinyMceService, $timeout, dialogService, navigationService, userService,
            eventCalendarLocalizationService, entityResource, angularHelper) {

            $scope.event = { id: 0, calendarid: 0, allday: false, descriptions: {}, organiser: {} };
            var locale = 'en-US';
            var dateformat = 'MM/DD/YYYY HH:mm:ss';
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
                               $scope.event.allday = state;
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
                        if ($scope.event.categories !== "" && $scope.event.categories !== null && $scope.event.categories !== undefined) {
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

                            if ($routeParams.create === "true") {
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
                                       var d = moment(e.date);
                                       $scope.event.starttime = d.format('MM/DD/YYYY HH:mm:ss');
                                   });
                                   $('#datetimepicker2').on('dp.change', function (e) {
                                       var d = moment(e.date);
                                       $scope.event.endtime = d.format('MM/DD/YYYY HH:mm:ss');
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
                eventResource.getRTEConfiguration().then(function (response) {
                    var tmp = response.data;
                    if (typeof tmp !== null && typeof tmp === 'object') {                        
                        rteDefaultConfiguration.editor.toolbar = tmp.toolbar;
                    }                    

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
                        description.config = rteDefaultConfiguration;
                    });
                });                
            };

            var updateView = function () {
                //Update descriptions with data for rte
                angular.forEach($scope.event.descriptions, function (description) {
                    description.label = '';
                    description.description = '';
                    description.view = 'rte';
                    description.hideLabel = true;
                    description.config = rteDefaultConfiguration;
                });

                $('ul.nav-tabs li').removeClass('active');
                $('ul.nav-tabs li:first').addClass('active');
                $('#tabContent').addClass('active');
            };

            $scope.populate = function (data) {
                $scope.event.organiser_id = data.id;
                $scope.event.organiser = { name: data.name, id: data.id, icon: data.icon };
            };

            //Load all locations
            locationResource.getall().then(function (response) {
                $scope.locations = response.data;
            }, function (response) {
                notificationsService.error("Error", "Could not load locations");
            });

            $scope.openCreateLocationDialog = function () {
                dialogService.open({
                    template: '/App_Plugins/EventCalendar/backoffice/ecTree/createLocation.html',
                    dialogData: {
                        isDialog: true
                    },
                    show: true,
                    callback: function (data) {
                        $scope.locations.push(data.location);
                        $scope.event.locationId = data.location.id;
                    }
                });
            };

            if ($routeParams.create == "true") {
                $scope.event.calendarid = $routeParams.id.replace("c-", "");
                initAssets();
            } else {
                //get a calendar id -> service
                eventResource.getById($routeParams.id.replace("e-","")).then(function (response) {
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
                                   $scope.images.push({ id: item.id, name: item.name, path: item.metaData.umbracoFile.Value });
                               });
                           });
                    }

                }, function (response) {
                    notificationsService.error("Error", $scope.currentNode.name + " could not be loaded");
                });
            }

            $scope.openMemberPicker = function () {
                dialogService.memberPicker({
                    multiPicker: false,
                    callback: $scope.populate
                });
            };

            $scope.deleteOrganiser = function () {
                $scope.event.organiser = {};
            };

            $scope.openIconPicker = function () {
                dialogService.iconPicker({ callback: populateIcon });
            };

            function populateIcon(item) {
                $scope.event.icon = item;
            };

            $scope.deleteIcon = function () {
                $scope.event.icon = "";
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

            $scope.deleteMediaItem = function (media) {
                $scope.event.mediaItems = _.without($scope.event.mediaItems, media.id);
                $scope.images = _.without($scope.images, media);
            };

            $scope.isPicture = function (path) {
                if (/\.(jpg|png|gif|jpeg)$/.test(path)) {
                    return true;
                }
                return false;
            };

            $scope.save = function (event) {
                eventResource.save(event).then(function (response) {
                    if ($routeParams.create == "true") {
                        window.location = "#/eventCalendar/ecTree/editEvent/e-" + response.data.id;
                    }
                    navigationService.syncTree({ tree: 'ecTree', path: ["-1", "calendarTree", "c-" + $scope.event.calendarid], forceReload: true });
                    notificationsService.success("Success", event.title + " has been saved");
                    $scope.event = response.data;
                    updateView();
                }, function (response) {
                    notificationsService.error("Error", event.title + " could not be saved");
                });
            };
        });