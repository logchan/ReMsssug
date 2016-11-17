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
                        thumbnail += String.format('<a href="/page/{2}"><img src="{0}" alt="{1}" /></a>', page.ThumbnailImage, page.Title, page.Path);
                        thumbnail += '<div class="caption">';
                        thumbnail += String.format('<h3><a href="/page/{0}">{1}</a></h3>', page.Path, page.Title);
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
