using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStageData : IStageData
{
    public NormalStageHandler m_StageHandler = null;


    public int Row = 0;     //关卡最大行列信息
    public int Column = 0;
    public int[,,] StageData;     //储存关卡信息，包含是否有地基


    public GameObject Units;
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
    }



    private EventListenerDelegate OnRoundUpdateBegain;
    private EventListenerDelegate OnRoundUpdateEnd;
    public void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateBegain,
        OnRoundUpdateBegain = (Message evt) =>
        {
            Game.Instance.SetCanInput(false);
            CoroutineManager.StartCoroutineTask(Update, 1f);
            CoroutineManager.StartCoroutineTask(IsUpdateEnd, ENUM_GameEvent.RoundUpdateEnd, 1f);
        });
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateEnd,
        OnRoundUpdateEnd = (Message evt) =>
        {
            ResetFireUpdateState();
            Game.Instance.NotifyEvent(ENUM_GameEvent.RoundEnd, null);
        });


    }




    public void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundUpdateBegain, OnRoundUpdateBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundUpdateEnd, OnRoundUpdateEnd);
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
        ClearBaseUnitAroundMessage();
        baseUnits.Clear();
        fireUnits.Clear();
        oilUnits.Clear();
        isOilUpdateEnd  = true;
        isFireUpdateEnd = false;
        GameObject.Destroy(Units);
}




    /// <summary>
    /// 依据元数据，按坐标构建关卡
    /// </summary>
    public override void BuildStage()
    {
        IGameUnitFactory m_GameUnitFactory = GameFactory.GetGameUnitFactory();

        Units = new GameObject("Units");
        Units.transform.position = Vector3.zero;

        //单元坐标
        int x = 0;
        int y = 0;
        for (int i = 0; i <= Row - 1; i++)
        {
            for (int j = 0; j <= Column - 1; j++)
            {
                BaseUnit targetUnit = m_GameUnitFactory.BuildBaseUnit(this,
                        (ENUM_Build_BaseUnit)StageData[0, i, j], x, y, Units);

                GameObject targetUpperUnit = m_GameUnitFactory.BuildUpperUnit(this,
                        (ENUM_Build_UpperUnit)StageData[1, i, j], targetUnit);
                if (targetUpperUnit != null)
                {
                    //给所有上层物体设置一个父物体
                    targetUpperUnit.transform.SetParent(Units.transform);
                }

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

    //清除单元对周围单元的引用
    public void ClearBaseUnitAroundMessage()
    {
        for (int i = 0; i < baseUnits.Count; i++)
        {
            if (baseUnits[i] != null)
            {
                baseUnits[i].ClearAround();
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
