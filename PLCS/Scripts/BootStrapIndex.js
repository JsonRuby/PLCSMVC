/// <reference path="~/Scripts/common/jquery-2.1.1.js" />
/// <reference path="../common/bootstrap.js" />  
$(function () {

    $("a").click(function (event) {
        if ($(this).attr("href") != undefined) {
            var hrefvalue = $(this).attr("href");
            if (hrefvalue.indexOf("/") >= 0) {
                $('#processModal').modal();
                if ($(this).parent().parent().hasClass("pagination") || $(this).parent().parent().hasClass("dropdown-menu")) {
                    event.stopPropagation();
                    event.preventDefault();
                    $("#searchForm").attr("action", "/management/management/" + hrefvalue.split('/')[hrefvalue.split('/').length - 1]);
                    $("#btnSearch").click();
                }
            }
        }
    });




});