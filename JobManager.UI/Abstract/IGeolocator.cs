using System;
using System.IO;
using System.Threading.Tasks;

namespace JobManager.UI
{
    public interface IGeolocator
    {
        Task<Stream> GetStaticMapAsync(int width, int height, Location main, params Location[] secondary);
        Task<Tuple<string, double[]>> GetPointAsync(Adress adress);
    }

}
