// Entry point for the application
const express = require("express");
const app = express();

const numOfMessagesAllowed = 21;

var users = [];
var messages = [];

function AddMessage(msg) {
    var message = "[" + GetTime() + "] " + msg;
    messages.push(message);
    console.log(message);

    TrimMessages();
}

function TrimMessages() {
    if(messages.length > numOfMessagesAllowed) {
        messages.splice(0, 1);
    }
}

function CheckUserExists(username) {
    if(users.includes(username)) {
        return true;
    }
    return false;
}

function AddNewUser(username) {
    users.push(username);
    return true;
}

function RemoveUser(username) {
    users = users.filter(function(user) {
        return user !== username;
    });
}

function GetTime() {
    let currentDate = new Date();
    let time = currentDate.getHours() + ":" + currentDate.getMinutes() + ":" + currentDate.getSeconds();
    return time;
}

// Welcome Message
app.get("/", (req, res) => {
    res.send("Welcome to the chat room!");
});

// Joined
app.get("/join/:username", (req, res) => {
    var username = req.params['username'];
    var alreadytaken = CheckUserExists(username);

    if(alreadytaken) {
        // username already taken, reject the join request
        res.json({
            "username": "alreadytaken",
        })
    } else if(true == false) {

    } else {
        // All is well, join the room
        AddNewUser(username);
        var joinedText = username + " has joined the chat room."
        AddMessage(joinedText);
        res.json({
            "username": username,
        });

        
    }
});

// Return a string array of all user names
app.get("/getuserlist/", (req, res) => {
    res.json({
        "stringArray": users,
    });
});

// Return a string array of all messages
app.get("/getchatmessages/", (req, res) => {
    res.json({
        "stringArray": messages,
    });
});

// Receive a message
app.get("/sendchatmessage/:user/:message", (req, res) => {
    var u = req.params['user'];
    var m = req.params['message'];
    var message = u + ": " + m;
    AddMessage(message);
    res.send("OK");
});

// Leave chat
app.get("/leavechat/:username", (req, res) => {
    var username = req.params['username'];
    RemoveUser(username);
    console.log("User, " + username + ", has left the chat room.");
    res.send("OK");
});



app.listen(8000, () => {
    console.log("Server has started.");
});
