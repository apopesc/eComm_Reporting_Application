#pragma checksum "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f941958f1ad0588197862f233028b2872be5b91e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_MarMaxxReports_EditReportSub), @"mvc.1.0.view", @"/Views/MarMaxxReports/EditReportSub.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\_ViewImports.cshtml"
using eComm_Reporting_Application;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\_ViewImports.cshtml"
using eComm_Reporting_Application.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f941958f1ad0588197862f233028b2872be5b91e", @"/Views/MarMaxxReports/EditReportSub.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f6129273a708ab386dbf9214856d17c71d8a66c4", @"/Views/_ViewImports.cshtml")]
    public class Views_MarMaxxReports_EditReportSub : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<eComm_Reporting_Application.Models.EditReportSubscriptionModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/editMarMaxxReport.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/css/editMarMaxxReport.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
  
    ViewData["Title"] = "Edit Report Subscription";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f941958f1ad0588197862f233028b2872be5b91e4938", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
            }
            );
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "f941958f1ad0588197862f233028b2872be5b91e6060", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n<div class=\"page-header\">\r\n    <div class=\"page-title\">\r\n        <h1 class=\"title-text\">Edit MarMaxx Report Subscription</h1>\r\n    </div>\r\n</div>\r\n\r\n<div class=\"hidden-selected-groups\">\r\n    ");
#nullable restore
#line 18 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
Write(Html.HiddenFor(selectedGroupNames => Model.selectedGroupNames));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    ");
#nullable restore
#line 19 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
Write(Html.HiddenFor(selectedGroupIDs => Model.selectedGroupIDs));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n\r\n<div class=\"hidden-dynamic-params\">\r\n");
#nullable restore
#line 23 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
     if (Model.dynamicParams != null)
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 25 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
         foreach (var parameter in Model.dynamicParams)
        {
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 27 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
       Write(Html.Hidden(parameter.Key, parameter.Value, new { @id = "selectedParamVal" }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 27 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
                                                                                          ;
        }

#line default
#line hidden
#nullable disable
#nullable restore
#line 28 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
         
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\r\n\r\n<div class=\"hidden-folder-name\">\r\n    ");
#nullable restore
#line 33 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
Write(Html.HiddenFor(selectedFolderName => Model.folderName));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
</div>

<div id=""hiddenParamNames"">
</div>

<div class=""filter-data"">
    <div class=""addnew-row textbox"">
        <div class=""addnew-textbox"">
            <label class=""filter-label"" for=""subscription-id"">Subscription ID: </label>
            <input type=""text"" id=""subscriptionID"" class=""subscription-textbox"" name=""subscription-id"" disabled");
            BeginWriteAttribute("value", " value=\"", 1324, "\"", 1353, 1);
#nullable restore
#line 43 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
WriteAttributeValue("", 1332, Model.subscriptionID, 1332, 21, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" />
        </div>
    </div>

    <div class=""addnew-row textbox"">
        <div class=""addnew-textbox"">
            <label class=""filter-label"" for=""subscription-name"">Subscription Name: </label>
            <input type=""text"" id=""subscriptionName"" class=""subscription-textbox"" name=""subscription-name""");
            BeginWriteAttribute("value", " value=\"", 1664, "\"", 1695, 1);
#nullable restore
#line 50 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
WriteAttributeValue("", 1672, Model.subscriptionName, 1672, 23, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" />
        </div>
    </div>

    <div class=""addnew-row"">
        <div class=""addnew-dropdown"">
            <label class=""filter-label"" for=""marMaxxReportName"">Report Name: </label>
            <select id=""marMaxxReportName"" name=""marMaxxReportName"" disabled>
                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f941958f1ad0588197862f233028b2872be5b91e11725", async() => {
#nullable restore
#line 58 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
                                                               Write(Model.reportName);

#line default
#line hidden
#nullable disable
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 58 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
                   WriteLiteral(Model.reportName);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginWriteTagHelperAttribute();
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __tagHelperExecutionContext.AddHtmlAttribute("disabled", Html.Raw(__tagHelperStringValueBuffer), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.Minimized);
            BeginWriteTagHelperAttribute();
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __tagHelperExecutionContext.AddHtmlAttribute("selected", Html.Raw(__tagHelperStringValueBuffer), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.Minimized);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
            </select>
        </div>
    </div>

    <div class=""addnew-row"">
        <div class=""addnew-dropdown"">
            <label class=""filter-label"" for=""marMaxxGroup"">Group:</label>
            <select id=""marMaxxGroup"" name=""marMaxxGroup"" multiple>
");
#nullable restore
#line 67 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
                 for (int i = 0; i < Model.groupIDs.Count; i++)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f941958f1ad0588197862f233028b2872be5b91e14890", async() => {
                WriteLiteral(" ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "title", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 69 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
AddHtmlAttributeValue("", 2447, Model.groupIDs[i], 2447, 18, false);

#line default
#line hidden
#nullable disable
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 69 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
                                                 WriteLiteral(Model.groupNames[i]);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "label", 7, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            AddHtmlAttributeValue("", 2501, "<b>ID:", 2501, 6, true);
            AddHtmlAttributeValue(" ", 2507, "</b>", 2508, 5, true);
#nullable restore
#line 69 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
AddHtmlAttributeValue("", 2512, Model.groupIDs[i], 2512, 18, false);

#line default
#line hidden
#nullable disable
            AddHtmlAttributeValue("", 2530, "</br>", 2530, 5, true);
            AddHtmlAttributeValue(" ", 2535, "<b>Name:", 2536, 9, true);
            AddHtmlAttributeValue(" ", 2544, "</b>", 2545, 5, true);
#nullable restore
#line 69 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
AddHtmlAttributeValue("", 2549, Model.groupNames[i], 2549, 20, false);

#line default
#line hidden
#nullable disable
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 70 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            </select>
        </div>
    </div>
</div>

<div id=""addNewDynamic"" class=""dynamic-div"">
    <div class=""dynamic-params-div"">
        <div id=""dynamicParams"">
        </div>
    </div>
    <button id=""backToMarMaxx"" class=""btnAddPage"" type=""button""");
            BeginWriteAttribute("onclick", " onclick=\"", 2872, "\"", 2936, 3);
            WriteAttributeValue("", 2882, "location.href=\'", 2882, 15, true);
#nullable restore
#line 81 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\EditReportSub.cshtml"
WriteAttributeValue("", 2897, Url.Action("Index", "MarMaxxReports"), 2897, 38, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2935, "\'", 2935, 1, true);
            EndWriteAttribute();
            WriteLiteral(">Back</button>\r\n    <div id=\"saveSubscription\" class=\"save-subscription\">\r\n    </div>\r\n</div>\r\n\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<eComm_Reporting_Application.Models.EditReportSubscriptionModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
