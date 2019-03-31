var adminmodule = angular.module("app-mps",[])

adminmodule.controller("admin-slabs", ['$scope', '$http', function ($scope, $http) {
    var SlabFullList = [];
    $scope.wait = true;
    $http.get('/Admin/LoadSlabs')
        .then(function (result) {
            SlabFullList = result.data;
            $scope.SlabList = SlabFullList;
            $scope.wait = false;
        },
        function(error){
            alert(error.statusText);
            $scope.wait = false;
        });
    $scope.changed = function () {
        $scope.SlabList = $scope.searchtext ? performfilter($scope.searchtext) : SlabFullList;
    }
    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return SlabFullList.filter(function (slab) {
            return slab.slabName.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.getSlab = function(slabId){
        $scope.wait = true;
        $http.get('/Admin/getSlab', { params: { id: slabId } })
        .then(function (result) {
            $scope.slab = result.data;
            $scope.wait = false;
        },function (error) {
            $scope.wait = false;
            alert(error.statusText);
        });
        //alert(slabId);
    }
    $scope.updateSlab = function(){
        $scope.wait = true;
        $http.post('/Admin/updateSlab', $scope.slab)
        .then(function(result){
            $scope.wait = false;
            alert('Expense Added/Updated');
            window.location.reload(true);
        },function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
}]);