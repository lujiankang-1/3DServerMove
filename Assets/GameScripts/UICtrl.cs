using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICtrl : MonoBehaviour
{
    //添加两个按钮用于发送数据和关闭连接
    public Button sen;
    public Button col;
    public Button Contect;
    //输入文本
    public InputField inputText;
    // Start is called before the first frame update
    void Start()
    {
        Contect.onClick.AddListener(ConnectServer);
        sen.onClick.AddListener(send_smg);//发送
        col.onClick.AddListener(close_btnClick);//关闭
    }

    public void ConnectServer()
    {
        NetManager.ConnectServer();
        Contect.gameObject.SetActive(false);
    }

    public void send_smg()
    {
        NetManager.Send(inputText.text);
    }
    public void close_btnClick()
    {
        NetManager.close();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
