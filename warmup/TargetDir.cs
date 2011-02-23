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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using settings;

    /// <summary>
    /// Template processor
    /// </summary>
    [DebuggerDisplay("{FullPath}")]
    public class TargetDir
    {
        readonly string _path;
        readonly string _replacementToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetDir"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public TargetDir(string path)
        {
            _path = path;
            _replacementToken = WarmupConfiguration.settings.ReplacementToken;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        public string FullPath
        {
            get { return Path.GetFullPath(_path); }
        }

        /// <summary>
        /// Replaces the tokens.
        /// </summary>
        /// <param name="name">The name.</param>
        public void ReplaceTokens(string name)
        {
            Console.WriteLine("Replacing tokens...");

            var startingPoint = new DirectoryInfo(FullPath);

            //move all directories
            MoveAllDirectories(startingPoint, name);

            startingPoint = new DirectoryInfo(startingPoint.FullName.Replace(_replacementToken, name));

            //move all files
            MoveAllFiles(startingPoint, name);

            //replace file content
            ReplaceTokensInTheFiles(startingPoint, name);
        }

        /// <summary>
        /// Moves the template to destination.
        /// </summary>
        /// <param name="target">The target.</param>
        public void MoveToDestination(string target)
        {
            if (string.IsNullOrEmpty(target)) return;
            if (!Directory.Exists(target)) return;
            if (target == FullPath) return;

            DirectoryInfo folder = new DirectoryInfo(FullPath);
            var destination = Path.Combine(target, folder.Name);
            Console.WriteLine(string.Format("move {0} to {1}", FullPath, destination));
            folder.MoveTo(destination);
        }

        /// <summary>
        /// Replaces the tokens in the files.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="name">The name.</param>
        private void ReplaceTokensInTheFiles(DirectoryInfo point, string name)
        {
            List<string> ignoredExtensions = GetIgnoredExtensions();
            foreach (var info in point.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (info.IsReadOnly || (info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }

                if ((from ext in ignoredExtensions 
                        where ext.Equals(info.Extension, StringComparison.InvariantCultureIgnoreCase) 
                        select ext).FirstOrDefault()!=null)
                {
                    continue;
                }

                //skip the .git directory
                if (new[] { "\\.git\\" }.Contains(info.FullName))
                {
                    continue;
                }

                //process contents
                string contents = File.ReadAllText(info.FullName);
                contents = contents.Replace(_replacementToken, name);
                File.WriteAllText(info.FullName, contents);
            }
        }

        /// <summary>
        /// Gets the ignored extensions.
        /// </summary>
        /// <returns></returns>
        private List<string> GetIgnoredExtensions()
        {
            var extension = new List<string>();
            foreach (IgnoredFileType ignoredFileType in WarmupConfiguration.settings.IgnoredFileTypeCollection)
            {
                extension.Add(string.Format(".{0}", ignoredFileType.Extension));
            }
            return extension;
        }

        /// <summary>
        /// Moves all files.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="name">The name.</param>
        private void MoveAllFiles(DirectoryInfo point, string name)
        {
            foreach (var file in point.GetFiles("*.*", SearchOption.AllDirectories))
            {
                string moveTo = file.FullName.Replace(_replacementToken, name);
                try
                {
                    file.MoveTo(moveTo);
                }
                catch (Exception)
                {
                    Console.WriteLine("Trying to move '{0}' to '{1}'", file.FullName, moveTo);
                    throw;
                }
            }
        }

        /// <summary>
        /// Moves all directories.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="name">The name.</param>
        private void MoveAllDirectories(DirectoryInfo dir, string name)
        {
            DirectoryInfo workingDirectory = dir;
            if (workingDirectory.Name.Contains(_replacementToken))
            {
                string newFolderName = dir.Name.Replace(_replacementToken, name);
                string moveTo = Path.Combine(dir.Parent.FullName, newFolderName);

                try
                {
                    workingDirectory.MoveTo(moveTo);
                    workingDirectory = new DirectoryInfo(moveTo);
                }
                catch (Exception)
                {
                    Console.WriteLine("Trying to move '{0}' to '{1}'", workingDirectory.FullName, moveTo);
                    throw;
                }
            }

            foreach (var info in workingDirectory.GetDirectories())
            {
                MoveAllDirectories(info, name);
            }
        }
    }
}