using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStageData : IStageData
{
    public int Row = 0;     //关卡最大行列信息
    public int Column = 0;
    public int[,,] StageData;     //储存关卡信息，包含是否有地基
    public Vector2 startPos = Vector2.zero;     //玩家初始化的位置

    public List<BaseUnit> baseUnits = new List<BaseUnit>();     //放置所有的地基单元的容器
    public List<BaseUnit> fireUnits = new List<BaseUnit>();     //放置所有火焰单元
    public List<BaseUnit> oilUnits = new List<BaseUnit>();      //放置所有油单元

    private int unitLength = 1;     //基本单位间的距离

    public NormalStageData(int _row, int _column, int[,,] _stageData)
    {
        Column = _column;
        Row = _row;
        StageData = _stageData;
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }


    public override bool IsFinished()
    {
        throw new System.NotImplementedException();
    }

    public override void Reset()
    {
        throw new System.NotImplementedException();
    }

    public void BuildStage()
    {
        GameObject BaseUnit = Resources.Load("Prefabs/BaseUnit") as GameObject;

        //资源加载的目标坐标
        int x = 0;
        int y = 0;
        for (int i = 0; i <= Row - 1; i++)
        {
            for (int j = 0; j <= Column - 1; j++)
            {
                if (StageData[0, i, j] != 0)
                {
                    //x,y坐标分别对应世界坐标下的x，z轴
                    var gameObject = GameObject.Instantiate(BaseUnit, new Vector3(x, 0, y), Quaternion.identity);
                    var thisUnit = new BaseUnit(gameObject, m_StageHandler);
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

    public override BaseUnit GetBaseUnit(int x, int y)
    {
        BaseUnit targetUnit = baseUnits[y * Column + x];
        return targetUnit;
    }

    public void SetStartPos(Vector2 pos)
    {
        startPos = pos;
    }

}
