using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatRoom : MonoBehaviour
{
    public static ChatRoom instance;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        } else
        {
            instance = this;
        }
    }

    public Transform userList;
    public Transform messageList;
    public TMPro.TMP_InputField input;
    public GameObject sendButton;

    int framesPerUpdate = 60;
    int currentFrameCount = 0;

    public void ClearList(Transform _list)
    {
        int childCount = _list.childCount;
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(_list.GetChild(0).gameObject);
        }
    }

    public GameObject AddToList(Transform _list, GameObject _obj)
    {
        GameObject newItem = Instantiate(_obj);
        newItem.transform.SetParent(_list);
        return newItem;
    }

    public void DrawUsers(string[] _users)
    {
        ClearList(userList);
        GameObject obj = Resources.Load<GameObject>("Prefabs/User");
        for (int i = 0; i < _users.Length; i++)
        {
            GameObject user = AddToList(userList, obj);
            user.GetComponent<TextItem>().SetText(_users[i]);
            if (User.instance.username == _users[i])
            {
                user.GetComponent<TextItem>().display.GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
            }
        }
    }

    public void DrawMessages(string[] _messages)
    {
        ClearList(messageList);
        GameObject obj = Resources.Load<GameObject>("Prefabs/Message");
        for (int i = 0; i < _messages.Length; i++)
        {
            GameObject message = AddToList(messageList, obj);
            message.GetComponent<TextItem>().SetText(_messages[i]);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        ClearList(userList);
        ClearList(messageList);
        StartCoroutine(UpdateChatRoom());
        input.ActivateInputField();
    }

    private void FixedUpdate()
    {
        currentFrameCount++;
        if (currentFrameCount >= framesPerUpdate)
        {
            currentFrameCount = 0;
            StartCoroutine(UpdateChatRoom());
        }
    }

    IEnumerator UpdateChatRoom()
    {
        proceed = false;
        Request.instance.Send(Command.GETUSERLIST);
        while (proceed == false)
        {
            yield return null;
        }

        proceed = false;
        Request.instance.Send(Command.GETCHATMESSAGES);
        while (proceed == false)
        {
            yield return null;
        }
    }

    public bool proceed = false;

    public void SendChatMessage()
    {
        if (input.text == "")
        {
            return;
        }

        StartCoroutine(ISendChatMessage());
    }

    IEnumerator ISendChatMessage()
    {
        sendButton.SetActive(false);
        input.gameObject.SetActive(false);

        proceed = false;
        Request.instance.Send(Command.SENDCHATMESSAGE);
        while (proceed == false)
        {
            yield return null;
        }
        proceed = false;
        Request.instance.Send(Command.GETCHATMESSAGES);
        while (proceed == false)
        {
            yield return null;
        }

        input.gameObject.SetActive(true);
        input.text = "";
        input.ActivateInputField();
        sendButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (sendButton.activeSelf)
            {
                SendChatMessage();
            }
        }
    }

    private void OnApplicationQuit()
    {
        Request.instance.Send(Command.LEAVE);
    }

}
