#pragma checksum "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "851a4c118845fcf568d1dd334cf5ac876bde389c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_SubscriptionGroups_Index), @"mvc.1.0.view", @"/Views/SubscriptionGroups/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"851a4c118845fcf568d1dd334cf5ac876bde389c", @"/Views/SubscriptionGroups/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f6129273a708ab386dbf9214856d17c71d8a66c4", @"/Views/_ViewImports.cshtml")]
    public class Views_SubscriptionGroups_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<eComm_Reporting_Application.Models.UserSubscriptionDropdownModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/userSubscriptions.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/css/userSubscriptions.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral("\r\n");
#nullable restore
#line 3 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
  
    ViewData["Title"] = "User Subscriptions Groups";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral(" \r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "851a4c118845fcf568d1dd334cf5ac876bde389c4953", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "851a4c118845fcf568d1dd334cf5ac876bde389c6075", async() => {
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
            WriteLiteral("\r\n\r\n<div class=\"page-header\">\r\n    <div class=\"page-title\">\r\n        <h1 class=\"title-text\">User Subscription Groups</h1>\r\n    </div>\r\n</div>\r\n\r\n<div class=\"hidden-groupIDs\">\r\n");
#nullable restore
#line 19 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
     for (int i = 0; i < Model.groupsIDList.Count; i++)
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 21 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
   Write(Html.HiddenFor(groupID => Model.groupsIDList[i]));

#line default
#line hidden
#nullable disable
#nullable restore
#line 21 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                                                         
    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>


<div class=""filter-data"">
    <div class=""first-row"">
        <div class=""main-dropdown"">
            <label class=""filter-label"" for=""masterGroupDropdown"">Master Group:</label>
            <select id=""masterGroupDropdown"" name=""masterGroupDropdown"" multiple>
");
#nullable restore
#line 31 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                 foreach (var master_group in @Model.masterGroupsList)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "851a4c118845fcf568d1dd334cf5ac876bde389c8877", async() => {
                WriteLiteral(" ");
#nullable restore
#line 33 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                                            Write(master_group);

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
#line 33 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                      WriteLiteral(master_group);

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
#line 34 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </select>\r\n        </div>\r\n        <div class=\"main-dropdown\">\r\n            <label class=\"filter-label\" for=\"groupDropdown\">Group:</label>\r\n            <select id=\"groupDropdown\" name=\"groupDropdown\" multiple>\r\n");
#nullable restore
#line 40 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                 foreach (var group in @Model.groupsList)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "851a4c118845fcf568d1dd334cf5ac876bde389c11671", async() => {
                WriteLiteral(" ");
#nullable restore
#line 42 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                                     Write(group);

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
#line 42 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                      WriteLiteral(group);

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
#line 43 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            </select>
        </div>
        <div class=""is-active"">
            <p class=""radio-label filter-label"">Is Active:</p>
            <div class=""radio-btns"">
                <input type=""checkbox"" id=""checkYes"" class=""checkbox-isactive"" name=""is_active"" value=""yes"" />
                <label id=""yesBox"" class=""yes-box form-check-label"" for=""yes"">Yes</label>
                <input type=""checkbox"" id=""checkNo"" class=""checkbox-isactive"" name=""is_active"" value=""no"" />
                <label for=""no"" class=""form-check-label"">No</label>
            </div>
        </div>
    </div>
    
    <button id=""btnViewUserData"" class=""btnViewData"" type=""button"">View Data</button>
</div>

<div class=""separator-line"">
</div>

<div class=""addNew"">
    <h4 class=""addNewText"">Add New User</h4>
    <button id=""addNewBtn"" class=""addNewBtn""><i class=""fas fa-plus-circle""");
            BeginWriteAttribute("onclick", " onclick=\"", 2313, "\"", 2389, 3);
            WriteAttributeValue("", 2323, "location.href=\'", 2323, 15, true);
#nullable restore
#line 65 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\SubscriptionGroups\Index.cshtml"
WriteAttributeValue("", 2338, Url.Action("AddNewUserSub", "SubscriptionGroups"), 2338, 50, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2388, "\'", 2388, 1, true);
            EndWriteAttribute();
            WriteLiteral("></i></button>\r\n</div>\r\n\r\n<div id=\"userSubscriptionData\">\r\n</div>\r\n\r\n<button id=\"btnSubmitUser\" class=\"btnSubmit\" type=\"button\">Submit</button>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<eComm_Reporting_Application.Models.UserSubscriptionDropdownModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
