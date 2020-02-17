using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存放从XML文件中读取的关卡元数据的包装类,增加了行,列信息
/// </summary>
public class StageMetaData
{
    public string Name;
    public int Type = 0;
    public int Row = 0;
    public int Column = 0;
    public int[,,] stageMetaData;

    public StageMetaData(int[,,] _stageMetaData, string name, int row, int column, int type)
    {
        stageMetaData = _stageMetaData;
        Name = name;
        Row = row;
        Column = column;
        Type = type;
    }
}
