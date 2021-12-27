using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using LiveShot.API.Upload.Exceptions;
using Microsoft.Extensions.Configuration;

namespace LiveShot.API.Upload.Imgur
{
    public class ImgurService : IUploadService
    {
        private readonly string? _clientId;

        public ImgurService(IConfiguration configuration)
        {
            _clientId = configuration.GetSection("UploadTypes")?.GetSection("Imgur")?["ClientID"];
        }

        public async Task<string> Upload(Bitmap bitmap)
        {
            if (_clientId is null)
                throw new InvalidClientIdException();

            var webRequest = await CreateWebRequest(bitmap);
            WebResponse response = await webRequest.GetResponseAsync();

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new(responseStream);

            string responseString = await responseReader.ReadToEndAsync();

            var responseData = JsonSerializer.Deserialize<ImgurResponse>(responseString);

            return responseData.Data.Link;
        }

        private async Task<HttpWebRequest> CreateWebRequest(Bitmap bitmap)
        {
            ImageConverter converter = new();
            byte[] bytes = ((byte[]) converter.ConvertTo(bitmap, typeof(byte[])))!;

            string uploadRequestString = "image=" + Uri.EscapeDataString(Convert.ToBase64String(bytes));

            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create("https://api.imgur.com/3/image");
            webRequest.Method = "POST";
            webRequest.Headers.Add("Authorization", $"Client-ID {_clientId}");
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;

            await using StreamWriter streamWriter = new(webRequest.GetRequestStream());
            await streamWriter.WriteAsync(uploadRequestString);

            return webRequest;
        }
    }
}