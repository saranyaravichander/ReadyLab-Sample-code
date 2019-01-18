

// <copyright file="UsingAnalyzerAnalyzer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UsingAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UsingAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UsingAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.UsingAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.UsingAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.UsingAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usability";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
            context.RegisterSemanticModelAction(this.HandleSemanticModel);
        }

        public void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var MethodList = Resources.DisallowedUsing.Split(',');
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            var usingNodes = from node in root.DescendantNodes() where node.IsKind(SyntaxKind.UsingDirective) select node;

            if (!usingNodes.Any())
            {
                return;
            }
            foreach (var node in usingNodes)
            {
                var name = node.ToString();

                foreach (var item in MethodList)
                {
                    if (name.Contains(item))
                    {
                        var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }         
        }

        public void HandleSemanticModel(SemanticModelAnalysisContext context)
        {
            var MethodList = Resources.DisallowedUsing.Split(',');
            var invocationExpressions = context.SemanticModel.SyntaxTree.GetRoot().DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.InvocationExpression));
            
            var model = context.SemanticModel;

            foreach (var item in invocationExpressions)
            {
                var methodNode = (InvocationExpressionSyntax)item;
                var memberAccessExpr = methodNode.Expression as MemberAccessExpressionSyntax;
                if (memberAccessExpr != null)
                {
                    var location = memberAccessExpr.GetLocation();
                    var path = System.IO.Path.GetFileName(location.GetLineSpan().Path);
                    var line = location.GetLineSpan().StartLinePosition.Line.ToString();

                    var symbolOfSyntaxNode = model.GetSymbolInfo(memberAccessExpr).Symbol as IMethodSymbol;
                    if (symbolOfSyntaxNode != null)
                    {
                        foreach (var method in MethodList)
                        {
                            if (symbolOfSyntaxNode.ContainingNamespace.ToString().Contains(method))
                            {
                                var diagnostic = Diagnostic.Create(Rule, methodNode.GetLocation());
                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }
    }
}