using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace MyTemplate.Web.Core.TagHelpers
{
    [HtmlTargetElement("div")]
    public class PaginationTagHelper : TagHelper
    {
        private JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

        private const string ControllerAttributeName = "asp-controller";
        private const string ActionAttributeName = "asp-action";

        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }

        //private const string ConfirmAttributeName = "data-ajax-confirm";
        //private const string HttpMethodAttributeName = "data-ajax-method";
        //private const string InsertionModeAttributeName = "data-ajax-mode";
        //private const string LoadingElementDurationAttributeName = "data-ajax-loading-duration";
        //private const string LoadingElementIdAttributeName = "data-ajax-loading";
        //private const string OnBeginAttributeName = "data-ajax-begin";
        //private const string OnCompleteAttributeName = "data-ajax-complete";
        //private const string OnFailureAttributeName = "data-ajax-failure";
        //private const string OnSuccessAttributeName = "data-ajax-success";
        //private const string UpdateTargetIdAttributeName = "data-ajax-update";
        //private const string UrlAttributeName = "data-ajax-url";

        //[HtmlAttributeName(ConfirmAttributeName)]
        //public string Confirm { get; set; }
        //[HtmlAttributeName(ConfirmAttributeName)]
        //public string HttpMethod { get; set; }
        //[HtmlAttributeName(InsertionModeAttributeName)]
        //public string InsertionMode { get; set; }
        //[HtmlAttributeName(LoadingElementDurationAttributeName)]
        //public string LoadingElementDuration { get; set; }
        //[HtmlAttributeName(LoadingElementIdAttributeName)]
        //public string LoadingElementId { get; set; }
        //[HtmlAttributeName(OnBeginAttributeName)]
        //public string OnBegin { get; set; }
        //[HtmlAttributeName(OnCompleteAttributeName)]
        //public string OnComplete { get; set; }
        //[HtmlAttributeName(OnFailureAttributeName)]
        //public string OnFailure { get; set; }
        //[HtmlAttributeName(OnSuccessAttributeName)]
        //public string OnSuccess { get; set; }
        //[HtmlAttributeName(UpdateTargetIdAttributeName)]
        //public string UpdateTargetId { get; set; }
        //[HtmlAttributeName(UrlAttributeName)]
        //public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Controller))
            {
                if (!string.IsNullOrEmpty(Action))
                {
                    output.Attributes.Add("pagination-url", $"{Action}");
                }
            }
            else if (string.IsNullOrEmpty(Action))
            {
                output.Attributes.Add("pagination-url", $"/{Controller}");
            }
            else
            {
                output.Attributes.Add("pagination-url", $"/{Controller}/{Action}");
            }
        }
    }
}
