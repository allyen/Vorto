using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Vorto.Models
{
    public class VortoValue
    {
        [JsonProperty("values")]
        public IDictionary<string, object> Values { get; set; }

        [JsonProperty("dtdId")]
        public int DtdId { get; set; }

        public string SerializeForPersistence()
        {
            var json = JObject.Parse(JsonConvert.SerializeObject(this, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, MaxDepth = 128 }));
            var propertiesToRemove = new String[] { "dtdId" };

            json.Descendants().OfType<JProperty>()
              .Where(p => propertiesToRemove.Contains(p.Name))
              .ToList()
              .ForEach(x => x.Remove());
            return json.ToString(Formatting.None);
        }
    }
}
