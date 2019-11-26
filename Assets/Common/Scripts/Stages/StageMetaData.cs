using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存放从XML文件中读取的关卡元数据的包装类,增加了行,列信息
/// </summary>
public class StageMetaData
{
    public int Row = 0;
    public int Column = 0;
    public int[,,] stageMetaData;

    public StageMetaData(int[,,] _stageMetaData, int row, int column)
    {
        stageMetaData = _stageMetaData;
        Row = row;
        Column = column;
    }
}
