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
    public override BaseUnit BuildBaseUnit(IStageHandler currentStage, Enum.ENUM_Build_BaseUnit baseType, int x, int y)
    {
        GameObject BaseUnitResource = Resources.Load("Prefabs/BaseUnit") as GameObject;
        GameObject baseUnitObject = GameObject.Instantiate(BaseUnitResource, new Vector3(x, 0, y), Quaternion.identity);
        BaseUnit baseUnit = new BaseUnit(baseUnitObject, currentStage);
        baseUnit.SetPosition(x, y);


        switch (baseType)
        {
            case Enum.ENUM_Build_BaseUnit.Ground:
                baseUnit.SetState(new Ground(baseUnit));
                return baseUnit;
            case Enum.ENUM_Build_BaseUnit.Fire:
                baseUnit.SetState(new Fire(baseUnit));
                return baseUnit;
            case Enum.ENUM_Build_BaseUnit.Water:
                baseUnit.SetState(new Water(baseUnit));
                return baseUnit;
            case Enum.ENUM_Build_BaseUnit.Oil:
                baseUnit.SetState(new Oil(baseUnit));
                return baseUnit;
            case Enum.ENUM_Build_BaseUnit.Block:
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
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public override GameObject BuildUpperUnit(IStageHandler currentStage, Enum.ENUM_Build_UpperUnit upperType, BaseUnit targetUnit)
    {
        switch (upperType)
        {
            case Enum.ENUM_Build_UpperUnit.Chest:
                GameObject ChestResource = Resources.Load("Prefabs/Chest") as GameObject;
                GameObject chestObject = GameObject.Instantiate(ChestResource, targetUnit.Model.transform.position, Quaternion.identity);
                Chest _chest;
                _chest = chestObject.AddComponent<Chest>();
                _chest.CurrentOn = targetUnit;
                return chestObject;

            case Enum.ENUM_Build_UpperUnit.RoadBlock:
                GameObject RoadBlockResource = Resources.Load("Prefabs/RoadBlock") as GameObject;
                GameObject roadBlockObject = GameObject.Instantiate(RoadBlockResource, targetUnit.Model.transform.position, Quaternion.identity);
                RoadBlock _roadBlock;
                _roadBlock = roadBlockObject.AddComponent<RoadBlock>();
                _roadBlock.CurrentOn = targetUnit;
                return roadBlockObject;

            case Enum.ENUM_Build_UpperUnit.OilTank:
                GameObject OilTankResource = Resources.Load("Prefabs/OilTank") as GameObject;
                GameObject oilTankObject = GameObject.Instantiate(OilTankResource, targetUnit.Model.transform.position, Quaternion.identity);
                OilTank _oilTank;
                _oilTank = oilTankObject.AddComponent<OilTank>();
                _oilTank.CurrentOn = targetUnit;
                return oilTankObject;

            case Enum.ENUM_Build_UpperUnit.WaterTank:
                GameObject WaterTankResource = Resources.Load("Prefabs/WaterTank") as GameObject;
                GameObject waterTankObject = GameObject.Instantiate(WaterTankResource, targetUnit.Model.transform.position, Quaternion.identity);
                WaterTank _waterTank;
                _waterTank = waterTankObject.AddComponent<WaterTank>();
                _waterTank.CurrentOn = targetUnit;
                return waterTankObject;

            case Enum.ENUM_Build_UpperUnit.Player:
                GameObject PlayerResource = Resources.Load("Prefabs/Player") as GameObject;
                GameObject playerObject = GameObject.Instantiate(PlayerResource, targetUnit.Model.transform.position, Quaternion.identity);
                Player _playerUnit;
                _playerUnit = playerObject.AddComponent<Player>();
                _playerUnit.CurrentOn = targetUnit;
                return playerObject;

            case Enum.ENUM_Build_UpperUnit.Survivor:
                GameObject SurvivorResource = Resources.Load("Prefabs/Survivor") as GameObject;
                GameObject survivorObject = GameObject.Instantiate(SurvivorResource, targetUnit.Model.transform.position, Quaternion.identity);
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
