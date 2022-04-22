angular.module("umbraco")
    .controller("seoChecker.generalSettingsController",
    function ($scope,$rootScope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.bindData = function () {

            seocheckerBackofficeResources.initializeGeneralSettings($scope.model).then(function (res) {
                $scope.model = res.data;
                $scope.loaded = true;
                seocheckerHelper.syncPath($scope.model.path);
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };
        $rootScope.$on("seobooleanCheckbox.changed", function (event, data) {
            if (data.alias === 'useFaceBook') {
                $scope.updateTabModel('socialTab');
            }
            if (data.alias === 'useTwitter') {
                $scope.updateTabModel('socialTab');
            }
            if (data.alias === 'robotsTxtEnabled') {
                $scope.updateTabModel('robotsTxt');
            }
            if (data.alias === 'xmlSitemapEnabled') {
                $scope.updateTabModel('xmlSitemapTab');
            }
        });
        
        $scope.updateTabModel = function(tabName) {
            seocheckerBackofficeResources.initializeGeneralSettings($scope.model)
                   .then(function (res) {
                       seocheckerHelper.updateTab($scope.model, res.data, tabName);
                   },
                       function (data) {
                           seocheckerHelper.showServerError();
                       });
        }

        $scope.save = function () {
            seocheckerBackofficeResources.saveGeneralSettings($scope.model).then(function (res) {
                var model = res.data;
                if (!model.isInValid) {
                    $scope.frm.$setPristine();
                    seocheckerHelper.showNotification(model.notificationStatus);
                }
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };

        //Initialize
        $scope.bindData();
    });