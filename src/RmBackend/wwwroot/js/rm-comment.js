
var commentReplyTo = 0;
var commentAreaId = '';
var commentEntryId = 0;
var commetnClosed = false;

function disableCommentForm() {
    $('#commentForm :input').prop('disabled', true);
}

function enableCommentForm() {
    $('#commentForm :input').prop('disabled', false);
}

function cancelReplyComment() {
    $('#comment-content-' + commentReplyTo).removeClass('comment-highlight');

    commentReplyTo = 0;
    $('#commentReplyMsg').hide();
}

function gotoComment(id) {
    var targetY = $('#comment-title-' + id).offset().top - 70;
    window.scrollTo(window.scrollX, targetY);
}

function replyComment(id) {
    commentReplyTo = id;
    $('#comment-content-' + commentReplyTo).addClass('comment-highlight');

    var user = $('#comment-itsc-' + id).text();
    $('#commentReplyMsg').html(
            String.format('Replying to <a href="javascript:void(0)" onclick="gotoComment({0})">{1}\'s comment</a>. <a href="javascript:void(0)" onclick="cancelReplyComment()">Cancel</a>', id , user)
        );
    $('#commentReplyMsg').show();
}

function checkComment() {
    if ($('#commentContent').get(0).value.trim().length === 0) {
        $('#commentErrMsg').show();
        $('#commentErrMsg').text('Content cannot be empty.');
        return false;
    }
    if ($('#commentTitle').get(0).value.trim().length === 0) {
        $('#commentTitle').get(0).value = 'No title';
    }
    return true;
}

function postComment() {
    if (!checkComment())
        return;

    var data = $('#commentForm').serialize();
    disableCommentForm();

    if (commentReplyTo !== 0) {
        data += '&ParentId=' + commentReplyTo;
    }

    $.ajax({
        url: apiserver + '/api/comment/post',
        method: 'POST',
        data: data,
        success: function(data) {
            if (data === 'success') {
                $('#commentMsg').show();
                $('#commentMsg').text('Successfully posted the comment! Refreshing comment area...');
                setTimeout(function() { initComments(commentAreaId, commentEntryId) }, 1000);
            } else {
                $('#commentErrMsg').show();
                $('#commentErrMsg').text('Failed to post comment: ' + data);
                enableCommentForm();
            }
        },
        statusCode: {
            401: function() {
                $('#commentErrMsg').show();
                $('#commentErrMsg').text('Failed to post comment (not logged in, or session expired). Please login in and retry.');
                enableCommentForm();
            },
            500: function() {
                $('#commentErrMsg').show();
                $('#commentErrMsg').text('Failed to post comment (server error)');
                enableCommentForm();
            }
        }
    });
}

function commentColMd6(inner) {
    return String.format('<div class="row"><div class="col-md-6">{0}</div></div>', inner);
}

function insertCommentInput(area, entryId) {
    var form = '<form class="form form-horizontal" id="commentForm" onsubmit="return false">';
    form += commentColMd6('<h4>Your comment:</h4>');
    form += commentColMd6('<p class="alert alert-info" id="commentMsg" style="display: none;"></p>');
    form += commentColMd6('<p class="alert alert-danger" id="commentErrMsg" style="display: none;"></p>');
    form += commentColMd6('<p class="alert alert-warning" id="commentReplyMsg" style="display: none;"></p>');
    form += '<div class="form-group row"><div class="col-md-6">';
    form += '<input name="Title" id="commentTitle" type="text" class="form-control" placeholder="Title" />';
    form += '</div></div>';
    form += '<div class="form-group row"><div class="col-md-6">';
    form += '<textarea name="Content" id="commentContent" rows="8" class="form-control" style="resize: none"></textarea>';
    form += '</div></div>';
    form += '<div class="form-group row"><div class="checkbox col-md-6">';
    form += '<label for="commentAnonymous"><input type="checkbox" name="IsAnonymous" id="commentAnonymous" class="checkbox" value="true"/>Post as anonymous</label>';
    form += '</div></div>';
    form += '<div class="form-group row"><div class="col-md-6">';
    form += '<button type="submit" class="btn btn-primary col-md-2">Post</button>';
    form += '</div></div>';
    form += String.format('<input type="hidden" name="EntryNumber" id="commentEntryId" value="{0}" />', entryId);
    form += '</form>';

    area.append(form);
    $('#commentForm').on('submit', postComment);
}

function displayComment(container, comment) {
    var id = comment.CommentId;
    var li = String.format('<li class="comment-li" id="comment-{0}"></li>', id);
    container.append(li);
    li = $('#comment-' + id);

    
    var createTime = timestr(comment.CreateTime);
    var modifyTime = timestr(comment.ModifyTime);
    var user = '';
    if (comment.IsAnonymous) {
        user = comment.User == null ? 'anonymous' : String.format('anonymous({0})', comment.User.Itsc);
    } else {
        user = comment.User == null ? 'unknown' : comment.User.Itsc;
    }

    var content = String.format('<h4 id="comment-title-{0}"></h4>', id);
    content += String.format('<p class="comment-info">By <span id="comment-itsc-{0}">{1}</span> @ {2}</p>', id, user, createTime);
    content += String.format('<div id="comment-content-{0}">{1}</div>', id, showdownConverter.makeHtml(comment.Content));

    if (createTime !== modifyTime) {
        content += String.format('<p class="comment-modified">Modified at {0}</p>', modifyTime);
    }

    if (!commentClosed)
        content += String.format('<a href="#commentForm" onclick="replyComment({0})">Reply</a>', id);

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
    commentAreaId = areaId;
    commentEntryId = entryId;
    commentReplyTo = 0;

    var area = $('#' + areaId);
    area.empty();

    $.get(apiserver + '/api/comment/get?entryId=' + entryId,
        function(data) {
            if ($.type(data) === 'string') {
                area.append('<p>' + data + '</p>');
            } else {
                commentClosed = data.length > 0 && data[data.length - 1] == null;
                if (commentClosed)
                    data.pop();

                displayComments(area, data);

                if (commentClosed) {
                    area.append('<p>Comment closed.</p>');
                } else {
                    insertCommentInput(area, entryId);
                }
            }
        });
}

