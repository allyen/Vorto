using System;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Our.Umbraco.Vorto.Helpers;
using Our.Umbraco.Vorto.Models;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Our.Umbraco.Vorto.Extensions
{
	public static class IPublishedContentExtensions
	{
		#region HasValue

		public static bool HasVortoValue(this IPublishedContent content, string propertyAlias, 
			string cultureName = null, bool recursive = false)
		{
			if (cultureName == null)
				cultureName = Thread.CurrentThread.CurrentUICulture.Name;

			if (!content.HasValue(propertyAlias, recursive))
				return false;

			var prop = content.GetProperty(propertyAlias, recursive);
			if (prop.Value is VortoValue)
			{
				var vortoModel = prop.Value as VortoValue;
				if (!vortoModel.Values.ContainsKey(cultureName) || vortoModel.Values[cultureName] == null
					|| vortoModel.Values[cultureName].ToString().IsNullOrWhiteSpace())
						return false;
			}

			return true;
		}

		#endregion

		#region IfHasValue

		// String
		//public static IHtmlString IfHasVortoValue(this IPublishedContent content, string propertyAlias, 
		//	string valueIfTrue, string valueIfFalse = null)
		//{
		//	return !content.HasVortoValue(propertyAlias) 
		//		? new HtmlString(valueIfFalse ?? string.Empty) 
		//		: new HtmlString(valueIfTrue);
		//}

		// No type
		//public static HelperResult IfHasVortoValue(this IPublishedContent content, string propertyAlias,
		//	Func<object, HelperResult> templateIfTrue, Func<object, HelperResult> templateIfFalse = null)
		//{
		//	return content.IfHasVortoValue()
		//}

		// Type
		//public static HelperResult IfHasVortoValue<T>(this IPublishedContent content, string propertyAlias,
		//	Func<T, HelperResult> templateIfTrue, Func<T, HelperResult> templateIfFalse = null)
		//{
		//	return new HelperResult(writer =>
		//	{
		//		if (!content.HasVortoValue(propertyAlias))
		//		{
		//			if (templateIfFalse != null)
		//				templateIfFalse(null).WriteTo(writer);
		//		}
		//		else
		//		{
		//			var value = content.GetVortoValue(propertyAlias);
		//			templateIfTrue(value).WriteTo(writer);
		//		}
		//	});
		//}

		#endregion

		#region GetValue

		public static object GetVortoValue(this IPublishedContent content, string propertyAlias, string cultureName = null,
			bool recursive = false, object defaultValue = null)
		{
			return content.GetVortoValue<object>(propertyAlias, cultureName, recursive, defaultValue);
		}

		public static T GetVortoValue<T>(this IPublishedContent content, string propertyAlias, string cultureName = null, bool recursive = false, T defaultValue = default(T))
			where T : class 
		{
			if (cultureName == null)
				cultureName = Thread.CurrentThread.CurrentUICulture.Name;

			if (content.HasVortoValue(propertyAlias, cultureName, recursive))
			{
				var prop = content.GetProperty(propertyAlias, recursive);
				if (prop.Value is VortoValue)
				{
                    return Vorto.GetVortoValue<T>(prop.Value, cultureName, defaultValue);
				}
				
				if (prop.Value is T)
				{
					return prop.Value as T;
				}
			}

			return defaultValue;
		}
		
		#endregion

		
	}
}
