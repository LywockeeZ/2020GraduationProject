﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStageData : IStageData
{
    public NormalStageHandler m_StageHandler = null;


    public int Row = 0;     //关卡最大行列信息
    public int Column = 0;
    public int[,,] StageData;     //储存关卡信息，包含是否有地基


    public List<BaseUnit> baseUnits = new List<BaseUnit>();     //放置所有的地基单元的容器
    public List<BaseUnit> fireUnits = new List<BaseUnit>();     //放置所有火焰单元
    public List<BaseUnit> oilUnits  = new List<BaseUnit>();     //放置所有油单元


    //油关卡不一定有，但火是一定有的
    public bool isOilUpdateEnd = true;
    public bool isFireUpdateEnd = false;


    private int unitLength = 1;     //基本单位间的距离



    public NormalStageData(StageMetaData _stageData)
    {
        Column = _stageData.Column;
        Row = _stageData.Row;
        StageData = _stageData.stageMetaData;
        RegisterEvent();
    }




    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateBegain,
        delegate (Message evt)
        {
            Game.Instance.SetCanInput(false);
            CoroutineManager.StartCoroutineTask(Update, 1f);
            CoroutineManager.StartCoroutineTask(IsUpdateEnd, ENUM_GameEvent.RoundUpdateEnd, 1f);
        });
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateEnd,
        delegate (Message evt)
        {
            ResetFireUpdateState();
            Game.Instance.NotifyEvent(ENUM_GameEvent.RoundEnd, null);
        });


    }




    public void SetStageHandler(NormalStageHandler stageHandler)
    {
        m_StageHandler = stageHandler;
    }




    public override void Update()
    {
        //通知火焰开始更新
        Game.Instance.NotifyEvent(ENUM_GameEvent.FireUpdate, null);
    }




    public override void Reset()
    {
        throw new System.NotImplementedException();
    }




    /// <summary>
    /// 依据元数据，按坐标构建关卡
    /// </summary>
    public override void BuildStage()
    {
        IGameUnitFactory m_GameUnitFactory = GameFactory.GetGameUnitFactory();

        //单元坐标
        int x = 0;
        int y = 0;
        for (int i = 0; i <= Row - 1; i++)
        {
            for (int j = 0; j <= Column - 1; j++)
            {
                BaseUnit targetUnit = m_GameUnitFactory.BuildBaseUnit(this,
                        (ENUM_Build_BaseUnit)StageData[0, i, j], x, y);

                GameObject targetUpperUnit = m_GameUnitFactory.BuildUpperUnit(this,
                        (ENUM_Build_UpperUnit)StageData[1, i, j], targetUnit);

                if (targetUnit.myState.stateType == ENUM_State.Fire)
                    fireUnits.Add(targetUnit);

                if (targetUnit.myState.stateType == ENUM_State.Oil)
                    oilUnits.Add(targetUnit);

                baseUnits.Add(targetUnit);
                x += unitLength;
            }
            y += unitLength;
            x = 0;

        }

        InitBaseUnitAroundMessage();
    }




    //初始化基本单元四周信息
    public void InitBaseUnitAroundMessage()
    {
        for (int i = 0; i < baseUnits.Count; i++)
        {
            if (baseUnits[i] != null)
            {
                baseUnits[i].GetAround();
            }
        }
    }




    public override BaseUnit GetBaseUnit(int x, int y)
    {
        BaseUnit targetUnit = baseUnits[y * Column + x];
        return targetUnit;
    }




    public int GetFireCounts()
    {
        return fireUnits.Count;
    }




    //用来判定回合是否更新结束
    public bool IsUpdateEnd()
    {
        if (oilUnits.Count == 0)
            isOilUpdateEnd = true;

        if (fireUnits.Count == 0)
            isFireUpdateEnd = true;

        bool _isEnd = false;
        if (isOilUpdateEnd && isFireUpdateEnd)
        {
            _isEnd = true;
        }
        return _isEnd;
    }




    //每次回合更新结束，火焰更新状态都要手动重置
    public void ResetFireUpdateState()
    {
        isFireUpdateEnd = false;
    }



}
