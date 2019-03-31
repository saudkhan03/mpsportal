$(document).ready(function(){
    var userName = $('#username').text().trim();
    if(userName)
    {
        $('#pnlHead').text('Logged in as '+userName);
        var u = $('#getUserDetailsUrl').data('request-url');
        $.post(u,{username:userName},function(res){
            $('.navbar-default > .container > .navbar-collapse > .nav').attr('style','display:block');
            $('.fa-spin').hide();
            $('#div1').html('<div class="head"><b>Hello '+res.user.firstName+' '+res.user.lastName+' </b></div>');
          //  var d2 = 'Name: '+res.user.firstName+' '+res.user.lastName+'<br>' ;
            var d2 = '<div class="data"> Email: '+res.user.email+'</div><br>';
            d2 += '<div class="data">DOB: '+res.user.dob.split('T')[0]+'</div><br>';
            d2 += '<div class="data">Address: '+res.user.address1+'</div><br>';
            if(res.imgUrl==''){
                d2 += '<div style="width:100px;height:100px;border:1px inset #CCC;position: absolute;top: 2px;left: 339px;"></div>';
            }
            else{
                d2 += '<img style="width:100px;height:100px;position: absolute;top: 2px;left: 339px;" src="'+res.imgUrl+'"></img>';
            }
            $('#div1').append(d2);

            var  r = res.role==''?'NA':res.role;
            $('#hidUserRole').val(r);
            var g2='';
            if(r=='Student'){
                g2 = '<div class="data">From class <b>'+res.student.slab.grade+'</b></div>';
                g2 += '<div class="data">for the year <b>'+res.student.academicYear+'</b></div>';
                g2 += '<div class="data">in <b>'+res.student.slab.slabName+'</b></div>';
            }
            if(r=='Staff' || r=='Teacher'){ 
                g2 = '<div class="data">You have the permission of a <b>'+r+'</b></div>';
            }
            $('#div2').html(g2);
            if(r=='Student'){
                var u = $('#getStudentDetailsUrl').data('request-url');
                $('.js-student').show();
                $('.js-loadStudent').show();
                $.post(u,{username:$('#username').text().trim()},function(res){
                    $('.js-loadStudent').hide();
                    console.log(res);
                    if(res.length==0){
                        $('#indexMarks').html("<div class='text-info'>No data found for student</div>")
                    }
                    else{
                        var matrix = '<table class="table table-bordered" style="color:#092A72">';
                        matrix += '<tr style="text-align:center">';
                        matrix += '<th>Subject</th>';
                        matrix += '<th>First Mid Term Examination (20)</th>';
                        matrix += '<th>Second Mid Term Examination (20)</th>';
                        matrix += '<th>First Terminal Examination (60)</th>';
                        matrix += '<th>Third Mid Term Examination (20)</th>';
                        matrix += '<th>Fourth Mid Term Examination (20)</th>';
                        matrix += '<th>Second Terminal Examination (60)</th>';
                        matrix += '</tr>';
                        $.each(res,function(i,v){
                            if(v.type=="Marks" || v.type=="Grade"){
                                matrix += '<tr>';
                                matrix += '<td>'+v.subject+'</td>';
                                matrix += '<td>'+v.exam1+'</td>';
                                matrix += '<td>'+v.exam2+'</td>';
                                matrix += '<td>'+v.exam3+'</td>';
                                matrix += '<td>'+v.exam4+'</td>';
                                matrix += '<td>'+v.exam5+'</td>';
                                matrix += '<td>'+v.exam6+'</td>';
                                matrix += '</tr>';
                            }
                        });
                        matrix +='</table>';
                        $('#indexMarks').html(matrix);
                    }
                });
            }
            //showRolesMenu();
        });
    }
});