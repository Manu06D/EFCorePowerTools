﻿using EFCorePowerTools.Shared.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReverseEngineer20.ReverseEngineer
{
    public class VisitorRelationScaffoldingModelFactory : RelationalScaffoldingModelFactory
    {
        private readonly List<TableInformationModel> _tables;
        private readonly DatabaseType _databaseType;

        public VisitorRelationScaffoldingModelFactory([NotNull] IOperationReporter reporter, [NotNull] ICandidateNamingService candidateNamingService, [NotNull] IPluralizer pluralizer, [NotNull] ICSharpUtilities cSharpUtilities, [NotNull] IScaffoldingTypeMapper scaffoldingTypeMapper, [NotNull] LoggingDefinitions loggingDefinitions, List<TableInformationModel> tables, DatabaseType databaseType) :
            base(reporter, candidateNamingService, pluralizer, cSharpUtilities, scaffoldingTypeMapper, loggingDefinitions)
        {
            _tables = tables;
            _databaseType = databaseType;
        }

        protected override EntityTypeBuilder VisitTable(ModelBuilder modelBuilder, DatabaseTable table)
        {
            string fullTableName;
            if (String.IsNullOrWhiteSpace(table.Schema))
            {
                fullTableName = table.Name;
            }
            else if (_databaseType == DatabaseType.SQLServer)
            {
                fullTableName = $"[{table.Schema}].[{table.Name}]";
            }
            else
            {
                fullTableName = $"{table.Schema}.{table.Name}";
            }
            var tableDefinition = _tables.FirstOrDefault(c => c.Name.Equals(fullTableName, StringComparison.OrdinalIgnoreCase));
            if (tableDefinition?.ExcludedColumns != null)
            {
                foreach (var column in tableDefinition?.ExcludedColumns)
                {
                    var columnToRemove = table.Columns.FirstOrDefault(c => c.Name.Equals(column, StringComparison.OrdinalIgnoreCase));
                    if (columnToRemove != null)
                        table.Columns.Remove(columnToRemove);
                }
            }

            return base.VisitTable(modelBuilder, table);
        }
    }
}
