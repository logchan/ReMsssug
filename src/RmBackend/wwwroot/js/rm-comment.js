
function displayComment(container, comment) {
    var id = comment.CommentId;
    var li = '<li class="comment-li" id="comment-' + id + '"></li>';
    container.append(li);
    li = $('#comment-' + id);

    var content = '<h4 id="comment-title-' + id + '"></h4>';
    var createTime = timestr(comment.CreateTime);
    var modifyTime = timestr(comment.ModifyTime);
    content += '<p class="comment-info">By ' + (comment.User == null ? 'anonymous' : comment.User.Itsc) + ' @ ' + createTime + '</p>';
    content += '<div>' + showdownConverter.makeHtml(comment.Content) + '</div>';
    if (createTime !== modifyTime) {
        content += '<p class="comment-modified">Modified at ' + modifyTime + '</p>';
    }
    content += '<a href="javascript:void(0) onclick="replyComment(' + id + ')>Reply</a>';

    li.html(content);
    $('#comment-title-' + id).text(comment.Title);

    if (comment.children != undefined) {
        li.append('<ul class="comment-ul" id="ul-comment-' + id + '"></ul>');
        for (var i in comment.children) {
            if (comment.children.hasOwnProperty(i)) {
                displayComment($('#ul-comment-' + id), comment.children[i]);
            }
        }
    }
}

function displayComments(area, data) {
    area.append('<ul class="comment-ul" id="comment-root"></ul>');
    area = $('#comment-root');

    var rootset = [];
    var i;
    for (i in data) {
        if (data.hasOwnProperty(i)) {
            var comment = data[i];
            if (comment.ParentId != null) {
                var parent = data.find(function (c) { return c.CommentId === comment.ParentId });
                if (parent != undefined) {
                    if (parent.children == undefined)
                        parent.children = [];
                    parent.children.push(comment);
                    continue;
                }
            }
            rootset.push(comment);
        }
    }

    for (i in rootset) {
        if (rootset.hasOwnProperty(i)) {
            displayComment(area, rootset[i]);
        }
    }
}

function initComments(areaId, entryId) {
    var area = $('#' + areaId);
    area.empty();

    $.get(apiserver + '/api/comment/get?entryId=' + entryId,
        function(data) {
            if ($.type(data) === 'string') {
                area.append('<p>' + data + '</p>');
            } else {
                displayComments(area, data);
            }
        });
}

