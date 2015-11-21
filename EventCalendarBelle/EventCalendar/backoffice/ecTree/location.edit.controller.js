angular.module("umbraco").controller("EventCalendar.LocationEditController",
        function ($scope, $routeParams, locationResource, notificationsService, assetsService, navigationService, dialogService, entityResource) {

            $scope.tabs = [{ id: "Content", label: "Content" }, { id: "Media", label: "Media" }, { id: "Descriptions", label: "Descriptions" }];
            $scope.images = [];
            var rteDefaultConfiguration = {
                editor: {
                    toolbar: ["code", "undo", "redo", "cut", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbmacro", "table", "umbembeddialog"],
                    stylesheets: [],
                    dimensions: { height: 400, width: '100%' }
                }
            };

            function initMedia() {
                if ($scope.location.mediaItems != null) {
                    entityResource.getByIds($scope.location.mediaItems, "Media")
                       .then(function (mediaArray) {
                           _.forEach(mediaArray, function (item) {
                               $scope.images.push({ id: item.id, name: item.name, thumbnail: item.metaData.umbracoFile.Value, image: item.metaData.umbracoFile.Value });
                           });
                       });
                }
            };

            function initRTE() {
                locationResource.getRTEConfiguration().then(function (response) {
                    var tmp = response.data;
                    if (typeof tmp !== null && typeof tmp === 'object') {
                        rteDefaultConfiguration.editor.toolbar = tmp.toolbar;
                    }
                    //Update descriptions with data for rte
                    _.each($scope.location.descriptions, function (description) {
                        description.label = '';
                        description.description = '';
                        description.view = 'rte';
                        description.hideLabel = true;
                        description.config = rteDefaultConfiguration;
                    });
                });
            };

            function initGMap() {
                assetsService.loadCss("/App_Plugins/EventCalendar/css/jquery-gmaps-latlon-picker.css");

                assetsService.loadJs('http://www.google.com/jsapi')
                    .then(function () {
                        google.load("maps", "3",
                           {
                               callback: initMap,
                               other_params: "sensor=false"
                           });
                    });
            };

            function initAssets() {
                initMedia();
                initRTE();
                initGMap();
            };

            //get a calendar id -> service
            locationResource.getById($routeParams.id).then(function (response) {
                $scope.location = response.data;                

                $scope.location.descriptions = [{
                    culture: "de-DE",
                    value: "Test DE"
                },
                {
                    culture: "en-EN",
                    value: "Test EN"
                }];

                initAssets();
                
            }, function (response) {
                notificationsService.error("Error", location.name + " could not be loaded");
            });

            function initMap() {
                //Google maps is available and all components are ready to use.
                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/jquery-gmaps-latlon-picker.js")
                    .then(function () {
                        //this function will execute when all dependencies have loaded
                        $(document).bind("location_changed", function (event, object) {
                            var lat = $(".gllpLatitude").val();
                            var lon = $(".gllpLongitude").val();
                            $scope.location.lat = lat;
                            $scope.location.lon = lon;
                        });
                        (new GMapsLatLonPicker()).init($(".gllpLatlonPicker"));
                    });
         
                $('umb-panel-header a[data-toggle="tab"]:first').on('shown', function (e) {
                        google.maps.event.trigger(map, 'resize');
                    });
            }

            $scope.openMediaPicker = function () {
                dialogService.mediaPicker({ onlyImages: true, callback: populateFile });
            };

            function populateFile(item) {
                if ($scope.location.mediaItems !== null && $scope.location.mediaItems !== undefined && 'mediaItems' in $scope.location) {
                    $scope.location.mediaItems.push(item.id);
                } else {
                    $scope.location.mediaItems = [];
                    $scope.location.mediaItems.push(item.id);
                }
                $scope.images.push(item);
            };

            $scope.deleteMediaItem = function (media) {
                $scope.location.mediaItems = _.without($scope.location.mediaItems, media.id);
                $scope.images = _.without($scope.images, media);
            };

            $scope.save = function (location) {
                locationResource.save(location).then(function (response) {
                    $scope.location = response.data;

                    notificationsService.success("Success", location.name + " has been created");
                    navigationService.hideNavigation();
                }, function (response) {
                    notificationsService.error("Error", location.name + " could not be created");
                });
            };
        });