using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommentAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CommentAnalyzer";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.CommentAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.CommentAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.CommentAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Documentation";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            var commentNodes = from node in root.DescendantTrivia() where node.IsKind(SyntaxKind.MultiLineCommentTrivia) || node.IsKind(SyntaxKind.SingleLineCommentTrivia) select node;

            if (!commentNodes.Any())
            {
                return;
            }
            foreach (var node in commentNodes)
            {
                string commentText = "";
                switch (node.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                        commentText = node.ToString().TrimStart('/');
                        break;
                    case SyntaxKind.MultiLineCommentTrivia:
                        var nodeText = node.ToString();

                        commentText = nodeText.Substring(2, nodeText.Length - 4);
                        break;
                }

                string regex = Resources.CommentRegex;
                if (Regex.Match(commentText, regex, RegexOptions.IgnoreCase).Success)
                {
                    var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }

        }
    }
}