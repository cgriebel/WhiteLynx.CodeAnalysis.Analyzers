using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using VerifyCS = WhiteLynx.CodeAnalysis.Analyzers.Test.CSharpCodeFixVerifier<
    WhiteLynx.CodeAnalysis.Analyzers.LogFormatPlaceholderCaseAnalyzer,
    WhiteLynx.CodeAnalysis.Analyzers.LogFormatPlaceholderCaseCodeFixProvider>;

namespace WhiteLynx.CodeAnalysis.Analyzers.Test
{
    [TestClass]
    public class WhiteLynxCodeAnalysisAnalyzersUnitTest
    {
        private async Task VerifyCodeFixAsync(string source, DiagnosticResult expected, string fixedSource)
            => await VerifyCodeFixAsync(source, new[] { expected }, fixedSource);

        private async Task VerifyCodeFixAsync(string source, DiagnosticResult[] expected, string fixedSource)
        {
            var test = new VerifyCS.Test
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("WhiteLynxCodeAnalysisAnalyzers").WithLocation(0).WithArguments("TypeName");
            await VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
