$(function() {

    $('.navbarForm').autoComplete();
    $('.navbarForm').on('autocomplete.select', function(evt, item) {
        var len = item.length;
        for (var i = 0; i < len; i++) {
            var id = item[i]['EMEMP'];
            var name = item[i]['EMNAME'];

            $("#navbarForm").append("<option value='" + id + "'>" + name + "</option>");

        }

    });


   

});




 





/*  $("#navbarForm").autocomplete({
    maxLength: 5,
   source :function( request, response ) {
  $.ajax({
     url: "DetailShow",
     dataType: "json",
     type:"get",
     data: {
        q: request.term
     },
     success: function( response ) {
       console.log(response)
        var len = response.length;
        console.log(len)
                    $("#suggest").empty();
                    for( var i = 0; i<len; i++){
                        var id = response[i];
                        var name = response[i];
                        $("#suggest").append("<option value='"+id+"'>"+name+"</option>");

                    }
     }
    
  });
 },
  }); */
// console.log($("#navbarForm").val())