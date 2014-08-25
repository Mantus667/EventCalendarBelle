angular.module('umbraco.services').factory('eventCalendarLocalizationService', function ($http, $q, userService) {
    var service = {
        resourceFileLoaded: false,
        dictionary: {},
        localize: function (key) {
            var deferred = $q.defer();

            if (service.resourceFileLoaded) {
                var value = service._lookup(key);
                deferred.resolve(value);
            }
            else {
                service.initLocalizedResources().then(function (dictionary) {
                    var value = service._lookup(key);
                    deferred.resolve(value);
                });
            }

            return deferred.promise;
        },
        _lookup: function (key) {
            return service.dictionary[key];
        },
        initLocalizedResources: function () {
            var deferred = $q.defer();
            userService.getCurrentUser().then(function (user) {
                $http.get("/App_plugins/EventCalendar/langs/" + user.locale + ".js", { cache: true })
                    .then(function (response) {
                        service.resourceFileLoaded = true;
                        service.dictionary = response.data;

                        return deferred.resolve(service.dictionary);
                    }, function (err) {
                        return deferred.reject("Lang file missing");
                    });
            });
            return deferred.promise;
        },
        getDateTimeFormat: function (locale) {
            var deferred = $q.defer();
            $http.get(Umbraco.Sys.ServerVariables.eventCalendar.localizationBaseUrl + "GetCurrentDateFormat?locale=" + locale)
                .then(function (response) {
                    return deferred.resolve(response.data);
                }, function (err) {
                    return deferred.reject("Couldn't get datetime-format");
                });
            return deferred.promise;
        }
    }

    return service;
});

angular.module("umbraco.directives").directive('eventcalendarLocalize', function (eventCalendarLocalizationService) {
    var linker = function (scope, element, attrs) {

        var key = scope.key;

        eventCalendarLocalizationService.localize(key).then(function (value) {
            if (value) {
                element.html(value);
            }
        });
    }

    return {
        restrict: "E",
        replace: true,
        link: linker,
        scope: {
            key: '@'
        }
    }
});