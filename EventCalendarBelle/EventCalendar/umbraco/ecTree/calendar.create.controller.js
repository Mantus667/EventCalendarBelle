angular.module("umbraco").controller("EventCalendar.CalendarCreateController",
        function ($scope, $routeParams, calendarResource, notificationsService, assetsService, navigationService) {

            $scope.calendar = { id: 0, calendarname: '', color: '#0000FF', isGCal: false, displayOnSite: false, gCalFeedUrl: '' };

            assetsService.loadCss("/App_Plugins/EventCalendar/css/colorpicker.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/EventCalendar.Custom.css");

            assetsService
                .load([
                    "/App_Plugins/EventCalendar/scripts/colorpicker.js"
                ])
                .then(function () {
                    //this function will execute when all dependencies have loaded
                    $('#colorSelector').ColorPicker({
                        color: '#0000ff',
                        onShow: function (colpkr) {
                            $(colpkr).fadeIn(500);
                            return false;
                        },
                        onHide: function (colpkr) {
                            $(colpkr).fadeOut(500);
                            return false;
                        },
                        onChange: function (hsb, hex, rgb) {
                            $scope.calendar.color = '#' + hex;
                            $('#colorSelector div').css('background-color', '#' + hex);
                        }
                    });
                });

            $scope.save = function (calendar) {
                calendarResource.save(calendar).then(function (response) {
                    $scope.calendar = response.data;

                    notificationsService.success("Success", calendar.calendarname + " has been created");
                    navigationService.reloadNode($scope.currentNode.parent());
                    navigationService.hideNavigation();
                }, function (response) {
                    notificationsService.error("Error", calendar.calendarname + " could not be created");
                });
            };

        });