
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

            var info = 'by <strong>' + data.User.Itsc + '</strong>, last updated at <strong>' + timestr(data.ModifyTime) + '</strong>';
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