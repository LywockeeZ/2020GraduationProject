using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
        Vector3 StartPos = parent.transform.position;
        //x,y坐标分别对应世界坐标下的x，z轴
        GameObject baseUnitObject = m_AssetFactory.InstantiateGameObject("BaseUnit", new Vector3(StartPos.x + x, StartPos.y,StartPos.z + y));
        //给所有基本单元设置一个父物体
        baseUnitObject.transform.SetParent(parent.transform);
        BaseUnit baseUnit = baseUnitObject.AddComponent<BaseUnit>();
        baseUnit.BaseUnitInit(baseUnitObject, currentStageData);
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
                baseUnit.SetState(new Water(baseUnit, false));
                return baseUnit;
            case ENUM_Build_BaseUnit.Oil:
                baseUnit.SetState(new Oil(baseUnit, false));
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
    /// 建造上层单元的基本步骤：获取目标地基单元->加载模型资源->挂载实例化类->初始化->设置父物体
    /// </summary>
    /// <param name="currentStage"></param>
    /// <param name="upperType"></param>
    /// <param name="targetUnit"></param>
    /// <returns></returns>
    public override GameObject BuildUpperUnit(ENUM_Build_UpperUnit upperType, BaseUnit targetUnit)
    {
        if (targetUnit == null) return null;

        IAssetFactory m_AssetFactory = GameFactory.GetAssetFactory();

        switch (upperType)
        {
            case ENUM_Build_UpperUnit.Null:
                return null;

            case ENUM_Build_UpperUnit.Chest:
                GameObject chestObject = m_AssetFactory.InstantiateGameObject("Chest", targetUnit.Model.transform.position);
                Chest _chest = chestObject.AddComponent<Chest>();
                _chest.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(chestObject);
                chestObject.transform.SetParent(targetUnit.Model.transform);
                return chestObject;

            case ENUM_Build_UpperUnit.RoadBlock:
                GameObject roadBlockObject = m_AssetFactory.InstantiateGameObject("RoadBlock", targetUnit.Model.transform.position);
                RoadBlock _roadBlock = roadBlockObject.AddComponent<RoadBlock>();
                _roadBlock.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(roadBlockObject);
                roadBlockObject.transform.SetParent(targetUnit.Model.transform);
                return roadBlockObject;

            case ENUM_Build_UpperUnit.OilTank:
                GameObject oilTankObject = m_AssetFactory.InstantiateGameObject("OilTank", targetUnit.Model.transform.position);
                OilTank _oilTank = oilTankObject.AddComponent<OilTank>();
                _oilTank.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(oilTankObject);
                oilTankObject.transform.SetParent(targetUnit.Model.transform);
                return oilTankObject;

            case ENUM_Build_UpperUnit.WaterTank:
                GameObject waterTankObject = m_AssetFactory.InstantiateGameObject<GameObject>("Prefabs/Units/WaterTank", targetUnit.Model.transform.position);
                WaterTank _waterTank = waterTankObject.AddComponent<WaterTank>();
                _waterTank.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(waterTankObject);
                waterTankObject.transform.SetParent(targetUnit.Model.transform);
                return waterTankObject;

            case ENUM_Build_UpperUnit.Player:
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                Player _playerUnit;
                if (playerObject == null)
                {
                    playerObject = m_AssetFactory.InstantiateGameObject("Player", targetUnit.Model.transform.position);
                    _playerUnit = playerObject.GetComponent<Player>();
                }
                else
                {
                    _playerUnit = playerObject.GetComponent<Player>();
                    playerObject.transform.forward = Vector3.forward;
                }
         
                //必须等此处赋值完后才能初始化，否则会空引用
                _playerUnit.CurrentOn = targetUnit;

                //等Navmesh加载完后再初始化，防止Navagent报错
                Action call = () => { _playerUnit.Init(); };
                CoroutineManager.StartCoroutineTask(call, 0.05f);

                Game.Instance.SetPlayerUnit(_playerUnit);
                return playerObject;

            case ENUM_Build_UpperUnit.Survivor:
                GameObject survivorObject = m_AssetFactory.InstantiateGameObject("Survivor", targetUnit.Model.transform.position);
                Survivor _survivor = survivorObject.AddComponent<Survivor>();
                _survivor.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(survivorObject);
                return survivorObject;
            case ENUM_Build_UpperUnit.Bee1:
                GameObject beeObject = m_AssetFactory.InstantiateGameObject<GameObject>("Prefabs/Units/Bee", targetUnit.Model.transform.position);
                Bee _bee = beeObject.AddComponent<Bee>();
                _bee.SetMoveMode(BeeMoveMode.Round4);
                _bee.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(beeObject);
                return beeObject;
            case ENUM_Build_UpperUnit.Stone:
                GameObject stoneObject = m_AssetFactory.InstantiateGameObject("Stone", targetUnit.Model.transform.position);
                RoadBlock _stone = stoneObject.AddComponent<Stone>();
                _stone.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(stoneObject);
                stoneObject.transform.SetParent(targetUnit.Model.transform);
                return stoneObject;
            case ENUM_Build_UpperUnit.Stool:
                GameObject stoolObject = m_AssetFactory.InstantiateGameObject("Stool", targetUnit.Model.transform.position);
                RoadBlock _stool = stoolObject.AddComponent<Stool>();
                _stool.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(stoolObject);
                stoolObject.transform.SetParent(targetUnit.Model.transform);
                return stoolObject;
            case ENUM_Build_UpperUnit.Bee2:
                GameObject bee2Object = m_AssetFactory.InstantiateGameObject<GameObject>("Prefabs/Units/Bee", targetUnit.Model.transform.position);
                Bee _bee2 = bee2Object.AddComponent<Bee>();
                _bee2.SetMoveMode(BeeMoveMode.Row4);
                _bee2.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(bee2Object);
                return bee2Object;
            case ENUM_Build_UpperUnit.Bee3:
                GameObject bee3Object = m_AssetFactory.InstantiateGameObject<GameObject>("Prefabs/Units/Bee", targetUnit.Model.transform.position);
                Bee _bee3 = bee3Object.AddComponent<Bee>();
                _bee3.SetMoveMode(BeeMoveMode.Column4);
                _bee3.CurrentOn = targetUnit;
                targetUnit.SetUpperGameObject(bee3Object);
                return bee3Object;

            default:
                Debug.LogError("未找到此类型的单元");
                throw new System.InvalidOperationException();
        }
       
    }
}
