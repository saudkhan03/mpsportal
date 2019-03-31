var adminmodule = angular.module("app-mps",[])

adminmodule.controller("admin-sendmail", ['$scope', '$http', function ($scope, $http) {
    var StudentFullList = [];
    $scope.wait = true;
    $http.get('/Admin/LoadStudentsMail')
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
    $http.get('/Admin/LoadStaffMail')
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
            'email':id,
            'name':name.split(' - ')[0],
            'userName' : name.split(' - ')[1],
            'oldPassword':''
        }
        $scope.user=user;
    }
    
}]);