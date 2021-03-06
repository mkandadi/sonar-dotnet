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

using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules
{
    public abstract class ExecutingSqlQueriesBase<TSyntaxKind> : SonarDiagnosticAnalyzer
        where TSyntaxKind : struct
    {
        protected const string DiagnosticId = "S2077";
        protected const string MessageFormat = "Make sure that executing SQL queries is safe here.";

        protected InvocationTracker<TSyntaxKind> InvocationTracker { get; set; }

        protected PropertyAccessTracker<TSyntaxKind> PropertyAccessTracker { get; set; }

        protected ObjectCreationTracker<TSyntaxKind> ObjectCreationTracker { get; set; }

        protected override void Initialize(SonarAnalysisContext context)
        {
            InvocationTracker.Track(context,
                InvocationTracker.MatchMethod(
                    new MemberDescriptor(KnownType.Microsoft_EntityFrameworkCore_RelationalQueryableExtensions, "FromSql")),
                Conditions.ExceptWhen(
                    OnlyParameterIsConstantOrInterpolatedString()),
                Conditions.ExceptWhen(
                    InvocationTracker.ArgumentAtIndexIsConstant(0)));

            InvocationTracker.Track(context,
                InvocationTracker.MatchMethod(
                    new MemberDescriptor(KnownType.Microsoft_EntityFrameworkCore_RelationalDatabaseFacadeExtensions, "ExecuteSqlCommandAsync"),
                    new MemberDescriptor(KnownType.Microsoft_EntityFrameworkCore_RelationalDatabaseFacadeExtensions, "ExecuteSqlCommand")),
                Conditions.ExceptWhen(
                    OnlyParameterIsConstantOrInterpolatedString()));

            PropertyAccessTracker.Track(context,
                PropertyAccessTracker.MatchProperty(
                    new MemberDescriptor(KnownType.System_Data_Odbc_OdbcCommand, "CommandText"),
                    new MemberDescriptor(KnownType.System_Data_OracleClient_OracleCommand, "CommandText"),
                    new MemberDescriptor(KnownType.System_Data_SqlClient_SqlCommand, "CommandText"),
                    new MemberDescriptor(KnownType.System_Data_SqlServerCe_SqlCeCommand, "CommandText")),
                PropertyAccessTracker.MatchSetter(),
                Conditions.ExceptWhen(
                    PropertyAccessTracker.AssignedValueIsConstant()));

            ObjectCreationTracker.Track(context,
                ObjectCreationTracker.MatchConstructor(
                    KnownType.Microsoft_EntityFrameworkCore_RawSqlString,
                    KnownType.System_Data_SqlClient_SqlCommand,
                    KnownType.System_Data_SqlClient_SqlDataAdapter,
                    KnownType.System_Data_Odbc_OdbcCommand,
                    KnownType.System_Data_Odbc_OdbcDataAdapter,
                    KnownType.System_Data_SqlServerCe_SqlCeCommand,
                    KnownType.System_Data_SqlServerCe_SqlCeDataAdapter,
                    KnownType.System_Data_OracleClient_OracleCommand,
                    KnownType.System_Data_OracleClient_OracleDataAdapter),
                ObjectCreationTracker.ArgumentAtIndexIs(0, KnownType.System_String),
                Conditions.ExceptWhen(
                    ObjectCreationTracker.ArgumentAtIndexIsConst(0)));
        }

        protected abstract InvocationCondition OnlyParameterIsConstantOrInterpolatedString();
    }
}
