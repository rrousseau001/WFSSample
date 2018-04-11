﻿using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Styles;

namespace Quickstart.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [MapActionFilter]
        public void UpdateOverlay(Map map, GeoCollection<object> args)
        {
            var layerName = args["layer"].ToString();
            LayerOverlay overlay = map.CustomOverlays["ShapeOverlay"] as LayerOverlay;
            overlay.Layers.Clear();
            if (layerName.ToLowerInvariant() == "us")
            {
                // States layer
                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/USStates.SHP"));
                AreaStyle areaStyle = new AreaStyle();
                areaStyle.FillSolidBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));
                areaStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, 156, 155, 154), 2);
                areaStyle.OutlinePen.DashStyle = LineDashStyle.Solid;
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = areaStyle;
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                overlay.Layers.Add(worldLayer);
            }
            else
            {
                // Cities layer
                ShapeFileFeatureLayer countyLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/cities_e.shp"));
                PointStyle pointStyle = new PointStyle();
                pointStyle.SymbolType = PointSymbolType.Square;
                pointStyle.SymbolSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.White);
                pointStyle.SymbolPen = new GeoPen(GeoColor.StandardColors.Black, 1);
                pointStyle.SymbolSize = 6;

                PointStyle stackStyle = new PointStyle();
                stackStyle.SymbolType = PointSymbolType.Square;
                stackStyle.SymbolSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.Maroon);
                stackStyle.SymbolPen = new GeoPen(GeoColor.StandardColors.Transparent, 0);
                stackStyle.SymbolSize = 2;

                pointStyle.CustomPointStyles.Add(stackStyle);
                GeoFont font = new GeoFont("Arial", 9, DrawingFontStyles.Bold);
                GeoSolidBrush txtBrush = new GeoSolidBrush(GeoColor.StandardColors.Maroon);
                TextStyle textStyle = new TextStyle("NAME", font, txtBrush);
                textStyle.XOffsetInPixel = 0;
                textStyle.YOffsetInPixel = -6;
                countyLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = pointStyle;
                countyLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
                countyLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                overlay.Layers.Add(countyLayer);
            }
        }
   
        [MapActionFilter]
        public void WFS(Map map)
        {
            //("https://demo.boundlessgeo.com/geoserver/wfs", "medford:bikelanes")
            var wfs = new ThinkGeo.MapSuite.Layers.WfsFeatureLayer("https://geoservices.informatievlaanderen.be/overdrachtdiensten/GRB/wfs", "GRB:WLAS");
            wfs.Open();
            var s = wfs.QueryTools.GetAllFeatures(ThinkGeo.MapSuite.Shapes.ReturningColumnsType.AllColumns);
            wfs.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.SimpleColors.BrightRed, 15, false);
            wfs.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            wfs.SentWebRequest += Wfs_SentWebRequest;
            wfs.RequestingDrawing += Wfs_RequestingDrawing;
            //do all events 
            var ov = new ThinkGeo.MapSuite.Mvc.LayerOverlay("SomeWFS", false, ThinkGeo.MapSuite.Mvc.TileType.SingleTile);
            ov.IsVisible = true;
            if (map.CustomOverlays.Contains("SomeWFS"))
            {
                map.CustomOverlays.Remove("SomeWFS");
            }
            map.CustomOverlays.Add(ov);

            //map.refresh
        }

        private void Wfs_RequestingDrawing(object sender, RequestingDrawingLayerEventArgs e)
        {
            ;
        }

        private void Wfs_SentWebRequest(object sender, SentWebRequestEventArgs e)
        {
            ;
        }
    }
}