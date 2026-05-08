using System.Text.Json.Serialization;

namespace HelpDesk.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KbArticleStatus
    {
        Draft = 1,
        Published = 2
    }
}
