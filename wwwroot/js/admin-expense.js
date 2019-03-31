var adminmodule = angular.module("app-mps",[])

adminmodule.controller("admin-expense", ['$scope', '$http', function ($scope, $http) {
    var ExpenseFullList = [];
    $scope.wait = true;
    $http.get('/Admin/LoadExpenseHeaders')
        .then(function (result) {
            ExpenseFullList = result.data;
            $scope.ExpenseHeaderList = ExpenseFullList;
            $scope.expenseButtonText = 'Add Expense';
            $scope.wait = false;
        },
        function(error){
            alert(error.statusText);
            $scope.wait = false;
        });
    
    $scope.changed = function () {
        $scope.ExpenseHeaderList = $scope.searchtext ? performfilter($scope.searchtext) : ExpenseFullList;
    }
    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return ExpenseFullList.filter(function (expense) {
            return expense.expenseHeaderName.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.getExpense = function (i,n,d) {
      //  alert(i+' '+n+' '+d);
        var ex = {
            'expenseId':i,
            'expenseName':n,
            'expenseDesc':d
        }
        $scope.expense = ex;
        $scope.expenseButtonText = 'Update Expense';
    }
    $scope.addExpenseHeader = function(){
        $scope.wait = true;
        $http.post('/Admin/AddExpenseHeader', $scope.expense)
        .then(function(result){
            $scope.wait = false;
            alert('Expense Added/Updated');
            window.location.reload(true);
        },function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
    $scope.clearExpenseForm = function(){
        $scope.expense.expenseId = '';
        $scope.expense.expenseName = '';
        $scope.expense.expenseDesc = '';
        $scope.expenseButtonText = 'Add Expense';
    }
}]);