var adminmodule = angular.module("app-mps",[])

adminmodule.controller("staff-add", ['$scope', '$http', function ($scope, $http) {
    $scope.wait = true;
    $scope.loaderMsg = 'Loading...';
    $http.get('/Admin/LoadAddStaff')
        .then(function (result) {
            var deArray =[];
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
            var sta = { gender : 'Male', 
                joiningDate : '',
                dob:'',
                password:'P@ssw0rd',
                //isTeacher:true,
                deductions:deArray,
                salary:0,
                staffRole : 'Staff'
            };
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
            $scope.staff=sta;
            $scope.isTeacherChange();
            $scope.wait = false;
           
        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
    });
    $scope.isTeacherChange = function() {
        if ($scope.staff.isTeacher) {
            $('#isTeacher').attr('style', 'background-color:green;outline:5px solid green');
            $('#lblisTeacher').text("IS TEACHER");
            $scope.staff.staffRole = 'Teacher';
        }
        else {
            $('#isTeacher').attr('style', 'background-color:yellow;outline:5px solid yellow');
            $('#lblisTeacher').text("NOT TEACHER");
            $scope.staff.staffRole = 'Staff';
        }
            
    }
    $scope.salaryChange = function(){
        var d = $scope.staff.deductions;
        var s = $scope.txtSalary;
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
                        if(s>30000 && v.name.indexOf('Provident fund')>-1){
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
                        if(s<30000 && v.name.indexOf('Provident fund')>-1){
                            v.display=v.value+(v.type=='A'?' Rs.':v.type)
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
        $scope.staff.deductions = tempd;
        $scope.staff.salary = final;
    }
    $scope.addStaff = function(){
        $scope.wait = true;
        $scope.loaderMsg = 'Creating new staff, please wait...';
        var jd = new Date(Number($('#joiningDate').val().split('-')[2]),Number($('#joiningDate').val().split('-')[1])-1,Number($('#joiningDate').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.staff.joiningDate = jd;
        var dob = new Date(Number($('#dob').val().split('-')[2]),Number($('#dob').val().split('-')[1])-1,Number($('#dob').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.staff.dob = dob;
        var deArray=[];
        var indices=[];
        $.each($('[id^="chkd_"]'),function(i,v){
            if($(this).is(':checked'))
                indices.push(v.id.split('_')[1]);
        });
        $.each($scope.staff.deductions,function(i,v){
            if($.inArray(v.index.toString(),indices)>-1){
                deArray.push(v);
            }
        });
        $scope.staff.deductions=deArray;
        // console.log($scope.staff.joiningDate);
        // console.log($scope.staff.dob);
        readFiles(save);
    }
    var readFiles = function(callback){
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
    var save = function(){
        $http.post('/Admin/Staff', $scope.staff)
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
                $('#joiningDate').val('');
                $('#dob').val('');
            }
        },function(error){
            alert(error.statusText);
            $('#joiningDate').val('');
            $('#dob').val('');
        });
    }
    $scope.makeUserName = function(){
        try{
            var s1 = $scope.staff.firstName.length>2?$scope.staff.firstName.substr(0,4):$scope.staff.firstName.toLocaleLowerCase();;
            var s2 = $scope.staff.lastName.length>2?$scope.staff.lastName.substr(0,4):$scope.staff.lastName.toLocaleLowerCase();;
        }
        catch(e)
        {

        }
        if(s1 && s2)
            $scope.staff.userName = s1 + '_'+ s2;
    }
}]);