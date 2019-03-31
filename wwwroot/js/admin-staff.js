var adminmodule = angular.module("app-mps",[])

adminmodule.controller("staff-edit", ['$scope', '$http', function ($scope, $http) {
    var StaffFullList = [];
    $scope.wait = true;
    $scope.loaderMsg = 'Loading...';
    $http.get('/Admin/LoadEditStaff')
        .then(function (result) {
            StaffFullList = result.data.staffList;
            $scope.StaffList = StaffFullList;
            $scope.roles = [
                {
                    'id':'Staff',
                    'name':'Staff',
                },
                {
                    'id':'Teacher',
                    'name':'Teacher',
                },
                {
                    'id':'Admin',
                    'name':'Administrator',
                },
                {
                    'id' : "Accountant",
                    'name' : "Accountant",
                },
                {
                    'id': "PrimeStaff",
                    'name' : "PrimeStaff",
                }
            ]
            $scope.wait = false;
    }, function (error) {
        alert(error.statusText);
        $scope.wait = false;
    });
    $scope.changed = function () {
        $scope.StaffList = $scope.searchtext ? performfilter($scope.searchtext) : StaffFullList;
    }
    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return StaffFullList.filter(function (staff) {
            return staff.staffName.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.getStaff = function (id) {
        $scope.wait = true;
        $('#picImg').attr('src', '/images/personPlaceholder.jpg');
        $('#picImg').attr('style', 'border: 1px solid #CCC;');
        $scope.loaderMsg = 'Getting staff...';
        $http.get('/Admin/getStaff', { params: { id: id } })
        .then(function (result) {
            $scope.staff = result.data;
            if(result.data.isActive){
                $('#isActive').attr('style', 'background-color:green;outline:5px solid green');
            }
            else{
                $('#isActive').attr('style', 'background-color:yellow;outline:5px solid yellow');
            }
            // if(result.data.isTeacher){
            //     $('#isTeacher').attr('style', 'background-color:green;outline:5px solid green');
            //     $('#lblisTeacher').text("TEACHER");
            // }
            // else {
            //     $('#isTeacher').attr('style', 'background-color:yellow;outline:5px solid yellow');
            //     $('#lblisTeacher').text("NOT A TEACHER");
            // }
            if(result.data.staffPic){
                // $http.post('/Expense/downloadAttachment', result.data.staffPic)
                // .then(function(result){
                //     if(result.data.exception==''){
                //         $('#picImg').attr('src', result.data.url);
                //         $('#picImg').attr('style','width: 150px; max-height: 150px;');
                //     }
                //     else{
                //         alert('Error occured while downloading profile picture \n'+result.data.exception);
                //     }
                // },function(error){
                //     //
                // });
                $('#picImg').attr('style','width: 150px; max-height: 150px;');
                document.querySelector('#picImg').setAttribute('src', result.data.staffPic.split('~')[1]);
            }
            if(result.data.aadhaarCard){
                document.querySelector('#aadhaarPic').setAttribute('src', result.data.aadhaarCard.split('~')[1]);
            }
            $scope.wait = false;
        }, function (error) {
            $scope.wait = false;
            alert(error.statusText);
        });
    }

    $scope.updateStaff = function () {
        $scope.wait = true;
        $scope.loaderMsg = 'Updating staff details, please wait...';
        var jd = new Date(Number($('#joiningDate').val().split('-')[2]),Number($('#joiningDate').val().split('-')[1])-1,Number($('#joiningDate').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.staff.joiningDate = jd;
        var dob = new Date(Number($('#dob').val().split('-')[2]),Number($('#dob').val().split('-')[1])-1,Number($('#dob').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.staff.dob = dob;
        $scope.staff.password="na";
        $scope.staff.salary = 1000;
        readfiles(update);
    }
    var readfiles = function(callback){
        var f1 = document.getElementById('filePic').files[0];
        var f2 = document.getElementById('aadhaarCard').files[0];
        r1 = new FileReader();
        r2 = new FileReader();
        if (f1) {
            r1.addEventListener("load", function () {
                $scope.staff.staffPic = f1.name+"~"+r1.result;    
            }, false);
            r1.readAsDataURL(f1);
        }
        if (f2) {
            r2.addEventListener("load", function () {
                $scope.staff.aadhaarCard = f2.name+"~"+r2.result;
            }, false);
            r2.readAsDataURL(f2);
        }
        setTimeout(function(){
            callback();
        },1000);
    }
    var update = function(){
        $http.post('/Admin/UpdateStaff', $scope.staff)
            .then(function(result){
                $scope.wait = false;
                if(result.data.res){
                    alert(result.data.status);
                    window.location.reload(true);
                }
                else
                {
                    alert(result.data.status+result.data.err);
                    $('#joiningDate').val('');
                        $('#dob').val('');
                }
            },function(error){
                $scope.wait = false;
                alert(error.statusText);
                $('#joiningDate').val('');
                        $('#dob').val('');
            });
    }
    $scope.getActiveClass = function(res){
        return res=='True' ? 'fa fa-circle-o activeUser' : 'fa fa-circle-o inactiveUser';
    }
}]);
