using System.Collections;
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
    public List<BaseUnit> oilUnits = new List<BaseUnit>();      //放置所有油单元

    //油关卡不一定有，但火是一定有的
    public bool isOilUpdateEnd = true;
    public bool isFireUpdateEnd = false;

    private int unitLength = 1;     //基本单位间的距离



    public NormalStageData(StageMetaData _stageData)
    {
        Column = _stageData.Column;
        Row = _stageData.Row;
        StageData = _stageData.stageMetaData;
        Game.Instance.RegisterGameEvent(Enum.ENUM_GameEvent.RoundUpdateBegain, new RoundUpdateBegainStageDataObserver(IsUpdateEnd));
    }

    public void SetStageHandler(NormalStageHandler stageHandler)
    {
        m_StageHandler = stageHandler;
    }


    public override void Update()
    {
        Game.Instance.NotifyGameEvent(Enum.ENUM_GameEvent.RoundUpdateBegain, null);
        foreach (var fireUnit in fireUnits)
        {
            fireUnit.myState.OnStateHandle();
        }
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
                        (Enum.ENUM_Build_BaseUnit)StageData[0, i, j], x, y);

                GameObject targetUpperUnit = m_GameUnitFactory.BuildUpperUnit(this,
                        (Enum.ENUM_Build_UpperUnit)StageData[1, i, j], targetUnit);

                if (targetUnit.myState.stateType == Enum.ENUM_State.Fire)
                    fireUnits.Add(targetUnit);

                if (targetUnit.myState.stateType == Enum.ENUM_State.Oil)
                    oilUnits.Add(targetUnit);

                baseUnits.Add(targetUnit);
                x += unitLength;
            }
            y += unitLength;
            x = 0;

        }
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



    public bool IsUpdateEnd()
    {
        if (oilUnits.Count == 0)
        {
            isOilUpdateEnd = true;
        }
        bool _isEnd = false;
        if (isOilUpdateEnd && isFireUpdateEnd)
        {
            _isEnd = true;
        }
        return _isEnd;
    }


}
