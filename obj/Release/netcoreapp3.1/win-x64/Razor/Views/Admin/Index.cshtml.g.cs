#pragma checksum "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "21149501148ac01b38aeca8bef73a69057324521"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Admin_Index), @"mvc.1.0.view", @"/Views/Admin/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"21149501148ac01b38aeca8bef73a69057324521", @"/Views/Admin/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f6129273a708ab386dbf9214856d17c71d8a66c4", @"/Views/_ViewImports.cshtml")]
    public class Views_Admin_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<eComm_Reporting_Application.Models.AdminPageModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/admin.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/css/admin.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 4 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
  
    ViewData["Title"] = "Admin";

#line default
#line hidden
#nullable disable
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "21149501148ac01b38aeca8bef73a690573245215070", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "21149501148ac01b38aeca8bef73a690573245216192", async() => {
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
            WriteLiteral("\r\n\r\n<div class=\"page-header\">\r\n    <div class=\"page-title\">\r\n        <h1 class=\"title-text\">Admin</h1>\r\n    </div>\r\n</div>\r\n\r\n<input type=\"hidden\" id=\"RequestVerificationToken\" name=\"RequestVerificationToken\"");
            BeginWriteAttribute("value", " value=\"", 482, "\"", 535, 1);
#nullable restore
#line 18 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
WriteAttributeValue("", 490, Xsrf.GetAndStoreTokens(Context).RequestToken, 490, 45, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" />

<div class=""groups-div"">

    <div class=""sub-header"">
        <div class=""page-title"">
            <h1 class=""title-text sub"">Add New Group</h1>
        </div>
    </div>
    <div id=""addNewGroups"" class=""addNewGroups"">
        <div class=""addnew-row"">
            <div class=""addnew-dropdown"">
                <label class=""filter-label"" for=""masterGroupDropdown"">Master Group:</label>
                <select id=""addNewMasterGroup"" name=""masterGroupDropdown"" class=""add-new-dropdown"">
                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "21149501148ac01b38aeca8bef73a690573245218518", async() => {
                WriteLiteral("Select a master group...");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
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
            WriteLiteral("\r\n");
#nullable restore
#line 33 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                     foreach (var master_group in @Model.masterGroupsList)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "21149501148ac01b38aeca8bef73a6905732452110664", async() => {
                WriteLiteral(" ");
#nullable restore
#line 35 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
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
#line 35 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
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
#line 36 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                </select>
            </div>
        </div>
        <div class=""addnew-row textbox"">
            <div class=""addnew-textbox"">
                <label class=""filter-label"" for=""group-input"">Group ID: </label>
                <input type=""text"" id=""addNewGroupID"" class=""subscription-textbox"" name=""group-input"" />
            </div>
        </div>
        <div class=""addnew-row textbox"">
            <div class=""addnew-textbox"">
                <label class=""filter-label"" for=""group-input"">Group Name: </label>
                <input type=""text"" id=""addNewGroupName"" class=""subscription-textbox"" name=""group-input"" />
            </div>
        </div>
        <div class=""addnew-row-last"">
            <button id=""addGroupSubmit"" class=""btnAddPage"" type=""button"">Add Group</button>
        </div>
    </div>

    <div id=""groupsTableDiv"" class=""groupsTableDiv"">
        <table id=""groupsTable"">
            <thead>
                <tr class=""groupsHeader"">
                    <th></t");
            WriteLiteral("h>\r\n                    <th>Master Group</th>\r\n                    <th>Group ID</th>\r\n                    <th>Group Name</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n");
#nullable restore
#line 68 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                 foreach (var group in @Model.groupsList)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        <td><button class=\"deleteBtn\"><i class=\"fa fa-trash\"></i></button></td>\r\n                        <td class=\"masterGroupEntry\">");
#nullable restore
#line 72 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                                                Write(group.masterGroup);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"groupIDEntry\">");
#nullable restore
#line 73 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                                            Write(group.groupID);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"groupNameEntry\">");
#nullable restore
#line 74 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                                              Write(group.groupName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n");
#nullable restore
#line 76 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            </tbody>
        </table>
    </div>
</div>

<div class=""master-groups-div"">
    <div class=""sub-header"">
        <div class=""page-title"">
            <h1 class=""title-text sub"">Add New Master Group</h1>
        </div>
    </div>
    <div id=""addNewGroups"" class=""addNewGroups"">
        <div class=""addnew-row textbox first"">
            <div class=""addnew-textbox"">
                <label class=""filter-label"" for=""group-input"">Master Group: </label>
                <input type=""text"" id=""masterGroup"" class=""subscription-textbox"" name=""group-input"" />
            </div>
        </div>

        <div class=""addnew-row-last"">
            <button id=""addMasterGroupSubmit"" class=""btnAddPage"" type=""button"">Add Master Group</button>
        </div>
    </div>

    <div id=""masterGroupsTableDiv"" class=""masterGroupsTableDiv"">
        <table id=""masterGroupsTable"">
            <thead>
                <tr class=""groupsHeader"">
                    <th></th>
                    <th>Mas");
            WriteLiteral("ter Group</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n");
#nullable restore
#line 110 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                 foreach (var master_group in @Model.masterGroupsList)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        <td><button class=\"deleteBtn\"><i class=\"fa fa-trash\"></i></button></td>\r\n                        <td class=\"new_masterGroupEntry\">");
#nullable restore
#line 114 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                                                    Write(master_group);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n");
#nullable restore
#line 116 "C:\Users\and06572\source\repos\eComm_Reporting_Application\eComm_Reporting_Application\Views\Admin\Index.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </tbody>\r\n        </table>\r\n    </div>\r\n</div>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Microsoft.AspNetCore.Antiforgery.IAntiforgery Xsrf { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<eComm_Reporting_Application.Models.AdminPageModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
