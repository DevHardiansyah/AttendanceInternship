 // filter department and section based on kpk 
 $(document).ready(function() {

     $("#kpk").keyup(function() {
         var kpk = $(this).val();
         console.log(kpk);
         $.ajax({
             url: 'getDepartement',
             type: 'get',
             data: { kpk: kpk },
             dataType: 'json',
             success: function(response) {

                 $("#departement").empty();
                 for (var i = 0; i < len; i++) {
                     var id = response[i]['DEPT_CODE'];
                     var name = response[i]['DEPT_NAME'];

                     $("#departement").append("<option value='" + id + "'>" + name + "</option>");
                 }
             }
         });

         $.ajax({
             url: 'getEmployee?kpk=' + kpk,
             type: 'get',
             dataType: 'json',
             success: function(response) {
                 console.log(response);
                 var len = response.length;
                 $("#getStatusEmployee").empty();
                 for (var i = 0; i < len; i++) {
                     var id = response[i]['DEPT_CODE'];
                     var name = response[i]['DEPT_NAME'];

                     $("#getStatusEmployee").val(response[i]['EMPAYT']);
                 }
             }
         });

     });



     $.ajax({
         url: 'getDepartement',
         type: 'get',
         dataType: 'json',
         success: function(response) {
             var len = response.length;
             $("#DepartmentHasRegistered").empty();
             for (var i = 0; i < len; i++) {
                 var id = response[i]['DEPT_CODE'];
                 var name = response[i]['DEPT_NAME'];

                 $("#DepartmentHasRegistered").append("<option value='" + id + "'>" + name + "</option>");
             }
         }
     });
     $("#DepartmentHasRegistered").click(function() {
         var departement = $(this).val();
         var alldept = 'ALL';
         console.log(departement);
         $.ajax({
             url: 'getSection',
             type: 'get',
             data: { DepartementCode: departement },
             dataType: 'json',
             success: function(response) {
                 console.log(response);
                 var len = response.length;
                 var check_section = $("#SectionHasRegistered").empty();
                 $("#SectionHasRegistered").append("<option value='" + alldept + "'>" + alldept + "</option>");
                 for (var i = 0; i < len; i++) {
                     var id = response[i]['SECT_CODE'];
                     var name = response[i]['SECT_NAME'];
                     $("#SectionHasRegistered").append("<option value='" + id + "'>" + name + "</option>");
                 }
             }
         });
     });
     // assign value to dropdown (section)
     $("#departement").click(function() {
         var departement = $(this).val();
         var alldept = 'ALL';
         console.log(departement);
         $.ajax({
             url: 'getSection',
             type: 'get',
             data: { DepartementCode: departement },
             dataType: 'json',
             success: function(response) {
                 console.log(response);
                 var len = response.length;
                 var check_section = $("#Section_attendance").empty();
                 $("#Section_attendance").append("<option value='" + alldept + "'>" + alldept + "</option>");
                 for (var i = 0; i < len; i++) {
                     var id = response[i]['SECT_CODE'];
                     var name = response[i]['SECT_NAME'];
                     $("#Section_attendance").append("<option value='" + id + "'>" + id + "-" + name + "</option>");
                 }
             }
         });
     });


 });