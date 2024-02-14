function check_Date() {

    // call the inpur hidden 
    var term_start_at = document.getElementById('term_start');
    var term_end_at = document.getElementById('term_end');

    // get value years
    var getYears = document.getElementById('years_filter');
    var filter__Years = getYears.options[getYears.selectedIndex].value;
    var end_at = '';

    // get value month
    var getMonth = document.getElementById('month_filter');
    var filter__Month = getMonth.options[getMonth.selectedIndex].value;



    var start_month = filter__Years.concat(filter__Month)
    switch (start_month) {
        // jan
        case filter__Years.concat("1222"):
            var beforeNewYears = filter__Years - 1;
            start_at = beforeNewYears + "1222";
            end_at = filter__Years.concat("0121");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(beforeNewYears);
            console.log(start_at);
            console.log(end_at);
            break;
            // feb    
        case filter__Years.concat("0122"):
            start_at = filter__Years.concat("0122");
            end_at = filter__Years.concat("0221");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // Mar    
        case filter__Years.concat("0222"):
            start_at = filter__Years.concat("0222");
            end_at = filter__Years.concat("0321");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // Apr    
        case filter__Years.concat("0322"):
            start_at = filter__Years.concat("0322")
            end_at = filter__Years.concat("0421");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // May    
        case filter__Years.concat("0422"):
            start_at = filter__Years.concat("0422");
            end_at = filter__Years.concat("0521");
            console.log(start_at);
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // June    
        case filter__Years.concat("0522"):
            start_at = filter__Years.concat("0522")
            end_at = filter__Years.concat("0621");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // Jul    
        case filter__Years.concat("0622"):
            start_at = filter__Years.concat("0622")
            end_at = filter__Years.concat("0721");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // Aug    
        case filter__Years.concat("0722"):
            start_at = filter__Years.concat("0722")
            end_at = filter__Years.concat("0821");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // September    
        case filter__Years.concat("0822"):
            start_at = filter__Years.concat("0822")
            end_at = filter__Years.concat("0921");
            console.log(start_at)
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // Octo    
        case filter__Years.concat("0922"):
            start_at = filter__Years.concat("0922")
            end_at = filter__Years.concat("1021");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

            // November    
        case filter__Years.concat("1022"):
            start_at = filter__Years.concat("1022")
            end_at = filter__Years.concat("1121");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(end_at);
            break;

            console.log(start_at);
            // December
        case filter__Years.concat("1122"):
            start_at = filter__Years.concat("1122")
            end_at = filter__Years.concat("1221");
            term_start_at.value = start_at
            term_end_at.value = end_at
            console.log(start_at);
            console.log(end_at);
            break;

    }


}