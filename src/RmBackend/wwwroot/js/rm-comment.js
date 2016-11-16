
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
    var li = '<li class="comment-li" id="comment-' + id + '"></li>';
    container.append(li);
    li = $('#comment-' + id);

    var content = '<h4 id="comment-title-' + id + '"></h4>';
    var createTime = timestr(comment.CreateTime);
    var modifyTime = timestr(comment.ModifyTime);
    content += '<p class="comment-info">By <span id="comment-itsc-' + id + '">' + (comment.User == null ? 'anonymous' : comment.User.Itsc) + '</span> @ ' + createTime + '</p>';
    content += '<div>' + showdownConverter.makeHtml(comment.Content) + '</div>';
    if (createTime !== modifyTime) {
        content += '<p class="comment-modified">Modified at ' + modifyTime + '</p>';
    }
    content += '<a href="javascript:void(0) onclick="replyComment(' + id + ')">Reply</a>';

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
                insertCommentInput(area);
            }
        });
}

