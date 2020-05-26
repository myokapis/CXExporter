using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Exporter.Models;

namespace Exporter.Services
{

    public interface IExportService
    {
        void Run(int? batchId);
    }

    public class ExportService : IExportService
    {
        private readonly IServices services;
        private readonly ICompressionService compressionService;
        private readonly IDataService dataService;
        private readonly IDocumentService documentService;
        private readonly IEmailService emailService;
        private readonly IEncryptionService encryptionService;
        private readonly ISftpService sftpService;

        public ExportService(in IServices services)
        {
            this.services = services;
            compressionService = services.Get<ICompressionService>();
            dataService = services.Get<IDataService>();
            documentService = services.Get<IDocumentService>();
            emailService = services.Get<IEmailService>();
            encryptionService = services.Get<IEncryptionService>();
            sftpService = services.Get<ISftpService>();
        }

        public virtual void Run(int? batchId)
        {
            dataService.GetBatch(batchId);

            documentService.ClearDirectories();

            var documentDataSets = dataService.GetData();
            documentService.WriteFiles(documentDataSets);

            compressionService.CompressFiles();
            encryptionService.EncryptFiles();

            documentService.CopyFiles();
            sftpService.SendFiles();

            documentService.ArchiveFiles();

            emailService.SendNotifications();
        }

    }

}
