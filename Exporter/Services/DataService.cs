using System.Collections.Generic;
using System.Data;
using System.Linq;
using Exporter.Models;


namespace Exporter.Services
{

    public interface IDataService : IService
    {
        void GetBatch(int? batchId);
        IEnumerable<IEnumerable<string>> GetData();
    }

    public class DataService : Service, IDataService
    {
        
        private readonly IConnectionService connectionService;
        private readonly IMappingService mappingService;
        private readonly IMetaObjectService metaObjectService;
        private readonly Sql[] sqlConfigs;

        internal DataService(IMetaObjectService metaDataService, IConnectionService connectionService,
            IMappingService mappingService)
        {
            this.connectionService = connectionService;
            this.mappingService = mappingService;
            this.metaObjectService = metaDataService;
            sqlConfigs = metaDataService.Process.Sql;
        }

        public void GetBatch(int? batchId)
        {
            var sqlConfig = sqlConfigs.First(c => c.CommandName == "Batch");
            var sqlParams = new Dictionary<string, object>(sqlConfig.SqlParams);
            if (batchId.HasValue) sqlParams.Add("BatchId", batchId);

            using (var conn = connectionService.GetConnection(sqlConfig.ConnectionName))
            {
                using(var cmd = connectionService.GetCommand(sqlConfig.CommandText, conn, sqlParams))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();

                        metaObjectService.Batch = new Batch
                        {
                            BatchId = rdr.GetInt32("Id"),
                            From = rdr.GetDateTime("FromDate"),
                            To = rdr.GetDateTime("ToDate")
                        };
                    }
                }
            }
        }

        // TODO: add optional params to be converted into datatable or just accept a datatable
        public IEnumerable<IEnumerable<string>> GetData()
        {
            var sqlConfig = sqlConfigs.First(c => c.CommandName == "Data");

            var sqlParams = (sqlConfig.SqlParams == null) ? new Dictionary<string, object>() :
                new Dictionary<string, object>(sqlConfig.SqlParams);

            using (var conn = connectionService.GetConnection(sqlConfig.ConnectionName))
            {
                using (var cmd = connectionService.GetCommand(sqlConfig.CommandText, conn, sqlParams))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        do
                        {
                            yield return mappingService.GetDocumentData(rdr);
                        } while (rdr.NextResult());
                    }
                }
            }
        }       

    }

}
