function loadNavbar() {
    $.get(apiserver + '/api/page/navbar',
        function (data) {
            var ul = $('#navbar-ul');
            data.sort(function (a, b) { return a.NavbarOrder - b.NavbarOrder });

            for (var i in data) {
                if (data.hasOwnProperty(i)) {
                    var page = data[i];
                    var li = '<li><a href="/page/' + page.Path + '">' + page.Title + '</a></li>';
                    ul.append(li);
                }
            }
        });
}

loadNavbar();