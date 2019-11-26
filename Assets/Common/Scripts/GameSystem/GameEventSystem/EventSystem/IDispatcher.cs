
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDispatcher
{
    void AddListener(int type, EventListenerDelegate listener);

    void RemoveListener(int type, EventListenerDelegate listener);

    void SendMessage(Message evt);

    void SendMessage(int type, params System.Object[] param);

    void Clear();

}
