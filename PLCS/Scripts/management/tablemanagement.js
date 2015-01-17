/// <reference path="../common/jquery-2.1.1.js" />
/// <reference path="../common/bootstrap.js" />  
/// <reference path="../JsHelper.js" />



$(function () {
    //日曆控件
    $(".date").datetimepicker({
        format: "yyyy/mm/dd",
        language: "zh-TW",
        autoclose: true,
        todayBtn: "linked",
        minView: "month"
    });
    //input 全選
    $(":text").click(function () {
        $(this).select();
    });

    //大寫+trim
    $(":text").keyup(function () { $(this).val($(this).val().replace(/(^\s*)|(\s*$)/g, "").toUpperCase()); });

    //新增按鈕

    $("#showSearch").click(function () {
        $("#filtertr").fadeToggle();
    });

    //編輯
    $(".glyphicon-edit").on('click', function () {
        var tdlist = $(this).parent().parent().children();
        var element = $(this).parent().next();
        $(element).data("tdlist", {
            id: tdlist[1].innerHTML,
            Region: tdlist[2].innerHTML,
            UserId: tdlist[3].innerHTML,
            Name: tdlist[4].innerHTML,
            Dept: tdlist[5].innerHTML,
            OrgDept: tdlist[6].innerHTML,
            ClosetNo: tdlist[7].innerHTML,
            ClosetNorm: tdlist[8].innerHTML,
            ClosetRemark: tdlist[9].innerHTML,
            SignDate: tdlist[10].innerHTML,
            Remark: tdlist[11].innerHTML
        });
        $(element).data("changeddata", {
            id: tdlist[1].innerHTML,
            Region: tdlist[2].innerHTML,
            UserId: tdlist[3].innerHTML,
            Name: tdlist[4].innerHTML,
            Dept: tdlist[5].innerHTML,
            OrgDept: tdlist[6].innerHTML,
            ClosetNo: tdlist[7].innerHTML,
            ClosetNorm: tdlist[8].innerHTML,
            ClosetRemark: tdlist[9].innerHTML,
            SignDate: tdlist[10].innerHTML,
            Remark: tdlist[11].innerHTML
        });

        for (var i = 1; i < tdlist.length; i++) {

            switch (i) {
                case 3:
                    $("<td style='padding:5px'>" +
                        "<input class='class_TextBoxEditEmployeeId'  type='text'   value='" + $(element).data("tdlist").UserId + "' />" +
                        "</td>").replaceAll(tdlist[i]);
                    break;
                case 10:
                    $("<td style='padding:5px'>" +
                        "<input  class='date'  type='text'   value='" + $(element).data("tdlist").SignDate + "' />" +
                        "</td>").replaceAll(tdlist[i]);
                    break;
                default:
            }
        }

        //確認+取消dom
        $(this).parent().css("padding", "8px 0");
        $("<span class='glyphicon glyphicon-ok' title='提交'></span>" +
            "<span class='glyphicon glyphicon-remove' title='取消'></span>")
            .insertBefore(this);
        $(this).hide();

        //註冊事件.
        $(".class_TextBoxEditEmployeeId").focusout(function () {
            var that = this;
            $(this).val($(this).val().replace(/(^\s*)|(\s*$)/g, "").toUpperCase());
            var eleUserName = $(this).parent().next();
            var eleUserDept = $(this).parent().next().next();
            var idelement = $(this).parent().prev().prev();

            if ($(this).val() == "") {
                $(this).parent().next().text("");
                $(this).parent().next().next().text("");
                $(idelement).data("changeddata").UserId = "";
                $(idelement).data("changeddata").Name = "";
                $(idelement).data("changeddata").Dept = "";
            } else {
                $.post("/management/GetNameAndDept",
                    { "UserId": $(that).val() },
                    function (data) {
                        var userName = data.split('|')[0];
                        var userDept = data.split('|')[1];
                        $(eleUserName).html(userName);
                        $(eleUserDept).html(userDept);
                        $(idelement).data("changeddata").UserId = $(that).val();
                        $(idelement).data("changeddata").Name = userName;
                        $(idelement).data("changeddata").Dept = userDept;
                    });
            }
        });

        ////date
        //$(".date").change(function () {
        //    if ($(this) != $("#SignDate")) {
        //        var idelement = $(this).parent().parent().children()[1];
        //        $(idelement).data("changeddata").SignDate = $(this).val();
        //    }
        //});


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


        //確認
        $(".glyphicon-ok").click(function () {
            var that = this;
            var idelement = $(this).parent().next();
            if ($(idelement).data("changeddata").UserId != "" && $(idelement).data("changeddata").Name == "") {

                $("<tr>" +
                         "<td  colspan='11' class='danger msg' >輸入的工號<strong>" +
                         $(idelement).data("changeddata").UserId + "</strong>無人事資料!請確認!</td>" +
                         "</tr>")
                         .insertAfter($(that).parent().parent()).fadeOut(2000);
                return;
            }
            $('#processModal').modal();
            $.post("/Management/Save",
            {
                data: $(idelement).data("changeddata")
            },
            function (data, status) {
                if (status == "success") {
                    if (data == "true") {
                        for (var k = 1; k < idelement.parent().children().length; k++) {
                            switch (k) {
                                case 3:
                                    $("<td>" + $(idelement).data("changeddata").UserId + "</td>")
                                        .replaceAll(idelement.next().next());
                                    break;
                                case 10:
                                    $("<td>" + $(idelement).data("changeddata").SignDate + "</td>")
                                        .replaceAll(idelement.next().next().next().next().next().next().next().next().next());
                                    break;
                                default:
                            }
                        }

                        $(that).next().next().show();
                        $(that).next().remove();
                        $(that).remove();
                        //CSS樣式
                        $(idelement.parent()).removeClass("danger").addClass("success");

                    } else if (data == "false") {
                        $("<tr>" +
                          "<td  colspan='11' class='danger msg' >更新失敗!請確認!本提示10秒後消失.</td>" +
                          "</tr>")
                          .insertAfter($(that).parent().parent()).fadeOut(10000);
                    } else if (data == "errorDept") {
                        $("<tr>" +
                          "<td  colspan='11' class='danger msg' >不允許變更到其他部門!本提示10秒後消失.</td>" +
                          "</tr>")
                          .insertAfter($(that).parent().parent()).fadeOut(10000);
                    } else if (data.indexOf("<!DOCTYPE html>") <= 0) {
                        var tmpdata = $.parseJSON(data)[0];
                        $("<tr class='alert alert-danger' role='alert'>" +
                            "<td><span class='glyphicon glyphicon-trash' title='清空'></span></td>" +
                            "<td hidden>" + tmpdata.Id + "</td>" +
                            "<td>" + tmpdata.Region + "</td>" +
                            "<td>" + tmpdata.UserId + "</td>" +
                            "<td>" + tmpdata.Name + "</td>" +
                            "<td>" + tmpdata.Dept + "</td>" +
                            "<td>" + tmpdata.OrgDept + "</td>" +
                            "<td>" + tmpdata.ClosetNo + "</td>" +
                            "<td>" + tmpdata.ClosetNorm + "</td>" +
                            "<td>" + tmpdata.ClosetRemark + "</td>" +
                            "<td>" + JsonDatetimeFormat(tmpdata.SignDate) + "</td>" +
                            "<td>" + tmpdata.Remark + "</td></tr>").insertAfter($(that).parent().parent());

                        $("<tr class='alert alert-danger' role='alert'><td  colspan='11'>" +
                            "<button type='button' class='close' data-dismiss='alert'><span aria-hidden='true'>&times;</span><span class='sr-only'>Close</span></button>" +
                            "數據重複!請先刪除佔用的位置 </td></tr>").insertAfter($(that).parent().parent());


                        //trash icon
                        $(".glyphicon-trash").click(function() {
                            var that_trash = this;
                            var tmpId = $(that_trash).parent().next().text();
                            $.post("/Management/Delete", {
                                Id: tmpId
                            }, function(d, s) {
                                if (s == "success") {
                                    if (d == "true") {
                                        $("<tr class='alert alert-danger' role='alert'><td  colspan='11'>" +
                                            "<button type='button' class='close' data-dismiss='alert'></button>" +
                                            "處理成功!</td></tr>").replaceAll($(that_trash).parent().parent().prev());
                                        $(that_trash).parent().parent().prev().fadeOut(2000);
                                        $(that_trash).parent().parent().remove();
                                    } else {
                                        $("<tr class='alert alert-danger' role='alert'><td  colspan='11'>" +
                                            "<button type='button' class='close' data-dismiss='alert'></button>" +
                                            "處理失敗!請確認!</td></tr>").replaceAll($(that_trash).parent().parent().prev());
                                    }
                                } else {
                                    alert(s + "網絡錯誤,請重試");
                                }
                            });


                        });

                    } else {
                        window.location = "/home/index";
                    }
                    $('#processModal').modal("hide");
                } else {
                    $("<tr><td  colspan='11' class='info msg' >" + status + "網絡錯誤!請重試!</td></tr>")
                        .insertAfter($(that).parent().parent());
                    //CSS樣式
                    $(idelement.parent()).removeClass("danger").addClass("info");
                }

            });
            $(that).parent().css("padding", "8px 8px");
            $('#processModal').modal().hide();
        });

        //取消
        $(".glyphicon-remove").click(function () {
            var that = this;
            var idelement = $(this).parent().next();
            for (var k = 1; k < idelement.parent().children().length; k++) {
                switch (k) {
                    case 3:
                        $("<td>" + $(idelement).data("tdlist").UserId + "</td>")
                            .replaceAll(idelement.next().next());
                        break;
                    case 4:
                        $("<td>" + $(idelement).data("tdlist").Name + "</td>").replaceAll(idelement.next().next().next());
                    case 5:
                        $("<td>" + $(idelement).data("tdlist").Dept + "</td>").replaceAll(idelement.next().next().next().next());
                    case 9:
                        $("<td>" + $(idelement).data("tdlist").SignDate + "</td>")
                            .replaceAll(idelement.next().next().next().next().next().next().next().next().next());
                        break;
                    default:
                }
            }

            if ($(that).parent().parent().next().next().hasClass("alert-danger")) {
                $(that).parent().parent().next().next().remove();
            }
            if ($(that).parent().parent().next().hasClass("alert-danger")) {
                $(that).parent().parent().next().remove();
            }
            $(that).parent().css("padding", "8px 8px");
            $(that).next().show();
            $(that).prev().remove();
            $(that).remove();
            //CSS樣式
            $(idelement.parent()).removeClass("danger").addClass("info");
        });



        //css樣式
        $($(this).parent().parent()).addClass("danger");
        $(this).hide();
        //如不隱藏,再重新bind需要使用live(已廢棄),on,trigger之類的,試了下,效果不理想.實現起來較麻煩...
    });



    //篩選條件


    $("#btnSearch").click(function () {

        $("#searchForm input[name='Region']").val($("#Region").val());
        $("#searchForm input[name='UserId']").val($("#UserId").val());
        $("#searchForm input[name='Name']").val($("#Name").val());
        $("#searchForm input[name='Dept']").val($("#Dept").val());
        $("#searchForm input[name='OrgDept']").val($("#OrgDept").val());
        $("#searchForm input[name='ClosetNo']").val($("#ClosetNo").val());
        $("#searchForm input[name='ClosetNorm']").val($("#ClosetNorm").val());
        $("#searchForm input[name='ClosetRemark']").val($("#ClosetRemark").val());
        $("#searchForm input[name='SignDate']").val($("#SignDate").val());
        $("#searchForm input[name='Remark']").val($("#Remark").val());

        $("#searchForm :submit").click();
    });






    //下載 亮點:前臺動態進度條...純手工啊..

    $("#myDownloadExcel").click(function () {
        var that = this;
        $(that).addClass("disabled");
        $.post("/management/nonpagedcount", function (data) {
            var count = parseInt(data);
            var pageSize = 1;
            var pageIndex = 1;
            var pageCount = Math.ceil(count / pageSize);
            var isFirstTime = false;
            var isLastTime = false;

            function Processing() {
                if (pageCount == 1) {
                    GoNext(UpdateModalProcess);
                } else {
                    $.post("/management/GenerateExcel", {
                        pageIndex: pageIndex,
                        pageSize: pageSize,
                        pageCount: pageCount,
                        isFirstTime: isFirstTime,
                        isLastTime: isLastTime
                    }, function (d) {
                        GoNext(UpdateModalProcess);
                    });
                }

            };

            function GoNext(callback) {
                //最後一步用隱藏form提交,避免post.data無法推到前臺下載的樣子..變成異步請求過來的data了..
                //5=>6.@_@
                if (pageIndex >= pageCount - 1) {
                    $("#exportForm input[name='pageIndex']").val(pageIndex == 1 ? pageIndex : (pageIndex + 1));
                    $("#exportForm input[name='pageSize']").val(pageSize);
                    $("#exportForm input[name='pageCount']").val(pageCount);
                    $("#exportForm input[name='isFirstTime']").val(isFirstTime);
                    $("#exportForm input[name='isLastTime']").val(isLastTime);
                    $("#exportForm :submit").click();

                    $(" <div class='progress'> " +
                         " <div class='progress-bar progress-bar-striped active' role='progressbar' " +
                         " aria-valuenow=" + Math.ceil((pageIndex + 1) / pageCount * 100) +
                         " aria-valuemin='0' aria-valuemax='100' " +
                         "style='width: " + Math.ceil((pageIndex + 1) / pageCount * 100) + "%;'> " +
                         " <span>下載完成100%</span> " +
                         " </div> " +
                         " </div>").replaceAll($(that).parent().prev().children());
                    $("#myDownloadModalCancel").click();
                    $(that).removeClass("disabled");

                    $("<h5>你將要下載的資料即爲當前頁面顯示的內容</h5>").replaceAll($(that).parent().prev().children());

                    //alert("what?");
                } else {
                    callback();
                }

            }

            function UpdateModalProcess() {
                $(" <div class='progress'> " +
                          " <div class='progress-bar progress-bar-striped active' role='progressbar' " +
                          " aria-valuenow=" + Math.ceil(pageIndex / pageCount * 100) +
                          " aria-valuemin='0' aria-valuemax='100' " +
                          "style='width: " + Math.ceil(pageIndex / pageCount * 100) + "%;'> " +
                          " <span>" + Math.ceil(pageIndex / pageCount * 100) + "%</span> " +
                          " </div> " +
                          " </div>").replaceAll($(that).parent().prev().children());
                pageIndex += 1;
                Processing(GoNext);
            }

            Processing(GoNext);


            //for (var i = 1; i <= pageCount; i++) {
            //    //JS是單線程的.不像JAVA,C之類的開啓多線程等方法返回結果..
            //    //使用回調可以解決.
            //    if (i == 1) {
            //        isFirstTime = true;
            //    }
            //    if (i == pageCount) {
            //        isLastTime = true;
            //    }
            //    var result = true;
            //    while (true) {
            //        $.post("/management/GenerateExcel", {
            //            pageIndex: i,
            //            pageSize: pageSize,
            //            pageCount: pageCount,
            //            isFirstTime: isFirstTime,
            //            isLastTime: isLastTime
            //        }, function (d) {
            //            if (d == "true") {
            //                //do something


            //                result = false;
            //            }
            //        });
            //    }
            //}


        });


    });

    //上傳
    //有時間再來研究如何向下載一樣...
    $("#myUploadExcel_bak").click(function () {
        var that = this;

        $("#importForm :submit").click();
        //
        $.post("/management/UploadExcel", {
            pageIndex: 1,
            pageSize: 1,
            pageCount: 1,
            isFirstTime: true,
            isLastTime: false
        }, function (r) {
            var count = parseInt(r);
            var pageSize = 1;
            var pageIndex = 2;
            var pageCount = Math.ceil(count / pageSize);
            var isFirstTime = false;
            var isLastTime = false;

            function Processing() {
                if (pageCount == 1) {
                    GoNext(UpdateModalProcess);
                } else {
                    $.post("/management/UploadExcel", {
                        pageIndex: pageIndex,
                        pageSize: pageSize,
                        pageCount: pageCount,
                        isFirstTime: isFirstTime,
                        isLastTime: isLastTime
                    }, function (d) {
                        if (d != "processing") {
                            alert(d);
                        }
                        GoNext(UpdateModalProcess);
                    });
                }

            };

            function GoNext(callback) {
                //最後一步用隱藏form提交,避免post.data無法推到前臺下載的樣子..變成異步請求過來的data了..
                //5=>6.@_@
                if (pageIndex = pageCount - 1) {
                    $(" <div class='progress'> " +
                        " <div class='progress-bar progress-bar-striped active' role='progressbar' " +
                        " aria-valuenow=" + Math.ceil((pageIndex + 1) / pageCount * 100) +
                        " aria-valuemin='0' aria-valuemax='100' " +
                        "style='width: " + Math.ceil((pageIndex + 1) / pageCount * 100) + "%;'> " +
                        " <span>上傳完成100%,即將檢查數據.</span> " +
                        " </div> " +
                        " </div>").replaceAll($(that).parent().prev().children());
                    //檢查(處理)
                    alert("OK");

                    //alert("what?");
                } else {
                    callback();
                }

            }

            function UpdateModalProcess() {
                $(" <div class='progress'> " +
                    " <div class='progress-bar progress-bar-striped active' role='progressbar' " +
                    " aria-valuenow=" + Math.ceil(pageIndex / pageCount * 100) +
                    " aria-valuemin='0' aria-valuemax='100' " +
                    "style='width: " + Math.ceil(pageIndex / pageCount * 100) + "%;'> " +
                    " <span>" + Math.ceil(pageIndex / pageCount * 100) + "%</span> " +
                    " </div> " +
                    " </div>").replaceAll($(that).parent().prev().children());
                pageIndex += 1;
                Processing(GoNext);
            }

            Processing(GoNext);
        });

    });

    $("#myUploadExcel").click(function () {
        $("<iframe id='managementIframe' name='managementIframe' " +
         " class='embed-responsive-item' src='/Iframe/MUploadStep1.html'></iframe>")
        .replaceAll($("#managementIframe"));
        //window.frames["managementIframe"].location.reload();
    });

});




