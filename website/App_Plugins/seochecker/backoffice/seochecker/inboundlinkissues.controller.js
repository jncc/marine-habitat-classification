angular.module("umbraco")
    .controller("seoChecker.inboundlinkissuesController",
    function ($scope, $timeout, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.loaded = false;
        $scope.bindData = function () {
            seocheckerBackofficeResources.loadAllInBoundLinkErrors($scope.model).then(function (res) {
                $scope.loaded = true;
                $scope.model = res.data;
                seocheckerHelper.syncPath($scope.model.path);
            });
        };

        $scope.save = function () {
            seocheckerBackofficeResources.saveInBoundLinkErrors($scope.model.data).then(function (res) {
                $scope.model = res.data;
                seocheckerHelper.showNotification($scope.model.notificationStatus);
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
                template: "/app_plugins/seochecker/dialogs/redirects/deleteredirects.html",
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

        $scope.editRedirect = function (id) {
            dialogService.open({
                // set the location of the view
                template: "/app_plugins/seochecker/dialogs/redirects/editredirects.html",
                // pass in data used in dialog
                dialogData: id,
                // function called when dialog is closed
                callback: function (value) {
                    if (value === true) {
                        $scope.model.resetPaging = true;
                        $scope.bindData();
                    }
                }
            });
        };

        $scope.createRedirect = function (id) {
            dialogService.open({
                // set the location of the view
                template: "/app_plugins/seochecker/dialogs/redirects/editredirects.html",
                // pass in data used in dialog
                dialogData: id,
                // function called when dialog is closed
                callback: function (value) {
                    if (value === true) {
                        $scope.model.resetPaging = true;
                        $scope.bindData();
                    }
                }
            });
        };

        $scope.showDialog = function (item) {
            var dialogOptions = {
                multiPicker: false,
                startNodeId: null,
                callback: function (data) {
                    item.id = data.id;
                    item.name = data.name;
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                },
                idType: "int"
            };
            if (item.contentType === 'media') {
                dialogService.mediaPicker(dialogOptions);

            } else {

                dialogService.contentPicker(dialogOptions);
            }

        };

        $scope.clear = function (item) {
            item.id = null;
            item.name = '';
        };

        $scope.buttonGroup = {
            defaultButton: {
                labelKey: "buttons_save",
                hotKey: "ctrl+s",
                hotKeyWhenHidden: false,
                handler: function() {
                    $scope.save();
                }
            },
            subButtons: [
                {
                    labelKey: "seoCheckerBulkActions_bulkActionClearAllButton",
                    hotKey: "ctrl+d",
                    hotKeyWhenHidden: false,
                    handler: function() {
                        dialogService.open({
                            template: '/app_plugins/seochecker/dialogs/confirm.html',
                            callback: function (result) {
                                if (result === true) {
                                    seocheckerBackofficeResources.clearInboundLinkIssues().then(function (res) {
                                        seocheckerHelper.showNotification(res.data);
                                        $scope.bindData();
                                    });
                                }
                            },
                            dialogData: {
                                localizationKey: 'seoCheckerBulkActions_bulkActionClearAllConfirmMessage'
                            }
                        });
                    }
                }
            ]
        };

        //Initialize
        $scope.bindData();
    });