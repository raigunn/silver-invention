using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using HtmlAgilityPack;

namespace AMP_POC.Pipelines.ActionExecuted
{
    /// <summary>
    /// todo: give this a better name, put it in a better place
    /// </summary>
    public class HtmlDocHelper
    {
        private string _htmlSource;
        private HtmlDocument _htmlDocument;
        public HtmlDocHelper(string htmlSource)
        {
            _htmlSource = htmlSource;
            _htmlDocument = GetHtmlDocument(_htmlSource);
        }


        private HtmlDocument GetHtmlDocument(string htmlContent)
        {
            var doc = new HtmlDocument
            {
                // OptionOutputAsXml = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };
            doc.LoadHtml(htmlContent);

            return doc;
        }

        /// <summary>
        /// Get everything from inside the <body/> tag with the 
        /// intent of amp-sanitizing it through Html2Amp
        /// </summary>
        /// <returns></returns>
        public string GetBody()
        {
            return _htmlDocument.DocumentNode.SelectSingleNode("//body").InnerHtml;
        }

        public string InsertBody(string newHtmlBody)
        {
            HtmlNode body = _htmlDocument.DocumentNode.SelectSingleNode("//body");
            body.InnerHtml = body.InnerHtml.Replace(body.InnerHtml, newHtmlBody);
            return _htmlDocument.DocumentNode.OuterHtml;
        }
    }
}