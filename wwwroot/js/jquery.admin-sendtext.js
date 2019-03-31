$(document).ready(function(){
    $.get('/Admin/GetRemaining',function(result){
        $('.loader').hide();
        $('.js-remaining').html('<b>'+result+'</b>');
    });

    var u = '/Admin/LoadStudentsPhoneNos';
    $("#gridStudents").jqGrid({
        //datatype: "local",
        //data: myData,
        url: u,
        datatype: "json",
        colNames: ['StudentId','Active','Name','Roll No.','Class'],
        //colNames: ['id', 'Email', 'First Name', 'Last Name', 'Phone', 'Veri E', 'Veri P'],
        colModel: [
                    { name: 'studentId', index: 'studentId', hidden:true, align: 'center'},
                    { name: 'isActive', index: 'isActive', width: 40, align: 'center',formatter: 'checkbox',edittype: 'checkbox', editoptions: {value: 'true:false', defaultValue: 'true'}},
                    { name: 'studentName', index: 'studentName', width: 120, align: 'center'},
                    { name: 'rollNumber', index: 'rollNumber', width: 70, align: 'center'},
                    { name: 'grade', index: 'grade', width: 120, align: 'center'}
                   ],
        cmTemplate: { resizable: false },
        loadonce: true,
        rowNum: 5000,
        //rowList: [50, 70, 90],
        //pager: '#pagerStudents',
        sortname: 'Name',
        viewrecords: true,
        sortorder: "asc",
       // width:1024,`
        multiselect:true,
        cmTemplate: { resizable: true },
        //ondblClickRow: function (rowId) {
        //    var rowData = jQuery(this).getRowData(rowId);
        //    var id = rowData['userId'];
        //    showAccount(id);
        //},
        loadComplete: function (data) {
            var  x = data;
        }
    });

    var cDiv = $('#gridStudents')[0].grid.cDiv;
    //cDiv = mygrid[0].grid.cDiv;
    $('#gridStudents').setCaption("");
    $("a.ui-jqgrid-titlebar-close", cDiv).unbind();
    $(cDiv).hide();
    //$("#gridStudents").jqGrid('navGrid', '#pager', { edit: false, add: false, del: false });
    $("#gridStudents").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    var u = '/Admin/LoadStaffPhoneNos';
    $("#gridStaff").jqGrid({
        //datatype: "local",
        //data: myData,
        url: u,
        datatype: "json",
        colNames: ['StaffId','Active','Name','Joined on','T/S'],
        //colNames: ['id', 'Email', 'First Name', 'Last Name', 'Phone', 'Veri E', 'Veri P'],
        colModel: [
                    { name: 'staffId', index: 'staffId', hidden:true, align: 'center'},
                    { name: 'isActive', index: 'isActive', width: 40, align: 'center',formatter: 'checkbox',edittype: 'checkbox', editoptions: {value: 'true:false', defaultValue: 'true'}},
                    { name: 'staffName', index: 'staffName', width: 120, align: 'center'},
                    { name: 'joiningDate', index: 'joiningDate', width: 100, align: 'center', sorttype: 'date', formatter: 'date', formatoptions: {newformat: 'd-M-Y'}, datefmt: 'd-M-Y'},
                    { name: 'isTeacher', index: 'isTeacher', width: 70, align: 'center'}
                   ],
        cmTemplate: { resizable: false },
        loadonce: true,
        rowNum: 5000,
        //rowList: [50, 70, 90],
        //pager: '#pagerStudents',
        sortname: 'Name',
        viewrecords: true,
        sortorder: "asc",
       // width:1024,
        multiselect:true,
        cmTemplate: { resizable: true },
        //ondblClickRow: function (rowId) {
        //    var rowData = jQuery(this).getRowData(rowId);
        //    var id = rowData['userId'];
        //    showAccount(id);
        //},
        loadComplete: function (data) {
            var  x = data;
        }
    });

    var cDiv = $('#gridStaff')[0].grid.cDiv;
    //cDiv = mygrid[0].grid.cDiv;
    $('#gridStaff').setCaption("");
    $("a.ui-jqgrid-titlebar-close", cDiv).unbind();
    $(cDiv).hide();
    //$("#gridStudents").jqGrid('navGrid', '#pager', { edit: false, add: false, del: false });
    $("#gridStaff").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    $('.js-addMails').click(function(){
        var grid = '';
        var emails =[];
        if($('#students').hasClass('active'))
            grid = '#gridStudents';
        else
            grid = '#gridStaff';
        $(grid +' input:checked').each(function(){
            if(!$(this).attr('disabled'))
            emails.push($(this).closest('tr > td').next().html());
        });
        var es='';
        $.each(emails,function(i,v){
            es +=v+';';
        });
        $('#txtTo').val(es.substr(0,es.length-1));
        validateSend();
        //console.log(es);
    });
    $("#txtTo").keyup(function(){
        var b = validateText($("#txtTo").val());
        if(!b){
            $(this).attr('style','border:1px solid red');
            $('#btnSend').attr('disabled',true);
        }
        else{
            $(this).attr('style','border:1px solid #ccc');
            if($('#txtTo').val().length>2 && $('#txtBody').val().length>0)
                $('#btnSend').attr('disabled',false);
        }
        validateSend();
    });
    function validateText(numbers) {
        var x = numbers.split(';');
        var res = true;
        if(x.length>1)
        {
            $.each(x,function(i,v){
                if(v.length!=10)
                    res=false;
            });
        }
        return res;
    }
    $('#btnSend').click(function(){
        var toemails = $('#txtTo').val().split(';');
        //var ccemails = $('#txtCC').val();
        //var sub = $('#txtSubject').val();
        var message = $('#txtBody').val();
        $('.loader').show();
        $.post('/Admin/SendText',{to:toemails, message:message},function(result){
            $('.loader').hide();
            if(result.res){
                alert('Message sent.');
                window.location.href='/Home/Index';
            }
            else
            {
                alert('Could not send text'+result.exep);
            }
        });
    });
    
    $('#txtBody').keyup(function(){
        validateSend();
    });
});
function  validateSend(){
    if($('#txtTo').val().length>0 && $('#txtTo').attr('style').indexOf('border:1px solid red')<0){
        if($('#txtBody').val().length>0){
            $('#btnSend').attr('disabled',false);
        }
    }
}