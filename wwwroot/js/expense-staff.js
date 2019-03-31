var expensemodule = angular.module("app-mps", ['ngConfirm']);

expensemodule.controller("staff-expense", ['$scope', '$http', function ($scope, $http) {
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
    $scope.salary = {
        'Date': today,
        'newSalaryDate': today
    };
    var StaffFullList = [];
    $scope.wait = true;
    $http.get('/Expense/LoadStaff')
        .then(function (result) {
            StaffFullList = result.data.staff;
            $scope.StaffList = StaffFullList
            var deArray=[];
            $.each(result.data.deductions,function(i,v){
                var obj = {
                    index:i,
                    name :v.name,
                    value:v.value,
                    type:v.type,
                    display:v.value+(v.type=='A'?' Rs.':v.type)
                };
                deArray.push(obj);
            });
            $scope.deductions = deArray;
            $scope.newEmail='';
            $scope.wait = false;
        },
        function(error){
            alert(error.statusText);
            $scope.wait = false;
        });
    $scope.changed = function () {
        $scope.StaffList = $scope.searchtext ? performfilter($scope.searchtext) : StaffFullList;
    }
    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return StaffFullList.filter(function (staff) {
            return staff.name.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.salaryChange = function() {
        var d = $scope.deductions;
        var s = $scope.salary.txtSalary;
        var indices=[];
        $.each($('[id^="chkd_"]'),function(i,v){
            if($(this).is(':checked'))
                indices.push(v.id.split('_')[1]);
        });
        var final=0;
        var tempd = [];
        $.each(d,function(i,v){
            var val = v.value;
            if($.inArray(v.index.toString(),indices)>-1){
                switch(v.type){
                    case '%':
                        if(v.name.indexOf('Provident fund')>-1){
                            if(final>0){
                                v.display = '  '+final+' - ('+val+v.type+' of '+s+') = '+(final - (Number(val) % final)).toString();
                                final = (final - (Number(val) % final)).toString();
                            }
                            else{
                                v.display = '  '+s+' - ('+val+v.type+' of '+s+') = '+(s - (Number(val) % s)).toString();
                                final = (s - (Number(val) % s)).toString();
                            }
                            tempd.push(v);
                        }
                        else if(v.name.indexOf('Provident fund')>-1){
                            v.display = val + v.type;
                            tempd.push(v);
                        }
                        else if(v.name.indexOf('Provident fund')==-1){
                            if(final>0){
                                v.display = '  '+final+' - ('+val+v.type+' of '+s+') = '+(final - (Number(val) % final)).toString();
                                final = (final - (Number(val) % final)).toString();
                            }
                            else{
                                v.display = '  '+s+' - ('+val+v.type+' of '+s+') = '+(s - (Number(val) % s)).toString();
                                final = (s - (Number(val) % s)).toString();
                            }
                            tempd.push(v);
                        }
                        else
                            tempd.push(v);
                        break;
                    case 'A':
                    case 'Rs':
                        if(final>0){
                            v.display = ' '+final+' - Rs. '+val+' = '+(final-Number(val)).toString();
                            final = (final-Number(val)).toString();
                        }
                        else{
                            v.display = ' '+s+' - Rs. '+val+' = '+(s-Number(val)).toString();
                            final = (s-Number(val)).toString();
                        }
                        tempd.push(v);
                        break;
                }
            }
            else{
                v.display = v.value+v.type;
                tempd.push(v);
            }
        });
        $scope.deductions = tempd;
        $scope.salary.newSalaryAmount = final;
    }
    $scope.getStaff = function (id) {
        $scope.wait = true;
        $scope.student = $http.get('/Expense/getStaff', { params: { id: id } })
        .then(function (result) {
            $scope.staff = result.data;
            $scope.wait = false;
        },
        function(error){
            $scope.wait = false;
            alert('ERROR while getting staff');
        });
        $('#addsalary').hide();
        $('#btnaddsalary').show();
    }
    $scope.getSalary = function(staffId){
        $scope.salaryWait=true;
        $http.get('/Expense/getSalary', { params: { staffId: staffId} })
        .then(function(result){
            $scope.staff.salaryPayments = result.data.salaryPayments;
            $scope.salaryWait=false;
        }
        ,function(error){
            alert(error.statusText);
            $scope.salaryWait=false;
        });
    }
    $scope.addSalary = function () {
        $scope.wait = true;
        $scope.salary.staffId = $scope.staff.staffId;
        $http.post('/Expense/addSalary', $scope.salary)
        .then(function(result){
            if(typeof(result)=="object"){
                $('#addsalary').toggle();$('#btnaddsalary').toggle();
                var newSalaryItem = result.data;
                $scope.staff.salaryPayments.push(newSalaryItem);
                $scope.wait = false;
            }
            else
            {
                $scope.wait = false;
                alert(result);
            }
        },function(err){
            $scope.wait = false;
            alert(err.statusText);
        });
    }
    $scope.deleteSalary = function(id){
        //alert(id);
        $http.post('/Expense/deleteSalary', id)
        .then(function(){
            $.each($scope.staff.salaryPayments, function(i,v){
                if(v.id === id) {
                    $scope.staff.salaryPayments.splice(i,1);
                    return false;
                }
            });
 
        }, 
        function(error){
            alert(error.statusText);
        });
    }
    $scope.cancelAddSalaryForm = function(){
        $('#addsalary').toggle();
        $('#btnUpdateSalary').toggle();
        $('#btnaddsalary').toggle();
        $scope.salary.Amount = '';
    }
    $scope.cancelUpdateSalaryForm = function(){
        $('#updateSalary').toggle();
        $('#btnaddsalary').toggle();
        $('#btnUpdateSalary').toggle();
        $scope.salary.newSalaryAmount='';
    }
    $scope.updateSalary = function(){
        $scope.wait = true;
        $scope.salary.staffId = $scope.staff.staffId;
        var sd = "";
        var indices=[];
        $.each($('[id^="chkd_"]'),function(i,v){
            if($(this).is(':checked'))
                indices.push(v.id.split('_')[1]);
        });
        $.each($scope.deductions,function(i,v){
            if($.inArray(v.index.toString(),indices)>-1){
                sd = sd.length>0? sd + "~"+v.name+": "+ v.value+v.type : sd + v.name+": "+ v.value+v.type;
            }
        });
        $scope.salary.deductionString = sd;
        $http.post('/Expense/updateSalary', $scope.salary)
        .then(function(result){
            if(typeof(result)=="object"){
                $('#updateSalary').toggle();$('#btnUpdateSalary').toggle();$('#btnaddsalary').toggle();
                var newSalaryItem = result.data;
                $scope.staff.totalSalary = result.data.salary;
                var sd = result.data.salarySetDate;
                // var dd = sd.getDate();
                // var mm = sd.getMonth()+1; //January is 0!

                // var yyyy = sd.getFullYear();
                // if(dd<10){
                //     dd='0'+dd;
                // } 
                // if(mm<10){
                //     mm='0'+mm;
                // } 
                var sd = sd.split('-')[2]+'-'+sd.split('-')[1]+'-'+sd.split('-')[0];
                $scope.staff.salarySetDate = sd;
                $scope.wait = false;
            }
            else
            {
                $scope.wait = false;
                alert(result);
            }
        },function(err){
            $scope.wait = false;
            alert(err.statusText);
        });
    }
    $scope.sendSalaryDetails = function(paymentId){
        $scope.wait = true;
        var e=$('#newEmail').val().length==0?$scope.staff.email:$('#newEmail').val();
        $http.get('/Expense/sendSalaryEmail?staffId='+$scope.staff.staffId+'&email='+e+'&paymentId='+paymentId)
        .then(function(result){
            if(result.data){alert('Mail sent');}
            else {alert('Could not send email,\nplease check your internet connection\nand try again')}
            $scope.wait = false;
        },function(err){
            $scope.wait = false;
            alert(err.statusText);
        });
    }
    $scope.getActiveClass = function(res){
        return res=='True' ? 'fa fa-circle-o activeUser' : 'fa fa-circle-o inactiveUser';
    }
}]);