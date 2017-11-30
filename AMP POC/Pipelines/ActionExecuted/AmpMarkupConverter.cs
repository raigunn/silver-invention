using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using HtmlAgilityPack;


namespace AMP_POC.Pipelines.ActionExecuted
{
    /// <summary>
    /// djd: to be deprecated in favor of Html2Amp library
    /// 
    /// current issue: how do we handle images without height and width params, which are required by AMP
    /// https://ampbyexample.com/advanced/how_to_support_images_with_unknown_dimensions/
    /// </summary>
    public class AmpMarkupConverter
    {
        private readonly string source;
        private MemoryStream stream;

        public AmpMarkupConverter(string source)
        {
            this.source = source;
        }

        private AmpMarkupConverter(MemoryStream stream)
        {
            this.stream = stream;
        }

        public static MemoryStream Convert(MemoryStream stream)
        {
            var converter = new AmpMarkupConverter(stream);
            return converter.ConvertStream();
        }

        private MemoryStream ConvertStream()
        {
            var result = UpdatePictureImages(stream);
            return result;
        }

        public static string Convert(string source)
        {
            var converter = new AmpMarkupConverter(source);
            return converter.Convert();
        }

        public string Convert()
        {
            var result = ReplaceIframeWithLink(source);
            result = StripInlineStyles(result);
            result = ReplaceEmbedWithLink(result);
            result = UpdatePictureImages(result);
            result = UpdateAmpImages(result);
            return result;
        }

        private string ReplaceIframeWithLink(string current)
        {
            // Uses HtmlAgilityPack (install-package HtmlAgilityPack)
            var doc = GetHtmlDocument(current);
            var elements = doc.DocumentNode.Descendants("iframe");
            foreach (var htmlNode in elements)
            {
                if (htmlNode.Attributes["src"] == null)
                {
                    continue;
                }
                var link = htmlNode.Attributes["src"].Value;
                var paragraph = doc.CreateElement("p");
                var text = link; // TODO: This might need to be expanded in the future
                var anchor = doc.CreateElement("a");
                anchor.InnerHtml = text;
                anchor.Attributes.Add("href", link);
                anchor.Attributes.Add("title", text);
                paragraph.InnerHtml = anchor.OuterHtml;

                var original = htmlNode.OuterHtml;
                var replacement = paragraph.OuterHtml;

                current = current.Replace(original, replacement);
            }

            return current;
        }

        private string StripInlineStyles(string current)
        {
            // Uses HtmlAgilityPack (install-package HtmlAgilityPack)
            var doc = GetHtmlDocument(current);
            var elements = doc.DocumentNode.Descendants();
            foreach (var htmlNode in elements)
            {
                if (htmlNode.Attributes["style"] == null)
                {
                    continue;
                }

                htmlNode.Attributes.Remove(htmlNode.Attributes["style"]);
            }

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            doc.Save(writer);
            return builder.ToString();
        }

        private string ReplaceEmbedWithLink(string current)
        {
            // Uses HtmlAgilityPack (install-package HtmlAgilityPack)
            var doc = GetHtmlDocument(current);
            var elements = doc.DocumentNode.Descendants("embed");
            foreach (var htmlNode in elements)
            {
                if (htmlNode.Attributes["src"] == null) continue;

                var link = htmlNode.Attributes["src"].Value;
                var paragraph = doc.CreateElement("p");
                var anchor = doc.CreateElement("a");
                anchor.InnerHtml = link;
                anchor.Attributes.Add("href", link);
                anchor.Attributes.Add("title", link);
                paragraph.InnerHtml = anchor.OuterHtml;
                var original = htmlNode.OuterHtml;
                var replacement = paragraph.OuterHtml;

                current = current.Replace(original, replacement);
            }

            return current;
        }


        private MemoryStream UpdatePictureImages(MemoryStream current)
        {
            var doc = new HtmlDocument();
            doc.Load(current);

            // Uses HtmlAgilityPack (install-package HtmlAgilityPack)
            //var doc = GetHtmlDocument(current);
            IList<HtmlNode> pictureList = doc.DocumentNode.Descendants("picture").ToList();
            const string ampImage = "amp-img";
            //string output = "";

            IList<HtmlNode> sourceNodes = new List<HtmlNode>();
            HtmlNode imgNode = null;
            foreach (var pictureNode in pictureList)
            {
                var parentNode = pictureNode.ParentNode;
                var childNodes = pictureNode.ChildNodes;
                foreach (var childNode in childNodes)
                {
                    if (childNode.Name.Equals("source"))
                    {
                        sourceNodes.Add(childNode);
                    }
                    else if (childNode.Name.Equals("img"))
                    {
                        imgNode = childNode;
                    }
                }

                foreach (var sourceNode in sourceNodes)
                {
                    var ampImgSrcNode = sourceNode.Clone();
                    ampImgSrcNode.Name = ampImage;
                    ampImgSrcNode.Attributes.Add("src", sourceNode.Attributes["srcset"].Value);
                    ampImgSrcNode.Attributes.Remove("srcset");
                    parentNode.InsertBefore(ampImgSrcNode, pictureNode);
                    //output += ampImgSrcNode.WriteTo();
                }

                var ampImgNode = imgNode.Clone();
                ampImgNode.Name = ampImage;
                //output += ampImgNode.WriteTo();
                parentNode.ReplaceChild(pictureNode, ampImgNode);//.Replace(pictureNode.OuterHtml, output);

            }
            return current;
        }

        

        private string UpdateAmpImages(string current)
        {
            // Uses HtmlAgilityPack (install-package HtmlAgilityPack)
            var doc = GetHtmlDocument(current);
            var imageList = doc.DocumentNode.Descendants("img");

            const string ampImage = "amp-img";
            var htmlNodes = imageList as IList<HtmlNode> ?? imageList.ToList();
            if (!htmlNodes.Any())
            {
                return current;
            }

            if (!HtmlNode.ElementsFlags.ContainsKey(ampImage))
            {
                HtmlNode.ElementsFlags.Add(ampImage, HtmlElementFlag.Closed);
            }

            foreach (var imgTag in htmlNodes)
            {
                var replacement = imgTag.Clone();
                replacement.Name = ampImage;
                replacement.Attributes.Remove("caption");
                //replacement.Attributes.Add("layout", "fixed-height");
                //replacement.Attributes.Add("layout", "responsive");
                //replacement.Attributes.Add("height", "112");
                //replacement.Attributes.Add("width", "112");
                current = current.Replace(imgTag.OuterHtml, replacement.OuterHtml);
            }

            return current;
        }






        private string UpdatePictureImages(string current)
        {
            // Uses HtmlAgilityPack (install-package HtmlAgilityPack)
            var doc = GetHtmlDocument(current);
            IList<HtmlNode> pictureList = doc.DocumentNode.Descendants("picture").ToList();
            const string ampImage = "amp-img";

            IList<HtmlNode> sourceNodes = new List<HtmlNode>();
            foreach (var pictureNode in pictureList)
            {
                var parentNode = pictureNode.ParentNode;  // parent of the <picture> node
                var parentNodeOriginal = parentNode.InnerHtml;
                var childNodes = pictureNode.ChildNodes;  // all children of the <picture> node
                foreach (var childNode in childNodes)
                {
                    if (childNode.Name.Equals("source"))
                    {
                        sourceNodes.Add(childNode);
                    }
                }

                foreach (var sourceNode in sourceNodes)
                {
                    var ampImgSrcNode = sourceNode.Clone();
                    ampImgSrcNode.Name = ampImage;
                    ampImgSrcNode.Attributes.Add("src", sourceNode.Attributes["srcset"].Value);
                    ampImgSrcNode.Attributes.Remove("srcset");
                    ampImgSrcNode.Attributes.Add("layout", "fill");
                    ampImgSrcNode.Attributes.Add("class", "contain");
                    //ampImgSrcNode.Attributes.Add("height", "100%");
                    //ampImgSrcNode.Attributes.Add("width", "100%");
                    parentNode.InsertBefore(ampImgSrcNode, pictureNode);
                }
                
                parentNode.RemoveChild(pictureNode);
                current = current.Replace(parentNodeOriginal, parentNode.InnerHtml);
            }
            return current;
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
    }
}