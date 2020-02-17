using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void EventListenerDelegate(Message evt);

public class Dispatcher : IDispatcher
{

    private static Dispatcher _instance;
    public static Dispatcher Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Dispatcher();
            return _instance;
        }
    }
 

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
                throw e;
            }

        }

    }





    public void AddListener(ENUM_GameEvent type, EventListenerDelegate listener)
    {
        AddListener((int)type, listener);
    }




    public void RemoveListener(ENUM_GameEvent type, EventListenerDelegate listener)
    {
        RemoveListener((int)type, listener);
    }




    public void SendMessage(ENUM_GameEvent type, params System.Object[] param)
    {
        SendMessage((int)type, param);
    }




}
