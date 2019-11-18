using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Message : IMessage
{

    public int Type { get; set; }
    public System.Object[] Params { get; set; }
    public System.Object Sender { get; set; }


    public override string ToString()
    {
        string arg = null;
        if (Params != null)
        {
            for (int i = 0; i < Params.Length; i++)
            {
                if ((Params.Length > 1 && Params.Length - 1 == i) || Params.Length == 1)
                {
                    arg += Params[i];
                }
                else
                {
                    arg += Params[i] + " , ";
                }
            }
        }

        return Type + " [ " + ((Sender == null) ? "null" : Sender.ToString()) + " ] " + " [ " + ((arg == null) ? "null" : arg.ToString()) + " ] ";
    }



    public Message Clone()
    {
        return new Message(Type, Params, Sender);
    }



    public Message(int type)
    {
        Type = type;
    }



    public Message(int type, params System.Object[] param)
    {
        Type = type;
        Params = param;
    }



    public Message(int type, System.Object sender, params System.Object[] param)
    {
        Type = type;
        Params = param;
        Sender = sender;
    }
}
