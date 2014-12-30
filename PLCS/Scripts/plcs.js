/// <reference path="jquery-2.1.1.js" /> 引用此文件可以智能感知

$(function () {

    //導航欄釘住按鈕

    $("#navpin").click(function () {
        if ($("#navbar-top").hasClass("navbar-fixed-top")) {
            $("body").css("padding-top", "0");
            $("#navbar-top").removeClass("navbar-fixed-top");
            $("#navbar-top").addClass("navbar-static-top");
            $("#sidebar").removeClass("sidebar-fixed");
            $("#sidebar").addClass("sidebar-unfixed");

        } else {
            $("body").css("padding-top", "51px");
            $("#navbar-top").removeClass("navbar-static-top");
            $("#navbar-top").addClass("navbar-fixed-top");
            $("#sidebar").removeClass("sidebar-unfixed");
            $("#sidebar").addClass("sidebar-fixed");

        }
    });

    //導航右側背景
    $("#navrightul").mouseover(function () {
        $(this > "li").mouseover(function () {
            $(this).css("background-color", "#428bca");
        });
    });


    //菜單鼠標樣式
    $(".nav-sidebar li").each(function () {
        $(this).mouseover(
            function () {
                if ($(".nav-sidebar li:first").hasClass("active")) {
                    $(".nav-sidebar li:first").removeClass("active");
                }
                $(this).addClass("active");
            });

        $(this).mouseout(
           function () {
               if ($(this).hasClass("active")) {
                   $(this).removeClass("active");
               }
           });
    });

    //查詢菜單
    $('#btnSearch').click(function () {
        $(this).text("loading...");
    });

    //左側菜單點擊效果
    $(".list-group-item").click(
        function () {
            $(".list-group-item").removeClass("active");
            $(this).addClass("active");
        });


    //panel heading 點擊之後show出panel boby
    $(".panel-heading").click(function () {
        $(this).nextAll(".panel-body").toggle();
    });

});