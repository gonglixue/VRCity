using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class BuildingGeoList : MonoBehaviour
{
    public List<buildingInfo> buildingList = new List<buildingInfo>();
    public List<TileInfo> tileList = new List<TileInfo>();

    void Awake()
    {
        loadKML();  // 加载tileList数据
        for(int i=0;i<tileList.Count;i++)
        {
            loadTileKML(i);
            // TODO: 每次循环就增加了一个tile信息，即可绘制该tile
        }
        
    }
    void Start()
    {
        //loadXML();
    }

    // Update is called once per frame
    void Update()
    {

    }



    void loadTileKML()
    {
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create((Application.dataPath + "/data/tile88.kml"), set));

        // 命名空间设置
        XmlNamespaceManager nsMgr = new XmlNamespaceManager(xml.NameTable);
        nsMgr.AddNamespace("ns", "http://www.opengis.net/kml/2.2");

        XmlNode xdn = xml.DocumentElement;

        //XmlNodeList placeMark = xml.GetElementsByTagName("kml:Placemark");
        XmlNodeList placeMark = xdn.SelectNodes("//ns:Placemark", nsMgr);
        Debug.Log("placemark list length: " + placeMark.Count);
        int length = placeMark.Count;

        //foreach(XmlNode place in placeMark)
        for (int i = 0; i < length; i++)
        {
            XmlNode place = placeMark[i];
            //Debug.Log(place.InnerXml);

            string name = place.SelectSingleNode(".//ns:name", nsMgr).InnerText;
            double latitude = double.Parse(place.SelectSingleNode(".//ns:latitude", nsMgr).InnerText);
            double longitude = double.Parse(place.SelectSingleNode(".//ns:longitude", nsMgr).InnerText);
            double heading = double.Parse(place.SelectSingleNode(".//ns:heading", nsMgr).InnerText);
            string modelHref = place.SelectSingleNode(".//ns:href", nsMgr).InnerText;
            Debug.Log(name);

            buildingInfo building = new buildingInfo(latitude, longitude, heading, name, modelHref);
            buildingList.Add(building);
        }

    }

    void loadTileKML(int i)
    {
        TileInfo singleTile = tileList[i];
        string tileColladaKmlPath = "\\" + singleTile.href;
        int lastFolderIndex = singleTile.href.LastIndexOf('\\');
        string singleTilePathPrefix = singleTile.href.Substring(0,lastFolderIndex);
        Debug.Log(singleTilePathPrefix);

        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create((Application.dataPath + "\\Resources"+tileColladaKmlPath), set));

        // 命名空间设置
        XmlNamespaceManager nsMgr = new XmlNamespaceManager(xml.NameTable);
        nsMgr.AddNamespace("ns", "http://www.opengis.net/kml/2.2");

        XmlNode xdn = xml.DocumentElement;
        XmlNodeList placeMark = xdn.SelectNodes("//ns:Placemark", nsMgr);
        //Debug.Log("placemark list length: " + placeMark.Count);
        int length = placeMark.Count;

        //foreach(XmlNode place in placeMark)
        for (int j = 1; j < length; j++)
        {
            XmlNode place = placeMark[j];

            string name = place.SelectSingleNode(".//ns:name", nsMgr).InnerText;
            double latitude = double.Parse(place.SelectSingleNode(".//ns:latitude", nsMgr).InnerText);
            double longitude = double.Parse(place.SelectSingleNode(".//ns:longitude", nsMgr).InnerText);
            double heading = double.Parse(place.SelectSingleNode(".//ns:heading", nsMgr).InnerText);
            string modelHref = singleTilePathPrefix + '/' + place.SelectSingleNode(".//ns:href", nsMgr).InnerText;
            //Debug.Log(modelHref);

            buildingInfo building = new buildingInfo(latitude, longitude, heading, name, modelHref);
            buildingList.Add(building);
        }
    }

    void loadKML()
    {
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create((Application.dataPath + "/data/Layer2.kml"), set));

        // 命名空间设置
        XmlNamespaceManager nsMgr = new XmlNamespaceManager(xml.NameTable);
        nsMgr.AddNamespace("ns", "http://www.opengis.net/kml/2.2");
        nsMgr.AddNamespace("xal", "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0");
        nsMgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
        nsMgr.AddNamespace("gx", "http://www.google.com/kml/ext/2.2");

        XmlNode xdn = xml.DocumentElement;

        //XmlNodeList Folder = xml.GetElementsByTagName("Folder");
        XmlNodeList Folder = xdn.SelectNodes(".//ns:Folder",nsMgr);
        //Debug.Log("folderplacemark list length: " + Folder.Count);
        int length = Folder.Count;

        foreach(XmlNode tileNode in Folder)
        {
            string name = tileNode.SelectSingleNode("./ns:name", nsMgr).InnerText;
            double north = double.Parse(tileNode.SelectSingleNode(".//ns:north", nsMgr).InnerText);
            double south = double.Parse(tileNode.SelectSingleNode(".//ns:south", nsMgr).InnerText);
            double west = double.Parse(tileNode.SelectSingleNode(".//ns:west", nsMgr).InnerText);
            double east = double.Parse(tileNode.SelectSingleNode(".//ns:east", nsMgr).InnerText);
            string href = tileNode.SelectSingleNode(".//ns:href", nsMgr).InnerText;
            
            //Debug.Log(name + ":" + href);
            TileInfo tile = new TileInfo(name, north, south, east, west, href);
            tileList.Add(tile);
        }
    }
}

public struct buildingInfo
{
    public double latitude;
    public double longitude;
    public double heading;
    public string name;
    public string modelHref;

    public buildingInfo(double _latitude, double _longitude, double _heading, string _name, string _href)
    {
        latitude = _latitude;
        longitude = _longitude;
        heading = _heading;
        name = _name;
        modelHref = _href;
    }
}

public struct TileInfo
{
    public string name;
    // border WGS84
    public double north;
    public double south;
    public double east;
    public double west;
    public string href;  // look up certain tile kml;

    public TileInfo(string _name, double _north, double _south, double _east, double _west, string _href)
    {
        name = _name;
        north = _north;
        south = _south;
        east = _east;
        west = _west;
        href = _href;
    }
}
