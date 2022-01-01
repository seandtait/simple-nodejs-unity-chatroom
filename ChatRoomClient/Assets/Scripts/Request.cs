using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Request : MonoBehaviour
{
    const string URL = "http://192.168.0.19:8000/";
    public static Request instance;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        } else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void Send(Command cmd)
    {
        switch(cmd)
        {
            case Command.JOIN:
                StartCoroutine(Join());
                break;
            case Command.GETUSERLIST:
                StartCoroutine(GetUserList());
                break;
            case Command.GETCHATMESSAGES:
                StartCoroutine(GetChatMessages());
                break;
            case Command.LEAVE:
                StartCoroutine(LeaveChat());
                break;
            case Command.SENDCHATMESSAGE:
                StartCoroutine(SendChatMessage());
                break;
        }
    }

    string response = "";

    public IEnumerator Get(string url, List<string> parameters)
    {
        response = "";

        string finalUrl = MakeURL(url, parameters);
        //Debug.Log("Final url: " + finalUrl);
        UnityWebRequest www = UnityWebRequest.Get(finalUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
            if (LoginPanel.instance != null)
            {
                LoginPanel.instance.SetError(www.error);
            }
        } else
        {
            response = www.downloadHandler.text;
        }
    }

    public IEnumerator Get(string url)
    {
        yield return StartCoroutine(Get(url, new List<string>()));
    }

    string MakeURL(string _url, List<string> _parameters)
    {
        if (_parameters.Count <= 0)
        {
            return _url;
        }

        foreach (var parameter in _parameters)
        {
            _url += parameter + "/";
        }
        return _url;
    }

    IEnumerator Join()
    {
        yield return Get(MakeURL(URL + "join/", new List<string>() { LoginPanel.instance.usernameField.text }));

        if (response == "")
        {
            User.instance.username = "";
        } else
        {
            User.instance.username = JsonUtility.FromJson<JoinResponse>(response).username;
        }

        
        if (User.instance.username == "alreadytaken")
        {
            // Join request failed
            User.instance.username = "";
            LoginPanel.instance.usernameField.text = "";
            LoginPanel.instance.SetError("Join request failed: try a different username");
            LoginPanel.instance.usernameField.ActivateInputField();
        } else
        {
            SceneManager.LoadScene("SceneChat");
        }
    }

    IEnumerator GetUserList()
    {
        yield return Get(MakeURL(URL + "getuserlist/", new List<string>()));
        ChatRoom.instance.DrawUsers(JsonUtility.FromJson<GetStringArrayResponse>(response).stringArray);
        ChatRoom.instance.proceed = true;
    }

    IEnumerator GetChatMessages()
    {
        yield return Get(MakeURL(URL + "getchatmessages/", new List<string>()));
        ChatRoom.instance.DrawMessages(JsonUtility.FromJson<GetStringArrayResponse>(response).stringArray);
        ChatRoom.instance.proceed = true;
    }

    IEnumerator SendChatMessage()
    {
        yield return Get(MakeURL(URL + "sendchatmessage/", new List<string>() { User.instance.username, ChatRoom.instance.input.text }));
        ChatRoom.instance.proceed = true;
    }

    IEnumerator LeaveChat()
    {
        yield return Get(MakeURL(URL + "leavechat/", new List<string>() { User.instance.username }));

    }


    class JoinResponse
    {
        public string username;
    }

    class GetStringArrayResponse
    {
        public string[] stringArray;
    }

}
