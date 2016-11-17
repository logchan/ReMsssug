
function tryLogin() {
    var username = $('#username').get(0).value.trim();
    if (username === '') {
        showMsg('Username shall not be empty', true);
        return;
    }

    var pwd = md5(md5($('#password').get(0).value + username) + username);
    $('#pwdhash').get(0).value = pwd;
    hideMsg();

    var data = $('#mainform').serialize();
    $('#mainform :input').prop('disabled', true);
    $.post(apiserver + '/api/user/login',
            data,
            function (rtn) {
                if (rtn) {
                    showMsg('Successfully logged in! Closing the page...');
                    setTimeout(function() { window.close(); }, 1000);
                } else {
                    showMsg('Login attempt rejected', true);
                    $('#mainform :input').prop('disabled', false);
                }
            })
        .fail(function() {
            showMsg('Failed to login', true);
            $('#mainform :input').prop('disabled', false);
        });
}

$('#mainform').on('submit', tryLogin);
checkLogin();
