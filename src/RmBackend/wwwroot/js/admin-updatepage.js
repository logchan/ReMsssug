
function enableForm() {
    $('#mainform :input').prop('disabled', false);
}

function disableForm() {
    $('#mainform :input').prop('disabled', true);
}

$('#mainform').on('submit',
function () {
    var data = $('#mainform').serialize();

    disableForm();
    window.scrollTo(0, 0);
    hideMsg();
    showMsg('Updating...');

    $.ajax({
        url: apiserver + '/api/admin/updatepage',
        data: data,
        method: 'POST',
        success: function(data) {
            if (data === 'success') {
                showMsg('Successfully updated the page. Refreshing...');
                setTimeout(function() { window.location.reload(); }, 1000);
            } else {
                showMsg('Failed to update the page: ' + data, true);
                enableForm();
            }
        },
        statusCode: {
            401: function() {
                showMsg('Failed to update the page (login expired). Log in and try again.', true);
                enableForm();
            },
            500: function () {
                showMsg('Failed to update the page (server error).', true);
                enableForm();
            }
        }
    });
});

function loadData() {
    $.get(apiserver + '/api/admin/page?id=' + id,
            function(data) {
                if (data.PageId != undefined) {
                    $('#enabled').get(0).checked = data.Enabled;
                    $('#path').get(0).value = data.Path;
                    $('#title').get(0).value = data.Title;
                    $('#subtitle').get(0).value = data.Subtitle;
                    $('#content').get(0).value = data.Content;
                    $('#rawContent').get(0).checked = data.RawContent;
                    $('#javaScriptFiles').get(0).value = data.JavaScriptFiles;
                    $('#cssFiles').get(0).value = data.CssFiles;
                    $('#requireLogin').get(0).checked = data.RequireLogin;
                    $('#requireFullMember').get(0).checked = data.RequireFullMember;
                    $('#requireAdmin').get(0).checked = data.RequireAdmin;
                    $('#homeOrder').get(0).value = data.HomeOrder;
                    $('#splashOrder').get(0).value = data.SplashOrder;
                    $('#navbarOrder').get(0).value = data.NavbarOrder;
                    $('#thumbnailImage').get(0).value = data.ThumbnailImage;
                    $('#splashImage').get(0).value = data.SplashImage;

                    hideMsg();
                    enableForm();
                } else {
                    showMsg('Failed to load data: ' + data, true);
                }
            })
        .fail(function() {
            showMsg('Failed to load data.', true);
        });
}

checkLogin();
loadData();
