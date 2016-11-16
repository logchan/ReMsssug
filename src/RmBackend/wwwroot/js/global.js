var apiserver = window.location.origin;
var loginserver = 'https://ihome.ust.hk/~glinaa/remsssug/login';
var loginChecked = false;
var userInfo = 'not logged in';
var loginCheckCallback = [];

function showMsg(msg, err) {
    var x = $(err ? '#errmsg' : '#msg');
    if (x && x.text) {
        x.text(msg);
        x.show();
    }
}

function hideMsg(err) {
    var x = $(err === true ? '#errmsg' : (err === false ? '#msg' : '#errmsg, #msg'));
    if (x && x.hide) {
        x.hide();
    }
}

function setSelectOption(selId, optionValue) {
    var sel = $('#' + selId).get(0);
    for (var i in sel.options) {
        if (sel.options.hasOwnProperty(i)) {
            var o = sel.options[i];
            if (o.value === optionValue) {
                sel.selectedIndex = i;
                break;
            }
        }
    }
}

function checkLogin() {
    $.get(apiserver + '/api/user/current',
        function(data) {
            userInfo = data;
            loginChecked = true;
            for (var i in loginCheckCallback) {
                if (loginCheckCallback.hasOwnProperty(i)) {
                    loginCheckCallback[i]();
                }
            }
        }).fail(function() {
        showMsg('Failed to check login status', true);
    });
}
