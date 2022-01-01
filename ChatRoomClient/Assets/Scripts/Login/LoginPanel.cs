using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    public static LoginPanel instance;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        } else
        {
            instance = this;
            Screen.SetResolution(960, 810, false);
            Application.runInBackground = true;
        }
    }

    public TMPro.TMP_InputField usernameField;
    public TMPro.TextMeshProUGUI errorField;

    public void ClickedJoin()
    {
        SetError("");
        if (usernameField.text == "")
        {
            SetError("Enter a username");
            return;
        }

        Request.instance.Send(Command.JOIN);
    }

    public void SetError(string _error)
    {
        errorField.text = _error;
    }

    private void Start()
    {
        SetError("");
        usernameField.ActivateInputField();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            ClickedJoin();
        }
    }

}
