﻿function setupPage(page) {

    document.title = page.Title + ' ' + document.title;
    $('#pageTitle').text(page.Title);
    $('#pageSubtitle').text(page.Subtitle);
    if (page.RawContent) {
        $('#pageArea').html(page.Content);
    } else {
        var converter = new showdown.Converter(); // no XSS filter since page is from admin and may contain necessary scripts
        $('#pageArea').html(converter.makeHtml(page.Content));
    }

    var file;
    var i;

    if (page.JavaScriptFiles != null) {
        var jsfiles = page.JavaScriptFiles.replace('\r\n', '\n').split('\n');
        for (i in jsfiles) {
            if (jsfiles.hasOwnProperty(i)) {
                file = jsfiles[i];
                $('#pageArea').append(
                    String.format('<script src="{0}"></script>', file)
                    );
            }
        }
    }
    
    if (page.CssFiles != null) {
        var cssfiles = page.CssFiles.replace('\r\n', '\n').split('\n');
        for (i in cssfiles) {
            if (cssfiles.hasOwnProperty(i)) {
                file = cssfiles[i];
                $('head').append(
                    String.format('<link rel="stylesheet" href="{0}"></link>', file)
                    );
            }
        }
    }

    initComments('commentArea', page.CommentEntryNumber);
}

function loadData() {
    $.get(apiserver + '/api/page/page?path=' + path,
            function(data) {
                if (data.PageId != undefined) {
                    setupPage(data);
                    hideMsg();
                } else {
                    showMsg('Failed to load page: ' + data, true);
                }
            })
        .fail(function() {
            showMsg('Failed to load page.', true);
        });
}

checkLogin();
loadData();
