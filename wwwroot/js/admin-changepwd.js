var adminmodule = angular.module("app-mps",[])

adminmodule.controller("admin-changepwd",['$scope', '$http', function ($scope, $http) {
    var StudentFullList = [];
    $scope.wait = true;
    $http.get('/Admin/LoadStudentsPwd')
        .then(function (result) {
            StudentFullList = result.data.students;
            $scope.StudentList = StudentFullList;
            $scope.wait = false;
        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
    });
    var StaffFullList = [];
    $scope.wait = true;
    $http.get('/Admin/LoadStaffPwd')
        .then(function (result) {
            StaffFullList = result.data.staffList;
            $scope.StaffList = StaffFullList;
            $scope.wait = false;
    }, function (error) {
        alert(error.statusText);
        $scope.wait = false;
    });
    $scope.changedStudent = function () {
        $scope.StudentList = $scope.searchTextStudent ? performfilterstudent($scope.searchTextStudent) : StudentFullList;
    }
    $scope.changedStaff = function () {
        $scope.StaffList = $scope.searchTextStaff ? performfilterstaff($scope.searchTextStaff) : StaffFullList;
    }
    performfilterstudent = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return StudentFullList.filter(function (student) {
            return student.studentName.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    performfilterstaff = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return StaffFullList.filter(function (staff) {
            return staff.staffName.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.getUser = function (id,name) {
        var user ={
            'id':id,
            'name':name.split('-')[1],
            'userName' : name.split('-')[3],
            'oldPassword':''
        }
        $scope.user=user;
        
    }

    $scope.oldPwdChange = function(){
        if($('#btnVerify').hasClass('btn-success')){
            $('#btnVerify').addClass('btn-info');
            $('#btnVerify').removeClass('btn-success');
            $('#btnVerify').html('Verify');
        }
        if($scope.user.oldPassword.length>3){
            $('#btnVerify').prop('disabled',false);
        }
        else{
            $('#btnVerify').prop('disabled',true);
        }
    }
    $scope.verify = function(){
        $scope.wait = true;
        var obj = {'userName':$scope.user.userName,'oldPassword':$scope.user.oldPassword};
        $http.post('/Admin/verifyUserPassword',obj)
        .then(function (result) {
          if(result.data){
            $('.js-newpwd').show();
            //$('.js-oldpwd').hide();
            $('#btnVerify').removeClass('btn-info');
            $('#btnVerify').addClass('btn-success');
            $('#btnVerify').html('<i class="fa fa-check"><i> Verified');
            $('#btnVerify').prop('disabled',true);
            $('#oldPassword').attr('readonly',true);
          }
          else{
            $('.js-newpwd').hide();
            alert('Wrong password!\nPlease retry');
            $scope.user.oldPassword='';
            $('#btnVerify').prop('disabled',true);
            $('#btnVerify').addClass('btn-info');
            $('#btnVerify').removeClass('btn-success');
            $('#btnVerify').html('Verify');
          }
           $scope.wait = false;
        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
        });
    }

    $scope.passwordChange = function(){
        var c = $scope.confirmPassword;
        var n = $scope.newPassword;
        if(c==n){
            $('#newPassword').attr('style','border:1px solid green');
            $('#confirmPassword').attr('style','border:1px solid green');
        }
        else{
            $('#newPassword').attr('style','border:1px solid red');
            $('#confirmPassword').attr('style','border:1px solid red');
            $scope.frmChangePwd.$invalid = true;
        }
    }
    $scope.changePassword = function(){
        $scope.wait = true;
        var obj = {'userName':$scope.user.userName,'oldPassword':$scope.user.oldPassword,'newPassword':$scope.newPassword};
        $http.post('/Admin/changePassword',obj)
        .then(function(result){
            $scope.wait = false;
            if(result.data.res){
                alert(result.data.status);
            }
            else{
                alert(result.data.status+'\n'+result.data.errors);
            }
            window.location.reload(true);
        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
        });
    }
    $scope.getActiveClass = function(res){
        return res=='True' ? 'fa fa-circle-o activeUser' : 'fa fa-circle-o inactiveUser';
    }
}]);