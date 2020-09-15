using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobManager.UI
{
    class GoogleLocator : IGeolocator
    {
        public Task<Tuple<string, double[]>> GetPointAsync(Adress adress)
        {
            throw new NotImplementedException();
        }
        public Task<Stream> GetStaticMapAsync(int width, int height, Location main, params Location[] secondary)
        {
            throw new NotImplementedException();
        }
    }
}
