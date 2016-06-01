$(function () {
    chatInitialized = false;
    setScreen(false);
    $("#ChatRoom").slideToggle();
    var chatHub = $.connection.chatHub;
    registerClientMethods(chatHub);
    $.connection.hub.start().done(function () {
        registerEvents(chatHub);
        chatHub.server.connect($('#hdUserName').val());
    });
});
function setScreen(isLogin) {
    if (!isLogin) {
        $("#divChat").hide();
        $("#divLogin").show();
    }
    else {
        $("#divChat").show();
        $("#divLogin").hide();
    }
}
function registerEvents(chatHub) {
    $('#btnSendMsg').click(function () {
        var msg = $("#txtMessage").val();
        if (msg.length > 0) {
            var userName = $('#hdUserName').val();
            chatHub.server.sendMessageToAll(userName, msg);
            $("#txtMessage").val('');
        }
    });
    $("#txtNickName").keypress(function (e) {
        if (e.which == 13) {
            $("#btnStartChat").click();
        }
    });
    $("#txtMessage").keypress(function (e) {
        if (e.which == 13) {
            $('#btnSendMsg').click();
        }
    });
    $("#ChatTitle").click(function () {
        $("#ChatTitle").html("Chat Room");
        $("#ChatRoom").slideToggle();
        var objDiv = document.getElementById("divChatWindow");
        objDiv.scrollTop = objDiv.scrollHeight;
    });
}
function registerClientMethods(chatHub) {
    chatHub.client.onConnected = function (userName, allUsers, messages) {
        if (chatInitialized) {
            return;
        }
        chatInitialized = true;
        $('#hdUserName').val(userName);
        for (i = 0; i < allUsers.length; i++) {
            AddUser(chatHub, allUsers[i]);
        }
        for (i = 0; i < messages.length; i++) {
            var message = messages[i];
            AddMessage(message.UserName, message.Message, message.Time);
        }
        $("#ChatTitle").html("Chat Room");
        setScreen(true);
    }
    chatHub.client.onNewUserConnected = function (name) {
        AddUser(chatHub, name);
    }
    chatHub.client.onUserDisconnected = function (userName) {
        $('#' + userName).remove();
        var ctrId = 'private_' + userName;
        $('#' + ctrId).remove();
    }
    chatHub.client.messageReceived = function (userName, message, time) {
        AddMessage(userName, message, time);
    }
    chatHub.client.sendPrivateMessage = function (fromUserName, toUserName, message, isMine) {
        var ctrId;
        if (isMine) {
            ctrId = 'private_' + toUserName;
        } else {
            ctrId = 'private_' + fromUserName;
        }
        if ($('#' + ctrId).length == 0) {
            createPrivateChatWindow(chatHub, ctrId, fromUserName);
        }

        $('#' + ctrId).find('#divMessage').append('<div class="message"><span class="userName">' + fromUserName + '</span>: ' + message + '</div>');
        var height = $('#' + ctrId).find('#divMessage')[0].scrollHeight;
        $('#' + ctrId).find('#divMessage').scrollTop(height);
    }
}
function AddUser(chatHub, name) {
    var userId = $('#hdUserName').val();
    var code;
    if (userId == name) {
        code = $('<div class="loginUser">' + name + "</div>");
    }
    else {
        if ($("#ChatRoom").css('display') == 'none') {
            $("#ChatTitle").html("Chat Room - A new user just arrived !");
        }
        code = $('<div id="' + name + '" class="user" >' + name + '<div>');
        $(code).click(function () {
            if (userId != name)
                OpenPrivateChatWindow(chatHub, name);
        });
    }
    $("#divusers").append(code);
}
function AddMessage(userName, message, time) {
    if ($("#ChatRoom").css('display') == 'none') {
        $("#ChatTitle").html("Chat Room - You have unread messages !");
    }
    $('#divChatWindow').append('<div class="message"><span class="userName">' + userName + ' (' + time + ')</span>: ' + message + '</div>');
    var height = $('#divChatWindow')[0].scrollHeight;
    $('#divChatWindow').scrollTop(height);
}
function OpenPrivateChatWindow(chatHub, userName) {
    var ctrId = 'private_' + userName;
    if ($('#' + ctrId).length > 0) return;
    createPrivateChatWindow(chatHub, ctrId, userName);
}
function createPrivateChatWindow(chatHub, ctrId, userName) {
    var div = '<div id="' + ctrId + '" class="ui-widget-content draggable" rel="0">' +
               '<div class="header">' +
                  '<div  style="float:right;">' +
                      '<img id="imgDelete"  style="cursor:pointer;" src="/fonts/delete.png"/>' +
                   '</div>' +
                   '<span class="selText" rel="0">' + userName + '</span>' +
               '</div>' +
               '<div id="divMessage" class="messageArea">' +
               '</div>' +
               '<div class="buttonBar">' +
                  '<input id="txtPrivateMessage" class="msgText" type="text"   />' +
                  '<input id="btnSendMessage" class="btn btn-primary" type="button" value="Send"   />' +
               '</div>' +
            '</div>';
    var $div = $(div);
    $div.find('#imgDelete').click(function () {
        $('#' + ctrId).remove();
    });
    $div.find("#btnSendMessage").click(function () {
        $textBox = $div.find("#txtPrivateMessage");
        var msg = $textBox.val();
        if (msg.length > 0) {
            chatHub.server.sendPrivateMessage(userName, msg);
            $textBox.val('');
        }
    });
    $div.find("#txtPrivateMessage").keypress(function (e) {
        if (e.which == 13) {
            $div.find("#btnSendMessage").click();
        }
    });
    AddDivToContainer($div);
    $div.find("#txtPrivateMessage").focus();
}
function AddDivToContainer($div) {
    $('#divContainer').prepend($div);
    $div.draggable({
        handle: ".header",
        stop: function () {

        }
    });
}
