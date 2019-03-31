
var adminmodule = angular.module("app-mps",[])

adminmodule.controller("student-edit", ['$scope', '$http', function ($scope, $http) {
    var FullList = [];
    $scope.wait = true;
    $scope.loaderMsg = 'Loading...';
    $http.get('/Admin/LoadEditStudent')
        .then(function (result) {
            var x = filterUnique(result.data.students);
            FullList = x;
            $scope.StudentList = FullList;
            $scope.grades = result.data.grades;
            $scope.totalfees = result.data.totalfees;
            $scope.slabs = result.data.slabs;
           // $scope.slabs.splice(0, 0, { "Id": "0", "Name": "Select" });
            $scope.wait = false;
        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
    });
    $scope.changed = function () {
        $scope.StudentList = $scope.searchtext ? performfilter($scope.searchtext) : FullList;
    }
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
        if(Number(g1) > Number(g2)){return true; }
        else{return false;}
    }
    $scope.getStudent = function (id) {
        $scope.wait = true;
        $('#picImg').attr('src', '/images/personPlaceholder.jpg');
        $('#picImg').attr('style', 'border: 1px solid #CCC;');
        $scope.loaderMsg = 'Getting student...';
        $scope.student = $http.get('/Admin/getStudent', { params: { id: id } })
        .then(function (result) {
            $scope.student = result.data;
            if(result.data.isActive)
            {
                $('#isActive').attr('style', 'background-color:green;outline:5px solid green');
            }
            else
            {
                $('#isActive').attr('style', 'background-color:yellow;outline:5px solid yellow');
            }
            if(result.data.studentPic){
                // $http.post('/Expense/downloadAttachment', result.data.studentPic)
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
                document.querySelector('#picImg').setAttribute('src', result.data.studentPic.split('~')[1]);
            }
            if(result.data.aadhaarCard){
                document.querySelector('#aadhaarPic').setAttribute('src', result.data.aadhaarCard.split('~')[1]);
            }
            if(result.data.fatherAadhaarCard){
                document.querySelector('#fatherAadhaarPic').setAttribute('src', result.data.fatherAadhaarCard.split('~')[1]);
            }
            if(result.data.motherAadhaarCard){
                document.querySelector('#motherAadhaarPic').setAttribute('src', result.data.motherAadhaarCard.split('~')[1]);
            }
            if(result.data.transferCert){
                document.querySelector('#transferCertPic').setAttribute('src', result.data.transferCert.split('~')[1]);
            }
            $scope.wait = false;
        }, function (error) {
            $scope.wait = false;
            alert(error.statusText);
        });;
    }

    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return FullList.filter(function (student) {
            var n =  student.studentName.split('-').splice(1,3).join("");
            return n.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
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
    $scope.updateStudent = function () {
        $scope.wait = true;
        $scope.loaderMsg = 'Updating student details, please wait...';
        $scope.student.totalFee = $('#txtFee').val();
        var ad = new Date(Number($('#admissionDate').val().split('-')[2]),Number($('#admissionDate').val().split('-')[1])-1,Number($('#admissionDate').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.student.admissionDate = ad;
        var dob = new Date(Number($('#dob').val().split('-')[2]),Number($('#dob').val().split('-')[1])-1,Number($('#dob').val().split('-')[0])+1,0,0,0).toUTCString();
        $scope.student.dob = dob;
        $scope.student.password="na";
        readFiles(update)
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
        },1000);
    }
    var update = function(){
        $http.post('/Admin/UpdateStudent', $scope.student)
            .then(function(result){
                if(result.data.res){
                    $scope.wait = false;
                    alert(result.data.status);
                    window.location.reload(true);
                }
                else
                {
                    $scope.wait = false;
                    alert(result.data.status);
                }
            },function(error){
                $scope.wait = false;
                alert(error.statusText);
                $('#admissionDate').val('');
                $('#dob').val('');
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
