using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace idee5.SourceGenerators;
/// <summary>
/// Analyzer checking the existence of a command handler template
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class GeneratorTemplateAnalyzer : DiagnosticAnalyzer {
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticDescriptors.TemplateNotConfigured, DiagnosticDescriptors.TemplateNotFound);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private void AnalyzeNamedType(SymbolAnalysisContext context) {
        var symbol = (INamedTypeSymbol)context.Symbol;
        ImmutableArray<AttributeData> attr = symbol.GetAttributes();
        if (attr != default) {
            foreach (AttributeData item in attr) {
                if (item.AttributeClass?.Name.EndsWith("GenerateCommandsAttribute") ?? false) {
                    string? arg1 = item.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (String.IsNullOrWhiteSpace(arg1)) context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TemplateNotConfigured, context.Symbol.Locations[0], symbol.Name));
                    else if (!context.Options.AdditionalFiles.Any(f => Path.GetFileName(f.Path) == arg1))
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TemplateNotFound, context.Symbol.Locations[0], arg1, symbol.Name));

                    // check for the optional templates, if they are defined
                    if (!item.NamedArguments.IsDefaultOrEmpty) {
                        KeyValuePair<string, TypedConstant> cmdArg = item.NamedArguments.FirstOrDefault(na => na.Key == CommandGeneratorHelpers.CmdTemplateParameterName);
                        if (cmdArg.Key != default && !context.Options.AdditionalFiles.Any(f => Path.GetFileName(f.Path) == cmdArg.Value.Value?.ToString()))
                            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TemplateNotFound, context.Symbol.Locations[0], cmdArg.Value.Value, symbol.Name));
                        KeyValuePair<string, TypedConstant> propArg = item.NamedArguments.FirstOrDefault(na => na.Key == CommandGeneratorHelpers.PropTemplateParameterName);
                        if (propArg.Key != default && !context.Options.AdditionalFiles.Any(f => Path.GetFileName(f.Path) == propArg.Value.Value?.ToString()))
                            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TemplateNotFound, context.Symbol.Locations[0], propArg.Value.Value, symbol.Name));
                    }
                }
            }
        }
    }
}
