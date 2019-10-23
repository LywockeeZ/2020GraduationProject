using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//主要负责构建关卡并推进关卡
public class Stage 
{
    //关卡最大行列信息
    public int Row = 0;
    public int Column = 0;
    //储存关卡信息，包含是否有地基
    public int[,] StageMessage;
    //玩家初始化的位置
    public Vector2 startPos = Vector2.zero;
    //放置所有的地基单元的容器
    public List<BaseUnit> baseUnits = new List<BaseUnit>();
    //放置所有火焰单元
    public List<BaseUnit> fireUnits = new List<BaseUnit>();
    private GameObject BaseUnit;
    private Vector3 buildPos = Vector3.zero;
    private int unitLength = 1;

    public Stage(int _row , int _column , int[,] _stageMessage)
    {
        Row = _row;
        Column = _column;
        StageMessage = _stageMessage;
        OnStageBegin();
    }

    public void OnStageBegin()
    {
        BaseUnit = Resources.Load("Prefabs/BaseUnit") as GameObject;
        BuildStage();
        InitBaseUnitAroundMessage();
    }

    public void OnStageUpdate()
    {
        int counts = fireUnits.Count;
        for (int i = 0; i < counts; i++)
        {
            if (fireUnits[i] != null)
            {
                fireUnits[i].StateRequest();
                Debug.Log("i:" + i);

            }
        }
    }

    public void OnStageEnd()
    {

    }

    public void FindUnitAndSetState(int x, int y, State state)
    {

    }

    public void BuildStage()
    {
        //资源加载的目标坐标
        int x = 0;
        int y = 0;
        for (int i = 0; i <= Row -1; i++)
        {
            for (int j = 0; j <= Column -1; j++)
            {
                if (StageMessage[i, j] != 0)
                {
                    //x,y坐标分别对应世界坐标下的x，z轴
                    var gameObject = GameObject.Instantiate(BaseUnit, new Vector3(x, 0, y), Quaternion.identity);
                    var thisUnit = new BaseUnit(gameObject, this);
                    //将位置信息设置给单元
                    thisUnit.SetPosition(x, y);
                    baseUnits.Add(thisUnit);
                }
                else baseUnits.Add(null);
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

    public BaseUnit GetBaseUnit(int x, int y)
    {
        BaseUnit targetUnit = baseUnits[y * Column + x];
        return targetUnit;
    }

    public void SetStartPos(Vector2 pos)
    {
        startPos = pos;
    }
}
