using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace idee5.SourceGenerators;
/// <summary>
/// Generate commands and handlers for all public methods of marked classes.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class CommandGenerator : IIncrementalGenerator {
    #region Private Fields
    private static string commandDefaultTemplate = "";
    private static string handlerDefaultTemplate = "";
    private static string propertyDefaultTemplate = "";

    #endregion Private Fields

    #region Public Methods

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // initialize the templates
        propertyDefaultTemplate = GetEmbeddedResource("idee5.SourceGenerators.templates.PropertyTemplate.txt");
        commandDefaultTemplate = GetEmbeddedResource("idee5.SourceGenerators.templates.CommandTemplate.txt");
        handlerDefaultTemplate = GetEmbeddedResource("idee5.SourceGenerators.templates.HandlerTemplate.txt");

        // read the additional texts and save their name
        IncrementalValuesProvider<(string Name, string Content)> namesAndContents = context.AdditionalTextsProvider
            .Select((text, cancellationToken) => (Path.GetFileName(text.Path), text.GetText(cancellationToken)!.ToString()));

        IncrementalValuesProvider<ClassInfo> list = context.SyntaxProvider.ForAttributeWithMetadataName(CommandGeneratorHelpers.AttributeMetaName,
            static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
            GetClassInfoOrNull);
        IncrementalValuesProvider<ClassInfo> candidates = list.Where(static ci => !String.IsNullOrWhiteSpace(ci.HandlerTemplate) && !String.IsNullOrWhiteSpace(ci.Namespace));

        context.RegisterSourceOutput(candidates.Combine(namesAndContents.Collect()), static (spc, source) => {
            spc.CancellationToken.ThrowIfCancellationRequested();
            ClassInfo item = source.Left;
            if (item != null) {
                string commandTemplate = commandDefaultTemplate;
                string propertyTemplate = propertyDefaultTemplate;
                // get the command source template
                (string Name, string Content) fileEntry = source.Right.FirstOrDefault(kvp => kvp.Name == item.CommandTemplate);
                if (fileEntry != default) commandTemplate = fileEntry.Content;
                // get the property source template
                fileEntry = source.Right.FirstOrDefault(kvp => kvp.Name == item.PropertyTemplate);
                if (fileEntry != default) propertyTemplate = fileEntry.Content;
                // create a list of all public methods without properties
                foreach (MethodInfo method in item.Methods) {
                    if (method != null) {
                        string code = GenerateCommandCode(method, commandTemplate, propertyTemplate, item.Namespace);
                        spc.AddSource($"{method.Name}Command.g.cs", code);
                    }
                }
            }
        });
        context.RegisterSourceOutput(candidates.Combine(namesAndContents.Collect()), static (spc, source) => {
            spc.CancellationToken.ThrowIfCancellationRequested();
            ClassInfo item = source.Left;
            if (item != null) {
                string commandHandlerTemplate = handlerDefaultTemplate;
                // get the handler source template
                (string Name, string Content) fileEntry = source.Right.FirstOrDefault(kvp => kvp.Name == item.HandlerTemplate);
                // warn about the missing template
                if (fileEntry != default) commandHandlerTemplate = fileEntry.Content;
                else
                    spc.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TemplateNotFound, item.Locations.FirstOrDefault(), item.HandlerTemplate, item.Name));

                // create a list of all public methods without properties
                foreach (MethodInfo method in item.Methods) {
                    if (method != null) {
                        string code = GenerateHandlerCode(method, commandHandlerTemplate, item.Namespace);
                        spc.AddSource($"{method.Name}CommandHandler.g.cs", code);
                    }
                }
            }
        });
    }

    #endregion Public Methods

    #region Private Methods

    private static string GenerateCommandCode(MethodInfo method, string commandTemplate, string propertyTemplate, string? ns) {
        string properties = "";
        string pList = "", pDoc = "";
        string constructorBody = "";

        for (int i = 0; i < method.Arguments.Length; i++) {
            ArgumentInfo parameter = method.Arguments[i];
            // convert to PascalCase
            string propName = parameter.Name.Substring(0, 1).ToUpper() + parameter.Name.Substring(1);
            // add the property
            properties += propertyTemplate
                .Replace("{{PropertyDescription}}", parameter.Description)
                .Replace("{{PropertyType}}", parameter.Type)
                .Replace("{{PropertyName}}", propName);
            // add the constructor and its documentation
            pList += $", {parameter.Type} {parameter.Name}";
            if (parameter.Description != null) pDoc += $"\t\t/// <param name=\"{parameter.Name}\">{parameter.Description}</param>" + Environment.NewLine;
            // add the property initialization
            constructorBody += $"\t\t\t{propName} = {parameter.Name};" + Environment.NewLine;
        }

        return commandTemplate.Replace("{{Namespace}}", ns)
            .Replace("{{MethodName}}", method.Name)
            .Replace("{{PropertyList}}", properties)
            // insert list of parameters and remove the first separator if there are no standard parameters in the template
            .Replace("{{ParamList}}", pList).Replace("(, ", "(")
            .Replace("{{ParamDoc}}", pDoc)
            .Replace("{{ConstructorBody}}", constructorBody);
    }

    private static string GenerateHandlerCode(MethodInfo method, string handlerTemplate, string? ns) {
        string pList = "";
        if (method.Arguments.Length > 0) {
            pList = $"command.{method.Arguments[0].Name.Substring(0, 1).ToUpper() + method.Arguments[0].Name.Substring(1)}";
            for (int i = 1; i < method.Arguments.Length; i++) {
                ArgumentInfo parameter = method.Arguments[i];
                if (parameter != null) {
                    // convert to PascalCase
                    string propName = parameter.Name.Substring(0, 1).ToUpper() + parameter.Name.Substring(1);
                    pList += $", command.{propName}";
                }
            }
        }
        return handlerTemplate.Replace("{{Namespace}}", ns)
            .Replace("{{MethodName}}", method.Name)
            .Replace("{{MethodParameters}}", pList);
    }

    private static ClassInfo GetClassInfoOrNull(GeneratorAttributeSyntaxContext context, CancellationToken token) {
        // we know it is a class declaration
        INamedTypeSymbol symbol = (INamedTypeSymbol)context.TargetSymbol;
        string? ns = (symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToString());
        AttributeData atr = context.Attributes.First(a => a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).EndsWith(CommandGeneratorHelpers.AttributeMetaName) ?? false);
        // something is wrong with the attribute, create a dummy record and skip
        if (atr == null) return new ClassInfo(symbol.Name, ImmutableArray<MethodInfo>.Empty, ImmutableArray<Location>.Empty, "", null, null, ns, null);

        string t1 = (string)(atr.ConstructorArguments.FirstOrDefault().Value ?? "");
        string? t2 = (string?)atr.NamedArguments.FirstOrDefault(na => na.Key == CommandGeneratorHelpers.CmdTemplateParameterName).Value.Value;
        string? t3 = (string?)atr.NamedArguments.FirstOrDefault(na => na.Key == CommandGeneratorHelpers.PropTemplateParameterName).Value.Value;
        token.ThrowIfCancellationRequested();
        // Extract the needed method data
        IEnumerable<MethodInfo> methods = symbol.GetMembers().OfType<IMethodSymbol>()
            .Where(static m => m.Kind == SymbolKind.Method && m.MethodKind == MethodKind.Ordinary && m.DeclaredAccessibility == Accessibility.Public)
            .Select(static m => {
                ArgumentInfo[] args = new ArgumentInfo[m.Parameters.Length];
                string? methodXML = m.GetDocumentationCommentXml();
                XElement[] pElements = Array.Empty<XElement>();
                if (!String.IsNullOrWhiteSpace(methodXML)) pElements = XElement.Parse(methodXML).Elements("param").ToArray();

                for (int i = 0; i < m.Parameters.Length; i++) {
                    XElement? pxml = pElements.SingleOrDefault(pe => pe.Attribute("name").Value == m.Parameters[i].Name);
                    args[i] = new ArgumentInfo(m.Parameters[i].Name, m.Parameters[i].Type.ToString(), pxml == null ? null : String.Join("", pxml.Nodes()));
                }
                return new MethodInfo(m.Name, args);
            });

        return new ClassInfo(symbol.Name, methods.ToImmutableArray(), symbol.Locations, t1, t2, t3, ns, symbol.MetadataName);
    }
    private static string GetEmbeddedResource(string path) {
        using Stream stream = System.Reflection.Assembly.GetAssembly(typeof(CommandGenerator)).GetManifestResourceStream(path);
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    #endregion Private Methods
}