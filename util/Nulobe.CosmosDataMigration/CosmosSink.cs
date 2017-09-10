﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulobe.CosmosDataMigration
{
    public class CosmosSink : IDataSinkSource, IDataSinkTarget
    {
        private readonly CosmosSinkOptions _options;

        public CosmosSink(CosmosSinkOptions options)
        {
            _options = options;
        }

        public string SinkName => "DocumentDB";

        public DataSinkArgumentCollection GetSourceSinkArguments(string databaseName, string collectionName) => GetSinkArguments(databaseName, collectionName);

        public DataSinkArgumentCollection GetTargetSinkArguments(string databaseName, string collectionName) => GetSinkArguments(databaseName, collectionName);

        private DataSinkArgumentCollection GetSinkArguments(string databaseName, string collectionName) =>
            new DataSinkArgumentCollection(new Dictionary<string, string>()
            {
                { "ConnectionString", string.Join(";", new Dictionary<string, string>()
                    {
                        { "AccountEndpoint", _options.ServiceEndpoint.ToString() },
                        { "AccountKey", _options.AuthorizationKey },
                        { "Database", databaseName }
                    }
                    .Select(kvp => $"{kvp.Key}={kvp.Value}")) },
                { "Collection", collectionName }
            });
    }
}