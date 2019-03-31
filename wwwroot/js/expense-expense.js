var expensemodule = angular.module("app-mps",[])

expensemodule.controller("expense-expense", ['$scope', '$http',function ($scope, $http) {
    var ExpenseFullList = [];
    $scope.wait = true;
    $http.get('/Expense/LoadExpenseHeaders')
    .then(function (result) {
        ExpenseFullList = result.data;
        $scope.ExpenseHeaderList = ExpenseFullList;
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
    $scope.getExpense = function(i,n,d){
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth()+1; //January is 0!
    
        var yyyy = today.getFullYear();
        if(dd<10){
            dd='0'+dd;
        } 
        if(mm<10){
            mm='0'+mm;
        } 
        var today = dd+'-'+mm+'-'+yyyy;
        var ex = {
            'expenseId':i,
            'expenseName':n,
            'expenseDesc':d,
            'expenseDate':today,
            'studentSlabLinkedId':0
            //'expenseAttachment1':'',
            //'expenseAttachment2':'',
           // 'expenseAttachment3':''
        }
        $scope.expense = ex;
        $scope.studentList =[];
    }
    $scope.getStudents = function(){
        $scope.wait = true;
        $http.get('/Expense/LoadStudentListForExpense')
        .then(function (result) {
            var e = {
                studentSlabId:0,
                studentName:'No linked student'
            }
            var s=[];
            s.push(e);
            $.each(result.data.students,function(i,v){
                s.push(v);
            });
            
            $scope.studentList = s;
            
            $scope.wait = false;
        },
        function(error){
            alert('Could not load student list\n'+error.statusText);
            $scope.wait = false;
        });
    }
    $scope.addExpense = function(){
        $scope.wait = true;
        //set date
        var ad = new Date(Number($('#expenseDate').val().split('-')[2]),Number($('#expenseDate').val().split('-')[1])-1,Number($('#expenseDate').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.expense.expenseDate = ad;
        //get attachments
        if($('#expenseAttachment1').val().length>0){
            $('#fileicon1').html('<i class="fa fa-refresh fa-spin"></i>');
            var f1 = document.getElementById('expenseAttachment1').files[0],
                r1 = new FileReader();
            r1.addEventListener("load", function () {
                $scope.expense.expenseAttachment1 = f1.name+"~"+r1.result;
                $('#fileicon1').html('');
                //end read attachments
                $http.post('/Expense/AddExpense', $scope.expense)
                .then(function(result){
                    if(result.data.res){
                        $scope.wait = false;
                        alert(result.data.status);
                        window.location.reload(true);
                    }
                    else
                    {
                        $scope.wait = false;
                        alert(result.data.status +'\n'+result.data.errors);
                    }
                },function(error){
                    $scope.wait=false;
                    alert(error.statusText);
                });
              }, false);

              if (f1) {
                r1.readAsDataURL(f1);
              }
        }
        else{
            $http.post('/Expense/AddExpense', $scope.expense)
            .then(function(result){
                if(result.data.res){
                    $scope.wait = false;
                    alert(result.data.status);
                    window.location.reload();
                }
                else
                {
                    $scope.wait = false;
                    alert(result.data.status +'\n'+result.data.errors);
                }
            },function(error){
                $scope.wait=false;
                alert(error.statusText);
            });
        }
    }
}]);