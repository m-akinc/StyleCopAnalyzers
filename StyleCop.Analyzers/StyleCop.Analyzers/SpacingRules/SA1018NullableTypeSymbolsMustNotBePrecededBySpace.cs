﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A nullable type symbol within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a nullable type symbol is not correct.</para>
    ///
    /// <para>A nullable type symbol should never be preceded by whitespace.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1018NullableTypeSymbolsMustNotBePrecededBySpace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1018NullableTypeSymbolsMustNotBePrecededBySpace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1018";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1018Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1018MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1018Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1018.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleQuestionToken, SyntaxKind.NullableType);
        }

        private void HandleQuestionToken(SyntaxNodeAnalysisContext context)
        {
            var nullableType = (NullableTypeSyntax)context.Node;
            var questionToken = nullableType.QuestionToken;

            if (questionToken.IsMissing)
            {
                return;
            }

            /* Do not test for the first character on the line!
             * The StyleCop documentation is wrong there, the actual StyleCop code does not accept it.
             */

            SyntaxToken precedingToken = questionToken.GetPreviousToken();
            var triviaList = precedingToken.TrailingTrivia.AddRange(questionToken.LeadingTrivia);
            if (triviaList.Any(t => t.IsKind(SyntaxKind.WhitespaceTrivia) || t.IsKind(SyntaxKind.EndOfLineTrivia)))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, questionToken.GetLocation()));
            }
        }
    }
}
