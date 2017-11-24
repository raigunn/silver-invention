using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
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
            var f = response.Filter;
            var o1 = response.Output;

            if (response.Filter == null) return;
            ResponseFilterStream filter = new ResponseFilterStream(response.Filter);
            //filter.TransformStream += filter_TransformStream;
            filter.TransformString += filter_TransformString;
            response.Filter = filter;
            var o2 = response.Output;
        }

        MemoryStream filter_TransformStream(MemoryStream stream)
        {
            return FixPaths(stream);
        }

        string filter_TransformString(string output)
        {
            return FixPaths(output);
        }
        private string FixPaths(string output)
        {
            output = AmpMarkupConverter.Convert(output);

            //output = output.Replace("<img", "<amp-img");
            return output;
        }

        private MemoryStream FixPaths(MemoryStream stream)
        {
            stream = AmpMarkupConverter.Convert(stream);
            return stream;
        }
    }
}