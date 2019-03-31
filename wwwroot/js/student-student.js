
var adminmodule = angular.module("app-mps",[])

adminmodule.controller("student-student", ['$scope', '$http', function ($scope, $http) {
    var FullList = [];
    var StudentFullList = [];
    $scope.wait = true;
    $http.get('/Students/getStudentList?activeOnly='+true)
        .then(function (result) {
            FullList = result.data;
            $scope.StudentFullList = FullList;
            $scope.StudentList = $scope.StudentFullList;
            $scope.Type="0";
            $scope.wait = false;
        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
    });

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
    $scope.getStudent = function (id, grade) {
        $('#hidGrade').val(grade);
        $scope.wait = true;
        $scope.selectedStudentId = id;
        $scope.selectedStudentGrade = grade;
        $scope.student = $http.get('/Students/getStudentMatrix', { params: { id: id, grade: grade } })
        .then(function (result) {
            $scope.studentMatrix = result.data;
            $scope.studentMatrix[0].studentId = id;
            var remarks=[];
            var attendance=[];
            var i=0;
            for(i=0;i<$scope.studentMatrix.length;i++){
                var v = $scope.studentMatrix[i];
                if(v.type=='Attendance'){
                    attendance.push(v.exam1);
                    attendance.push(v.exam2);
                    attendance.push(v.exam3);
                    attendance.push(v.exam4);
                    attendance.push(v.exam5);
                    attendance.push(v.exam6);
                }
                if(v.type=='Text' && v.subject=='Remarks'){
                    remarks.push(v.exam1);
                    remarks.push(v.exam2);
                    remarks.push(v.exam3);
                    remarks.push(v.exam4);
                    remarks.push(v.exam5);
                    remarks.push(v.exam6);
                }
            }
            $scope.attendanceArray = attendance;
            $scope.remarksArray = remarks;
            $scope.getTotals();
            $scope.wait = false;
        }, function (error) {
            $scope.wait = false;
            alert(error.statusText);
        });
        generateCharacterCert(id, grade);
    }
    generateCharacterCert = function(id,grade){
        $scope.student = $http.get('/Students/getStudentCharacterCert', { params: { id: id, grade: grade } })
        .then(function (result) {
            //$scope.characterMatrix = result.data;
            var social = [], residential=[];
            $.each(result.data,function(i,v){
                if(v.Character_Social)
                    social.push(v.Character_Social);
                if(v.Character_Residential)
                    residential.push(v.Character_Residential);
            });
            var char = [];
            for(var i=0;i<social.length;i++){
                var c ={
                    "social":social[i],
                    "sg":"-",
                    "residential":residential[i],
                    "rg":"-"
                };
                char.push(c);
            }
            $scope.characterMatrix = char;
            $scope.promoted = "PROMOTED";
            //$scope.wait = false;
        }, function (error) {
            //$scope.wait = false;
            alert(error.statusText);
        });
    }
    $scope.getTotals = function(){
        $scope.exam1Tot=0,$scope.exam2Tot=0,$scope.exam3Tot=0;
        $scope.exam4Tot=0,$scope.exam5Tot=0,$scope.exam6Tot=0;
        $scope.exam1Per=0,$scope.exam2Per=0,$scope.exam3Per=0;
        $scope.exam4Per=0,$scope.exam5Per=0,$scope.exam6Per=0;
        $scope.Tot1=0,$scope.Tot2=0,$scope.Tot3=0,$scope.finalAvg=0;
        $scope.perTot1=0,$scope.perTot2=0,$scope.perTot3=0,$scope.perFinalAvg=0;
        // $.each($scope.studentMatrix,function(i,v){
        var i=0;
        for(i=0;i<$scope.studentMatrix.length;i++){
            var v = $scope.studentMatrix[i];
            $scope.exam1Tot +=isNaN(v.exam1)?0:Number(v.exam1);
            $scope.exam2Tot +=isNaN(v.exam2)?0:Number(v.exam2);
            $scope.exam3Tot +=isNaN(v.exam3)?0:Number(v.exam3);
            $scope.exam4Tot +=isNaN(v.exam4)?0:Number(v.exam4);
            $scope.exam5Tot +=isNaN(v.exam5)?0:Number(v.exam5);
            $scope.exam6Tot +=isNaN(v.exam6)?0:Number(v.exam6);
            if(!isNaN(v.exam1) && !isNaN(v.exam2) && !isNaN(v.exam3)){
                $scope.Tot1 += Number(v.exam1) + Number(v.exam2) + Number(v.exam3);
            }
            // else if(v.exam1.length==1 && v.exam2.length==1 && v.exam2.length==1){
            //     $scope.Tot1='A';
            // }
            if(!isNaN(v.exam4) && !isNaN(v.exam5) && !isNaN(v.exam6)){
                $scope.Tot2 += Number(v.exam4) + Number(v.exam5) + Number(v.exam6);
            }
            $scope.Tot3 = $scope.Tot1 + $scope.Tot2;
            $scope.finalAvg = Math.round($scope.Tot3 /2)
        }//);
        if(Number($('#hidGrade').val())<=6){
            $scope.exam1Per = (($scope.exam1Tot * 100) / 120).toFixed(1);
            $scope.exam2Per = (($scope.exam2Tot * 100) / 120).toFixed(1);
            $scope.exam3Per = (($scope.exam3Tot * 100) / 360).toFixed(1);
            $scope.exam4Per = (($scope.exam4Tot * 100) / 120).toFixed(1);
            $scope.exam5Per = (($scope.exam5Tot * 100) / 120).toFixed(1);
            $scope.exam6Per = (($scope.exam6Tot * 100) / 360).toFixed(1);
            $scope.perTot1 = (($scope.Tot1 * 100) / 600).toFixed(1);
            $scope.perTot2 = (($scope.Tot2 * 100) / 600).toFixed(1);
            $scope.perTot3 = (($scope.Tot3 * 100) / 1200).toFixed(1);
            $scope.perFinalAvg = (($scope.finalAvg * 100) / 600).toFixed(1);
        }
        if(Number($('#hidGrade').val())>6){
            $scope.exam1Per = (($scope.exam1Tot * 100) / 200).toFixed(1);
            $scope.exam2Per = (($scope.exam2Tot * 100) / 200).toFixed(1);
            $scope.exam3Per = (($scope.exam3Tot * 100) / 600).toFixed(1);
            $scope.exam4Per = (($scope.exam4Tot * 100) / 200).toFixed(1);
            $scope.exam5Per = (($scope.exam5Tot * 100) / 200).toFixed(1);
            $scope.exam6Per = (($scope.exam6Tot * 100) / 600).toFixed(1);
            $scope.perTot1 = (($scope.Tot1 * 100) / 1000).toFixed(1);
            $scope.perTot2 = (($scope.Tot2 * 100) / 1000).toFixed(1);
            $scope.perTot3 = (($scope.Tot3 * 100) / 2000).toFixed(1);
            $scope.perFinalAvg = (($scope.finalAvg * 100) / 1000).toFixed(1);
        }
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
            $http.get('/Students/getStudentList?activeOnly='+$('#chkActive').is(':checked'))
                .then(function (result) {
                    FullList = [];
                    $scope.StudentFullList = result.data;
                    $scope.StudentList = $scope.StudentFullList;
                    FullList = $scope.StudentFullList;
                    $scope.wait = false;
                }, function (error) {
                    alert(error.statusText);
                    $scope.wait = false;
            });
        }
    }
    $scope.updateMatrix = function(){
        $scope.wait = true;
        for(i=0;i<$scope.studentMatrix.length;i++){
            if($scope.studentMatrix[i].type=='Attendance')
            {
                $scope.studentMatrix[i].exam1 = $scope.attendanceArray[0] ;
                $scope.studentMatrix[i].exam2 = $scope.attendanceArray[1] ;
                $scope.studentMatrix[i].exam3 = $scope.attendanceArray[2] ;
                $scope.studentMatrix[i].exam4 = $scope.attendanceArray[3] ;
                $scope.studentMatrix[i].exam5 = $scope.attendanceArray[4] ;
                $scope.studentMatrix[i].exam6 = $scope.attendanceArray[5] ;
            }
            if($scope.studentMatrix[i].type=='Text' && $scope.studentMatrix[i].subject=='Remarks'){
                $scope.studentMatrix[i].exam1 = $scope.remarksArray[0];
                $scope.studentMatrix[i].exam2 = $scope.remarksArray[1];
                $scope.studentMatrix[i].exam3 = $scope.remarksArray[2];
                $scope.studentMatrix[i].exam4 = $scope.remarksArray[3];
                $scope.studentMatrix[i].exam5 = $scope.remarksArray[4];
                $scope.studentMatrix[i].exam6 = $scope.remarksArray[5];
            }
        }
        var x= $scope.studentMatrix;
        $http.post('/Students/saveMatrix',x)
                .then(function (result) {
                    $scope.wait = false;
                    alert('Marks details were updated.');
                }, function (error) {
                    alert(error.statusText);
                    $scope.wait = false;
            });
    }
    $scope.getGrade=function(x){
        if(x.indexOf('-')>-1)
            return "-";
        else{
            if(x.indexOf('E')>-1)
                return "E";
            else if(x.indexOf('D')>-1)
                return "D";
            else if(x.indexOf('C')>-1)
                return "C";
            else if(x.indexOf('B')>-1)
                return "B";
            else if(x.indexOf('A')>-1)
                return "A";
        }
            
    }
    $scope.printMatrix = function(){
        $scope.wait = true;
        for(i=0;i<$scope.studentMatrix.length;i++){
            if($scope.studentMatrix[i].type=='Attendance')
            {
                $scope.studentMatrix[i].exam1 = $scope.attendanceArray[0] ;
                $scope.studentMatrix[i].exam2 = $scope.attendanceArray[1] ;
                $scope.studentMatrix[i].exam3 = $scope.attendanceArray[2] ;
                $scope.studentMatrix[i].exam4 = $scope.attendanceArray[3] ;
                $scope.studentMatrix[i].exam5 = $scope.attendanceArray[4] ;
                $scope.studentMatrix[i].exam6 = $scope.attendanceArray[5] ;
            }
            if($scope.studentMatrix[i].type=='Text' && $scope.studentMatrix[i].subject=='Remarks'){
                $scope.studentMatrix[i].exam1 = $scope.remarksArray[0];
                $scope.studentMatrix[i].exam2 = $scope.remarksArray[1];
                $scope.studentMatrix[i].exam3 = $scope.remarksArray[2];
                $scope.studentMatrix[i].exam4 = $scope.remarksArray[3];
                $scope.studentMatrix[i].exam5 = $scope.remarksArray[4];
                $scope.studentMatrix[i].exam6 = $scope.remarksArray[5];
            }
        }
        var obj= {
            'matrix': $scope.studentMatrix,
            'exam1Tot':$scope.exam1Tot,
            'exam2Tot':$scope.exam2Tot,
            'exam3Tot':$scope.exam3Tot,
            'exam4Tot':$scope.exam4Tot,
            'exam5Tot':$scope.exam5Tot,
            'exam6Tot':$scope.exam6Tot,
             'exam1Per':$scope.exam1Per,
             'exam2Per':$scope.exam2Per,
             'exam3Per':$scope.exam3Per,
             'exam4Per':$scope.exam4Per,
             'exam5Per':$scope.exam5Per,
             'exam6Per':$scope.exam6Per,
             'Tot1':$scope.Tot1,
             'Tot2':$scope.Tot2,
             'Tot3':$scope.Tot3,
             'perTot1':$scope.perTot1,
             'perTot2':$scope.perTot2,
             'perTot3':$scope.perTot3,
             'finalAvg':$scope.finalAvg,
             'perFinalAvg':$scope.perFinalAvg,
        }
        $http.post('/Students/printMatrix',obj)
                .then(function (result) {
                    $scope.wait = false;
                    if(result.data.res)
                        window.open(window.location.href.split('/')[0]+'/'+result.data.file,'_blank');
                    else
                        alert(result.data.status +' '+result.data.errors);
                }, function (error) {
                    alert(error.statusText);
                    $scope.wait = false;
            });
    }
    $scope.printCharacterCert = function(){
        console.log($scope.characterMatrix);
        var obj = {"promoted":$scope.promoted,"notready":$scope.notready,"characterMatrix":$scope.characterMatrix};
        $http.post('/Students/printCharacterCert',obj)
                .then(function (result) {
                    $scope.wait = false;
                    if(result.data.res)
                        window.open(window.location.href.split('/')[0]+'/'+result.data.file,'_blank');
                    else
                        alert(result.data.status +' '+result.data.errors);
                }, function (error) {
                    alert(error.statusText);
                    $scope.wait = false;
            });
    }
}]);