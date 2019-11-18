using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void EventListenerDelegate(Message evt);

public class Dispatcher : IDispatcher
{
    private Dictionary<int, EventListenerDelegate> events = new Dictionary<int, EventListenerDelegate>();

    public void AddListener(int type, EventListenerDelegate listener)
    {
        if (listener == null)
        {
            Debug.Log("AddListener: listener不能为空");
            return;
        }

        EventListenerDelegate myListener = null;
        events.TryGetValue(type, out myListener);
        events[type] = (EventListenerDelegate)Delegate.Combine(myListener, listener);

    }




    public void RemoveListener(int type, EventListenerDelegate listener)
    {
        if (listener == null)
        {
            Debug.Log("RemoveListener: listener不能为空");
            return;
        }
        events[type] = (EventListenerDelegate)Delegate.Remove(events[type], listener);
    }



    public void Clear()
    {
        events.Clear();
    }



    public void SendMessage(Message evt)
    {
        EventListenerDelegate listenerDelegate;
        if (events.TryGetValue(evt.Type, out listenerDelegate))
        {
            try
            {
                if (listenerDelegate != null)
                {
                    listenerDelegate(evt);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("SendMessage:"+"," +evt.Type.ToString()+","+ e.Message+","+ e.StackTrace+","+ e);
            }
        }
    }


    public void SendMessage(int type, params System.Object[] param)
    {
        EventListenerDelegate listenerDelegate;
        if (events.TryGetValue(type, out listenerDelegate))
        {
            Message evt = new Message(type, param);
            try
            {
                if (listenerDelegate != null)
                {
                    listenerDelegate(evt);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("SendMessage:" + "," + evt.Type.ToString() + "," + e.Message + "," + e.StackTrace + "," + e);
            }

        }

    }





    public void AddListener(MessageType type, EventListenerDelegate listener)
    {
        AddListener((int)type, listener);
    }



    public void AddListener(BattleEvent type, EventListenerDelegate listener)
    {
        AddListener((int)type, listener);
    }



    public void AddListener(ProtocolEvent type, EventListenerDelegate listener)

    {

        AddListener((int)type, listener);

    }



    public void RemoveListener(MessageType type, EventListenerDelegate listener)

    {

        RemoveListener((int)type, listener);

    }



    public void RemoveListener(BattleEvent type, EventListenerDelegate listener)

    {

        RemoveListener((int)type, listener);

    }



    public void RemoveListener(ProtocolEvent type, EventListenerDelegate listener)

    {

        RemoveListener((int)type, listener);

    }



    public void SendMessage(MessageType type, params System.Object[] param)

    {

        SendMessage((int)type, param);

    }



    public void SendMessage(BattleEvent type, params System.Object[] param)

    {

        SendMessage((int)type, param);

    }



    public void SendMessage(ProtocolEvent type, params System.Object[] param)

    {

        SendMessage((int)type, param);

    }

}
