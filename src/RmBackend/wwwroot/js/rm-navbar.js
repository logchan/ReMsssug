function loadNavbar() {
    $.get(apiserver + '/api/page/navbar',
        function (data) {
            var ul = $('#navbar-ul');
            data.sort(function (a, b) { return a.NavbarOrder - b.NavbarOrder });

            for (var i in data) {
                if (data.hasOwnProperty(i)) {
                    var page = data[i];
                    var li = String.format('<li><a href="/page/{0}">{1}</a></li>', page.Path, page.Title);
                    ul.append(li);
                }
            }
        });
}

loadNavbar();