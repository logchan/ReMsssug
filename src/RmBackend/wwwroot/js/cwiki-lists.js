function putReviewListData(ul, data) {
    for (var i in data) {
        if (data.hasOwnProperty(i)) {
            var entry = data[i];
            ul.append('<li><a href="/cwiki/review?id=' + entry.CourseReviewId + '">'
                + '<span class="course-code">' + entry.Course.Code + '</span>'
                + '<span class="review-title">' + entry.Title + '</span>'
                + '</a></li>');
        }
    }
}
