angular.module("umbraco")
    .controller("seoChecker.ignoredConfigurationController",
    function ($scope, $timeout, $routeParams, notificationsService, localizationService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.loaded = false;
        $scope.bindData = function () {
            seocheckerBackofficeResources.ignoredConfigurationIssues($scope.model).then(function (res) {
                $scope.loaded = true;
                $scope.model = res.data;
                seocheckerHelper.syncPath($scope.model.path);
            });
        };

        $scope.filter = function () {
            $scope.bindData();
        };

        $scope.sort = function (column) {
            $scope.model.setSortColumn = column;
            $scope.bindData();
        };

        $scope.setRecordCount = function () {
            $scope.model.resetPaging = true;
            $scope.bindData();
        };

        $scope.goToPage = function (pageNumber) {
            $scope.model.paging.currentPage = pageNumber;
            $scope.bindData();
        };

        $scope.showResult = function () {
            return $scope.model.hasRecords || ($scope.model.filter != null && $scope.model.filter.length > 0);
        };

        $scope.handleSelectAll = function () {
            seocheckerHelper.handleSelectAll($scope.model.selectAll, $scope.model.data);
        };

        $scope.anyItemSelected = function () {
            return seocheckerHelper.anyItemSelected($scope.model.data);
        }

        $scope.isSortDirection = function (columnName, sortDirection) {
            return seocheckerHelper.isSortDirection(columnName, sortDirection, $scope.model.orderByProperty, $scope.model.orderByDirection);
        }


        $scope.deleteSelected = function () {
            dialogService.open({
                template: '/app_plugins/seochecker/dialogs/confirm.html',
                callback: function (result) {
                    if (result === true) {
                        var data = seocheckerHelper.getSelectedItems($scope.model.data);
                        seocheckerBackofficeResources.removeIgnoredConfigurationIssues(data).then(function (res) {
                            seocheckerHelper.showNotification(res.data);
                            $scope.bindData();
                        });
                    }
                },
                dialogData: {
                    localizationKey: 'seoCheckerBulkActions_bulkActionDeleteConfirmMessage'
                }
            });
        };

        //Initialize
        $scope.bindData();
    });