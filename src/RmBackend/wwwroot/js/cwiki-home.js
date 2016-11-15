function putReviewListData(ul, data) {
    for (var i in data) {
        if (data.hasOwnProperty(i)) {
            var entry = data[i];
            ul.append('<li><a href="cwiki/review?id=' + entry.CourseReviewId + '">'
                + '<span class="course-code">' + entry.Course.Code + '</span>'
                + '<span class="review-title">' + entry.Title + '</span>'
                + '</a></li>');
        }
    }
}

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

loadData();