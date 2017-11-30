using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Html2Amp;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.MvcEvents.ResultExecuted;

namespace AMP_POC.Pipelines.ActionExecuted
{
    /// <summary>
    ///    https://weblog.west-wind.com/posts/2009/Nov/13/Capturing-and-Transforming-ASPNET-Output-with-ResponseFilter
    ///    https://stackoverflow.com/questions/15067049/asp-net-mvc-response-filter-is-null-when-using-actionfilterattribute-in-regist
    ///    http://romsteady.blogspot.com/2008/12/workaround-aspnet-response-filter-is.html
    /// </summary>
    public class AmpResultExecutedFilter
    {
        public void Process(ResultExecutedArgs args)
        {
            //todo: better sitecore device detection?
            var device = args.Context.HttpContext.Request.QueryString["amp"];
            if (device != "1") return;
            var response = args.Context.HttpContext.Response;

            // this is called on every control, but only the layout will have the Filter property set
            // so all others shoudl just exit
            if (response.Filter == null) return;

            // instantiate a filter from the response and subscribe to the TransformString event.
            ResponseFilterStream filter = new ResponseFilterStream(response.Filter);
            filter.TransformString += filter_TransformString;

            // assign the tranformed filter back to the response == done
            response.Filter = filter;
        }

        string filter_TransformString(string output)
        {
            return CallHtml2AmpConverter(output);
        }

        private string CallHtml2AmpConverter(string output)
        {
            // todo handle config better
            // configure and instantiate the HtmlToAmp converter
            RunConfiguration config = new RunConfiguration();
            config.RelativeUrlsHost = "http://google-amp-poc.local/";
            config.ShouldDownloadImages = true;
            var converter = new HtmlToAmpConverter().WithConfiguration(config);

            // instantiate utility to parse out the body
            HtmlDocHelper agility = new HtmlDocHelper(output);

            // get just the body of the current request
            string body = agility.GetBody();
            // run the body through the Html2Amp converter
            string ampHtml = converter.ConvertFromHtml(body).AmpHtml;
            // put the converted body back into the request doc
            string newOutput = agility.InsertBody(ampHtml);
            return newOutput;
        }
    }
}