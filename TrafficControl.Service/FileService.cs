using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficControl.Persistence;

namespace TrafficControl.Service
{
    public class FileService : IFileService
    {
        private readonly TrafficControlContext dbContext;
        private readonly string basePath;

        public FileService(TrafficControlContext dbContext, string basePath)
        {
            this.dbContext = dbContext;
            this.basePath = basePath;
        }

        public async Task<Guid> Store(string fileName, Stream file)
        {
            var fileId = Guid.NewGuid();
            Directory.CreateDirectory(Path.Combine(basePath, fileId.ToString()));
            var filePath = Path.Combine(fileId.ToString(), fileName);

            using var fileStream = System.IO.File.Create(Path.Combine(basePath, filePath));
            await file.CopyToAsync(fileStream);

            dbContext.Add(new Persistence.File
            {
                Id = fileId,
                FilePath = filePath
            });

            await dbContext.SaveChangesAsync();

            return fileId;
        }

        public async Task<string> GetPath(Guid id)
        {
            var file = await dbContext.Files.SingleAsync(f => f.Id == id);

            return Path.Combine(basePath, file.FilePath);
        }
    }
}
