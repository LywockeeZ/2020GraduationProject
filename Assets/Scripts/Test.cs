using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //public int n = 0;
    public Vector2 startPos = new Vector2(7, 3);
    public GameObject Player;
    private int[,] stageMessage = new int[4, 9]{ { 0, 1, 1, 1, 1, 1, 1, 1, 1 },{ 0, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 0 },{ 0, 0, 0, 0, 0, 1, 1, 1, 0 }};
    private Stage currentStage = null;


    private void Awake()
    {
        currentStage = new Stage(4, 9, stageMessage);
    }
    void Start()
    {
        //if (stage.baseUnits[n].Up != null)
        //{
        //    stage.baseUnits[n].Up.Model.GetComponent<MeshRenderer>().material.color = Color.black;

        //}
        //if (stage.baseUnits[n].Down != null)
        //{
        //    stage.baseUnits[n].Down.Model.GetComponent<MeshRenderer>().material.color = Color.black;

        //}
        //if (stage.baseUnits[n].Left != null)
        //{
        //    stage.baseUnits[n].Left.Model.GetComponent<MeshRenderer>().material.color = Color.black;

        //}
        //if (stage.baseUnits[n].Right != null)
        //{
        //    stage.baseUnits[n].Right.Model.GetComponent<MeshRenderer>().material.color = Color.black;

        //}
        Player = BuildPlayer();

        //设置初始火焰
        BaseUnit fire1 = currentStage.GetBaseUnit(7, 0);
        BaseUnit fire2 = currentStage.GetBaseUnit(3, 1);
        BaseUnit fire3 = currentStage.GetBaseUnit(5, 1);
        fire1.SetState(new Fire(fire1));
        fire2.SetState(new Fire(fire2));
        fire3.SetState(new Fire(fire3));
        currentStage.OnStageUpdate();

    }

    void Update()
    {
        
    }

    GameObject BuildPlayer()
    {
        //获取初始单元
        BaseUnit startUnit = currentStage.GetBaseUnit((int)startPos.x, (int)startPos.y);
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

    public void FireTest()
    {
        BaseUnit fireTest = currentStage.GetBaseUnit(3, 2);
        fireTest.SetState(new Fire(fireTest));
    }
}
