var expensemodule = angular.module("app-mps", ['ngConfirm']);

expensemodule.controller("student-edit", ['$scope', '$http', function ($scope, $http) {
    $scope.fees = { Date: new Date().toDateString() };
    var FullList = [];
    $scope.wait = true;
    $http.get('/Expense/LoadStudents')
        .then(function (result) {
            var x = filterUnique(result.data.students);
            FullList = x;
            $scope.StudentList = FullList;
            $scope.wait = false;
        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
        });
    var filterUnique = function(list){
        var x= [];
        var oldid='';
        var removed = 1;
        $.each(list,function(i,v){
            if(oldid!=v.studentId){
                oldid = v.studentId;
                x.push(v);
            }
            else{
                //check grades of old and higher
                //true if old is greater than new
                if(compareGrades(x[i-removed].studentName.split('-')[3],v.studentName.split('-')[3])){ 
                    //keep if old
                    //dont push
                    removed++;
                }
                else{
                    x.splice(i-removed,1);
                    x.push(v);
                    //remove old and add new if grade is higher
                    removed++;
                }
            }
        });
        return x;
    }
    var compareGrades = function(g1, g2){
        if(g1 == 'LKG'){g1 = -1;}
        else if(g1 == 'UKG'){g1 = 0;}
        else{g1 = g1.split(' ')[0];}
        if(g2 == 'LKG'){g2 = -1;}
        else if(g2 == 'UKG'){g2 = 0;}
        else{g2 = g2.split(' ')[0];}
        if(Number(g1) > Number(g2)){return true; }
        else{return false;}
    }
    $scope.changed = function () {
        $scope.StudentList = $scope.searchtext ? performfilter($scope.searchtext) : FullList;
    }
    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return FullList.filter(function (student) {
            var n =  student.studentName.split('-').splice(1,3).join("");
            return n.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.getStudent = function (id) {
        $scope.wait = true;
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
        $scope.fees.Date = today;
        $scope.student = $http.get('/Expense/getStudent', { params: { id: id } })
        .then(function (result) {
            $scope.student = result.data;
            var oldAY='';
            var newDates = [];
            $.each($scope.student.feeDates,function(i,v){
                if(oldAY != v.academicYear){
                    oldAY = v.academicYear;
                    newDates.push(v);
                }
            });
            $scope.student.feeDates = newDates;
            $.each($scope.student.feeDates,function(i,v){
                if(v.academicYear == result.data.academicYear)
                        $scope.ayselected = v;
            });
            if(!$scope.ayselected){ 
                $http.get('/Expense/getSlabsAndGrades', { params: { id: id } })
                .then(function(result){
                    $scope.slabs = result.data.slabs;
                    $scope.grades = result.data.grades;
                    var acadSlab = new Object();
                    acadSlab.slabName = $scope.student.slabName;
                   // var g = Number($scope.student.grade)==NaN?'Ukg':+Number($scope.student.grade)+1;
                    acadSlab.grade =$scope.student.grade;// g.toString();
                    acadSlab.totalFee = 0;
                    $scope.acadSlab = acadSlab;
                    $scope.acadSlab.totalFee=$scope.getAYFees();
                },
                function(error){
                    $scope.slabs = null;
                    alert('Could not load the list of slabs, please try again by refeshing this page');
                });
                
                $scope.student.totalFee=0;
            }
            var totalpaidfees=0;
            $.each($scope.student.fees,function(i,v){
                totalpaidfees = totalpaidfees + v.paidFees;
            });
            $scope.totalPaidFees = totalpaidfees;
            //expenses if any
            $scope.totalFeeWithExpense = $scope.student.totalFee;
            if($scope.student.expenses)
                if($scope.student.expenses.length>0){
                    $.each($scope.student.expenses,function(i,v){
                        $scope.totalFeeWithExpense = $scope.totalFeeWithExpense + v.amount;
                    });
                }
            //
            
            $('#addfee').hide();
            $('#btnaddfee').show();
            //$('#addfeedates').hide();
            $('#btnaddfeedates').show();
            $scope.wait = false;
        }, function (error) {
            $scope.wait = false;
            alert(error.statusText);
        });
    }
    $scope.getFees = function(studentId, academicYear){

        if(!studentId) return false; // this should work only when the academic year comboi is changed

        $scope.feesWait=true;
        $http.get('/Expense/getFees', { params: { studentId: studentId,academicYear:academicYear } })
        .then(function(result){
            $scope.student.fees = result.data.fees;
            $scope.student.expenses = result.data.expenses;
            var totalpaidfees=0;
            $.each($scope.student.fees,function(i,v){
                totalpaidfees = totalpaidfees + v.paidFees;
            });
            $scope.totalPaidFees = totalpaidfees;
            $scope.student.totalFee = result.data.totalFee;
            $scope.feesWait=false;
        }
        ,function(error){
            alert(error.statusText);
            $scope.feesWait=false;
        });
    }
    $scope.getAYFees = function(){
        if($scope.acadSlab.grade && $scope.acadSlab.slabName){
            $http.get('/Admin/getFee', { params: { slabName: $scope.acadSlab.slabName, grade: $scope.acadSlab.grade } })
            .then(function (result) {
               // if(result.data!=0){
                    $('#txtAYFee').val(result.data);
                   // return result.data
               // }
                //else
                //{
                  //  return 0;
                //}

            }, function (error) {
                alert(error.statusText);
            });
            return 0;
        }
    }
    $scope.addAcademicYear = function(){
        $scope.wait = true;
        var datesObj = new Object();
        datesObj.StudentId = $scope.student.studentId;
        datesObj.slabName = $scope.acadSlab.slabName;
        datesObj.grade = $scope.acadSlab.grade;
        //alert($scope.student.studentId+' '+$scope.acadSlab.slabName+' '+$scope.acadSlab.grade+' '+$scope.acadSlab.totalFee);
        $http.post('/Expense/addAcademicYear', datesObj)
        .then(function(result){
            if(result.data.exep==''){
                $scope.wait = false;
                window.location.reload();
            }
            else{
                $scope.wait = false;
                alert(result.data.exep);
            }
        },function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
    $scope.addFees = function (id) {
        $scope.wait = true;
        $scope.fees.Id = id;
        $http.post('/Expense/addFee', $scope.fees)
        .then(function(result){
            $('#addfee').toggle();$('#btnaddfee').toggle();
            var newFeeItem = result.data;
            $scope.student.fees.push(newFeeItem);
            var totalfees=0;
            $.each($scope.student.fees,function(i,v){
                totalfees = totalfees + v.paidFees;
            });
            $scope.totalPaidFees = totalfees;
            $scope.wait = false;
        },function(result){
            $scope.wait = false;
            aler(error.statusText);
        });
    }
    $scope.deleteFee = function(id){
        $scope.wait = false;
        //alert(id);
        $http.post('/Expense/deleteFee', id)
        .then(function(){
            $.each($scope.student.fees, function(i){
                if($scope.student.fees[i].id === id) {
                    $scope.student.fees.splice(i,1);
                    return false;
                }
            });
            var totalfees=0;
            $.each($scope.student.fees,function(i,v){
                totalfees = totalfees + v.paidFees;
            });
            $scope.totalPaidFees = totalfees;
            $scope.wait = false;
        }, 
        function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
    $scope.printBill = function(id,paidFees,paidDate,s){
        //alert('Printing for '+$scope.student.firstName+' '+$scope.student.lastName+'\n'+$scope.student.totalFee+' - '+paidFees+'\n'+'on'+paidDate +' ('+id+')');
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
        var n = '';
        if(s.middleName)
            n = s.firstName+' '+s.middleName+' '+s.lastName;
        else
            {
                if(s.lastName)
                    n = s.firstName+' '+s.lastName;
                else
                    n = s.firstName;
            }
        var printModal =  {
            'generatedFor':'',
            'paidFees' : paidFees,
            'paidDate' : paidDate,
            'today' : today,
            'rollNo' : s.rollNumber,
            'grade': s.grade,
            'name' : n,
            'admissionFee':0,
            'cautionDeposit':0,
            'tutionFee':0,
            'hostelExpenses':0,
            'others':0,
            'total':0,
            'paymentType':'Cash',
            'paymentId':id
        };
        $scope.printModal = printModal;
        $(".btn-group button").each(function(i,v){
            if($(v).attr('id') == 'btnPrintBillCash'){
                $(v).removeClass('btn-default');
                $(v).addClass('btn-info');
            }
            else{
                $(v).addClass('btn-default');
                $(v).removeClass('btn-info');
            }
        });
        var x = $scope.printModal;
        $scope.printModal.total = Number(x.admissionFee) + Number(x.cautionDeposit) + Number(x.tutionFee) + Number(x.hostelExpenses) + Number(x.others);
        $('#printBillModal').modal();
        
    }
    $scope.cancelAddFeeForm = function(){
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
        $('#addfee').toggle();
        $('#btnaddfee').toggle();
        $scope.fees.Amount = '';
        $scope.fees.Date = today; 
    }
    $scope.getPrintModalClass = function() {
        var x = $scope.printModal;
        if(x){
            x.total = Number(x.admissionFee) + Number(x.cautionDeposit) + Number(x.tutionFee) + Number(x.hostelExpenses) + Number(x.others);
            return x.total != x.paidFees ? 'printStudentBillRed' : 'printStudentBillBlue';
        }
    }
    $scope.print = function(){
        $scope.wait = true;
        $scope.printModal.generatedFor = $scope.student.studentId
        $http.post('/Expense/printBill', $scope.printModal)
        .then(function(result){ 
            $scope.wait = false;
            if(result.data.res){
                $scope.getFees($scope.student.studentId,$scope.ayselected.academicYear)
                window.open(window.location.href.split('/')[0]+'/Bills/'+result.data.file,'_blank');
            }
            else
                alert(result.data.status +' '+result.data.errors);
            
        },function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
    $scope.showBill = function(id){
        $scope.wait=true;
        $http.post('/Expense/getBill', id)
        .then(function(result){ 
            $scope.wait = false;
            if(result.data.res)
                window.open(window.location.href.split('/')[0]+'/Bills/'+result.data.file,'_blank');
            else
                alert(result.data.status +' '+result.data.err);
            
        },function(error){
            $scope.wait = false;
            alert(error.statusText);
        });
    }
    $scope.getActiveClass = function(res){
        return res=='True' ? 'fa fa-circle-o activeUser' : 'fa fa-circle-o inactiveUser';
    }
    $scope.changeListActive = function(){
        var loaded = false;
        $.each($scope.StudentFullList,function(i,v){
            if(v.studentName.indexOf('False')>-1)
                loaded=true;
        });
        $scope.wait = true;
        if(loaded){
            FullList =[];
            if($('#chkActive').is(':checked')){
                $.each($scope.StudentList,function(i,v){
                    if(v.studentName.indexOf('False')==-1)
                        FullList.push(v);
                });
                $scope.StudentList = FullList;
            }
            else{
                $scope.StudentList = $scope.StudentFullList;
                FullList = $scope.StudentFullList;
            }
            $scope.wait = false;
        }
        else
        {
            $http.get('/Admin/getStudentList?activeOnly='+$('#chkActive').is(':checked'))
                .then(function (result) {
                    FullList = [];
                    var x = filterUnique(result.data);
                    $scope.StudentFullList = x;
                    $scope.StudentList = $scope.StudentFullList;
                    FullList = $scope.StudentFullList;
                    $scope.wait = false;
                }, function (error) {
                    alert(error.statusText);
                    $scope.wait = false;
            });
        }
    }
}]);