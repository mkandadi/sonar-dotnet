﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2018 SonarSource SA
 * mailto: contact AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using SonarAnalyzer.Common;
using SonarAnalyzer.Helpers.VisualBasic;

namespace SonarAnalyzer.Helpers
{
    public class VisualBasicInvocationTracker : InvocationTracker<SyntaxKind>
    {
        public VisualBasicInvocationTracker(IAnalyzerConfiguration analyzerConfiguration, DiagnosticDescriptor rule)
            : base(analyzerConfiguration, rule)
        {
        }

        protected override SyntaxKind[] TrackedSyntaxKinds { get; } =
            new[] { SyntaxKind.InvocationExpression };

        protected override GeneratedCodeRecognizer GeneratedCodeRecognizer { get; } =
            VisualBasic.GeneratedCodeRecognizer.Instance;

        public override InvocationCondition ArgumentAtIndexIsConstant(int index) =>
            (context) =>
            {
                var argumentList = ((InvocationExpressionSyntax)context.Invocation).ArgumentList;
                return argumentList != null &&
                    argumentList.Arguments.Count > index &&
                    argumentList.Arguments[index].GetExpression().IsConstant(context.SemanticModel);
            };

        public override InvocationCondition ArgumentAtIndexEquals(int index, string value) =>
            (context) =>
            {
                var argumentList = ((InvocationExpressionSyntax)context.Invocation).ArgumentList;
                if (argumentList == null ||
                    argumentList.Arguments.Count <= index)
                {
                    return false;
                }
                var constantValue = context.SemanticModel.GetConstantValue(argumentList.Arguments[index].GetExpression());
                return constantValue.HasValue &&
                    constantValue.Value is string constant &&
                    constant == value;
            };

        public override InvocationCondition IsTypeOfExpression() =>
            (context) => context.Invocation is InvocationExpressionSyntax invocation
                        && invocation.Expression is MemberAccessExpressionSyntax memberAccessSyntax
                        && memberAccessSyntax.Expression != null
                        && memberAccessSyntax.Expression.RawKind == (int)SyntaxKind.GetTypeExpression;

        protected override string GetMethodName(SyntaxNode invocationExpression) =>
            ((InvocationExpressionSyntax)invocationExpression).Expression.GetIdentifier()?.Identifier.ValueText;
    }
}
