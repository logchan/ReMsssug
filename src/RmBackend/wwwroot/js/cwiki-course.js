function loadData() {
    $.get(apiserver + '/api/cwiki/course?param=' + code, function (data) {
        var ul = $('#reviews');
        putReviewListData(ul, data);
        hideMsg();
    }).fail(function () {
        showMsg('Failed to load data.', true);
    });
}

loadData();