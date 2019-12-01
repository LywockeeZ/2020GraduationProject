using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnitFactory : IGameUnitFactory
{
    /// <summary>
    /// 建造下层单元基本步骤：依坐标实例地基单元->改变地基单元的状态
    /// </summary>
    /// <param name="currentStage"></param>
    /// <param name="baseType"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public override BaseUnit BuildBaseUnit(NormalStageData currentStageData, ENUM_Build_BaseUnit baseType, int x, int y, GameObject parent)
    {
        //如果枚举值为0则返回空
        if (baseType == ENUM_Build_BaseUnit.Null)
            return null;

        IAssetFactory m_AssetFactory = GameFactory.GetAssetFactory();

        //x,y坐标分别对应世界坐标下的x，z轴
        GameObject baseUnitObject = m_AssetFactory.LoadModel("BaseUnit", new Vector3(x, 0, y));
        //给所有基本单元设置一个父物体
        baseUnitObject.transform.SetParent(parent.transform);
        BaseUnit baseUnit = new BaseUnit(baseUnitObject, currentStageData);
        baseUnit.SetPosition(x, y);


        switch (baseType)
        {
            case ENUM_Build_BaseUnit.Ground:
                baseUnit.SetState(new Ground(baseUnit));
                return baseUnit;
            case ENUM_Build_BaseUnit.Fire:
                baseUnit.SetState(new Fire(baseUnit));
                return baseUnit;
            case ENUM_Build_BaseUnit.Water:
                baseUnit.SetState(new Water(baseUnit));
                return baseUnit;
            case ENUM_Build_BaseUnit.Oil:
                baseUnit.SetState(new Oil(baseUnit));
                return baseUnit;
            case ENUM_Build_BaseUnit.Block:
                baseUnit.SetState(new Block(baseUnit));
                return baseUnit;
            default:
                Debug.Log("未找到此类型的状态");
                throw new System.InvalidOperationException();
        }
    }



    /// <summary>
    /// 建造上层单元的基本步骤：获取目标地基单元->加载模型资源->挂载实例化类->初始化
    /// </summary>
    /// <param name="currentStage"></param>
    /// <param name="upperType"></param>
    /// <param name="targetUnit"></param>
    /// <returns></returns>
    public override GameObject BuildUpperUnit(NormalStageData currentStageData, ENUM_Build_UpperUnit upperType, BaseUnit targetUnit)
    {
        if (targetUnit == null) return null;

        IAssetFactory m_AssetFactory = GameFactory.GetAssetFactory();

        switch (upperType)
        {
            case ENUM_Build_UpperUnit.Null:
                return null;

            case ENUM_Build_UpperUnit.Chest:
                GameObject chestObject = m_AssetFactory.LoadModel("Chest", targetUnit.Model.transform.position);
                Chest _chest;
                _chest = chestObject.AddComponent<Chest>();
                _chest.CurrentOn = targetUnit;
                return chestObject;

            case ENUM_Build_UpperUnit.RoadBlock:
                GameObject roadBlockObject = m_AssetFactory.LoadModel("RoadBlock", targetUnit.Model.transform.position);
                RoadBlock _roadBlock;
                _roadBlock = roadBlockObject.AddComponent<RoadBlock>();
                _roadBlock.CurrentOn = targetUnit;
                return roadBlockObject;

            case ENUM_Build_UpperUnit.OilTank:
                GameObject oilTankObject = m_AssetFactory.LoadModel("OilTank", targetUnit.Model.transform.position);
                OilTank _oilTank;
                _oilTank = oilTankObject.AddComponent<OilTank>();
                _oilTank.CurrentOn = targetUnit;
                return oilTankObject;

            case ENUM_Build_UpperUnit.WaterTank:
                GameObject waterTankObject = m_AssetFactory.LoadModel("WaterTank", targetUnit.Model.transform.position);
                WaterTank _waterTank;
                _waterTank = waterTankObject.AddComponent<WaterTank>();
                _waterTank.CurrentOn = targetUnit;
                return waterTankObject;

            case ENUM_Build_UpperUnit.Player:
                GameObject playerObject = m_AssetFactory.LoadModel("Player", targetUnit.Model.transform.position);
                Player _playerUnit;
                _playerUnit = playerObject.AddComponent<Player>();
                _playerUnit.CurrentOn = targetUnit;
                Game.Instance.SetPlayerUnit(_playerUnit);
                return playerObject;

            case ENUM_Build_UpperUnit.Survivor:
                GameObject survivorObject = m_AssetFactory.LoadModel("Survivor", targetUnit.Model.transform.position);
                Survivor _survivor;
                _survivor = survivorObject.AddComponent<Survivor>();
                _survivor.CurrentOn = targetUnit;
                return survivorObject;

            default:
                Debug.Log("未找到此类型的单元");
                throw new System.InvalidOperationException();
        }
       
    }
}
