using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace warmup
{
    /// <summary>
    /// GIT template extractor
    /// </summary>
    public class GitTemplateExtractor
    {
        private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;
        private readonly TargetDir _target;
        private readonly string _templateName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitTemplateExtractor"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="templateName">Name of the template.</param>
        public GitTemplateExtractor(TargetDir target, string templateName)
        {
            _target = target;
            _templateName = templateName;
        }

        /// <summary>
        /// Extracts the template.
        /// </summary>
        public void Extract()
        {
            var topParent = new DirectoryInfo(_target.FullPath);
            var directories = topParent.GetDirectories();
            var files = topParent.GetFiles();

            if (TemplateNotFound(directories, files)) return;
            
            CleanTopParent(directories, files);

            var templateDir = directories.FirstOrDefault(d => d.Name.Equals(_templateName, Comparison));
            if (templateDir != null) MoveTemplateContent(templateDir, topParent);
        }

        /// <summary>
        /// Cleans the top parent.
        /// </summary>
        /// <param name="directories">The directories.</param>
        /// <param name="files">The files.</param>
        private void CleanTopParent(IEnumerable<DirectoryInfo> directories, IEnumerable<FileInfo> files)
        {
            foreach (var directory in directories.Where(directory => 
                directory.Name != _templateName))
                DeleteDirectory(directory);
            foreach (var file in files.Where(file => 
                !file.Name.Equals(_templateName + file.Extension, Comparison)))
                SafeDeleteFile(file);
        }

        /// <summary>
        /// Checks if the template is found.
        /// </summary>
        /// <param name="directories">The directories.</param>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        private bool TemplateNotFound(IEnumerable<DirectoryInfo> directories, IEnumerable<FileInfo> files)
        {
            return !directories.Any(di => di.Name.Equals(_templateName, Comparison)) &&
                   !files.Any(f => f.Name.Equals(_templateName + f.Extension, Comparison));
        }

        /// <summary>
        /// Safe delete file.
        /// </summary>
        /// <param name="file">The file.</param>
        private static void SafeDeleteFile(FileInfo file)
        {
            file.Attributes = FileAttributes.Normal;
            file.Delete();
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        private static void DeleteDirectory(DirectoryInfo directory)
        {
            foreach (var dir in directory.GetDirectories())
                DeleteDirectory(dir);
            foreach (var file in directory.GetFiles())
                SafeDeleteFile(file);
            directory.Attributes = FileAttributes.Normal;
            directory.Delete();
        }

        /// <summary>
        /// Moves the content of the template.
        /// </summary>
        /// <param name="templateDir">The template dir.</param>
        /// <param name="destinationDir">The destination dir.</param>
        private static void MoveTemplateContent(DirectoryInfo templateDir, DirectoryInfo destinationDir)
        {
            foreach (var dir in templateDir.GetDirectories())
                dir.MoveTo(Path.Combine(destinationDir.FullName, dir.Name));
            foreach (var file in templateDir.GetFiles())
                file.MoveTo(Path.Combine(destinationDir.FullName, file.Name));
            templateDir.Delete();
        }
    }
}