#pragma checksum "C:\Users\Thiago\source\repos\RplpAvecBD\RplpAvecBD\Views\Teacher\ResultatMiseAJourParametres.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "26f919cc7ef56fad9da7c90a454b594288157a38"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Teacher_ResultatMiseAJourParametres), @"mvc.1.0.view", @"/Views/Teacher/ResultatMiseAJourParametres.cshtml")]
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
#line 1 "C:\Users\Thiago\source\repos\RplpAvecBD\RplpAvecBD\Views\_ViewImports.cshtml"
using RplpAvecBD;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Thiago\source\repos\RplpAvecBD\RplpAvecBD\Views\_ViewImports.cshtml"
using RplpAvecBD.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"26f919cc7ef56fad9da7c90a454b594288157a38", @"/Views/Teacher/ResultatMiseAJourParametres.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c4e2224b49701e6f2a4de5f603fa483f02d55e7f", @"/Views/_ViewImports.cshtml")]
    public class Views_Teacher_ResultatMiseAJourParametres : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "C:\Users\Thiago\source\repos\RplpAvecBD\RplpAvecBD\Views\Teacher\ResultatMiseAJourParametres.cshtml"
  
    ViewData["Title"] = "Paramètres";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""text-left"">
    <br />
    <h2 style=""color:forestgreen"">Succès !</h2>
    <br />
    <h6>Vos informations ont été mises à jour avec succès.</h6>
    <br />
</div>
<hr />
<div>
    <input type=""button"" class=""btn btn-primary"" id=""btnReturn"" value=""Aller à l'accueil""");
            BeginWriteAttribute("onclick", " onclick=\"", 337, "\"", 394, 3);
            WriteAttributeValue("", 347, "location.href=\'", 347, 15, true);
#nullable restore
#line 15 "C:\Users\Thiago\source\repos\RplpAvecBD\RplpAvecBD\Views\Teacher\ResultatMiseAJourParametres.cshtml"
WriteAttributeValue("", 362, Url.Action("Index", "Teacher"), 362, 31, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 393, "\'", 393, 1, true);
            EndWriteAttribute();
            WriteLiteral(" />\r\n</div>\r\n");
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