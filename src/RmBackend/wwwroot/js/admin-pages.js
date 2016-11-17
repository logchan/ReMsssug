function deletePage(id) {
    var path = $('#pagePath-' + id).text();
    if (path == undefined || path === '') {
        return;
    }

    var confirm = prompt('WARNING: are you sure? Unlike Cwiki, you cannot undo this. Consider disabling the page only. Input the path to confirm:');
    if (path !== confirm) {
        return;
    }

    hideMsg();
    showMsg('Deleting page...');

    $.ajax({
        url: apiserver + '/api/admin/deletepage?id=' + id,
        method: 'DELETE',
        success: function(data) {
            if (data === 'success') {
                showMsg('Successfully deleted the page! Reloading in one second...');
                setTimeout(function() { window.location.reload() }, 1000);
            } else {
                showMsg('Failed to delete page: ' + data, true);
            }
        },
        error: function() {
            showMsg('Failed to delete page (login expired?)', true);
        }
    });
}

function createPage() {
    $('#createPageBtn').prop('disabled', true);
    hideMsg();
    showMsg('Creating page...');
    $.post(apiserver + '/api/admin/newpage',
            function(data) {
                var id = Number(data);
                if (isNaN(id)) {
                    showMsg('Failed to create page: ' + data, true);
                } else {
                    showMsg('Successfully created page. Redirecting to edit...');
                    setTimeout(function() {
                            window.location = '/admin/updatePage?id=' + id;
                        },
                        1000);
                }
            })
        .fail(function() {
            showMsg('Failed to create page. Check login status?', true);
        });
}

function loadData() {
    $.get(apiserver + '/api/admin/pages',
        function (data) {
            var table = $('#dataTable');

            for (var i in data) {
                if (data.hasOwnProperty(i)) {
                    var page = data[i];
                    var tr = '<tr>';
                    tr += String.format('<td id="pagePath-{0}">{1}</td>', page.PageId, page.Path);
                    tr += String.format('<td>{0}</td>', page.Title);
                    tr += String.format('<td>{0}</td>', page.Enabled);
                    tr += String.format('<td><a href="/admin/updatepage?id={0}" class="btn btn-default">Edit</a><a href="javascript:void(0)" onclick="deletePage({0})" class="btn btn-danger">Delete</a></td>', page.PageId);
                    tr += '</tr>';
                    table.append(tr);
                }
            }

            hideMsg();
        }).fail (function() {
        showMsg('Failed to load data.', true);
    });
}

checkLogin();
loadData();
