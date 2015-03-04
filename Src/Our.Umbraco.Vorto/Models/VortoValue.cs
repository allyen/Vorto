using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var json = JObject.Parse(JsonConvert.SerializeObject(this, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            var propertiesToRemove = new String[] { "dtdId" };

            json.Descendants().OfType<JProperty>()
              .Where(p => propertiesToRemove.Contains(p.Name))
              .ToList()
              .ForEach(x => x.Remove());
            return json.ToString(Formatting.None);
        }
    }
}
