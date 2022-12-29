using idee5.Common;
using idee5.SourceGenerators;
using VerifyCS = idee5.SoureGeneratorTests.Verifiers.CSharpAnalyzerVerifier<idee5.SourceGenerators.GeneratorTemplateAnalyzer>;

namespace idee5.SoureGeneratorTests {
    /// <summary>
    /// The generator template analyzer tests.
    /// </summary>
    [TestClass]
    public class GeneratorTemplateAnalyzerTests {
        /// <summary>
        /// No source no diagnostic.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task NoSourceNoDiagnostic() {
            const string testSource = "";

            // No code, so no diagnostic will be triggered
            await VerifyCS.VerifyAnalyzerAsync(testSource);
        }

        /// <summary>
        /// Noconfig has diagnosic.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task NoConfigHasDiagnosic() {
            // Arrange
            const string testSource = @"using idee5.Common;
namespace idee5.SoureGeneratorTests {
    [GenerateCommands("""")]
    internal class TestClass {
        public int Prop1 { get; }
        public void ParamerlessMethod() { }
        protected void ProtectedMethod(string p1) { }
        private void PrivateMethod() { }
    }
}";
            var expected = VerifyCS.Diagnostic(DiagnosticDescriptors.TemplateNotConfigured.Id).WithArguments("TestClass").WithSpan(4, 20, 4, 29);
            // Act
            var analyzerTest = new VerifyCS.Test {
                TestState = {
                    Sources = {testSource},
                    ExpectedDiagnostics = {expected},
                    AdditionalReferences = { typeof(GenerateCommandsAttribute).Assembly},
                    AdditionalFiles = {
                        ("HandlerTemplate.txt", "// just do nothing")
                    }
                }
            };

            // Assert
            await analyzerTest.RunAsync();
        }

        /// <summary>
        /// Incorrect config has diagnosic.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task IncorrectConfigHasDiagnosic() {
            // Arrange
            const string testSource = @"using idee5.Common;
namespace idee5.SoureGeneratorTests {
    [GenerateCommands(""Handler"")]
    internal class TestClass {
        public int Prop1 { get; }
        public void ParamerlessMethod() { }
        protected void ProtectedMethod(string p1) { }
        private void PrivateMethod() { }
    }
}";
            var expected = VerifyCS.Diagnostic(DiagnosticDescriptors.TemplateNotFound.Id).WithArguments("Handler", "TestClass").WithSpan(4, 20, 4, 29);
            // Act
            var analyzerTest = new VerifyCS.Test {
                TestState = {
                    Sources = {testSource},
                    ExpectedDiagnostics = {expected},
                    AdditionalReferences = { typeof(GenerateCommandsAttribute).Assembly},
                    AdditionalFiles = {
                        ("HandlerTemplate.txt", "// just do nothing")
                    }
                }
            };

            // Assert
            await analyzerTest.RunAsync();
        }

        /// <summary>
        /// Correct template config has no diagnosic.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task CorrectTemplateConfigHasNoDiagnosic() {
            // Arrange
            const string sourceFileName = "../../../TestClass.cs";
            var testSource = File.ReadAllText(sourceFileName);

            // Act
            var analyzerTest = new VerifyCS.Test {
                TestState = {
                    Sources = {testSource},
                    AdditionalReferences = { typeof(GenerateCommandsAttribute).Assembly},
                    AdditionalFiles = {
                        ("HandlerTemplate.txt", "// just do nothing"),
                        ("NoDocProperty.txt","\t\tpublic {{PropertyType}} {{PropertyName}} { get; }")
                    }
                }
            };

            // Assert
            await analyzerTest.RunAsync();
        }
        /// <summary>
        /// Missing property template config has diagnosic.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task CorrectHandlerConfigHasNoDiagnosic() {
            // Arrange
            const string sourceFileName = "../../../TestClass.cs";
            var testSource = File.ReadAllText(sourceFileName);
            var expected = VerifyCS.Diagnostic(DiagnosticDescriptors.TemplateNotFound.Id).WithArguments("NoDocProperty.txt", "TestClass").WithSpan(6, 20, 6, 29);

            // Act
            var analyzerTest = new VerifyCS.Test {
                TestState = {
                    Sources = {testSource},
                    ExpectedDiagnostics ={expected},
                    AdditionalReferences = { typeof(GenerateCommandsAttribute).Assembly},
                    AdditionalFiles = {
                        ("HandlerTemplate.txt", "// just do nothing")
                    }
                }
            };

            // Assert
            await analyzerTest.RunAsync();
        }
    }
}