using Microsoft.AspNet.Razor.TagHelpers;

namespace SSW.DataOnion.Sample.WebUI.TagHelpers
{
    [HtmlTargetElement("form", Attributes = AjaxForm)]
    public class UnobtrusiveFormTagHelper : TagHelper
    {
        private const string AjaxForm = "asp-ajax";

        [HtmlAttributeName("asp-onsuccess")]
        public string AspOnSuccess { get; set; }

        [HtmlAttributeName(AjaxForm)]
        public bool AspAjax { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            output.Attributes.Add("data-ajax", true);
            output.Attributes.Add("data-onsuccess", AspOnSuccess);
        }
    }
}
