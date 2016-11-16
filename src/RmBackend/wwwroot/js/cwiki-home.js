function loadLatestComment() {
    $.get(apiserver + '/api/cwiki/latestcomment', function (data) {
        var ul = $('#latestcomment');
        putReviewListData(ul, data);
        hideMsg();
    }).fail(function () {
        showMsg('Failed to load latest comment.', true);
    });
}

function loadData() {
    $.get(apiserver + '/api/cwiki/latestreviews', function (data) {
        var ul = $('#latestReview');
        putReviewListData(ul, data);
        loadLatestComment();
    }).fail(function() {
        showMsg('Failed to load latest review.', true);
    });
}

checkLogin();
loadData();