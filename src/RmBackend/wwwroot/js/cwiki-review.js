
function redirectToCwiki() {
    setTimeout(function() {
            window.location = '/cwiki';
        }, 1000);
}

function loadData() {
    $.get(apiserver + '/api/cwiki/review?reviewId=' + id, function(data) {
        if (data.CourseReviewId != undefined) {
            hideMsg();

            $('#title').text(data.Title);
            $('#courseCode').text(data.Course.Code);
            $('#content').html(showdownConverter.makeHtml(data.Content));

            var info = String.format('by <strong>{0}</strong>, last updated at <strong>{1}</strong>', data.User.Itsc, timestr(data.ModifyTime));
            $('#info').html(info);
            initComments('commentArea', data.CommentEntryNumber);
        } else {
            showMsg('Failed to load review: ' + data);
            redirectToCwiki();
        }
        
    }).fail(function () {
        showMsg('Failed to load review.', true);
        redirectToCwiki();
    });
}

checkLogin();
loadData();