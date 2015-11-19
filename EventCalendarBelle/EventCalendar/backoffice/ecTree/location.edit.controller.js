angular.module("umbraco").controller("EventCalendar.LocationEditController",
        function ($scope, $routeParams, locationResource, notificationsService, assetsService, navigationService, dialogService, entityResource) {

            $scope.tabs = [{ id: "Content", label: "Content" }, { id: "Media", label: "Media" }];
            $scope.images = [];

            //get a calendar id -> service
            locationResource.getById($routeParams.id).then(function (response) {
                $scope.location = response.data;

                if ($scope.location.mediaItems != null) {
                    entityResource.getByIds($scope.location.mediaItems, "Media")
                       .then(function (mediaArray) {
                           _.forEach(mediaArray, function (item) {
                               $scope.images.push({ id: item.id, name: item.name, thumbnail: item.metaData.umbracoFile.Value, image: item.metaData.umbracoFile.Value });
                           });
                       });
                }
            }, function (response) {
                notificationsService.error("Error", location.name + " could not be loaded");
            });

            assetsService.loadCss("/App_Plugins/EventCalendar/css/jquery-gmaps-latlon-picker.css");

            assetsService.loadJs('http://www.google.com/jsapi')
                .then(function () {
                    google.load("maps", "3",
                       {
                         callback: initMap,
                         other_params: "sensor=false"
                       });
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
         
                    $('a[data-toggle="tab"]').on('shown', function (e) {
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