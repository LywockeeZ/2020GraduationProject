using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏单元工厂的操作界面，使用了工厂模式与建造者模式
/// </summary>
public abstract class IGameUnitFactory
{
    public abstract BaseUnit BuildBaseUnit(NormalStageData currentStageData, ENUM_Build_BaseUnit baseType, int x, int y);

    public abstract GameObject BuildUpperUnit(NormalStageData currentStageData, ENUM_Build_UpperUnit upperType, BaseUnit targetUnit);
}
