using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //单例模式
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    //行动点数
    public int ActionPoints { get { return _actionPoints; } }
    private int _actionPoints = 4;

    //回合数
    public int Rounds { get { return _rounds; } }
    private int _rounds = 1;

    //火焰数
    private int _fireCounts = 0;

    public GameObject Player;

    private Stage currentStage;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        /////////////////////////////////////////////////////////////
        //构建地图
        int[,] stageMessage = new int[4, 9] { { 0, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0, 1, 1, 1, 0 } };
        currentStage = new Stage(4, 9, stageMessage);
        //设置出生点
        currentStage.SetStartPos(new Vector2(7, 3));
        //设置初始火焰
        BaseUnit fire1 = currentStage.GetBaseUnit(7, 0);
        BaseUnit fire2 = currentStage.GetBaseUnit(3, 1);
        BaseUnit fire3 = currentStage.GetBaseUnit(5, 1);
        fire1.SetState(new Fire(fire1));
        fire2.SetState(new Fire(fire2));
        fire3.SetState(new Fire(fire3));
        GameObject chest1 = BuildChest(new Vector2(6, 0));
        GameObject chest2 = BuildChest(new Vector2(4, 1));
        ////////////////////////////////////////////////////////////
        Player = BuildPlayer();
        Debug.Log("FireCounts:" + _fireCounts);

    }

    void Update()
    {
        if ( GetFireCounts() == 0)
        {
            GameOver();
        }
        if (_actionPoints == 0)
        {
            NextRound();
        }
    }

    public void Init()
    {

    }

    public void ResetActionPoints()
    {
        _actionPoints = 4;
    }

    public void ReducePoints(int value)
    {
        _actionPoints -= value;
        Debug.Log("ActionPoints:" + _actionPoints);
    }

    public void NextRound()
    {
        ResetActionPoints();
        _rounds++;
        currentStage.OnStageUpdate();
        Debug.Log("Round:" + _rounds);
        Debug.Log("FireCounts:" + GetFireCounts());

    }

    public void GameOver()
    {
        Debug.Log("Over");
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
        Player _player;
        _player = playerObject.AddComponent<Player>();
        //初始化
        _player.SetCurrentOnUnit(startUnit);


        return playerObject;
    }
    ////////////////////////////////////////////////////////////////////
    ///
    GameObject BuildChest( Vector2 pos)
    {
        BaseUnit startUnit = currentStage.GetBaseUnit((int)pos.x, (int)pos.y);
        GameObject ChestResource = Resources.Load("Prefabs/Chest") as GameObject;
        GameObject chestObject = Instantiate(ChestResource, startUnit.Model.transform.position, Quaternion.identity);
        Chest _chest;
        _chest = chestObject.AddComponent<Chest>();
        _chest.SetCurrentOnUnit(startUnit);

        return chestObject;
    }
}
