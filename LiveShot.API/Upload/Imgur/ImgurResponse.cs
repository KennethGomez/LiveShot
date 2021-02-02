using System.Text.Json.Serialization;

namespace LiveShot.API.Upload.Imgur
{
    public struct ImgurResponse
    {
        [JsonPropertyName("data")] public ImgurResponseData Data { get; init; }
    }

    public struct ImgurResponseData
    {
        [JsonPropertyName("link")] public string Link { get; init; }
    }
}