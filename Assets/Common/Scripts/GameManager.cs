using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //单例模式
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    //行动点数
    public int ActionPoints { get { return _actionPoints; } }
    private int _actionPoints = 3;
    //上回合剩余未扣除点数
    private int lastRoundRemainP = 0;
    private int costTotalPoints = 0;

    //回合数
    public int Rounds { get { return _rounds; } }
    private int _rounds = 1;

    //火焰数
    private int _fireCounts = 0;
    public int lastRoundsFireCounts = 0;

    public GameObject Player;
    public bool canInput = false;

    //临时UI
    public Text actionPointText;
    public Text roundsText;
    public Text costPointText;
    public GameObject gameoverPanel;
    public GameObject UI;

    private Stage currentStage;
    private bool isRoundsEnd = false;
    public bool isGameOver = false;
    private Player _playerUnit = null;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        /////////////////////////////////////////////////////////////
        //构建地图
        //int[,] stageMessage = new int[4, 9] { { 0, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0, 1, 1, 1, 0 } };
        //int[,] stageMessage = new int[10, 10] { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        //{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }};
        int[,] stageMessage = new int[9, 11] { {0,0,0,0,0,0,1,0,0,0,0 },
                                               {0,0,0,0,0,0,1,1,0,0,0 },
                                               {0,0,0,1,1,1,1,1,1,0,0 },
                                               {0,0,0,1,1,1,1,1,1,1,1 },
                                               {0,0,0,1,1,1,1,1,1,1,1 },
                                               {0,1,1,1,1,1,1,1,1,1,1 },
                                               {1,1,1,1,1,1,1,1,1,0,0 },
                                               {0,1,1,1,1,1,1,1,1,0,0 },
                                               {0,0,0,1,1,0,0,0,0,0,0 }};

        currentStage = new Stage(9, 11, stageMessage);
        //设置出生点
        currentStage.SetStartPos(new Vector2(6, 0));
        //设置初始火焰
        BaseUnit fire1 = currentStage.GetBaseUnit(5, 5);
        //BaseUnit fire2 = currentStage.GetBaseUnit(1, 4);
        //BaseUnit fire3 = currentStage.GetBaseUnit(5, 4);
        //BaseUnit fire4 = currentStage.GetBaseUnit(4, 6);
        fire1.SetState(new Fire(fire1));
        //fire2.SetState(new Fire(fire2));
        //fire3.SetState(new Fire(fire3));
        //fire4.SetState(new Fire(fire4));
        //设置水
        BaseUnit water1 = currentStage.GetBaseUnit(8, 5);
        water1.SetState(new Water(water1));
        //设置油
        BaseUnit Oil1 = currentStage.GetBaseUnit(3, 2);
        BaseUnit Oil2 = currentStage.GetBaseUnit(4, 2);
        BaseUnit Oil3 = currentStage.GetBaseUnit(5, 2);
        BaseUnit Oil4 = currentStage.GetBaseUnit(6, 2);
        BaseUnit Oil5 = currentStage.GetBaseUnit(3, 3);
        //BaseUnit Oil6 = currentStage.GetBaseUnit(2, 2);
        Oil1.SetState(new Oil(Oil1));
        Oil2.SetState(new Oil(Oil2));
        Oil3.SetState(new Oil(Oil3));
        Oil4.SetState(new Oil(Oil4));
        Oil5.SetState(new Oil(Oil5));
        //Oil6.SetState(new Oil(Oil6));
        //设置油桶
        GameObject oilTank1 = BuildOilTank(new Vector3(3, 2));
        GameObject oilTank2 = BuildOilTank(new Vector3(5, 7));
        GameObject oilTank3 = BuildOilTank(new Vector3(10, 4));
        ////设置箱子
        ////GameObject chest1 = BuildChest(new Vector2(6, 0));
        ////GameObject chest2 = BuildChest(new Vector2(4, 1));
        ////GameObject chest3 = BuildChest(new Vector2(2, 1));
        //设置水桶
        GameObject waterTank1 = BuildWaterTank(new Vector2(4, 7));
        GameObject waterTank2 = BuildWaterTank(new Vector2(8, 4));
        //设置路障
        GameObject roadBlock1 = BuildRoadBlock(new Vector2(2, 6));
        GameObject roadBlock2 = BuildRoadBlock(new Vector2(5, 6));
        GameObject roadBlock3 = BuildRoadBlock(new Vector2(7, 6));
        GameObject roadBlock4 = BuildRoadBlock(new Vector2(9, 4));
        //设置幸存者
        GameObject survivor = BuildSurvivor(new Vector2(1, 7));
        ////////////////////////////////////////////////////////////
        Player = BuildPlayer();
    }

    void Update()
    {
        actionPointText.text = "行动点数: " + ActionPoints.ToString();
        roundsText.text = "第" + Rounds.ToString() + "回合";
        if (GetFireCounts() == 0 && !isGameOver)
        {
            GameOver();
        }
        if (_actionPoints == 0)
        {
            //防止多次执行关卡回合清算
            if (!isRoundsEnd) OnRoundEnd();
            //当关卡回合清算结束后开启下一回合
            if (!isGameOver)
            {
                if (currentStage.IsUpdateEnd()) NextRound();
            }
        }
    }

    public void Init()
    {

    }

    public void ResetActionPoints()
    {
        _actionPoints = 3;
    }

    public void ReducePoints(int value , int additionValue)
    {
        _actionPoints -= value;
        costTotalPoints += value;
        if (_actionPoints == 0 && additionValue != 0)
        {
            lastRoundRemainP = 1;
        }
        else
        {
            _actionPoints -= additionValue;
            costTotalPoints += additionValue;
        }
        Debug.Log("ActionPoints:" + _actionPoints);
    }

    public void OnRoundEnd()
    {
        isRoundsEnd = true;
        canInput = false;
        lastRoundsFireCounts = GetFireCounts();
        currentStage.OnStageUpdate();
    }

    public void NextRound()
    {
        ResetActionPoints();

        ReducePoints(lastRoundRemainP, 0);
        lastRoundRemainP = 0;

        isRoundsEnd = false;
        canInput = true;
        _rounds++;
        currentStage.ResetBool();
        Debug.Log("Round:" + _rounds);
        Debug.Log("FireCounts:" + GetFireCounts());
    }

    public void GameOver()
    {
        isGameOver = true;
        canInput = false;
        gameoverPanel.SetActive(true);
        costPointText.text = "花费点数：" + costTotalPoints.ToString();
        Debug.Log("Over");
    }

    public void GameRestart()
    {
        Resources.UnloadUnusedAssets();
        AssetBundle.UnloadAllAssetBundles(true);
        SceneManager.LoadScene(1);
    }

    public int GetFireCounts()
    {
        _fireCounts = currentStage.fireUnits.Count;
        return _fireCounts;
    }

    ////////////////////////////////////////////////////////////////////
    //构建角色流程
    GameObject BuildPlayer()
    {
        //获取初始单元
        BaseUnit startUnit = currentStage.GetBaseUnit((int)currentStage.startPos.x, (int)currentStage.startPos.y);
        //加载模型资源
        GameObject PlayerResource = Resources.Load("Prefabs/Player") as GameObject;
        GameObject playerObject = Instantiate(PlayerResource, startUnit.Model.transform.position, Quaternion.identity);
        //挂载实例化类
        _playerUnit = playerObject.AddComponent<Player>();
        //初始化
        _playerUnit.CurrentOn = startUnit;


        return playerObject;
    }
    ////////////////////////////////////////////////////////////////////
    //构建箱子流程
    GameObject BuildChest( Vector2 pos)
    {
        BaseUnit startUnit = currentStage.GetBaseUnit((int)pos.x, (int)pos.y);
        GameObject ChestResource = Resources.Load("Prefabs/Chest") as GameObject;
        GameObject chestObject = Instantiate(ChestResource, startUnit.Model.transform.position, Quaternion.identity);
        Chest _chest;
        _chest = chestObject.AddComponent<Chest>();
        _chest.CurrentOn = startUnit;

        return chestObject;
    }
    /////////////////////////////////////////////////////////////////////
    //构建路障流程
    GameObject BuildRoadBlock(Vector2 pos)
    {
        BaseUnit startUnit = currentStage.GetBaseUnit((int)pos.x, (int)pos.y);
        GameObject RoadBlockResource = Resources.Load("Prefabs/RoadBlock") as GameObject;
        GameObject roadBlockObject = Instantiate(RoadBlockResource, startUnit.Model.transform.position, Quaternion.identity);
        RoadBlock _roadBlock;
        _roadBlock = roadBlockObject.AddComponent<RoadBlock>();
        _roadBlock.CurrentOn = startUnit;

        return roadBlockObject;
    }
    ////////////////////////////////////////////////////////////////////
    //构建水桶流程
    GameObject BuildWaterTank(Vector2 pos)
    {
        BaseUnit startUnit = currentStage.GetBaseUnit((int)pos.x, (int)pos.y);
        GameObject WaterTankResource = Resources.Load("Prefabs/WaterTank") as GameObject;
        GameObject waterTankObject = Instantiate(WaterTankResource, startUnit.Model.transform.position, Quaternion.identity);
        WaterTank _waterTank;
        _waterTank = waterTankObject.AddComponent<WaterTank>();
        _waterTank.CurrentOn = startUnit;

        return waterTankObject;
    }
    ////////////////////////////////////////////////////////////////////
    //构建油桶流程
    GameObject BuildOilTank(Vector2 pos)
    {
        BaseUnit startUnit = currentStage.GetBaseUnit((int)pos.x, (int)pos.y);
        GameObject OilTankResource = Resources.Load("Prefabs/OilTank") as GameObject;
        GameObject oilTankObject = Instantiate(OilTankResource, startUnit.Model.transform.position, Quaternion.identity);
        OilTank _oilTank;
        _oilTank = oilTankObject.AddComponent<OilTank>();
        _oilTank.CurrentOn = startUnit;

        return oilTankObject;
    }
    ///////////////////////////////////////////////////////////////////
    //构建幸存者
    GameObject BuildSurvivor(Vector2 pos)
    {
        BaseUnit startUnit = currentStage.GetBaseUnit((int)pos.x, (int)pos.y);
        GameObject SurvivorResource = Resources.Load("Prefabs/Survivor") as GameObject;
        GameObject survivorObject = Instantiate(SurvivorResource, startUnit.Model.transform.position, Quaternion.identity);
        Survivor _survivor;
        _survivor = survivorObject.AddComponent<Survivor>();
        _survivor.CurrentOn = startUnit;

        return survivorObject;
    }

    ///////////////////////////////////////////////////////////////////

    public void SetCanInput(bool value)
    {
        canInput = value;
    }

    public void SetPlayerUnit(Player playerUnit)
    {
        _playerUnit = playerUnit;
    }

    public Player GetPlayerUnit()
    {
        return _playerUnit;
    }

    public bool IsGameOver()
    {
        bool isOver = false;
        if (GetFireCounts() == lastRoundsFireCounts)
        {
            isOver = true;
        }
        else
        if (GetFireCounts() == 0)
        {
            isOver = true;
        }
        return isOver;
    }

}
