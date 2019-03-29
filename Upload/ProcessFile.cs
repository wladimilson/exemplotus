using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using tusdotnet.Interfaces;

namespace ExemploTus.Upload
{
    public class ProcessFile
    {
        public static async Task SaveFileAsync(ITusFile file, IHostingEnvironment env)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            var metadata = await file.GetMetadataAsync(token);
            var fileName = Path.GetRandomFileName();
            if(metadata.ContainsKey("filename"))
                fileName = metadata["filename"].GetString(Encoding.UTF8);

            var filePath = Path.Combine(env.ContentRootPath, "arquivos", fileName);

            CreateDirectory(Path.Combine(env.ContentRootPath, "arquivos"));
            
            using(var stream = await file.GetContentAsync(token))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        private static void CreateDirectory(string path)
        {
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}