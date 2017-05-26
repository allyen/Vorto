using Our.Umbraco.Vorto.Helpers;
using Our.Umbraco.Vorto.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Our.Umbraco.Vorto
{
    public static class Vorto
	{
		#region Event Handlers

		public static event EventHandler<FilterLanguagesEventArgs> FilterLanguages;

        public static T GetVortoValue<T>(object value, int dataTypeId, PublishedContentType contentType, string cultureName = null, T defaultValue = default(T))
        {
            if (cultureName == null)
                cultureName = Thread.CurrentThread.CurrentUICulture.Name;

            if (value is VortoValue)
            {
                // Get the serialized value
                var vortoModel = value as VortoValue;

                object langValue = null;
                if (!vortoModel.Values.TryGetValue(cultureName, out langValue))
                    return defaultValue;
                
                // Get target datatype
                var targetDataType = VortoHelper.GetTargetDataTypeDefinition(dataTypeId);
                var properyType = CreateDummyPropertyType(targetDataType.Id, targetDataType.PropertyEditorAlias, contentType);

                var source = properyType.ConvertDataToSource(langValue, false);
                var result = properyType.ConvertSourceToObject(source, false);
                if (result is T)
                    return (T) result;

                var convert = result.TryConvertTo<T>();
                if (convert.Success)
                    return convert.Result;
                else
                    return defaultValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Creates dummy property type.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns></returns>
        internal static PublishedPropertyType CreateDummyPropertyType(int dataTypeId, string propertyEditorAlias, PublishedContentType contentType)
        {
            // We need to check if `PropertyValueConvertersResolver` exists,
            // otherwise `PublishedPropertyType` will throw an exception outside of the Umbraco context.; e.g. unit-tests.
            if (!PropertyValueConvertersResolver.HasCurrent)
                return null;

            return (PublishedPropertyType) UmbracoContext.Current.Application.ApplicationCache.RequestCache.GetCacheItem("VORTO_" + contentType?.Alias + "_" + dataTypeId, () =>
                 new PublishedPropertyType(contentType, new PropertyType(new DataTypeDefinition(-1, propertyEditorAlias) { Id = dataTypeId })));
        }

        internal static void CallFilterLanguages(FilterLanguagesEventArgs args)
		{
			if (FilterLanguages != null)
				FilterLanguages(null, args);
		}

		#endregion
	}

	#region Event Args

	public class FilterLanguagesEventArgs : EventArgs
	{
		public int CurrentPageId { get; set; }
		public int ParentPageId { get; set; }

		public IList<Our.Umbraco.Vorto.Models.Language> Languages { get; set; }
	}

	#endregion
}
