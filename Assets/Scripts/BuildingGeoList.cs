using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class BuildingGeoList : MonoBehaviour
{
    public List<buildingInfo> buildingList = new List<buildingInfo>();

    void Awake()
    {
        loadXML();
    }
    void Start()
    {
        //loadXML();
    }

    // Update is called once per frame
    void Update()
    {

    }



    void loadXML()
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

            //XmlNode test = place.SelectSingleNode("ns:name", nsMgr);
            //Debug.Log(test.InnerText);
            //Debug.Log("------");

            string name = place.SelectSingleNode(".//ns:name", nsMgr).InnerText;
            double latitude = double.Parse(place.SelectSingleNode(".//ns:latitude", nsMgr).InnerText);
            double longitude = double.Parse(place.SelectSingleNode(".//ns:longitude", nsMgr).InnerText);
            double heading = double.Parse(place.SelectSingleNode(".//ns:heading", nsMgr).InnerText);
            string modelHref = place.SelectSingleNode(".//ns:href", nsMgr).InnerText;
            Debug.Log(name);

            buildingInfo building = new buildingInfo(latitude, longitude, heading, name, modelHref);
            buildingList.Add(building);
        }

        /*
        XmlNodeList latitudeList = xml.GetElementsByTagName("kml:longitude");
        XmlNodeList longitudeList = xml.GetElementsByTagName("kml:latitude");
        XmlNodeList headingList = xml.GetElementsByTagName("kml:heading");
        XmlNodeList nameList = xml.GetElementsByTagName("kml:name");
        XmlNodeList modelHrefList = xml.GetElementsByTagName("kml:href");
        for(int i=0;i<length;i++)
        {
            
            double latitude = double.Parse(latitudeList[i].InnerText);
            double longitude = double.Parse(longitudeList[i].InnerText);
            double heading = double.Parse(headingList[i].InnerText);
            string name = nameList[i].InnerText;
            string modelHref = modelHrefList[i].InnerText;
            buildingInfo building = new buildingInfo(latitude,longitude,heading,name, modelHref);
            Debug.Log(building.name);
            buildingList.Add(building);
        }
        */
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
