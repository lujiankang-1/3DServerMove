using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCtrl : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;

    public Dictionary<string, BaseHuman> players = new Dictionary<string, BaseHuman>();

    private void Start()
    {
        EventDispatcher.instance.AddListener<string>("Enter", OnEnter);
        EventDispatcher.instance.AddListener<string>("Move", OnMove);
        EventDispatcher.instance.AddListener<string>("Leave", OnLeave);
        EventDispatcher.instance.AddListener<string>("List", OnList);
        EventDispatcher.instance.AddListener<string>("Move", OnMove);
    }
    public void OnDestroy()
    {
        EventDispatcher.instance.RemoveListener<string>("Enter", OnEnter);
        EventDispatcher.instance.RemoveListener<string>("Move", OnMove);
        EventDispatcher.instance.RemoveListener<string>("Leave", OnLeave);
        EventDispatcher.instance.RemoveListener<string>("List", OnList);
        EventDispatcher.instance.RemoveListener<string>("Move", OnMove);
    }

    public void OnList(string msgArgs)
    {
        //Debug.Log("OnList" + msgArgs);
        string[] split = msgArgs.Split(',');
        int count = (split.Length - 1) / 6;
        for(int i = 0; i < count; i++)
        {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            if(desc == NetManager.GetDesc())
            {
                continue;
            }
            GameObject obj = Instantiate(Enemy);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            var human = obj.GetComponent<SyncHuman>();
            human.desc = desc;
            players.Add(desc, human);
        }
    }
    public void OnEnter(string msgArgs)
    {
        //Debug.Log("OnEnter" + msgArgs);
        string[] split = msgArgs.Split(',');

        string desc = split[0];
        
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        //int hp = int.Parse(split[5]);
        GameObject obj;
        if (desc == NetManager.GetDesc())
        {
            obj = Instantiate(Player);
        }
        else
        {
            obj = Instantiate(Enemy);
        }

        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0, eulY, 0);

        var human = obj.GetComponent<BaseHuman>();
        human.desc = desc;

        players.Add(desc, human);
    }

    public void OnMove(string msgArgs)
    {
        string[] split = msgArgs.Split(',');

        string desc = split[0];

        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        if (!players.ContainsKey(desc))
        {
            return;
        }
        Vector3 pos = new Vector3(x, y, z);
        players[desc].MoveTo(pos);
    }

    public void OnLeave(string msgArgs)
    {
        string[] split = msgArgs.Split(',');

        string desc = split[0];

        if (!players.ContainsKey(desc))
        {
            return;
        }
        Destroy(players[desc].gameObject);
        players.Remove(desc);
    }
}
