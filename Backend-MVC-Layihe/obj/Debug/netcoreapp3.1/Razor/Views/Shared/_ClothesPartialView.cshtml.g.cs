#pragma checksum "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\Shared\_ClothesPartialView.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4444f43595fac8fbc9ceb0d617c394842dd7c655"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__ClothesPartialView), @"mvc.1.0.view", @"/Views/Shared/_ClothesPartialView.cshtml")]
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
#line 1 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\_ViewImports.cshtml"
using Backend_MVC_Layihe.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\_ViewImports.cshtml"
using Backend_MVC_Layihe.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\_ViewImports.cshtml"
using Newtonsoft.Json;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4444f43595fac8fbc9ceb0d617c394842dd7c655", @"/Views/Shared/_ClothesPartialView.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"46516354065fa2b428bc33712ed33a2dae0a6b02", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared__ClothesPartialView : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("img-fluid w-100"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\Shared\_ClothesPartialView.cshtml"
 foreach (Clothes clothes in Model)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"col-lg-3 col-md-4 col-sm-6 pb-1\">\r\n        <div class=\"product-item bg-light mb-4\">\r\n            <div class=\"product-img position-relative overflow-hidden\">\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "4444f43595fac8fbc9ceb0d617c394842dd7c6554205", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "src", 2, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            AddHtmlAttributeValue("", 264, "~/assets/img/", 264, 13, true);
#nullable restore
#line 6 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\Shared\_ClothesPartialView.cshtml"
AddHtmlAttributeValue("", 277, clothes.ClothesImages.FirstOrDefault(c=>c.IsMain==true).Name, 277, 61, false);

#line default
#line hidden
#nullable disable
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "alt", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 7 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\Shared\_ClothesPartialView.cshtml"
AddHtmlAttributeValue("", 367, clothes.ClothesImages.FirstOrDefault(c=>c.IsMain==true).Alternative, 367, 68, false);

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
            WriteLiteral("\r\n                <div class=\"product-action\">\r\n                    <a class=\"btn btn-outline-dark btn-square\"");
            BeginWriteAttribute("href", " href=\"", 547, "\"", 554, 0);
            EndWriteAttribute();
            WriteLiteral("><i class=\"fa fa-shopping-cart\"></i></a>\r\n                    <a class=\"btn btn-outline-dark btn-square\"");
            BeginWriteAttribute("href", " href=\"", 659, "\"", 666, 0);
            EndWriteAttribute();
            WriteLiteral("><i class=\"far fa-heart\"></i></a>\r\n                    <a class=\"btn btn-outline-dark btn-square\"");
            BeginWriteAttribute("href", " href=\"", 764, "\"", 771, 0);
            EndWriteAttribute();
            WriteLiteral("><i class=\"fa fa-sync-alt\"></i></a>\r\n                    <a class=\"btn btn-outline-dark btn-square\"");
            BeginWriteAttribute("href", " href=\"", 871, "\"", 878, 0);
            EndWriteAttribute();
            WriteLiteral("><i class=\"fa fa-search\"></i></a>\r\n                </div>\r\n            </div>\r\n            <div class=\"text-center py-4\">\r\n                <a class=\"h6 text-decoration-none text-truncate\"");
            BeginWriteAttribute("href", " href=\"", 1066, "\"", 1073, 0);
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 16 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\Shared\_ClothesPartialView.cshtml"
                                                                    Write(clothes.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a>\r\n                <div class=\"d-flex align-items-center justify-content-center mt-2\">\r\n                    <h5>$");
#nullable restore
#line 18 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\Shared\_ClothesPartialView.cshtml"
                    Write(clothes.DiscountPrice);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h5><h6 class=\"text-muted ml-2\"><del>$");
#nullable restore
#line 18 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\Shared\_ClothesPartialView.cshtml"
                                                                                 Write(clothes.Price);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</del></h6>
                </div>
                <div class=""d-flex align-items-center justify-content-center mb-1"">
                    <small class=""fa fa-star text-primary mr-1""></small>
                    <small class=""fa fa-star text-primary mr-1""></small>
                    <small class=""fa fa-star text-primary mr-1""></small>
                    <small class=""fa fa-star text-primary mr-1""></small>
                    <small class=""fa fa-star text-primary mr-1""></small>
                    <small>(99)</small>
                </div>
            </div>
        </div>
    </div>
");
#nullable restore
#line 31 "C:\Users\efend\source\repos\Backend-MVC-Layihe\Backend-MVC-Layihe\Views\Shared\_ClothesPartialView.cshtml"

}

#line default
#line hidden
#nullable disable
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591