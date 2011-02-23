// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace warmup
{
    using System;
    using System.IO;

    /// <summary>
    /// Template exporter for local filesystem
    /// </summary>
    internal class FolderExporter : IExporter
    {
        /// <summary>
        /// Exports the template to specified location
        /// </summary>
        /// <param name="sourceControlWarmupLocation">The source control warmup location.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="targetDir">The target dir.</param>
        public void Export(string sourceControlWarmupLocation, string templateName, TargetDir targetDir)
        {
            string baseDir = Path.Combine(sourceControlWarmupLocation, templateName);
            Console.WriteLine("Copying to: {0}", targetDir.FullPath);
            CopyDirectory(baseDir, targetDir.FullPath);
        }

        /// <summary>
        /// Copies the directory.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        private void CopyDirectory(string source, string destination)
        {
            if (destination[destination.Length - 1] != Path.DirectorySeparatorChar)
                destination += Path.DirectorySeparatorChar;
            if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
            string[] files = Directory.GetFileSystemEntries(source);
            foreach (var element in files)
            {
                // Sub directories

                if (Directory.Exists(element))
                    CopyDirectory(element, destination + Path.GetFileName(element));
                    // Files in directory

                else
                    File.Copy(element, destination + Path.GetFileName(element), true);
            }
        }
    }
}