/// <reference path="bootstrap.js" />
/// <reference path="jquery-2.1.1.js" /> 

$(function () {
    $(function () {
        $("a").click(function () {
            var hrefvalue = $(this).attr("href");
            if (hrefvalue.indexOf("/") >= 0) {
                $('#processModal').modal();
            }
        });

    });

    //panel heading 點擊之後show出panel boby
    $(".panel-heading").click(function () {
        $(this).nextAll(".panel-body").toggle();
    });

    
 
});