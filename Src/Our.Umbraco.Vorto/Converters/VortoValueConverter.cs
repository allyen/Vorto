using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Our.Umbraco.Vorto.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Vorto.Converters
{
	[PropertyValueType(typeof(VortoValue))]
	[PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
	public class VortoValueConverter : PropertyValueConverterBase
	{
		public override bool IsConverter(PublishedPropertyType propertyType)
		{
			return propertyType.PropertyEditorAlias.Equals("Our.Umbraco.Vorto");
		}

		public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
		{
			try
			{
				if (source != null && !source.ToString().IsNullOrWhiteSpace())
				{
                    var value = JsonConvert.DeserializeObject<VortoValue>(source.ToString());
                    value.DtdId = propertyType != null ? propertyType.DataTypeId : 0;
                    return value;
				}
			}
			catch (Exception e)
			{
				LogHelper.Error<VortoValueConverter>("Error converting Vorto value", e);
			}

			return null;
		}
	}
}
