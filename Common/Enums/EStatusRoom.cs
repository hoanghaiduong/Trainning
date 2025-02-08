using System.Text.Json.Serialization;

namespace Trainning.Common.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EStatusRoom
    {
        AVAILABLE = 1,
        UNAVAILABLE = 0,
    }
}