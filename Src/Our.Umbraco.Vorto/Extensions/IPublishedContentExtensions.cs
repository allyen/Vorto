using Our.Umbraco.Vorto.Models;
using System.Threading;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Vorto.Extensions
{
    public static class IPublishedContentExtensions
	{
		#region HasValue

		private static bool DoHasVortoValue(this IPublishedContent content, string propertyAlias,
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
				if (vortoModel.Values == null || !vortoModel.Values.ContainsKey(cultureName)
                    || vortoModel.Values[cultureName] == null
					|| vortoModel.Values[cultureName].ToString().IsNullOrWhiteSpace())
						return false;
			}

			return true;
		}

        public static bool HasVortoValue(this IPublishedContent content, string propertyAlias,
            string cultureName = null, bool recursive = false, string fallbackCultureName = null)
        {
            var hasValue = content.DoHasVortoValue(propertyAlias, cultureName, recursive);
            if (!hasValue && !string.IsNullOrEmpty(fallbackCultureName) && !fallbackCultureName.Equals(cultureName))
                hasValue = content.DoHasVortoValue(propertyAlias, fallbackCultureName, recursive);
            return hasValue;
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

		private static object DoGetVortoValue(this IPublishedContent content, string propertyAlias, string cultureName = null,
			bool recursive = false, object defaultValue = null)
		{
			return content.GetVortoValue<object>(propertyAlias, cultureName, recursive, defaultValue);
		}

        public static object GetVortoValue(this IPublishedContent content, string propertyAlias, string cultureName = null,
            bool recursive = false, object defaultValue = null, string fallbackCultureName = null)
        {
            var result = content.DoGetVortoValue(propertyAlias, cultureName, recursive);
            if (result == null && !string.IsNullOrEmpty(fallbackCultureName) && !fallbackCultureName.Equals(cultureName))
                result = content.DoGetVortoValue(propertyAlias, fallbackCultureName, recursive, defaultValue);

            return result;
        }

		private static T DoGetVortoValue<T>(this IPublishedContent content, string propertyAlias, string cultureName = null, 
            bool recursive = false, T defaultValue = default(T))
		{
			if (cultureName == null)
				cultureName = Thread.CurrentThread.CurrentUICulture.Name;

			if (content.HasVortoValue(propertyAlias, cultureName, recursive))
			{
				var prop = content.GetProperty(propertyAlias, recursive);
				if (prop.Value is VortoValue)
				{
                    return Vorto.GetVortoValue<T>(prop.Value, (prop.Value as VortoValue).DtdId, content.ContentType, cultureName, defaultValue);
				}
				
				if (prop.Value is T)
				{
					return (T)prop.Value;
				}
			}

			return defaultValue;
		}


        public static T GetVortoValue<T>(this IPublishedContent content, string propertyAlias, string cultureName = null,
            bool recursive = false, T defaultValue = default(T), string fallbackCultureName = null)
        {
            var result = content.DoGetVortoValue<T>(propertyAlias, cultureName, recursive);
            if (result == null && !string.IsNullOrEmpty(fallbackCultureName) && !fallbackCultureName.Equals(cultureName))
                result = content.DoGetVortoValue<T>(propertyAlias, fallbackCultureName, recursive, defaultValue);

            return result;
        }

	    #endregion
	}
}
