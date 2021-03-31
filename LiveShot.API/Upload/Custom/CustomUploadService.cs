using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using LiveShot.API.Properties;
using LiveShot.API.Upload.Exceptions;
using Microsoft.Extensions.Configuration;

namespace LiveShot.API.Upload.Custom
{
    public class CustomUploadService : IUploadService
    {
        private readonly IConfiguration _configuration;

        public CustomUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Upload(Bitmap bitmap)
        {
            var webRequest = await CreateWebRequest(bitmap);

            WebResponse response = await webRequest.GetResponseAsync();

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new(responseStream);

            await responseReader.ReadToEndAsync();

            return Resources.Upload_Success;
        }

        private async Task<HttpWebRequest> CreateWebRequest(Bitmap bitmap)
        {
            string uploadType = _configuration["UploadType"] ?? throw new InvalidUploadTypeException();

            var uploadConfig = _configuration
                .GetSection("UploadTypes")
                ?.GetSection(uploadType) ?? throw new InvalidUploadTypeException();

            string endpoint = uploadConfig["Endpoint"] ?? throw new InvalidUploadTypeException();
            
            string uploadRequestString = GetUploadRequestString(uploadConfig, bitmap);

            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(endpoint);
            webRequest.Method = uploadConfig["Method"] ?? "POST";

            var headers = uploadConfig.GetSection("Headers")?.GetChildren();

            if (headers != null)
            {
                foreach (var child in headers)
                {
                    webRequest.Headers.Add(child.Key, child.Value);
                }
            }
            
            webRequest.ContentType =  uploadConfig["ContentType"] ?? "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;

            await using StreamWriter streamWriter = new(webRequest.GetRequestStream());
            await streamWriter.WriteAsync(uploadRequestString);

            return webRequest;
        }

        private static string GetUploadRequestString(IConfiguration uploadConfig, Bitmap bitmap)
        {
            ImageConverter converter = new();
            byte[] bytes = (byte[]) converter.ConvertTo(bitmap, typeof(byte[]));
            
            string uploadRequestString = $"{HttpUtility.UrlEncode(uploadConfig["BodyImageKey"] ?? "image")}=" +
                                         Uri.EscapeDataString(Convert.ToBase64String(bytes));

            var body = uploadConfig.GetSection("Body")?.GetChildren();

            if (body == null) 
                return uploadRequestString;
            
            string[] bodyArray = body
                .Select(c => $"{HttpUtility.UrlEncode(c.Key)}={HttpUtility.UrlEncode(c.Value)}")
                .ToArray();

            uploadRequestString += "&" + string.Join("&", bodyArray);

            return uploadRequestString;
        }
    }
}