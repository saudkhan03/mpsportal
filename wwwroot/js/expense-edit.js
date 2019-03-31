var expensemodule = angular.module("app-mps",[])

expensemodule.controller("expense-edit", ['$scope', '$http', function ($scope, $http) {
    var ExpenseFullList = [];
    $scope.wait = true;
    $http.get('/Expense/LoadEditExpenses')
    .then(function (result) {
        ExpenseFullList = result.data.expenses;
        $scope.ExpenseList = ExpenseFullList;
        $scope.wait = false;
    },
    function(error){
        alert(error.statusText);
        $scope.wait = false;
    });

    $scope.changed = function () {
        $scope.ExpenseList = $scope.searchtext ? performfilter($scope.searchtext) : ExpenseFullList;
    }
    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return ExpenseFullList.filter(function (expense) {
            return expense.expenseName.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.getExpense = function(id){
        $scope.wait = true;
        $http.get('/Expense/getExpense', { params: { id: id } })
        .then(function (result) {
            $scope.expense=result.data;
            if(result.data.attachment1){
                $('#attachment').show();
                $('#attachmentnew').hide();
                $('#oldattachment').hide();
                puticon($('#fileicon1'),result.data.attachment1.fileExtension);
            }
            else{
                $('#attachment').hide();
                $('#attachmentnew').show();
                $('#oldattachment').hide();
            }
            var today = new Date($scope.expense.expenseDate);
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
            $scope.expense.expenseDate = today;
            $scope.wait = false;
        }, function (error) {
            $scope.wait = false;
            alert(error.statusText);
        });
        $scope.getStudents();
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
    $scope.editExpense = function(){
        $scope.wait = true;
        //set date
        var ad = new Date(Number($('#expenseDate').val().split('-')[2]),Number($('#expenseDate').val().split('-')[1])-1,Number($('#expenseDate').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.expense.expenseDate = ad;
        $scope.expense.expenseId = $scope.expense.id;
        if($('#expenseAttachment1').val().length>0){
           // $('#fileicon1').html('<i class="fa fa-refresh fa-spin"></i>');
            var f1 = document.getElementById('expenseAttachment1').files[0],
                r1 = new FileReader();
            r1.addEventListener("load", function () {
                $scope.expense.expenseAttachment1 = f1.name+"~"+r1.result;
            //    $('#fileicon1').html('');
                $http.post('/Expense/UpdateExpense', $scope.expense)
                .then(function(result){
                    $scope.wait = false;
                    if(result.data.res){
                        alert('updated expense');
                        window.location.reload(true);
                    }
                    else
                    alert(result.data.status+'\n - '+result.data.errors);
                },function(error){
                    $scope.wait = false;
                    alert(error.statusText);
                });
            }, false);
            if (f1) {
                r1.readAsDataURL(f1);
              }
        }
        else{
            $http.post('/Expense/UpdateExpense', $scope.expense)
                .then(function(result){
                    $scope.wait = false;
                    if(result.data.res){
                        alert('updated expense');
                        window.location.reload(true);
                    }
                    else
                    alert(result.data.status+'\n - '+result.data.errors);
                    
                },function(error){
                    $scope.wait = false;
                    alert(error.statusText);
                });
        }
    }
    $scope.download1 = function(){
        $scope.wait = true;
        // var att = {
        //     'name':$scope.expense.attachment1.name,
        //     'fileExtension':$scope.expense.attachment1.fileExtension,
        //     'content':$scope.expense.attachment1.content,
        //     'contentType':$scope.expense.attachment1.contentType
        // }
        $http.post('/Expense/downloadAttachment', $scope.expense.attachment1)
        .then(function(result){
            $scope.wait = false;
            window.location.href=result.data.url;
        },function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
    var puticon = function(span,icon){
        switch(icon)
        {
            case 'pdf':
                span.html('');
                span.html('<i class="fa fa-file-pdf-o"></i>');
                break;
            case 'jpg':
            case 'jpeg':
            case 'JPG':
            case 'JPEG':
            case 'bmp':
            case 'BMP':
            case 'tiff':
            case 'png':
            case 'PNG':
                span.html('');
                span.html('<i class="fa fa-file-image-o"></i>');
                break;
            case 'docx':
            case 'doc':
                span.html('');
                span.html('<i class="fa fa-file-word-o"></i>');
                break;
            case 'xsl':
            case 'xlsx':
                span.html('');
                span.html('<i class="fa fa-file-excel-o"></i>');
                break;
            case 'txt':
            case 'log':
                span.html('');
                span.html('<i class="fa fa-file-text-o"></i>');
                break;
            case 'zip':
            case 'rar':
                span.html('');
                span.html('<i class="fa fa-file-archive-o"></i>');
                break;
            default :
            span.html('');
            span.html('<i class="fa fa-file"></i>');
        }
    }
}]);