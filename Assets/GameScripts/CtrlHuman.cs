using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlHuman : BaseHuman
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        NetManager.Send("List|" + NetManager.GetDesc());
    }

    new void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit);
            if (hit.collider.CompareTag("Terrain"))
            {
                targetPos = hit.point;
                string sendStr = "Move|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += hit.point.x + ",";
                sendStr += hit.point.y + ",";
                sendStr += hit.point.z + ",";
                NetManager.Send(sendStr);
            }
        }
    }

}
