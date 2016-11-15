
function redirectToCwiki() {
    setTimeout(function() {
            window.location = '/cwiki';
        }, 1000);
}

function loadData() {
    $.get(apiserver + '/api/cwiki/review?reviewId=' + id, function(data) {
        if (data.CourseReviewId != undefined) {
            hideMsg();

            var converter = new showdown.Converter();
            $('#title').text(data.Title);
            $('#courseCode').text(data.Course.Code);
            $('#content').html(converter.makeHtml(data.Content));

            var time = data.ModifyTime;
            time = time.substr(0, time.indexOf('.'));
            time = time.replace('T', ' ');
            var info = 'by <strong>' + data.User.Itsc + '</strong>, last updated at <strong>' + time + '</strong>';
            $('#info').html(info);
        } else {
            showMsg('Failed to load review: ' + data);
            redirectToCwiki();
        }
        
    }).fail(function () {
        showMsg('Failed to load review.', true);
        redirectToCwiki();
    });
}

loadData();