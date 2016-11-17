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
                area.append(
                    String.format('<div class="row"><div class="col-md-12" id="row-{0}"><h3>{0}</h3></div></div>', subject)
                    );
                currSubject = subject;
            }

            var div = $('#row-' + subject);
            div.append(
                String.format('<a href="/cwiki/course?code={0}" class="btn btn-default course-code-btn">{0}</a>', course.Code)
                );
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

checkLogin();
loadData();