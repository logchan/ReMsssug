var subjects = [];
var courses = [];
var converter = new showdown.Converter();

function disableForm() {
    $('#mainform :input').prop('disabled', true);
}

function enableForm() {
    $('#mainform input, #mainform textarea, #mainform button').prop('disabled', false);
    if (!isUpdate) {
        $('#mainform select').prop('disabled', false);
    }
}

function preview() {
    var btn = $('#previewToggle');
    if (btn.text() === 'Preview') {
        $('#contentGroup').hide();
        $('#previewBox').html(converter.makeHtml($('#content').get(0).value));
        $('#previewGroup').show();
        btn.text('Write');
    } else {
        $('#contentGroup').show();
        $('#previewGroup').hide();
        btn.text('Preview');
    }
}

$('#mainform').on('submit', function (e) {
    hideMsg();

    var courseId = $('#code').get(0).value;
    if (courseId === '') {
        showMsg('Error: no course selected', true);
        return false;
    }
    
    var title = $('#title').get(0).value.trim();
    if (title === '') {
        showMsg('Error: empty title', true);
        return false;
    }

    var content = $('#content').get(0).value.trim();
    if (content === '') {
        showMsg('Error: empty content', true);
        return false;
    }

    var formData = $('#mainform').serialize();

    disableForm();

    var url = apiserver + (isUpdate ? '/api/cwiki/update' : '/api/cwiki/add');
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                var id = Number(data);
                if (isNaN(id)) {
                    showMsg('Failed: ' + data, true);
                    enableForm();
                } else {
                    showMsg('Successfully ' +
                        (isUpdate ? 'updated' : 'added') +
                        '! Redirecting to viewing in one second...');
                    // TODO: auto redirect to details page
                }
            },
            statusCode: {
                401: function(data) {
                    showMsg('Failed (not logged in)! Please login in a new window then try again.', true);
                    enableForm();
                },
                500: function(data) {
                    showMsg('Failed (server error)! Please contact admin.', true);
                    enableForm();
                }
            }
        });

    return false; // prevent default
});

function subjectSelectionChanged() {
    var codes = $('#code');
    codes.empty();

    var subject = $('#subject').get(0).value;
    for (var i in courses) {
        if (courses.hasOwnProperty(i)) {
            var course = courses[i];
            if (course.Code.indexOf(subject) === 0) {
                codes.append('<option value="' + course.CourseId + '">' + course.Code.substr(4) + '</option>');
            }
        }
    }
}

function loadDataComplete() {
    hideMsg();
    enableForm();
}

function loadCurrentReview() {
    $.get(apiserver + '/api/cwiki/review?reviewId=' + reviewId,
        function (data) {
            if (data.CourseReviewId != undefined) {

                $('#title').get(0).value = data.Title;
                $('#content').get(0).value = data.Content;
                setSelectOption('subject', data.Course.Code.substr(0, 4));
                subjectSelectionChanged();
                setSelectOption('code', data.CourseId.toString());

                loadDataComplete();
            } else {
                showMsg('Failed to load current review: ' + data);
            }
        }).fail(function () {
            showMsg('Failed to load current review', true);
        });
}

function loadData() {
    $.get(apiserver + '/api/cwiki/allcourses',
        function (data) {
            data = data.sort(function (a, b) {
                return a.Code < b.Code ? -1 : (a.Code === b.Code ? 0 : 1);
            });
            for (var i in data) {
                if (data.hasOwnProperty(i)) {
                    var course = data[i];
                    var subject = course.Code.substr(0, 4);
                    if (subjects.indexOf(subject) < 0) {
                        subjects.push(subject);
                        $('#subject').append('<option value="' + subject + '">' + subject + '</option>');
                    }
                }
            }
            courses = data;

            if (isUpdate) {
                loadCurrentReview();
            } else {
                subjectSelectionChanged();
                loadDataComplete();
            }

        }).fail(function () {
            showMsg('Failed to load courses.', true);
        });
}

loadData();