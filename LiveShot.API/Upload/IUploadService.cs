using System.Drawing;
using System.Threading.Tasks;

namespace LiveShot.API.Upload
{
    public interface IUploadService
    {
        Task<string> Upload(Bitmap bitmap);
    }
}