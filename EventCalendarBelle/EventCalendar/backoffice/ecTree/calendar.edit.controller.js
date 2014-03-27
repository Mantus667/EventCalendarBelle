angular.module("umbraco").controller("EventCalendar.CalendarEditController",
        function ($scope, $routeParams, calendarResource, notificationsService, assetsService) {

            //get a calendar id -> service
            calendarResource.getById($routeParams.id).then(function (response) {
                $scope.calendar = response.data;
            }, function (response) {
                notificationsService.error("Error", calendar.calendarname + " could not be loaded");
            });

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
                    $('#colorSelector2').ColorPicker({
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
                            $scope.calendar.textColor = '#' + hex;
                            $('#colorSelector2 div').css('background-color', '#' + hex);
                        }
                    });
                });

            $scope.save = function (calendar) {
                calendarResource.save(calendar).then(function (response) {
                    notificationsService.success("Success", calendar.calendarname + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", calendar.calendarname + " could not be saved");
                });
            };
        });