using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Exporter.Models;

namespace Exporter.Services
{

    public interface IConnectionService : IService
    {
        DbCommand GetCommand(in string commandText, in DbConnection conn, in Dictionary<string, object> parameters = null);
        DbConnection GetConnection(string name = "Default");
    }

    public class SqlConnectionService : Service, IConnectionService
    {
        private Dictionary<string, Lazy<string>> connStrings;
        private readonly IMetaObjectService metaObjectService;

        internal SqlConnectionService(IMetaObjectService metaObjectService)
        {
            this.metaObjectService = metaObjectService;

            connStrings = metaObjectService.OrgUnit.DataConnections
                .ToDictionary(d => d.Name, d => new Lazy<string>(GetConnectionString(d)));
        }

        public DbCommand GetCommand(in string commandText, in DbConnection conn, in Dictionary<string, object> parameters = null)
        {
            var cmd = new SqlCommand(commandText, conn as SqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();

            if (parameters == null) return cmd;

            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue($"@{parameter.Key}", parameter.Value);
            }

            return cmd;
        }

        public DbConnection GetConnection(string name = "Default")
        {
            return new SqlConnection(connStrings[name].Value);
        }

        private string GetConnectionString(DataConnection dataConnection)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = dataConnection.SqlInstance;
            builder.InitialCatalog = dataConnection.Database;
            builder.IntegratedSecurity = true;
            builder.PersistSecurityInfo = true;
            builder.ConnectTimeout = dataConnection.ConnectTimeout;
            return builder.ToString();
        }

    }

}
