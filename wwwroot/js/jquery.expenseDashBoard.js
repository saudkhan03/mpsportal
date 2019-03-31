
    $(document).ready(function () {
        var selectValues = [];
        var d = new Date().getFullYear();
        $('#months').val(new Date().getMonth()+1);
        $('#monthsConsolidated').val(new Date().getMonth()+1);
        selectValues.push(d-3,d-2,d-1,d);
        var output = [];
        $.each(selectValues, function(key, value)
        {
            if(value==d)
                output.push('<option value="'+ value +'" selected >'+ value +'</option>');
            else
                output.push('<option value="'+ value +'" >'+ value +'</option>');
        });

        $('#years').html(output.join(''));
        $('#yearsConsolidated').html(output.join(''));

        GetDashBoard(d);
        GetMonthlyDashBoard(Number($('#years').val()),Number($('#months').val()));
        GetConsolidatedDashboard(d);
        GetConsolidatedMonthlyDashboard(Number($('#years').val()),Number($('#months').val()));
        $('#years').change(function(){
            reset('ExpenseYearlyChart');
            GetDashBoard(Number($('#years').val()));
        });
        $('#months').change(function(){
            reset('ExpenseMonthlyChart');
            GetMonthlyDashBoard(Number($('#years').val()),Number($('#months').val()));
        });
        $('#yearsConsolidated').change(function(){
            reset('ConsolidatedChart');
            GetConsolidatedDashboard(Number($('#yearsConsolidated').val()));
        });
        $('#monthsConsolidated').change(function(){
            reset('ConsolidatedMonthlyChart');
            GetConsolidatedMonthlyDashboard(Number($('#yearsConsolidated').val()),Number($('#monthsConsolidated').val()));
        });
    });
    var GetDashBoard = function(year){
        $.get('/Reports/getExpensesForDashBoard',{year:year},function(data){
        var months = [];
        var evals = [];
        var vals = [];
        var bgCols =[];
        var borCols = [];
        bgCols.push('rgba(54, 162, 235, 0.2)');
        borCols.push('rgba(54, 162, 235, 1)');
        bgCols.push('rgba(0, 163, 53, 0.2)');
        borCols.push('rgba(0, 163, 53, 1)');
        $.each(data.expenses, function(i, v){
            var m = new Date(v[0].expenseDate).getShortMonthName();
            if(months.indexOf(m)==-1)
                months.push(m);
            var tempv=0;
            $.each(v, function(index, value){
                tempv +=Number(value.expenseAmount);
            });
            evals.push(tempv);
            tempv=0;
        });
        $.each(data.fees, function(i, v){
            var m = new Date(v[0].paidDate).getShortMonthName();
            if(months.indexOf(m)==-1)
                months.push(m);
            var tempv=0;
            $.each(v, function(index, value){
                tempv +=Number(value.paidFees);
            });
            vals.push(tempv);
            tempv=0;
        });
        var ctx = document.getElementById("CanExpenseYearlyChart");
        var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: months,
                    datasets: [{
                        label: 'Expenses',
                        data: evals,
                        backgroundColor: bgCols[0],
                        borderColor: borCols[0],
                        borderWidth: 1
                    },
                    {
                        label: 'Fee Payments',
                        data: vals,
                        backgroundColor: bgCols[1],
                        borderColor: borCols[1],
                        borderWidth: 1
                    }]
                },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero:false,
                        }
                    }]
                }
            }
        }); //end of chart
        });//end of get
    }

    var GetMonthlyDashBoard = function(year,month){
        $.get('/Reports/getMonthlyExpensesForDashBoard',{year:year,month:month},function(data){
            var days = [];
            var evals = [];
            var vals = [];
            var bgCols =[];
            var borCols = [];
            bgCols.push('rgba(54, 162, 235, 0.2)');
            borCols.push('rgba(54, 162, 235, 1)');
            bgCols.push('rgba(0, 163, 53, 0.2)');
            borCols.push('rgba(0, 163, 53, 1)');
            $.each(data.expenses, function(i, v){
                var day = new Date(v[0].expenseDate).getDate();
                if(days.indexOf(day)==-1)
                    days.push(day);
                var tempv=0;
                $.each(v, function(index, value){
                    tempv +=Number(value.expenseAmount);
                });
                evals.push(tempv);
                tempv=0;
            });
            $.each(data.fees, function(i, v){
                var day = new Date(v[0].paidDate).getDate()
                if(days.indexOf(day)==-1)
                    days.push(day);
                var tempv=0;
                $.each(v, function(index, value){
                    tempv +=Number(value.paidFees);
                });
                vals.push(tempv);
                tempv=0;
            });
            var ctx = document.getElementById("CanExpenseMonthlyChart");
            var myChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: days,
                        datasets: [{
                            label: 'Expenses',
                            data: evals,
                            backgroundColor: bgCols[0],
                            borderColor: borCols[0],
                            borderWidth: 1
                        },
                        {
                            label: 'Fee Payments',
                            data: vals,
                            backgroundColor: bgCols[1],
                            borderColor: borCols[1],
                            borderWidth: 1
                        }]
                    },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    // legend: {
                    //     display: true,
                    //     labels: {
                    //         fontColor: 'rgb(255, 99, 132)'
                    //     }
                    // },
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero:false
                            }
                        }]
                    }
                }
            }); //end of chart
        });
    }

    var GetConsolidatedDashboard = function(year){
        $.get('/Reports/getConsolidatedDashBoard',{year:year},function(data){
            var v1 = data.expenses;
            var v2 = data.fees;
            var config = {
                type: 'pie',
                data: {
                    datasets: [{
                        data: [
                            v1,
                            v2
                        ],
                        backgroundColor: [
                            'rgba(54, 162, 235, 0.4)',
                            'rgba(0, 163, 53, 0.4)'
                        ],
                        borderColor:[
                            'rgba(54, 162, 235, 1)',
                            'rgba(0, 163, 53, 1)'
                        ],
                        label: 'Consolidated chart'
                    }],
                    labels: [
                        'Expenses',
                        'Fee payments',
                    ]
                },
                options: {
                    responsive: true
                }
            };
            var ctx = document.getElementById('CanConsolidatedChart').getContext('2d');
			var myPie = new Chart(ctx, config);
        });
    }

    var GetConsolidatedMonthlyDashboard = function(year,month){
        $.get('/Reports/getConsolidatedMonthlyDashBoard',{year:year,month:month},function(data){
            var v1 = data.expenses;
            var v2 = data.fees;
            var config = {
                type: 'pie',
                data: {
                    datasets: [{
                        data: [
                            v1,
                            v2
                        ],
                        backgroundColor: [
                            'rgba(54, 162, 235, 0.4)',
                            'rgba(0, 163, 53, 0.4)'
                        ],
                        borderColor:[
                            'rgba(54, 162, 235, 1)',
                            'rgba(0, 163, 53, 1)'
                        ],
                        label: 'Consolidated monthly chart'
                    }],
                    labels: [
                        'Expenses',
                        'Fee payments',
                    ]
                },
                options: {
                    responsive: true
                }
            };
            var ctx = document.getElementById('CanConsolidatedMonthlyChart').getContext('2d');
			var myPie = new Chart(ctx, config);
        });
    }
    var reset = function(c){
        $('#Can'+c).remove(); // this is my <canvas> element
        $('#'+c).append('<canvas id="Can'+c+'"><canvas>');
    }

    Date.prototype.monthNames = [
        "January", "February", "March",
        "April", "May", "June",
        "July", "August", "September",
        "October", "November", "December"
    ];

    Date.prototype.getMonthName = function() {
        return this.monthNames[this.getMonth()];
    };
    Date.prototype.getShortMonthName = function () {
        return this.getMonthName().substr(0, 3);
    };