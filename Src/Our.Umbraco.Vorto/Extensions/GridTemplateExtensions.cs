using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.Vorto.Extensions
{
    public static class GridTemplateExtensions
    {
        public static MvcHtmlString GetVortoGridHtml<T>(this IPublishedContent contentItem, HtmlHelper html)
        {
            return GetVortoGridHtml(contentItem, html, "bodyText", "Grid/bootstrap3");
        }

        public static MvcHtmlString GetVortoGridHtml(this IPublishedContent contentItem, HtmlHelper html, string propertyAlias)
        {
            return GetVortoGridHtml(contentItem, html, propertyAlias, "Grid/bootstrap3");
        }

        public static MvcHtmlString GetVortoGridHtml(this IPublishedContent contentItem, HtmlHelper html, string propertyAlias, string view)
        {
            var model = contentItem.GetVortoValue(propertyAlias);

            return html.Partial(view, model, new ViewDataDictionary());
        }
    }
}
