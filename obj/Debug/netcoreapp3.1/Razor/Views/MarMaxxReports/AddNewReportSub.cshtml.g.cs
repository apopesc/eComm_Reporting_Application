#pragma checksum "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2a9c25e4ed96a73f365f712f34052b4717945717"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_MarMaxxReports_AddNewReportSub), @"mvc.1.0.view", @"/Views/MarMaxxReports/AddNewReportSub.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2a9c25e4ed96a73f365f712f34052b4717945717", @"/Views/MarMaxxReports/AddNewReportSub.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f6129273a708ab386dbf9214856d17c71d8a66c4", @"/Views/_ViewImports.cshtml")]
    public class Views_MarMaxxReports_AddNewReportSub : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<eComm_Reporting_Application.Models.AddNewReportSubDropdownModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/addNewMarMaxxReport.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/css/addNewMarMaxxReport.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "EXCELOPENXML", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "PDF", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "CSV", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "Daily", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "Weekly", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_8 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "Quarterly", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 2 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
  
    ViewData["Title"] = "Add New Report Subscription";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b47179457176736", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "2a9c25e4ed96a73f365f712f34052b47179457177858", async() => {
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
            WriteLiteral("\r\n\r\n<div class=\"initial-hidden-selected-report\">\r\n    ");
#nullable restore
#line 12 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
Write(Html.HiddenFor(selectedFolderName => Model.selectedFolder));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    ");
#nullable restore
#line 13 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
Write(Html.HiddenFor(selectedReportName => Model.selectedReport));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
</div>

<div class=""page-header"">
    <div class=""page-title"">
        <h1 class=""title-text"">Add New MarMaxx Report Subscription</h1>
    </div>
</div>

<div class=""filter-data"">
    <div class=""addnew-row"">
        <div class=""addnew-dropdown"">
            <label class=""filter-label"" for=""marMaxxFolderDropdown"">Folder:</label>
            <select id=""marMaxxFolderDropdown"" name=""marMaxxFolderDropdown"" onchange=""selectedFolder();"" multiple>
");
#nullable restore
#line 27 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
                 foreach (var folder in @Model.folders)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b471794571710483", async() => {
                WriteLiteral(" ");
#nullable restore
#line 29 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
                                                 Write(folder.folderName);

#line default
#line hidden
#nullable disable
                WriteLiteral(" ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 29 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
                      WriteLiteral(folder.folderName);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 30 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            </select>
        </div>
    </div>

    <div class=""addnew-row"">
        <div class=""addnew-dropdown"">
            <label class=""filter-label"" for=""marMaxxReportDropdown"">Report Name:</label>
            <select id=""marMaxxReportDropdown"" name=""marMaxxReportDropdown"">
            </select>
        </div>
    </div>

    <div class=""addnew-row-last"">
        <button id=""backToMarMaxx"" class=""btnAddPage"" type=""button""");
            BeginWriteAttribute("onclick", " onclick=\"", 1541, "\"", 1605, 3);
            WriteAttributeValue("", 1551, "location.href=\'", 1551, 15, true);
#nullable restore
#line 44 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
WriteAttributeValue("", 1566, Url.Action("Index", "MarMaxxReports"), 1566, 38, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1604, "\'", 1604, 1, true);
            EndWriteAttribute();
            WriteLiteral(@">Back</button>
        <button id=""viewReportParams"" class=""btnAddPage"" type=""button"" hidden>View Report Parameters</button>
    </div>
</div>

<div id=""addNewDynamic"" class=""dynamic-div"">
    <div class=""dynamic-params-div"">

        <div id=""staticParams"">
            <div class=""addnew-row textbox"">
                <div class=""addnew-textbox"">
                    <label class=""filter-label"" for=""subscription-name"">Subscription Name: </label>
                    <input type=""text"" id=""subscriptionName"" class=""subscription-textbox"" name=""subscription-name"" />
                </div>
            </div>

            <div class=""addnew-row"">
                <div class=""addnew-dropdown"">
                    <label class=""filter-label"" for=""marMaxxGroup"">Group:</label>
                    <select id=""marMaxxGroup"" name=""marMaxxGroup"" multiple>
");
#nullable restore
#line 64 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
                         for (int i = 0; i < Model.groupIDs.Count; i++)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b471794571715075", async() => {
                WriteLiteral(" ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "title", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 66 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
AddHtmlAttributeValue("", 2620, Model.groupIDs[i], 2620, 18, false);

#line default
#line hidden
#nullable disable
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 66 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
                                                         WriteLiteral(Model.groupNames[i]);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "label", 7, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            AddHtmlAttributeValue("", 2674, "<b>ID:", 2674, 6, true);
            AddHtmlAttributeValue(" ", 2680, "</b>", 2681, 5, true);
#nullable restore
#line 66 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
AddHtmlAttributeValue("", 2685, Model.groupIDs[i], 2685, 18, false);

#line default
#line hidden
#nullable disable
            AddHtmlAttributeValue("", 2703, "</br>", 2703, 5, true);
            AddHtmlAttributeValue(" ", 2708, "<b>Name:", 2709, 9, true);
            AddHtmlAttributeValue(" ", 2717, "</b>", 2718, 5, true);
#nullable restore
#line 66 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
AddHtmlAttributeValue("", 2722, Model.groupNames[i], 2722, 20, false);

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
#line 67 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\MarMaxxReports\AddNewReportSub.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    </select>
                </div>
            </div>

            <div class=""addnew-row"">
                <div class=""addnew-dropdown"">
                    <label class=""filter-label"" for=""fileFormat"">File Format:</label>
                    <select id=""fileFormat"" name=""fileFormat"">
                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b471794571719071", async() => {
                WriteLiteral("EXCELOPENXML");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b471794571720261", async() => {
                WriteLiteral("PDF");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b471794571721442", async() => {
                WriteLiteral("CSV");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
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
                    <label class=""filter-label"" for=""schedule"">Schedule:</label>
                    <select id=""schedule"" name=""schedule"">
                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b471794571722938", async() => {
                WriteLiteral("Daily");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_6.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b471794571724121", async() => {
                WriteLiteral("Weekly");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_7.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_7);
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
            WriteLiteral("\r\n                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a9c25e4ed96a73f365f712f34052b471794571725616", async() => {
                WriteLiteral("Quarterly");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_8.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_8);
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
        </div>

        <div id=""dynamicParams"">

        </div>
    </div>


    <div id=""saveSubscription"" class=""save-subscription"">
    </div>
</div>

<div id=""hiddenParamNames"">
</div>

<div id=""hiddenSelectedReport"">
</div>


");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<eComm_Reporting_Application.Models.AddNewReportSubDropdownModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
