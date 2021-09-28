using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class PhotonCtrl : MonoBehaviour,IPhotonPeerListener
{
    PhotonPeer peer;

    private void Start()
    {
        peer = new PhotonPeer(this, ConnectionProtocol.Udp);
        peer.Connect("1.117.165.167:5055", "Lite");
    }

    private void Update()
    {
        peer.Service();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEvent(EventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:
                Debug.Log("链接成功");
                break;
            case StatusCode.Disconnect:
                Debug.Log("链接失败");
                break;
            case StatusCode.ExceptionOnConnect:
                Debug.Log("链接异常");
                break;
            default:
                break;
        }
    }

}
