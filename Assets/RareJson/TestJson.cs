using UnityEngine;
using System.Collections;
using Rare;

public class TestJson : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        // json对象构造示例
        JsonObject root = new JsonObject();                                              
        root["age"] = 25;
        root["name"] = "rare";
        JsonObject obj = new JsonObject();
        root["person"] = obj;
        obj["age"] = 1;
        obj["name"] = "even";
        JsonArray arr = new JsonArray();
        arr[0] = "ComputerGrphic";
        arr[1] = "Unity3D";
        arr[2] = "Graphic";
        root["books"] = arr;

        // json取值
        int a = root["age"];
        Debug.Log("age:"+a.ToString());
        string v = root["name"];
        Debug.Log("name:" + v);

        // save json and format
        string strSerializeFile = Application.dataPath + "json/home.json";
        RareJson.Serialize(root, strSerializeFile, true);

        // json解析
        string strJsonName = Application.dataPath + "json/Contents.json";
        JsonNode node = RareJson.ParseJsonFile(strJsonName);

        // json not format
        strSerializeFile = Application.dataPath + "json/serialize.json";
        RareJson.Serialize(node, strSerializeFile, false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
