function loadData() {
    $.get(apiserver + '/api/page/home',
            function (data) {
                var div = $('#thumbnailArea');
                data.sort(function (a, b) { return a.HomeOrder - b.HomeOrder });

                for (var i in data) {
                    if (data.hasOwnProperty(i)) {
                        var page = data[i];
                        if (page.HomeOrder === 0)
                            continue;

                        var thumbnail = '<div class="col-md-4"><div class="thumbnail">';
                        thumbnail += '<img src="' + page.ThumbnailImage + '" alt="' + page.Title + '" />';
                        thumbnail += '<div class="caption">';
                        thumbnail += '<h3><a href="/page/' + page.Path + '">' + page.Title + '</a></h3>';
                        thumbnail += '</div></div></div>';
                        div.append(thumbnail);
                    }
                }

                hideMsg();
            })
        .fail(function() {
            showMsg('Failed to load data.', true);
        });
}

loadData();
checkLogin();
