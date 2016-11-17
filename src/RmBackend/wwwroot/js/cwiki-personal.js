function deleteReview(id) {
    var code = $('#courseCode-' + id).text();
    if (code == undefined || code === '') {
        return;
    }

    var confirm = prompt('WARNING: are you sure? Input the course code to confirm:');
    if (code !== confirm) {
        return;
    }

    $.ajax({
        url: apiserver + '/api/cwiki/delete?reviewId=' + id,
        method: 'DELETE',
        success: function(data) {
            if (data === 'success') {
                showMsg('Successfully deleted the review.');
                $('#status-' + id).text('Deleted');
                $('#actions-' + id).empty();
            } else {
                showMsg('Failed to deleted the review: ' + data, true);
            }
        },
        statusCode: {
            401: function() {
                showMsg('Failed to deleted the review (login expired). Please log in and try again.', true);
            },
            500: function() {
                showMsg('Failed to deleted the review (server error).', true);
            }
        }
    });
}

function loadData() {
    var url = apiserver + '/api/cwiki/user?param=' + userInfo.UserId;
    $.get(url,
            function(data) {
                var tb = $('#dataTable');
                for (var i in data) {
                    if (data.hasOwnProperty(i)) {
                        var review = data[i];
                        var id = review.CourseReviewId;
                        var deleted = review.Status === 2;
                        var tr = (deleted ? '<tr class="review-deleted">' : '<tr>');

                        tr += String.format('<td id="courseCode-{0}">{1}</td>', id, review.Course.Code);
                        tr += String.format('<td>{0}</td>', review.Title);
                        tr += String.format('<td id="status-{0}">{1}</td>', id, postStatus[review.Status]);

                        if (!deleted) {
                            tr += String.format('<td id="actions-{0}">', id)
                            tr += String.format('<a href="/cwiki/write?reviewId={0}" class = "btn btn-default" style="margin-right: 4px">Edit</a>', id);
                            tr += String.format('<a href="javascript:void(0)" onclick="deleteReview({0})" class="btn btn-danger">Delete</a>', id);
                            tr += '</td>';
                        }
                        else {
                            tr += '<td></td>';
                        }
                        tb.append(tr + '</tr>');
                    }
                }
                hideMsg();
            })
        .fail(function() {
            showMsg('Failed to load data.', true);
        });
}

loginCheckCallback.push(function() {
    if (userInfo === 'not logged in') {
        showMsg('Please login first.', true);
    } else {
        loadData();
    }
});

checkLogin();
