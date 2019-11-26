using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IMessage
{
    /// <summary>
    /// 事件类型，Key
    /// </summary>
    int Type { get; set; }



    /// <summary>
    /// 发送者
    /// </summary>
    System.Object Sender { get; set; }



    /// <summary>
    /// 参数
    /// </summary>
    System.Object[] Params { get; set; }



    /// <summary>
    /// 转字符串
    /// </summary>
    /// <returns></returns>
    string ToString();

}
