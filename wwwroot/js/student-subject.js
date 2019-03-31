var adminmodule = angular.module("app-mps",[])

adminmodule.controller("student-subject", ['$scope', '$http', function ($scope, $http) {
    var SubjectFullList = [];
    $scope.wait = true;
    $http.get('/Students/getSubjectList')
        .then(function (result) {
            $scope.SubjectFullList = result.data;
            $scope.SubjectList = $scope.SubjectFullList;
            $scope.Type="0";
            $scope.subjectButtonText='Add Subject';
            var ex = {
                'id':'',
                'active':true
            };
            $scope.subject = ex;
            $scope.wait = false;
        }, function (error) {
            alert(error.statusText);
            $scope.wait = false;
    });
    $scope.changed = function () {
        $scope.SubjectList = $scope.searchtext ? performfilter($scope.searchtext) : SubjectFullList;
    }
    performfilter = function (searchtext) {
        filterby = searchtext.toLocaleLowerCase();
        return SubjectFullList.filter(function (subject) {
            var n =  subject.AcademicEntityName;
            return n.toLocaleLowerCase().indexOf(filterby) != -1;
        });
    }
    $scope.getSubject= function(i,n,t,v,g,a){
        var type,value;
        switch(t){
            case "Subject": type="0"; break;
            case "Attendance": type="1"; break;
            case "Text": type="2"; break;
            case "Value": type="3"; break;
            case "Character_Social": type="4";break;
            case "Character_Residential": type="5";break;
        }
        switch(v){
            case "Marks": value="0"; break;
            case "Grade": value="1"; break;
            case "Attendance": value="2"; break;
            case "Text": value="3"; break;
            case "Value": value="4"; break;
        }
        var ex = {
            'id':i,
            'academicEntityName':n,
            'academicEntityType':type,
            'academicEntityValueType':value,
            'academicEntityGrade':g,
            'active':a
        };
        $scope.subject = ex;
        $scope.subjectButtonText = 'Update Subject';
        if(type=="1" || type=="4" || type=="5"){
            $('#ValueList').attr('disabled','disabled');
        }
        else
        $('#ValueList').removeAttr('disabled');
    }
    $scope.clearSubjectForm = function(){
        $scope.subject.id = '';
        $scope.subject.academicEntityName = '';
        $scope.subject.academicEntityType = '';
        $scope.subject.academicEntityValueType = '';
        $scope.subject.academicEntityGrade='';
        $scope.subjectButtonText = 'Add Subject';
    }
    $scope.getActiveClass = function(res){
        return res ? 'fa fa-circle-o activesubject' : 'fa fa-circle-o inactiveUser';
    }
    $scope.updateSubject = function(){
        var type,value;
        switch($scope.subject.academicEntityType){
            case "0": type="Subject"; break;
            case "1": type="Attendance"; break;
            case "2": type="Text"; break;
            case "3": type="Value"; break;
            case "4": type="Character_Social";break;
            case "5": type="Character_Residential";break;
        }
        switch($scope.subject.academicEntityValueType){
            case "0": value="Marks"; break;
            case "1": value="Grade"; break;
            case "2": value="Attendance"; break;
            case "3": value="Text"; break;
            case "4": value="Value"; break;
        }
        
        $scope.subject.academicEntityType = type;
        $scope.subject.academicEntityValueType =value;
        //console.log($scope.subject);
        if($scope.subject.id){
            $http.post('/Students/UpdateSubject', $scope.subject)
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
            });
        }
        else{
            $http.post('/Students/SaveSubject', $scope.subject)
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
            });
        }
    }
    $scope.changeTypeDropDown = function(){
        if($scope.subject.academicEntityType=="4" || $scope.subject.academicEntityType == "5"){
            $scope.subject.academicEntityValueType = "1";
            $('#ValueList').attr('disabled','disabled')
        }else if($scope.subject.academicEntityType=="1" ){
            $scope.subject.academicEntityValueType = "2";
            $('#ValueList').attr('disabled','disabled')
        }
        else{
            
                $scope.subject.academicEntityValueType = '';
                $('#ValueList').removeAttr('disabled');
        }
    }
}]);