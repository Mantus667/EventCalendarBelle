angular.module("umbraco")
    .controller("EventCalendar.GridEditors.UpcomingEventsEditorController", 
    ["$scope", "$rootScope", "$timeout", "$routeParams", "$filter", "calendarResource", "EventCalendar.GridEditors.Services.EventListDialogService",
        function ($scope, $rootScope, $timeout, $routeParams, $filter, calendarResource, ecelDialogService) {
        $scope.title = "Click to choose calendar for list of events";
        $scope.icon = "icon-item-arrangement";

        $scope.setValue = function (data) {
            $scope.control.value = data;
        };

        $scope.setPreview = function (model) {
            if ("enablePreview" in $scope.control.editor.config && $scope.control.editor.config.enablePreview) {
                $scope.preview = "<" + $scope.control.editor.config.titleTag + ">" + model.title + "</" + $scope.control.editor.config.titleTag + ">";
                calendarResource.getEvents(model.calendar,model.count, true).then(function (response) {
                    if (response.data) {
                        $scope.preview += "<ul>";
                        angular.forEach(response.data, function (event) {
                            $scope.preview += "<li>";
                            $scope.preview += "<b>" + event.title + "</b><br />";
                            $scope.preview += "<small>" + (event.start != "") ? $filter('date')(event.start) : "" + (event.end != "") ? " - " + $filter('date')(event.end) : "" + "</small>";
                            if (model.teaser && event.description !== "") {
                                $scope.preview += "<br /><span>" + event.description.substring(0, 50) + "</span>";
                            }
                            $scope.preview += "</li>";
                        });
                        $scope.preview += "</ul>";
                    }
                });
            }
        };

        $scope.setData = function () {
            ecelDialogService.open({
                dialogData: $scope.control.value,
                callback: function (data) {                    
                    $scope.setValue(data);
                    $scope.setPreview($scope.control.value);
                }
            });            
        };

        $scope.setValue($scope.control.value || {
                title: '',
                count: ("defaultEventQuantity" in $scope.control.editor.config && $scope.control.editor.config.defaultEventQuantity) ? $scope.control.editor.config.defaultEventQuantity : null,
                teaser: false,
                calendar: null
        });

        $timeout(function () {
            if ($scope.control.$initializing) {
                $scope.setData();
            } else if ($scope.control.value) {
                $scope.setPreview($scope.control.value);
            }
        }, 200);
    }]);

angular.module("umbraco")
    .controller("EventCalendar.GridEditors.Dialogs.EventListDialogController",
    ["$scope", "editorState", "calendarResource", function ($scope, editorState, calendarResource) {

        $scope.calendars = [{ id: '0', calendarname: '-- All calendar --' }];
        $scope.dialogOptions = $scope.$parent.dialogOptions;

        $scope.save = function () {

            // Make sure form is valid
            if (!$scope.ecelForm.$valid)
                return;            

            $scope.submit($scope.dialogData);
        };

        //Load all calendar
        calendarResource.getall().then(function (response) {
            angular.forEach(response.data, function (calendar) {
                $scope.calendars.push({ id: calendar.id.toString(), calendarname: calendar.calendarname });
            });
        }, function (response) {
            notificationsService.error("Error", "Could not load calendar");
        });
    }]);

/* Calendar Grid Editor */
angular.module("umbraco")
    .controller("EventCalendar.GridEditors.CalendarGridEditorController",
    ["$scope", "$rootScope", "$timeout", "$routeParams", "$filter", "assetsService", "calendarResource", "EventCalendar.GridEditors.Services.CalendarDialogService",
        function ($scope, $rootScope, $timeout, $routeParams, $filter, assetsService, calendarResource, eccDialogService) {
        $scope.title = "Click to select calendar";
        $scope.icon = "icon-calendar-alt";
        $scope.preview = false;

        $scope.setValue = function (data) {
            $scope.control.value = data;
        };

        $scope.setPreview = function (model) {
            if ("enablePreview" in $scope.control.editor.config && $scope.control.editor.config.enablePreview) {
                $scope.preview = true;
                assetsService.loadCss("/css/EventCalendar/fullcalendar.css");
                assetsService.loadCss("http://cdn.jsdelivr.net/qtip2/2.2.0/jquery.qtip.min.css");
                assetsService
                .load([
                    "/scripts/EventCalendar/fullcalendar.min.js",
                    "/scripts/EventCalendar/gcal.js",
                    "/scripts/EventCalendar/lang-all.js",
                    "http://cdn.jsdelivr.net/qtip2/2.2.0/jquery.qtip.min.js"
                ]).then(function () {
                    calendarResource.getEventSources(model.calendar).then(function (data) {
                        $("#calendar").fullCalendar({
                            header: {
                                left: 'prev,next today',
                                center: 'title',
                                right: 'month,basicWeek,basicDay'
                            },
                            eventSources: data,
                            eventRender: function (event, element) {
                                if (event.type == 0) {
                                    if (event.end != null) {
                                        element.qtip({
                                            content: {
                                                text: "<h2>" + event.title + "</h2><h5>Start:" + event.start.format('llll') + "</h5><h5>End:" + event.end.format('llll') + "</h5><div>" + event.description + "</div>",
                                                title: event.title
                                            },
                                            style: "qtip-blue"
                                        });
                                    } else {
                                        element.qtip({
                                            content: {
                                                text: "<h2>" + event.title + "</h2><h5>Start:" + event.start.format('llll') + "</h5><div>" + event.description + "</div>",
                                                title: event.title
                                            },
                                            style: "qtip-blue"
                                        });
                                    }
                                } else {
                                    element.qtip({
                                        content: {
                                            text: "<h2>" + event.title + "</h2><h5>Start:" + event.start.format('ll') + "</h5><div>" + event.description + "</div>",
                                            title: event.title
                                        },
                                        style: "qtip-blue"
                                    });
                                }
                            }
                        });
                    });
                });
            }
        };

        $scope.setData = function () {
            eccDialogService.open({
                dialogData: $scope.control.value,
                callback: function (data) {
                    $scope.setValue(data);
                    $scope.setPreview($scope.control.value);
                }
            });
        };

        $scope.setValue($scope.control.value || { calendar: null });

        $timeout(function () {
            if ($scope.control.$initializing) {
                $scope.setData();
            } else if ($scope.control.value) {
                $scope.setPreview($scope.control.value);
            }
        }, 400);
    }]);

angular.module("umbraco")
    .controller("EventCalendar.GridEditors.Dialogs.CalendarDialogController",
    ["$scope", "editorState", "calendarResource", function ($scope, editorState, calendarResource) {

        $scope.calendars = [{ id: '0', calendarname: '-- All calendar --' }];
        $scope.dialogOptions = $scope.$parent.dialogOptions;

        $scope.save = function () {
            // Make sure form is valid
            if (!$scope.eccForm.$valid)
                return;

            $scope.submit($scope.dialogData);
        };

        //Load all calendar
        calendarResource.getall().then(function (response) {
            angular.forEach(response.data, function (calendar) {
                $scope.calendars.push({ id: calendar.id.toString(), calendarname: calendar.calendarname });
            });
        }, function (response) {
            notificationsService.error("Error", "Could not load calendar");
        });
    }]);