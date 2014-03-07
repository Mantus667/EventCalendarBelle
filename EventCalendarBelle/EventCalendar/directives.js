angular.module("umbraco.directives").directive("eclocalize", ["$log", "ecLocalizationService"],
    function (a, b) {
        return {
            restrict: "E",
            scope: {
                key: "@"
            },
            replace: !0,
            link: function (scope, elem, attrs) {
                var d = scope.key;
            }
        }
    });