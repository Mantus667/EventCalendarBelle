angular.module("umbraco").controller("EventCalendar.LocationCreateController",
        function ($scope, $routeParams, locationResource, notificationsService, assetsService, navigationService) {

            $scope.location = { id: 0, lat: 50.11, lon: 8.68 };

            //assetsService.loadCss("/App_Plugins/EventCalendar/css/jquery-gmaps-latlon-picker.css");

            //assetsService
            //    .loadJs("http://maps.googleapis.com/maps/api/js?sensor=false")
            //    .then(function () {
            //        assetsService.loadJs("http://maps.gstatic.com/intl/de_de/mapfiles/api-3/15/1a/main.js")
            //            .then(function () {
            //                assetsService
            //                .loadJs("/App_Plugins/EventCalendar/scripts/jquery-gmaps-latlon-picker.js")
            //                .then(function () {
            //                    //this function will execute when all dependencies have loaded
            //                    //console.log(window.google);
            //                    //if (window.google != undefined) {
            //                    //    $(".gllpLatlonPicker").each(function () {
            //                    //        (new GMapsLatLonPicker()).init($(this));
            //                    //    });
            //                    //}
            //                    $(document).bind("location_changed", function (event, object) {
            //                        var lat = $(".gllpLatitude").val();
            //                        var lon = $(".gllpLongitude").val();
            //                        $scope.location.lat = lat;
            //                        $scope.location.lon = lon;
            //                        console.log(lat + " - " + lon);
            //                    });
            //                });
            //            });
            //    });            

            $scope.save = function (location) {
                console.log(location);
                locationResource.save(location).then(function (response) {
                    $scope.location = response.data;

                    notificationsService.success("Success", location.name + " has been created");
                    navigationService.reloadNode($scope.currentNode.parent());
                    navigationService.hideNavigation();
                }, function (response) {
                    notificationsService.error("Error", location.name + " could not be created");
                });
            };

        });