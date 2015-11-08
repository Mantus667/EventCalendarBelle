angular.module("umbraco").controller("EventCalendar.CalendarEditController",
        function ($scope, $routeParams, calendarResource, notificationsService, assetsService) {

            $scope.tabs = [{ id: "Content", label: "Content" }, { id: "Events", label: "Events" }];

            function initAssets() {
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

                assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                .then(function () {
                    $('#hide').bootstrapSwitch({
                        onColor: "success",
                        onText: "<i class='icon-check icon-white'></i>",
                        offText: "<i class='icon-delete'></i>",
                        onSwitchChange: function (event, state) {
                            $scope.calendar.displayOnSite = state;
                        }
                    });
                    $('#hide').bootstrapSwitch('state', $scope.calendar.displayOnSite, false);
                    $('#useGoogle').bootstrapSwitch({
                        onColor: "success",
                        onText: "<i class='icon-check icon-white'></i>",
                        offText: "<i class='icon-delete'></i>",
                        onSwitchChange: function (event, state) {
                            $scope.calendar.isGCal = state;
                        }
                    });
                    $('#useGoogle').bootstrapSwitch('state', $scope.calendar.isGCal, false);
                });
            };

            assetsService.loadCss("/App_Plugins/EventCalendar/css/colorpicker.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.min.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/EventCalendar.Custom.css");

            //get a calendar id -> service
            calendarResource.getById($routeParams.id).then(function (response) {
                initAssets();
            }, function (response) {
                notificationsService.error("Error", calendar.calendarname + " could not be loaded");
            });            

            $scope.save = function (calendar) {
                calendarResource.save(calendar).then(function (response) {
                    notificationsService.success("Success", calendar.calendarname + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", calendar.calendarname + " could not be saved");
                });
            };
        });