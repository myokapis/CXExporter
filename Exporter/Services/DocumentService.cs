using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Exporter.Extensions;

namespace Exporter.Services
{

    public interface IDocumentService : IService
    {
        
        void ArchiveFiles();
        void ClearDirectories();
        void CopyFiles();
        void WriteFiles(IEnumerable<IEnumerable<string>> recordsets);
    }

    internal class DocumentService : Service, IDocumentService
    {
        private readonly IMetaObjectService metaObjectService;

        internal DocumentService(IMetaObjectService metaObjectService)
        {
            this.metaObjectService = metaObjectService;
        }

        public void ArchiveFiles()
        {
            foreach (var file in metaObjectService.WorkingDirectory.GetFiles())
            {
                file.MoveSafe(metaObjectService.ArchiveDirectory);
            }
        }

        public void ClearDirectories()
        {
            foreach(var file in metaObjectService.WorkingDirectory.GetFiles())
            {
                file.Delete();
            }
        }

        public void CopyFiles()
        {
            // bail out if the directory is not found
            // TODO: log when the directory is not found
            if ((metaObjectService.OutputDirectory == null) || !metaObjectService.OutputDirectory.Exists)
                return;

            foreach (var file in metaObjectService.WorkingDirectory.GetFiles())
            {
                file.CopySafe(metaObjectService.OutputDirectory);
            }
        }

        

        public void WriteFiles(IEnumerable<IEnumerable<string>> documentDataSets)
        {
            var i = 0;

            foreach(var documentDataSet in documentDataSets)
            {
                var document = metaObjectService.Process.Documents[i];

                using (var writer = metaObjectService.GetStreamWriter(document))
                {
                    foreach (var dataRecord in documentDataSet)
                    {
                        writer.WriteLine(dataRecord);
                    }
                }

                i++;
            }
        }

    }

}
