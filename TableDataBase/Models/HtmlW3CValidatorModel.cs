using System;
using Newtonsoft.Json;

namespace TableDataBase.Models
{
	public class HtmlW3CValidatorModel
	{
        [JsonProperty("messages")]
        public List<HtmlW3CValidatorMMessage> Messages { get; set; }
    }

    public class HtmlW3CValidatorMMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("lastLine")]
        public int LastLine { get; set; }

        [JsonProperty("lastColumn")]
        public int LastColumn { get; set; }
    }
}

