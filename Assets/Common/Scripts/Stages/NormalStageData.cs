using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStageData : IStageData
{
    public NormalStageHandler m_StageHandler = null;


    public int Row = 0;     //关卡最大行列信息
    public int Column = 0;
    public int[,,] StageData;     //储存关卡信息，包含是否有地基
    public string StageName;


    public GameObject Units;      //单元的父物体
    public List<BaseUnit> baseUnits = new List<BaseUnit>();     //放置所有的地基单元的容器
    public List<BaseUnit> fireUnits = new List<BaseUnit>();     //放置所有火焰单元
    public List<BaseUnit> oilUnits  = new List<BaseUnit>();     //放置所有油单元

    public List<GameObject> stateModel = new List<GameObject>(); //各个状态的模型



    //油关卡不一定有，但火是一定有的
    public bool isOilUpdateEnd = true;
    public bool isFireUpdateEnd = false;


    private int unitLength = 1;     //基本单位间的距离



    public NormalStageData(StageMetaData _stageData)
    {
        Column = _stageData.Column;
        Row = _stageData.Row;
        StageData = _stageData.stageMetaData;
        StageName = _stageData.Name;
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
        DetachEvent();
        fireUnits.Clear();
        oilUnits.Clear();
        isOilUpdateEnd  = true;
        isFireUpdateEnd = false;

        //依次通过对象池回收
        foreach (var unit in baseUnits)
        {
            if (unit != null)
            {
                unit.End();
            }
        }
        baseUnits.Clear();
        //Debug.Log(baseUnits.Count);
        StageSpawnPoint.SpawnPoint.TryGetValue(StageName, out GameObject spawnPoint);
        spawnPoint?.GetComponent<StageSpawnPoint>().Release();

    }




    /// <summary>
    /// 依据元数据，按坐标构建关卡
    /// </summary>
    public override void BuildStage()
    {

        IGameUnitFactory m_GameUnitFactory = GameFactory.GetGameUnitFactory();

        //开始构建之前保证上层单元容器为空，若不空说明重置关卡失败
        if (baseUnits.Count!= 0)
        {
            Debug.LogError("关卡未完全重置：baseUnit未被完全清除");
        }

        //初次构建时新建父物体
        if (Units == null)
        {
            Units = new GameObject("Units");
        }

        StageSpawnPoint.SpawnPoint.TryGetValue(StageName, out GameObject spawnPoint);
        if (spawnPoint == null)
        {
            Debug.Log("未找到生成点，已启用默认生成点");
            Units.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            Units.transform.parent = spawnPoint.transform;
            Units.transform.localPosition = new Vector3(0, 0, 0);
            Units.transform.localScale = new Vector3(1, 1, 1);
        }


        //单元坐标
        int x = 0;
        int y = 0;
        for (int i = 0; i <= Row - 1; i++)
        {
            for (int j = 0; j <= Column - 1; j++)
            {
                BaseUnit targetUnit = m_GameUnitFactory.BuildBaseUnit(this,
                        (ENUM_Build_BaseUnit)StageData[0, i, j], x, y, Units);

                GameObject targetUpperUnit = m_GameUnitFactory.BuildUpperUnit(
                        (ENUM_Build_UpperUnit)StageData[1, i, j], targetUnit);

                baseUnits.Add(targetUnit);
                x += unitLength;
            }
            y += unitLength;
            x = 0;

        }

        InitBaseUnitAroundMessage();

        //基本单元加载完再初始化关卡上的东西
        spawnPoint?.GetComponent<StageSpawnPoint>().Initialize();

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
        Game.Instance.NotifyEvent(ENUM_GameEvent.SetOilTexture);
        Game.Instance.NotifyEvent(ENUM_GameEvent.SetWaterTexture);
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
        if (x >= 0 && x <= Column - 1 && y >= 0 && y <= Row - 1)
        {
            BaseUnit targetUnit = baseUnits[y * Column + x];
            return targetUnit;
        }
        else return null;
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
