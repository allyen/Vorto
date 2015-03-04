using Our.Umbraco.Vorto.Helpers;
using Our.Umbraco.Vorto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Vorto
{
	public static class Vorto
	{
		#region Event Handlers

		public static event EventHandler<FilterLanguagesEventArgs> FilterLanguages;

        public static T GetVortoValue<T>(object value, int dataTypeId, string cultureName = null, T defaultValue = default(T))
        {
            if (cultureName == null)
                cultureName = Thread.CurrentThread.CurrentUICulture.Name;

            if (value is VortoValue)
            {
                // Get the serialized value
                var vortoModel = value as VortoValue;
                var langValue = vortoModel.Values[cultureName];

                // If the value is of type T, just return it
                //if (value is T)
                //	return (T)value;

                // Get target datatype
                var targetDataType = VortoHelper.GetTargetDataTypeDefinition(dataTypeId);

                // Umbraco has the concept of a IPropertyEditorValueConverter which it 
                // also queries for property resolvers. However I'm not sure what these
                // are for, nor can I find any implementations in core, so am currently
                // just ignoring these when looking up converters.
                // NB: IPropertyEditorValueConverter not to be confused with
                // IPropertyValueConverter which are the ones most people are creating
                var properyType = CreateDummyPropertyType(targetDataType.Id, targetDataType.PropertyEditorAlias);
                var converters = PropertyValueConvertersResolver.Current.Converters.ToArray();

                // In umbraco, there are default value converters that try to convert the 
                // value if all else fails. The problem is, they are also in the list of
                // converters, and the means for filtering these out is internal, so
                // we currently have to try ALL converters to see if they can convert
                // rather than just finding the most appropreate. If the ability to filter
                // out default value converters becomes public, the following logic could
                // and probably should be changed.
                foreach (var converter in converters.Where(x => x.IsConverter(properyType)))
                {
                    // Convert the type using a found value converter
                    var value2 = converter.ConvertDataToSource(properyType, langValue, false);

                    // If the value is of type T, just return it
                    if (value2 is T)
                        return (T)value2;

                    // Value is not final value type, so try a regular type conversion aswell
                    var convertAttempt = value2.TryConvertTo<T>();
                    if (convertAttempt.Success)
                        return convertAttempt.Result;
                }

                // Value is not final value type, so try a regular type conversion
                var convertAttempt2 = langValue.TryConvertTo<T>();
                if (convertAttempt2.Success)
                    return convertAttempt2.Result;

                return defaultValue;
            }

            return defaultValue;
        }

        private static PublishedPropertyType CreateDummyPropertyType(int dataTypeId, string propertyEditorAlias)
        {
            return new PublishedPropertyType(null,
                new PropertyType(new DataTypeDefinition(-1, propertyEditorAlias)
                {
                    Id = dataTypeId
                }));
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
