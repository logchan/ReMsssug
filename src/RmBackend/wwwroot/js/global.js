var apiserver = window.location.origin;
var loginserver = 'https://ihome.ust.hk/~glinaa/cgi-bin/remsssug/login.php';
var loginChecked = false;
var userInfo = 'not logged in';
var loginCheckCallback = [];
var postStatus = ['Draft', 'Posted', 'Deleted', 'Need Modification', 'Need Approval'];

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

function timestr(time) {
    var date = new Date(Date.parse(time) + 28800000); // to UTC+8
    time = date.toISOString();

    if (time.indexOf('.') > 0)
        time = time.substr(0, time.indexOf('.'));
    time = time.replace('T', ' ');
    return time;
}

// http://stackoverflow.com/questions/610406/javascript-printf-string-format/4673436#4673436
if (!String.format) {
    String.format = function (format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

function confirmLogout() {
    if (confirm('Do you want to log out?')) {
        $.get(apiserver + '/api/user/logout',
                function() {
                    window.location.reload();
                })
            .fail(function() {
                window.location.reload();
            });
    }
}

function setupUser() {
    var ul = $('#navbar-right-ul');
    ul.empty();

    if (userInfo === 'not logged in') {
        ul.append('<li><a href="/login" target="_blank">Login</a></li>');
    } else {
        ul.append(
            String.format('<li><a href="javascript:void(0)" onclick="confirmLogout()">{0}</a></li>', userInfo.Itsc)
            );
    }
}

loginCheckCallback.push(setupUser);
