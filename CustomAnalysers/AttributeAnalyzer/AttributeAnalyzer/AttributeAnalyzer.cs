// <copyright file="AttributeAnalyzer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace AttributeAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AttributeAnalyzer";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Attributes";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        public static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var x = (MethodDeclarationSyntax)context.Node;
            var y = x.AttributeLists;
            var isValid = false;
            foreach (var att1 in y)
            {
                foreach (var att2 in att1.Attributes)
                {
                    if (att2.Name.ToString() == Resources.NecessaryAttribute || att2.Name.ToString() == Resources.AlternateAttribute)
                    {
                        isValid = true;
                    }
                }
            }

            if (!isValid)
            {
                var location = x.Identifier.GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(Rule, location));
            }
        }
    }
}
