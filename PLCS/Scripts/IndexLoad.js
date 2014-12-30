/// <reference path="jquery-2.1.1.js" />
/// <reference path="BootStrapIndex.js" />
$(function () {
    $("a").click(function () {
        var hrefvalue = $(this).attr("href");
        if (hrefvalue.indexOf("/") >= 0) {
            $('#processModal').modal();
        }
    });
});