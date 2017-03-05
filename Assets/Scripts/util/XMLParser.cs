using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public static class XMLParser {
    public static string name = "Allfun_变量";// 静态变量 供外部访问
    public static int add(int a, int b)//静态方法 供外部访问
    {
        return a + b;
    }

    static void LoadXml()
    {
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create((Application.dataPath + "data/tile88.xml"), set));

    }
}
