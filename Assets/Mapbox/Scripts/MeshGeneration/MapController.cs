using UnityEngine;
using System.Collections.Generic;
using Mapbox.MeshGeneration.Data;
using Mapbox.Scripts.Utilities;
using Mapbox.Scripts.UI;

namespace Mapbox.MeshGeneration
{
	public class MapController : MonoBehaviour
	{
		private IFileSource _fileSource;
		public static Rect ReferenceTileRect { get; set; }
		public static float WorldScaleFactor { get; set; }

		public MapVisualization MapVisualization;
		public float TileSize = 100;  // һƬtiles��unity�����е�size��100,ī��������ϵ611

        [SerializeField]
        private bool _snapYToZero = true;

		[Geocode]
		public string LatLng;
		public int Zoom = Config.zoom;
		public Vector4 Range;

		private GameObject _root;
		private Dictionary<Vector2, UnityTile> _tiles;

		public void Awake()
		{
			_fileSource = MapboxConvenience.Instance.FileSource;
			MapVisualization.Initialize(this, _fileSource);
			_tiles = new Dictionary<Vector2, UnityTile>();
		}

		public void Start()
		{
			Execute();
		}

        public void Update()
        {
            if(_snapYToZero)
            {
                var ray = new Ray(new Vector3(0, 1000, 0), Vector3.down);
                RaycastHit rayhit;
                if (Physics.Raycast(ray, out rayhit))
                {
                    _root.transform.position = new Vector3(0, -rayhit.point.y, 0);
                    _snapYToZero = false;
                }                
            }
        }

		public void Execute()
		{
			var parm = LatLng.Split(',');
			Execute(double.Parse(parm[0]), double.Parse(parm[1]), Zoom, Range);
		}

		public void Execute(double lat, double lng, int zoom, Vector4 frame)  // frame = (1,1,1,1)
		{
			//frame goes like left-top-right-bottom here
			if (_root != null)
			{
				foreach (Transform t in _root.transform)
				{
					Destroy(t.gameObject);
				}
			}

			_root = new GameObject("worldRoot");

			var v2 = Conversions.LatLonToMeters(lat, lng);  //��γ����תī��������
			var tms = Conversions.MetersToTile(v2, zoom);   //�ڱ��Ŵ�2^zoom�η��ĵ�ͼ�ϣ��ҵ�v2�����Ӧ��Tiles ID
			ReferenceTileRect = Conversions.TileBounds(tms, zoom);  // ���ص�Rect�ǻ���ī��������. RefernceTile:ָ����γ�����ڵ�tile
            WorldScaleFactor = TileSize / ReferenceTileRect.width;  // һƬtiles��unity����ϵ�еĿ��/һƬtiles��ī���������еĿ�ȣ����ڴ�ī��������scale��unity����
            _root.transform.localScale = Vector3.one * WorldScaleFactor;
            Debug.Log("reference tile rect width: "+ReferenceTileRect.width);
            Debug.Log("reference:" + (ReferenceTileRect.max.x - ReferenceTileRect.min.x));
			//creating tiles on demand, we can use something like Thiago's slippy map here as well
			for (int i = (int)(tms.x - frame.x); i <= (tms.x + frame.z); i++)
			{
				for (int j = (int)(tms.y - frame.y); j <= (tms.y + frame.w); j++)
				{
                    GameObject tileObject = new GameObject("Tile - " + i + " | " + j);
					var tile = tileObject.AddComponent<UnityTile>();
                    
					_tiles.Add(new Vector2(i, j), tile);
					tile.Zoom = zoom;
					tile.RelativeScale = Conversions.GetTileScaleInMeters(0, Zoom) / Conversions.GetTileScaleInMeters((float)lat, Zoom);
					tile.TileCoordinate = new Vector2(i, j);
					tile.Rect = Conversions.TileBounds(tile.TileCoordinate, zoom);  // ī��������
					tile.transform.position = new Vector3(tile.Rect.center.x - ReferenceTileRect.center.x, 0, tile.Rect.center.y - ReferenceTileRect.center.y);
					tile.transform.SetParent(_root.transform, false);
					MapVisualization.ShowTile(tile);

                    tileObject.AddComponent<TileIntro>().setTileInfo(new Vector2(i, j), tile.Rect, tile.Zoom);
                    
				}
			}

            // another zoom level
            /*
            var v2_2 = Conversions.LatLonToMeters(lat, lng);
            var tms_2 = Conversions.MetersToTile(v2_2, zoom - 1);
            Rect referenceRect2 = Conversions.TileBounds(tms_2, zoom - 1);
            float worldScaleFactor2 = (TileSize / referenceRect2.width) * 2;
            GameObject _root2 = new GameObject("worldRoog2");
            _root2.transform.localScale = Vector3.one * worldScaleFactor2;
            var tile2 = new GameObject("Tile-" + tms_2.x + " | " + tms_2.y).AddComponent<UnityTile>();
            tile2.Zoom = zoom - 1;
            tile2.RelativeScale = Conversions.GetTileScaleInMeters(0, Zoom - 1) / Conversions.GetTileScaleInMeters((float)lat, Zoom - 1);
            tile2.TileCoordinate = new Vector2(tms_2.x, tms_2.y);
            tile2.Rect = Conversions.TileBounds(tile2.TileCoordinate, zoom - 1);
            tile2.transform.position = new Vector3(0, -5, 0);
            tile2.transform.SetParent(_root2.transform, false);
            MapVisualization.ShowTile(tile2);
            */
        }

		public void Execute(double lat, double lng, int zoom, Vector2 frame)
		{
			Execute(lat, lng, zoom, new Vector4(frame.x, frame.y, frame.x, frame.y));
		}

		public void Execute(double lat, double lng, int zoom, int range)
		{
			Execute(lat, lng, zoom, new Vector4(range, range, range, range));
		}

		public void Request(Vector2 pos, int zoom)
		{
			if (!_tiles.ContainsKey(pos))
			{
				var tile = new GameObject("Tile - " + pos.x + " | " + pos.y).AddComponent<UnityTile>();
				_tiles.Add(pos, tile);
				tile.transform.SetParent(_root.transform, false);
				tile.Zoom = zoom;
                tile.TileCoordinate = new Vector2(pos.x, pos.y);
				tile.Rect = Conversions.TileBounds(tile.TileCoordinate, zoom);
                tile.RelativeScale = Conversions.GetTileScaleInMeters(0, Zoom) / 
				Conversions.GetTileScaleInMeters((float)Conversions.MetersToLatLon(tile.Rect.center).Latitude, Zoom);
				tile.transform.localPosition = new Vector3(tile.Rect.center.x - ReferenceTileRect.center.x, 
				                                           0,
				                                           tile.Rect.center.y - ReferenceTileRect.center.y);
				MapVisualization.ShowTile(tile);
			}
		}

        
	}
}