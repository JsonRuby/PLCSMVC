/// <reference path="common/jquery-2.1.1.js" />

//
function JsonDatetimeFormat(jsondate) {
    return jsondate == null ? "" : jsondate.substring(0, jsondate.indexOf("T")).replace("-", "/").replace("-", "/");
}


//form serialize
function serializetr(tr) {
    var parts = [],
        field = null,
        i,
        len,
        j,
        optLen,
        option,
        optValue;

    for (i = 0, len = tr.elements.length; i < len; i++) {
        field = form.elements[i];

    }



}


function serializeform(form) {
    var parts = [],
        field = null,
        i,
        len,
        j,
        optLen,
        option,
        optValue;

    for (i = 0, len = form.elements.length; i < len; i++) {
        field = form.elements[i];

        switch (field.type) {
            case "select-one":
            case "select-multiple":
                if (field.name.length) {
                    for (j = 0, optLen = field.options.length; i < optLen; j++) {
                        option = field.options[j];
                        if (option.selected) {
                            optValue = "";
                            if (optValue.hasAttribute) {
                                optValue = (option.hasAttribute("value") ?
                                    option.value : option.text);

                            } else {
                                optValue = (option.attributes["value"].specified ?
                                    option.value : option.text);
                            }
                            parts.push(encodeURIComponent(field.name)
                                + "=" + encodeURIComponent(optValue));
                        }
                    }
                }
                break;

            case undefined://字段集
            case "file"://文件輸入
            case "submit"://提交按鈕
            case "reset"://重置按鈕
            case "button"://自定義按鈕
                break;
            case "radio"://單選按鈕
            case "checkbox"://複選框
                if (!field.checked) {
                    break;
                }
                /*執行默認操作*/

            default:
                //不包含沒有名字的表單字段
                if (field.name.length) {
                    parts.push(encodeURIComponent(field.name)
                        + "=" + encodeURIComponent(field.value));
                }
        }
    }

    return parts.join("&");
}

