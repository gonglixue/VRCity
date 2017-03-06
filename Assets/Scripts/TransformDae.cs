using UnityEngine;
using System.Collections;
using Mapbox.Scripts.Utilities;
using Mapbox.MeshGeneration.Data;
namespace Mapbox.MeshGeneration
{


    public class TransformDae : MonoBehaviour {
        public double longitude = 13.3905676;
        public double latitude = 52.5387557;
        public double heading = 358.7239657;
        public float TileSize = 100;
        public int zoom = 16;
        
        public double north = 52.542316087433;  // lat
        public double south = 52.54121947358615;
        public double west = 13.396366385489324;  // lon
        public double east = 13.398223088795774;
        public double worldScaleFactor;

        [SerializeField]
        private GameObject prefab_ade;

        private float scaleFactor;
        private GameObject _root;
        private GameObject _building;
        private double deltax;
        private double deltay;

	    // Use this for initialization
	    void Start () {
            
            Vector3 position;
            Quaternion rotate;

            Vector2 v2 = Conversions.LatLonToMeters(latitude, longitude);  // 该建筑的经纬度转墨卡托坐标
            //var tms = Conversions.MetersToTile(v2, zoom);  // 该建筑所在的tiles ID

            Vector2 referenceTileMeter = Conversions.LatLonToMeters(52.5387557, 13.3905676);
            var tms = Conversions.MetersToTile(referenceTileMeter, zoom);
            Rect ReferenceTileRect = Conversions.TileBounds(tms, zoom);
            //Rect ReferenceTileRect = MapController.ReferenceTileRect;
            

            
            var north_east = Conversions.LatLonToMeters(north, east);
            var south_west = Conversions.LatLonToMeters(south, west);
            Debug.Log("my tile box width"+(north_east.x - south_west.x));
            Debug.Log("my tile box height:" + (north_east.y - south_west.y));
            scaleFactor = ((north_east.y - south_west.y) / 611.5f);
            worldScaleFactor = TileSize / ReferenceTileRect.width;
            Debug.Log("world scale factor:" + worldScaleFactor);
            _root = new GameObject("BuildingRoot");
            _root.transform.localScale = Vector3.one * (scaleFactor);

            deltax = v2.x - ReferenceTileRect.center.x;
            deltay = v2.y - ReferenceTileRect.center.y;
            position = new Vector3((float)((deltax)*worldScaleFactor), 0, (float)((deltay)*worldScaleFactor));
            rotate = Quaternion.AngleAxis(-89.8f, Vector3.right);
            //rotate = Quaternion.AngleAxis(-89.98f, Vector3.right) * Quaternion.AngleAxis((float)heading, Vector3.forward);
            //rotate = rotate * Quaternion.AngleAxis()
            _building = (GameObject)Instantiate(prefab_ade, position,rotate,_root.transform);
            //_building.transform.localScale = Vector3.one * scaleFactor;


        }
	
	    // Update is called once per frame
	    void Update () {
            _building.transform.position = new Vector3((float)(deltax * worldScaleFactor), 0, (float)(deltay * worldScaleFactor));
	    }


    }

}
