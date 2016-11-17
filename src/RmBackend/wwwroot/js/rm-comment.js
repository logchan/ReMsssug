
function replyComment(id) {
    
}

function insertCommentInput(area) {
    var form = '<form class="form form-horizontal" id="commentForm" onsubmit="return false">';
    form += '<div class="row"><div class="col-md-6"><h4>Your comment:</h4></div></div>';
    form += '<div class="row"><div class="col-md-6"><p id="commentIndicator"></p></div></div>';
    form += '<div class="form-group row"><div class="col-md-6">';
    form += '<input name="Title" id="commentTitle" type="text" class="form-control" placeholder="Title" />';
    form += '</div></div>';
    form += '<div class="form-group row"><div class="col-md-6">';
    form += '<textarea name="Content" id="commentContent" rows="8" class="form-control" style="resize: none"></textarea>';
    form += '</div></div>';
    form += '<div class="form-group row"><div class="col-md-6">';
    form += '<button type="submit" class="btn btn-primary col-md-2">Post</button>';
    form += '</div></div>';
    form += '</form>';

    area.append(form);
}

function displayComment(container, comment) {
    var id = comment.CommentId;
    var li = String.format('<li class="comment-li" id="comment-{0}"></li>', id);
    container.append(li);
    li = $('#comment-' + id);

    
    var createTime = timestr(comment.CreateTime);
    var modifyTime = timestr(comment.ModifyTime);
    var user = (comment.User == null ? 'anonymous' : comment.User.Itsc);

    var content = String.format('<h4 id="comment-title-{0}"></h4>', id);
    content += String.format('<p class="comment-info">By <span id="comment-itsc-{0}">{1}</span> @ {2}</p>', id, user, createTime);
    content += String.format('<div>{0}</div>', showdownConverter.makeHtml(comment.Content));

    if (createTime !== modifyTime) {
        content += String.format('<p class="comment-modified">Modified at {0}</p>', modifyTime);
    }
    content += String.format('<a href="javascript:void(0) onclick="replyComment({0})">Reply</a>', id);

    li.html(content);
    $('#comment-title-' + id).text(comment.Title);

    var ulid = 'ul-comment-' + id;
    if (comment.children != undefined) {
        li.append(String.format('<ul class="comment-ul" id="{0}"></ul>', ulid));
        for (var i in comment.children) {
            if (comment.children.hasOwnProperty(i)) {
                displayComment($('#' + ulid), comment.children[i]);
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
                insertCommentInput(area);
            }
        });
}

