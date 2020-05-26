using System.IO;
using Exporter.Models;

namespace Exporter.Services
{

    public static class ServicesFactory
    {

        public static IServices GetServices(DirectoryInfo exportRoot, OrgUnit orgUnit, Process process)
        {
            var metaObjectService = new MetaObjectService(exportRoot, orgUnit, process);
            var mappingService = new MappingService(metaObjectService);
            var connectionService = new SqlConnectionService(metaObjectService);
            var services = new Services();

            return services.Register<ICompressionService>(new CompressionService())
                .Register<IDataService>(new DataService(metaObjectService, connectionService, mappingService))
                .Register<IEmailService>(new EmailService())
                .Register<IEncryptionService>(new EncryptionService())
                .Register<IDocumentService>(new DocumentService(metaObjectService))
                .Register<ILoggingService>(new LoggingService())
                .Register<IMappingService>(mappingService)
                .Register<IMetaObjectService>(metaObjectService)
                .Register<ISftpService>(new SftpService()) as IServices;
        }

    }

}
