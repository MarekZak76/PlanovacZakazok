using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace JobManager.UI
{
    public class Map : Observable
    {
        private readonly IGeolocator locator;
        private BitmapSource mapImagery;
        private BitmapSource copyMapImagery;
        private byte[] copyMapData;

        public Map()
        {
            locator = new BingLocator();
        }

        public IGeolocator Geolocator { get => locator; }
        public byte[] MapData { get; set; }
        public BitmapSource MapImagery
        {
            get { return mapImagery; }
            set { SetProperty(ref mapImagery, value); }
        }

        public async Task<bool> CreateMapAsync(int width, int height, Location office, params Location[] places)
        {           
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Stream responseStream = await locator.GetStaticMapAsync(width, height, office, places);

                if(responseStream == null)
                {
                    MapImagery = null;
                    MapData = null;
                    return false;
                }

                responseStream.CopyTo(memoryStream);
                MapData = memoryStream.ToArray();
                memoryStream.Position = 0;

                try
                {
                    BitmapDecoder bitmapDecoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    MapImagery = bitmapDecoder.Frames[0];
                }
                catch (NotSupportedException ex)
                {
                    Trace.TraceError(ex.Message);
                    return false;
                }

                return true;
            }
        }
        public void ConvertDataToImagery()
        {
            using (MemoryStream memoryStream = new MemoryStream(MapData))
            {
                try
                {
                    BitmapDecoder bitmapDecoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    MapImagery = bitmapDecoder.Frames[0];
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
        }
        public void BackupData()
        {
            copyMapImagery = mapImagery;
            copyMapData = MapData;
        }
        public void RestoreData()
        {
            MapImagery = copyMapImagery;
            MapData = copyMapData;
        }
    }
}
