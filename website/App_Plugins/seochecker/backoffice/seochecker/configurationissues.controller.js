angular.module("umbraco")
    .controller("seoChecker.configurationissuesController",
    function ($scope, $timeout, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.loaded = false;
        $scope.bindData = function () {
            seocheckerBackofficeResources.configurationIssues($scope.model).then(function (res) {
                $scope.loaded = true;
                $scope.model = res.data;
                seocheckerHelper.syncPath($scope.model.path);
            });
        };

        $scope.clearDialog = function () {
            dialogService.open({
                template: '/app_plugins/seochecker/dialogs/confirm.html',
                callback: function (result) {
                    if (result === true) {
                        seocheckerBackofficeResources.clearConfigurationIssues().then(function (res) {
                            seocheckerHelper.showNotification(res.data);
                            $scope.bindData();
                        });
                    }
                },
                dialogData: {
                    localizationKey: 'seoCheckerBulkActions_bulkActionClearAllConfirmMessage'
                }
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
                // set the location of the view
                template: "/app_plugins/seochecker/dialogs/validation/deleteconfigurationissues.html",
                // pass in data used in dialog
                dialogData: seocheckerHelper.getSelectedItems($scope.model.data),
                // function called when dialog is closed
                callback: function (value) {
                    if (value === true) {
                        $scope.model.resetPaging = true;
                        $scope.bindData();
                    }
                }
            });
        };

        //Initialize
        $scope.bindData();
    });