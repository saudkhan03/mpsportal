var adminmodule = angular.module("app-mps",[])

adminmodule.controller("admin-deductions", ['$scope', '$http', function ($scope, $http) {
    var DeductionFullList = [];
    $scope.wait = true;
    $http.get('/Admin/LoadDeductions')
        .then(function (result) {
            DeductionFullList = result.data;
            $scope.DeductionList = DeductionFullList;
            $scope.deductionButtonText = 'Add Deduction';
            $scope.wait = false;
        },
        function(error){
            alert(error.statusText);
            $scope.wait = false;
        });
    $scope.changed = function () {
        $scope.DeductionList = $scope.searchtext ? performfilter($scope.searchtext) : DeductionFullList;
    }
    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return DeductionFullList.filter(function (d) {
            return d.name.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.getDeduction = function(deductionId){
        $scope.wait = true;
        $http.get('/Admin/getDeduction', { params: { id: deductionId } })
        .then(function (result) {
            $scope.deduction = result.data;
            $scope.wait = false;
        },function (error) {
            $scope.wait = false;
            alert(error.statusText);
        });
        $scope.deductionButtonText = 'Update Deduction';
    }
    $scope.addDeduction = function(){
        $scope.wait = true;
        $http.post('/Admin/AddDeduction', $scope.deduction)
        .then(function(result){
            $scope.wait = false;
            alert('Deduction Added/Updated');
            window.location.reload(true);
        },function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
    $scope.clearDeductionForm = function(){
        $scope.deduction.id = '';
        $scope.deduction.deductionName = '';
        $scope.deduction.deductionDesc = '';
        $scope.deduction.deductionType = 'Rs';
        $scope.deduction.deductionValue = ''
        $scope.deductionButtonText = 'Add Deduction';
    }
    $scope.deleteDeduction = function(){
        $scope.wait = true;
        $http.post('/Admin/DeleteDeduction', $scope.deduction)
        .then(function(result){
            $scope.wait = false;
            alert('Deduction deleted');
            window.location.reload(true);
        },function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
}]);