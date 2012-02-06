using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Ionic.Zip;

namespace warmup
{
    public class GitHub : IExporter
    {
        public void Export(string sourceControlWarmupLocation, string templateName, TargetDir targetDir)
        {
            var client = new WebClient();

            if (!templateName.EndsWith("/")) templateName += "/";

            var zipUrl = templateName + "zipball/master";

            var tempFile = Path.GetTempFileName();

            var tempDirectory = Guid.NewGuid().ToString();

            var tempPath = Path.Combine(Path.GetTempPath(), tempDirectory);

            client.DownloadFile(zipUrl, tempFile);

            ZipFile zipFile = new ZipFile(tempFile);

            zipFile.ExtractAll(tempPath, ExtractExistingFileAction.OverwriteSilently);

            var sourceDirectory = Directory.GetDirectories(tempPath).First();

            Folder.CopyDirectory(Path.Combine(tempPath, sourceDirectory), targetDir.FullPath);
        }
    }
}
