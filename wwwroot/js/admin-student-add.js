var adminmodule = angular.module("app-mps",[])

adminmodule.controller("student-add", ['$scope', '$http', function ($scope, $http) {
    $scope.wait = true;
    $scope.loaderMsg = 'Loading...';
    $http.get('/Admin/LoadAddStudent')
        .then(function (result) {
            var stu = { gender : 'Male', 
                admissionDate : '',
                dob:'',
                password:'P@ssw0rd',
                rollNumber:result.data.rollNumber
            };
            $scope.grades = result.data.grades;
            $scope.totalfees = result.data.totalfees;
            $scope.slabs = result.data.slabs;
            $scope.academicYear = result.data.academicYear;
            $scope.student = stu;
            $scope.wait = false;

        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
    });
    $scope.getFees = function () {
        // alert($scope.student.Grade + ' ' + $scope.student.SlabName);
        $http.get('/Admin/getFee', { params: { slabName: $scope.student.slabName, grade: $scope.student.grade } })
        .then(function (result) {
            // $("#cmbFee").prop("disabled", false);
            if(result.data!=0)
                $('#txtFee').val(result.data);
        }, function (error) {
            alert(error.statusText);
        });;
    }
    $scope.addStudent = function () {
        $scope.wait = true;
        $scope.loaderMsg = 'Creating new student, please wait...';
        $scope.student.totalFee = $('#txtFee').val();
        var ad = new Date(Number($('#admissionDate').val().split('-')[2]),Number($('#admissionDate').val().split('-')[1])-1,Number($('#admissionDate').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.student.admissionDate = ad;
        var dob = new Date(Number($('#dob').val().split('-')[2]),Number($('#dob').val().split('-')[1])-1,Number($('#dob').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.student.dob = dob;
       // $scope.student.password="na";
       readFiles(save);
    }
    var readFiles = function(callback){
        var f1 = document.getElementById('filePic').files[0];
        var f2 = document.getElementById('aadhaarCard').files[0];
        var f3 = document.getElementById('fatherAadhaarCard').files[0];
        var f4 = document.getElementById('motherAadhaarCard').files[0];
        var f5 = document.getElementById('transferCert').files[0];
        r1 = new FileReader();
        r2 = new FileReader();
        r3 = new FileReader();
        r4 = new FileReader();
        r5 = new FileReader();
        if (f1) {
            r1.addEventListener("load", function () {
                $scope.student.studentPic = f1.name+"~"+r1.result;    
            }, false);
            r1.readAsDataURL(f1);
        }
        if (f2) {
            r2.addEventListener("load", function () {
                $scope.student.aadhaarCard = f2.name+"~"+r2.result;
            }, false);
            r2.readAsDataURL(f2);
        }

        if (f3) {
            r3.addEventListener("load", function () {
                $scope.student.fatherAadhaarCard = f3.name+"~"+r3.result;
            }, false);
            r3.readAsDataURL(f3);
        }
        if (f4) {
            r4.addEventListener("load", function () {
                $scope.student.motherAadhaarCard = f4.name+"~"+r4.result;
            }, false);
            r4.readAsDataURL(f4);
        }
        if (f5) {
            r5.addEventListener("load", function () {
                $scope.student.transferCert = f5.name+"~"+r5.result;
            }, false);
            r5.readAsDataURL(f5);
        }
        setTimeout(function(){
            callback();
        },1010);
    }

    var save = function(){
        $http.post('/Admin/Students', $scope.student)
        .then(function(result){
            if(result.data.res){
                $scope.wait = false;
                alert(result.data.status);
                window.location.href='/Home/Index';
            }
            else
            {
                $scope.wait = false;
                alert(result.data.status +'\n'+result.data.errors.split('-').join('\n'));
                $('#admissionDate').val('');
                $('#dob').val('');
            }
        },function(error){
            alert(error.statusText);
            $('#admissionDate').val('');
            $('#dob').val('');
        });
    }
    
    $scope.makeUserName = function(){
        try{
            
            var s1 = $scope.student.firstName.length>2?$scope.student.firstName.substr(0,4).toLocaleLowerCase():$scope.student.firstName.toLocaleLowerCase();
            var s2 = $scope.student.lastName.length>2?$scope.student.lastName.substr(0,4).toLocaleLowerCase():$scope.student.lastName.toLocaleLowerCase();
            //check if username exists
            
        }
        catch(e)
        {

        }
        if(s1 && s2)
            $scope.student.userName = s1 + '_'+ s2;
    }
    checkUserName = function(){
        $http.get('/Auth/checkUserName?n='+$scope.student.userName).then(function(result){
            if(!res){
                
            }
        },function(error){
            alert(error.statusText);
        });
    }

}]);