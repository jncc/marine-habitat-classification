angular.module("umbraco.directives")
    .directive('seocheckerLicenseinfo', function (seocheckerBackofficeResources) {
        return {
            transclude: true,
            restrict: 'E',
            replace: true,
            link: function (scope, element) {
                seocheckerBackofficeResources.getLicenseInfo().then(function (res) {
                    if (res.data.licenseError === true) {
                        element.html('<div class="alert alert-error seocheckerLicenseInfo">' + res.data.licenseMessage + '</div>');
                    }
                    if (res.data.trialLicense === true) {
                        element.html('<div class="alert alert-warning seocheckerLicenseInfo">' + res.data.licenseMessage + '</div>');
                    }
                });
            }
        };
    });