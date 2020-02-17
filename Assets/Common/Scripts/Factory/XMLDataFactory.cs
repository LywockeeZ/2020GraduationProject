using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMLDataFactory : IDataFactory
{
    /// <summary>
    /// 由关卡数据文件加载关卡
    /// </summary>
    /// <returns></returns>
    public override Dictionary<string, IStageHandler> LoadStageData()
    {
        Dictionary<string, IStageHandler> dicAllStages = new Dictionary<string, IStageHandler>();
        Stack<IStageHandler> staStageHandlers = new Stack<IStageHandler>();
        List<StageMetaData> stageMetaDatas = XmlTool.LoadStageDataXml();

        //加载中间关卡
        for (int i = 0; i < stageMetaDatas.Count; i++)
        {
            LoadStageByType(stageMetaDatas[i], stageMetaDatas[i].Type,
                staStageHandlers, dicAllStages);
        }

        return dicAllStages;
    }


    /// <summary>
    /// 根据类型，生成对应的关卡类
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private void LoadStageByType(StageMetaData metaData, int type, 
                                            Stack<IStageHandler>staStageHandlers, 
                                                Dictionary<string, IStageHandler>dicAllStages)
    {
        //先建立关卡构建数据类
        NormalStageData stageData = new NormalStageData(metaData);

        //再依据类型生成不同的关卡类
        switch ((ENUM_StageType)type)
        {
            case ENUM_StageType.Normal:
                NormalStageScore normalScore = new NormalStageScore();
                IStageHandler normalStage = new NormalStageHandler(normalScore, stageData);
                //第一个关卡直接入栈，后面的关卡依次设置关卡链
                if (staStageHandlers.Count != 0)
                {
                    staStageHandlers.Peek().SetNextHandler(normalStage);
                    staStageHandlers.Push(normalStage);
                }
                else
                    staStageHandlers.Push(normalStage);
                dicAllStages.Add(metaData.Name, normalStage);
                break;

            case ENUM_StageType.Teach:
                TeachStageScore teachScore = new TeachStageScore();
                IStageHandler teachStage = new TeachStageHandler(teachScore, stageData);
                if (staStageHandlers.Count != 0)
                {
                    staStageHandlers.Peek().SetNextHandler(teachStage);
                    staStageHandlers.Push(teachStage);
                }
                else
                    staStageHandlers.Push(teachStage);
                dicAllStages.Add(metaData.Name, teachStage);
                break;

            case ENUM_StageType.NormalSingle:
                NormalStageScore normalScoreSingle = new NormalStageScore();
                NormalStageHandler normalStageSingle = new NormalStageHandler(normalScoreSingle, stageData);
                dicAllStages.Add(metaData.Name, normalStageSingle);
                break;

            case ENUM_StageType.TeachSingle:
                TeachStageScore teachScoreSingle = new TeachStageScore();
                TeachStageHandler teachStageSingle = new TeachStageHandler(teachScoreSingle, stageData);
                dicAllStages.Add(metaData.Name, teachStageSingle);
                break;

            default:
                throw new System.Exception("关卡类型[" + type + "]错误" + ",无此类型关卡");
        }
    }
}
