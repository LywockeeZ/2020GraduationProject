using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 具体的关卡类，含有对关卡内容和关卡分数（过关条件）的引用
/// </summary>
public class NormalStageHandler : IStageHandler
{
    //构建关卡的初始位置
    public int PosX = 0;
    public int PosY = 0;

    //该关卡最大行动点数
    public int RoundActionPts = 3;
    public int Rounds = 1;



    public NormalStageData  m_StatgeData  = null;      //关卡的内容，负责更新
    public NormalStageScore m_StageScore  = null;      //关卡的条件，负责判断
    public IStageHandler    m_NextHandler = null;      //下一个关卡



    //设置下一个关卡
    public override IStageHandler SetNextHandler(IStageHandler NextHandler)
    {
        m_NextHandler = NextHandler;
        return m_NextHandler;
    }




    public NormalStageHandler(NormalStageScore StageScore, NormalStageData StageData, int x, int y)
    {
        m_StageScore = StageScore;
        m_StatgeData = StageData;
        m_StageScore.SetStageHandler(this);     //设置其对当前关卡的引用
        m_StatgeData.SetStageHandler(this);
        PosX = x;
        PosY = y;
    }


    private EventListenerDelegate OnRoundBegain;
    private EventListenerDelegate OnRoundEnd;
    public void RegisterEvent()
    {
        m_StageScore.RegisterEvent();
        m_StatgeData.RegisterEvent();

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
        OnRoundBegain = (Message evt) =>
        {
            //这里委托里的Rounds与类中的的Rounds并不是同一个，这里的是被封装到委托方法中去的
            //Debug.Log("Rounds:" + Rounds);
            GUIManager.Instance.SetRoundsText(Rounds++);
        });

    }



    public void DetachEvent()
    {
        m_StageScore.DetachEvent();
        m_StatgeData.DetachEvent();
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundBegain, OnRoundBegain);
    }




    /// <summary>
    /// 判断关卡是否结束,并返回下一关
    /// </summary>
    /// <returns></returns>
    public override IStageHandler CheckStage()
    {
        if (m_StageScore.CheckStage())
            return m_NextHandler;
        else
            return this;
    }




    public override void Update()
    {
        //Debug.Log("AP:"+Game.Instance.GetCurrentAP());
        //Debug.Log("IsCanInput:"+Game.Instance.GetCanInput());
        //Debug.Log("FireCounts: " + m_StatgeData.GetFireCounts());

        //判断回合是否结束
        m_StageScore.CheckRound();
    }




    public override void Reset()
    {
        DetachEvent();
        m_StatgeData.Reset();
        Rounds = 1;
    }




    public override void Start()
    {
        Debug.Log("BuildStart");
        RegisterEvent();
        m_StatgeData.BuildStage(PosX, PosY);
        Game.Instance.SetMaxAP(RoundActionPts);
    }




    public override BaseUnit GetBaseUnit(int x, int y)
    {
        return m_StatgeData.GetBaseUnit(x, y);
    }

}
