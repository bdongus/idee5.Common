using idee5.SourceGenerators;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace idee5.SoureGeneratorTests;
/// <summary>
/// The command generator tests.
/// </summary>
[TestClass]
public class CommandGeneratorTests : VerifyBase {
    /// <summary>
    /// Can generate sources.
    /// </summary>
    /// <returns>A Task.</returns>
    [TestMethod]
    public Task CanGenerateSources() {
        // Arrange
        const string sourceFileName = "../../../TestClass.cs";
        var source = File.ReadAllText(sourceFileName);
        const string templatePath = "../../../HandlerTemplate.txt";
        var template = File.ReadAllText(templatePath);
        const string propTemplatePath = "../../../NoDocProperty.txt";
        var propTemplate = File.ReadAllText(propTemplatePath);
        var texts = ImmutableArray.Create<AdditionalText>(new InMemoryAdditionalText(templatePath, template),
            new InMemoryAdditionalText(propTemplatePath, propTemplate)
            );
        var generator = new CommandGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator).AddAdditionalTexts(texts);

        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(assembly => !assembly.IsDynamic)
                            .Select(assembly => MetadataReference
                                                .CreateFromFile(assembly.Location))
                            .Cast<MetadataReference>();
        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references
            );

        // Act
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        return Verify(driver);
    }

    /// <summary>
    /// Warnings the on missing template.
    /// </summary>
    [TestMethod]
    public void WarningOnMissingTemplate() {
        // Arrange
        const string sourceFileName = "../../../TestClass.cs";
        var source = File.ReadAllText(sourceFileName);
        var generator = new CommandGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // collect the referenced assemblies
        var references = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(assembly => !assembly.IsDynamic)
                            .Select(assembly => MetadataReference
                                                .CreateFromFile(assembly.Location))
                            .Cast<MetadataReference>();
        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references
            );

        // Act
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        Assert.AreEqual(diagnostics.Length, 1);
        Assert.AreEqual(diagnostics.First().Id, "I50002");
    }
}