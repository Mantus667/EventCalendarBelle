angular.module("umbraco")
    .controller("EventCalendar.GridEditors.UpcomingEventsEditorController", 
    ["$scope", "$rootScope", "$timeout", "$routeParams", "$filter", "calendarResource", "EventCalendar.GridEditors.Services.EventListDialogService", function ($scope, $rootScope, $timeout, $routeParams, $filter, calendarResource, ecelDialogService) {
        $scope.title = "Click to insert item";
        $scope.icon = "icon-item-arrangement";

        $scope.setValue = function (data) {
            $scope.control.value = data;
        };

        $scope.setPreview = function (model) {
            if ("enablePreview" in $scope.control.editor.config && $scope.control.editor.config.enablePreview) {
                $scope.preview = "<" + $scope.control.editor.config.titleTag + ">" + model.title + "</" + $scope.control.editor.config.titleTag + ">";
                calendarResource.getEvents(model.calendar,model.count, true).then(function (response) {
                    if (response.data) {
                        console.log(response);
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