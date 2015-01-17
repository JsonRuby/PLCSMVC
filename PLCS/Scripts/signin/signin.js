/// <reference path="../common/jquery-2.1.1.js" />
/// <reference path="../common/bootstrap.js" />  
/// <reference path="../JsHelper.js" />


$(function () {

    if ($("input[name=LoginUserName]").val() == "") {
        $("input[name=LoginUserName]").select();
    } else {
        $("input[name=LoginPassword]").select();
    }

    $("#loginbtn").click(function () {
        if ($(":text").val() != "" && $(":password").val() != "") {
            $('#processModal').modal();
        }
    });

    //input 全選
    $(":text,:password").click(function () {
        $(this).select();
    });


    $("#rememberMe").click(function () {
        $("#hiddenrememberMe").val($(this).checked ? "checked" : "");
    });
})