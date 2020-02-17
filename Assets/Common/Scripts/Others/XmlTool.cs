using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public static class XmlTool 
{
    public static void XmlCreate(string FileName)
    {
        string path = Application.streamingAssetsPath + "/" + FileName + ".xml";
        if (!File.Exists(path))
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlElement root = xmlDoc.CreateElement("第1关");
            root.SetAttribute("Row", "5");
            root.SetAttribute("Column", "5");

            XmlElement BaseUnitData = xmlDoc.CreateElement("底层类型");
            XmlElement UpperUnitData = xmlDoc.CreateElement("上层类型");

            XmlElement b_row1 = xmlDoc.CreateElement("_1");
            b_row1.InnerText = "1,1,1,1,1";
            XmlElement b_row2 = xmlDoc.CreateElement("_2");
            b_row2.InnerText = "1,1,1,1,1";
            XmlElement b_row3 = xmlDoc.CreateElement("_3");
            b_row3.InnerText = "1,1,1,1,1";
            XmlElement b_row4 = xmlDoc.CreateElement("_4");
            b_row4.InnerText = "1,1,1,1,1";
            XmlElement b_row5 = xmlDoc.CreateElement("_5");
            b_row5.InnerText = "1,1,1,1,1";

            XmlElement u_row1 = xmlDoc.CreateElement("_1");
            u_row1.InnerText = "1,1,1,1,1";
            XmlElement u_row2 = xmlDoc.CreateElement("_2");
            u_row2.InnerText = "1,1,1,1,1";
            XmlElement u_row3 = xmlDoc.CreateElement("_3");
            u_row3.InnerText = "1,1,1,1,1";
            XmlElement u_row4 = xmlDoc.CreateElement("_4");
            u_row4.InnerText = "1,1,1,1,1";
            XmlElement u_row5 = xmlDoc.CreateElement("_5");
            u_row5.InnerText = "1,1,1,1,1";

            BaseUnitData.AppendChild(b_row5);
            BaseUnitData.AppendChild(b_row4);
            BaseUnitData.AppendChild(b_row3);
            BaseUnitData.AppendChild(b_row2);
            BaseUnitData.AppendChild(b_row1);

            UpperUnitData.AppendChild(u_row5);
            UpperUnitData.AppendChild(u_row4);
            UpperUnitData.AppendChild(u_row3);
            UpperUnitData.AppendChild(u_row2);
            UpperUnitData.AppendChild(u_row1);

            root.AppendChild(BaseUnitData);
            root.AppendChild(UpperUnitData);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(path);



        }
    }

    /// <summary>
    /// 用于加载关卡元数据，返回一个包含全部关卡数据的链表
    /// </summary>
    /// <param name="FileName">文件名,不用加后缀</param>
    /// <param name="StageName">关卡名,例:"第1关","第2关"</param>
    public static List<StageMetaData> LoadStageDataXml()
    {
        List<StageMetaData> stageMetaDatas = new List<StageMetaData>();

        string path = Application.streamingAssetsPath + @"/StageData.xml";
        if (File.Exists(path))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode root = xmlDoc.SelectSingleNode("关卡数据");
            //获取关卡原数据链表
            XmlNodeList stages = root.ChildNodes;
            for (int k = 0; k < stages.Count; k++)
            {
                XmlNodeList baseUnitData = stages[k].SelectSingleNode("底层类型").ChildNodes;
                XmlNodeList upperUnitData = stages[k].SelectSingleNode("上层类型").ChildNodes;

                string name = stages[k].Name;
                int type = XmlConvert.ToInt32(stages[k].Attributes["Type"].Value);
                int row = XmlConvert.ToInt32(stages[k].Attributes["Row"].Value);
                int column = XmlConvert.ToInt32(stages[k].Attributes["Column"].Value);

                //存放拆分后的数据
                string[] singleBaseData;
                string[] singleUpperData;
                int[,,] StageMetadata = new int[2, row, column];
                for (int i = 0; i <= row - 1; i++)
                {
                    //将数据分隔提取出来
                    singleBaseData = baseUnitData[row - 1 - i].InnerText.Split(',');
                    singleUpperData = upperUnitData[row - 1 - i].InnerText.Split(',');
                    for (int j = 0; j <= column - 1; j++)
                    {
                        //将分隔出的字符转换为int
                        StageMetadata[0, i, j] = int.Parse(singleBaseData[j]);
                        StageMetadata[1, i, j] = int.Parse(singleUpperData[j]);
                    }
                }
                stageMetaDatas.Add(new StageMetaData(StageMetadata, name,row, column, type));
            }

            return stageMetaDatas;
        }
        else
        {
            Debug.Log("数据文件不存在");
            throw new System.InvalidOperationException();
        }
        
    }

}
