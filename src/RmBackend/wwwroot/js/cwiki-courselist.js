var courses = [];

function displayCourses() {
    var area = $('#dataArea');
    area.empty();

    var currSubject = '';
    for (var i in courses) {
        if (courses.hasOwnProperty(i)) {
            var course = courses[i];
            var subject = course.Code.substr(0, 4);
            if (subject !== currSubject) {
                area.append('<div class="row"><div class="col-md-12" id="row-' + subject + '"><h3>' + subject + '</h3></div></div>');
                currSubject = subject;
            }

            var div = $('#row-' + subject);
            div.append('<a href="/cwiki/course?code=' + course.Code + '" class="btn btn-default course-code-btn">' + course.Code + '</a>');
        }
    }
}

function loadData() {
    $.get(apiserver + '/api/cwiki/courses', function (data) {
        data = data.sort(function (a, b) {
            return a.Code < b.Code ? -1 : (a.Code === b.Code ? 0 : 1);
        });

        courses = data;
        displayCourses();
        hideMsg();
    }).fail(function () {
        showMsg('Failed to load data.', true);
    });
}

loadData();