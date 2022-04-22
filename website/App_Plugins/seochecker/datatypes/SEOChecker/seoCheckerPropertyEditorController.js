angular.module("umbraco")
    .controller("seoChecker.seoCheckerPropertyEditorController",
    function ($scope, $routeParams, $http, $sanitize ) {
        var nodeId = $routeParams.id;
        $scope.editMode = $routeParams.section === "content";
        $scope.titleLengthVisible = function () {
            return $scope.editMode && $scope.model.value.seoTitle.length > 0;
        };

        $scope.descriptionLengthVisible = function () {
            return $scope.editMode &&  $scope.model.value.seoDescription.length > 0;
        };

        //Validate length
        $scope.validateTitleLength = function () {
            if ($scope.editMode) {
                var limit = $scope.model.value.config.maxTitleLength;
                $scope.seoTitleLength = $scope.parseTemplate($scope.model.value.seoTitle).length;

                if ($scope.seoTitleLength > limit) {
                    $scope.seoTitleLengthClass = "error";
                } else {
                    $scope.seoTitleLengthClass = "valid";
                }
            }
        };

        $scope.validateDescriptionLength = function () {
            if ($scope.editMode) {
                var maxLimit = $scope.model.value.config.maxDescriptionLength;
                var minLimit = $scope.model.value.config.minDescriptionLength;
                $scope.seoDescriptionLength = $scope.model.value.seoDescription.length;

                if ($scope.model.value.seoDescription.length < minLimit ||
                    $scope.model.value.seoDescription.length > maxLimit) {
                    $scope.seoDescriptionLengthClass = "error";
                } else {
                    $scope.seoDescriptionLengthClass = "valid";
                }
            }
        };

        $scope.parseTemplate = function (titleValue) {
            titleValue = $sanitize(titleValue);
            if ($scope.editMode && $scope.model.value.snippetTitleTemplate.indexOf('@@seotitle@@') != -1)
            {
                return $scope.model.value.snippetTitleTemplate.replace('@@seotitle@@', titleValue);
            }
            return  titleValue;
        };
        
        $scope.formatTitle = function () {
            if ($scope.editMode) {
                var title = $scope.model.value.seoTitle;
                if (title == '') {
                    title = $scope.model.value.defaultTitlePropertyValue;
                }
                title = $scope.parseTemplate(title);
                title = $scope.getFirst(title, '', $scope.model.config.maxTitleLength);
                title = $scope.parseKeyword($scope.model.value.focusKeyword, title);
                return title;
            }
            return '';
        };
        
        $scope.formatSnippetUrl = function () {
            if ($scope.editMode) {
                var url = $scope.model.value.snippetUrl.replace('http://', '');
                url = $scope.parseKeyword($scope.model.value.focusKeyword, url);
                return url;
            }
            return '';
        };

        $scope.formatDescription = function () {
            if ($scope.editMode) {
                var description = $scope.model.value.seoDescription;
                if (description == '') {
                    description = $scope.model.value.defaultDescriptionValue;
                }
                description = $scope.getFirst(description, '...', $scope.model.config.maxDescriptionLength);
                description = $scope.parseKeyword($scope.model.value.focusKeyword, description);
                description = $sanitize(description);
                return description;
            }
            return '';
        };

        $scope.parseKeyword = function (focusKeyword, stringValue) {
            if ($scope.editMode) {
                //Regular expressions can't handle unicode so therefore a split on words
                if (!(focusKeyword == null || stringValue == null)) {
                    var valueArray = $scope.parseText(stringValue);
                    for (var i = 0; i < valueArray.length; i++) {
                        var focusArr = focusKeyword.split(' ');
                        for (var y = 0; y < focusArr.length; y++) {
                            if (focusArr[y].length > 0 && focusArr[y].toLowerCase() == valueArray[i].toLowerCase()) {
                                //should be valid keyword
                                valueArray[i] = '<strong>' + valueArray[i] + '</strong>';
                            }
                        }
                    }
                    stringValue = valueArray.join("");
                }

                return stringValue;
            }
            return '';
        };
        
        $scope.parseText = function (val) {
            var arr = [];
            var tmp = [];
            var chars = [' ', '\'', '"', ',', '.', ':', ';'];
            for (var i = 0; i < val.length; i++) {
                if (chars.indexOf(val[i]) >= 0) {
                    arr.push(tmp.join(''));
                    arr.push(val[i]);
                    tmp = [];
                } else {
                    tmp.push(val[i]);
                }
            }
            arr.push(tmp.join(''));
            return arr;
        };

            //Open keyword selection tool
            $scope.openKeywordSelectionTool = function () {
                window.open($scope.model.config.keywordSelectionTool, '_blank', 'width=900,height=700'); return false;
            };
        
            //Get first x Characters
            $scope.getFirst = function (txt, suffixWhenLonger, limit) {
                if (txt.length > limit) {
                    txt = txt.substring(0, limit) + suffixWhenLonger;
                }
            
                return txt;
            };
        
        //Validation

            $scope.doValidatePage = function () {
                $scope.showValidationResult = true;
                $scope.validationResult = null;
                $scope.validatingPage = true;
                $http.get('backoffice/SEOChecker/SEOCheckerApi/Validate?id=' + nodeId)
                    .then(function (res) {
                        $scope.validationResult = res.data;
                        $scope.validatingPage = false;
                    });
            };

        if ($scope.editMode) {
            $scope.$watch('model.value.requestId',
                function () {
                    if ($scope.model.value.config.validationMode === "always" || $scope.postback && $scope.model.value.config.validationMode === "onsave") {
                        $scope.doValidatePage();
                    }
                    $scope.postback = true;
                });

            $scope.validateTitleLength();
            $scope.validateDescriptionLength();
        }
    });

