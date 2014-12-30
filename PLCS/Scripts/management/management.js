/// <reference path="../bootstrap.js" />
/// <reference path="../jquery-2.1.1.js" />

$(function () {
    //新增按鈕
    $("#managementAddRecord").click(function () {
        alert("what new do you want?");
    });

    //編輯
    $(".glyphicon-edit").click(function () {
        var tmpTrObj = $(this.parentNode.parentNode);
        var tmpTdObj = $(tmpTrObj).children(); 
        var oldlist = new Array();
        for (var i = 1; i < tmpTdObj.length; i++) {
            oldlist[i] = tmpTdObj.eq(i)[0].innerHTML;

            var tmpvalue = tmpTdObj.eq(i)[0].innerHTML;

            switch (i) {
            case 3:
                $("<td style='padding:5px'><input   class='class_TextBoxEditEmployeeId'  type='text'   value='" + tmpvalue + "' /></td>").replaceAll(tmpTdObj.eq(i)[0]);
            case 9:
                $("<td style='padding:5px'><input  class='date'  type='text'   value='" + tmpvalue + "' /></td>").replaceAll(tmpTdObj.eq(i)[0]);
            default:

            }
        }
        $("<span class='glyphicon glyphicon-ok'></span><span class='glyphicon glyphicon-remove'></span>").replaceAll(this);


     


        //不能寫在$(function(){裏});需要重新註冊按鈕事件.
        $(".class_TextBoxEditEmployeeId").blur(function () {
            $(this).val($(this).val().replace(/(^\s*)|(\s*$)/g, "").toUpperCase());
            var tmpthis = this;
            $.post("/services/ManagementHandler.ashx",
                { "UserId": $(tmpthis).val() },
                function (data) {
                    var userName = data.split('|')[0];
                    var userDept = data.split('|')[1];
                    var nextName = tmpTdObj.eq(4)[0];
                    var nextDept = tmpTdObj.eq(5)[0];

                    $(nextName).html(userName);
                    $(nextDept).html(userDept);
                });
        });

        //日曆控件
        $(".date").datetimepicker({
            format: "yyyy/mm/dd",
            language: "zh-TW",
            autoclose: true,
            todayBtn: "linked",
            minView: "month"
        });

     
        $(":text").click(function () {
            $(this).select();
        });

        //save button confirmation
        $(".glyphicon-saved").click(function () {
            var newlist = new Array();
            for (var j = 1; j < tmpTdObj.length; j++) {
                newlist[j] = tmpTdObj.eq(j)[0].innerHTML;
            }
        });

        $(".glyphicon-remove").click(function() {
            for (var k = 1; k < tmpTdObj.length; k++) {
                tmpTdObj.eq(k)[0].innerHTML = oldlist[k]; 
            }
        });

        $("<span class='glyphicon glyphicon-ok'></span><span class='glyphicon glyphicon-remove'></span>").replaceAll(this);
    });





});

