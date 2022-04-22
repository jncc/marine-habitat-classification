angular.module("umbraco.directives")
    .directive('seocheckerHeader', function (seocheckerResourceService) {
        return {
            transclude: true,
            restrict: 'E',
            replace: true,
            link: function (scope, element) {
                seocheckerResourceService.localize('SEOChecker').then(function (value) {
                    element.html('<h1>' + value + '</h1>');
                });
            }
        };
    });

