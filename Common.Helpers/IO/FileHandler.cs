using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Common.Helpers.IO
{
    public static class FileHandler
    {
        private const int BufferSize = 8192;
        private static readonly ILogger _logger = LogProvider.GetLogger(nameof(FileHandler));

        public static IEnumerable<string> EnumerateFilesFromFolder(
            string uncPath, string extension = "*", bool searchAll = false, bool createDirectory = false)
        {
            extension = ConvertToFileExtensionFilter(extension);
            var option = searchAll ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return CheckDirectory(uncPath, createDirectory) ?
                Directory.EnumerateFiles(uncPath, extension, option) : Array.Empty<string>();
        }

        public static string ConvertToFileExtensionFilter(string fileExtension)
        {
            var sb = new StringBuilder();
            if (string.IsNullOrEmpty(fileExtension))
                fileExtension = "*";
            else if (fileExtension.StartsWith("."))
                sb.Append("*");
            else if (!fileExtension.StartsWith("*."))
                sb.Append("*.");
            sb.Append(fileExtension);
            return sb.ToString();
        }

        public static bool CheckDirectory(string uncPath, bool createDirectory = false)
        {
            bool exists = Directory.Exists(uncPath);
            if (createDirectory)
                CreateDirectory(uncPath);
            else if (!exists)
                _logger.LogWarning("Folder not found: '{0}'.", uncPath);
            return exists;
        }

        private static void CreateDirectory(string uncPath)
        {
            try
            {
                //var security = new DirectorySecurity("ReadFolder", AccessControlSections.Access);
                // If the directory already exists, this method does not create a new directory.
                Directory.CreateDirectory(uncPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "CreateDirectory access is denied, folder not created: '{0}'", uncPath);
            }
        }

        public static void ValidateExtension(ref string filePath, string suffix)
        {
            if (!suffix?.StartsWith(".") ?? false)
            {
                var sb = new StringBuilder(".");
                sb.Append(suffix);
                suffix = sb.ToString();
            }
            string extension = Path.GetExtension(filePath);
            if (string.IsNullOrWhiteSpace(extension) &&
                !string.IsNullOrWhiteSpace(suffix))
            {
                var sb = new StringBuilder(filePath);
                sb.Append(suffix);
                filePath = sb.ToString();
            }
        }

        public static string GetValidatedFileName(string filePath, string suffix)
        {
            string validFileName = filePath ?? "";
            if (!suffix?.StartsWith(".") ?? false)
                suffix = $".{suffix}";
            string extension = Path.GetExtension(filePath);
            if (string.IsNullOrWhiteSpace(extension) &&
                !string.IsNullOrWhiteSpace(suffix))
                validFileName = $"{filePath}{suffix}";
            return validFileName;
        }

        private static string ReplaceInvalidFileNameChars(this string fileName, char replacement = '_')
        {
            string validFileName = fileName ?? "";
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidFileChars)
                validFileName = validFileName.Replace(c, replacement);
            return validFileName;
        }

        private static string ReplaceInvalidPathChars(this string filePath, char replacement = '_')
        {
            string validFileName = filePath ?? "";
            char[] invalidFileChars = Path.GetInvalidPathChars();
            foreach (char c in invalidFileChars)
                validFileName = validFileName.Replace(c, replacement);
            return validFileName;
        }

        private static string GetDestination(string sourceFilePath, string destinationFolderName)
        {
            string destination = "";
            if (FileCheckOk(sourceFilePath, true))
            {
                string directory = Path.GetDirectoryName(sourceFilePath);
                string destinationDirectory = Path.Combine(directory, destinationFolderName);
                CreateDirectory(destinationDirectory);
                string fileName = Path.GetFileName(sourceFilePath);
                destination = Path.Combine(destinationDirectory, fileName);
            }
            return destination;
        }

        public static void CopyFileToFolder(string filePath, string folderName)
        {
            string fromTo = string.Empty;
            try
            {
                string destination = GetDestination(filePath, folderName);
                fromTo = string.Format(" from '{0}' to '{1}'.", filePath, destination);
                File.Copy(filePath, destination);
                _logger.LogDebug("File copied{0}.", fromTo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to copy file{0}.", fromTo);
            }
        }

        public static void MoveFileToFolder(string filePath, string folderName)
        {
            string fromTo = string.Empty;
            try
            {
                string destination = GetDestination(filePath, folderName);
                fromTo = string.Format(" from '{0}' to '{1}'.", filePath, destination);
                File.Move(filePath, destination);
                _logger.LogDebug("File moved{0}.", fromTo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to move file{0}.", fromTo);
            }
        }

        private static string NormaliseFilePath(string filePath) =>
            !Path.HasExtension(filePath) && // directory separator = '\\'
            !filePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ?
            filePath += Path.DirectorySeparatorChar : filePath;

        public static bool FileCheckOk(
            string filePath, bool checkFile = false)
        {
            bool isExisting = false;

            // Directory must end with a "\\" character
            if (!checkFile)
                filePath = NormaliseFilePath(filePath);

            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
                _logger.LogWarning("Folder not found: '{0}'.", directory);
            else if (checkFile && !File.Exists(filePath))
                _logger.LogWarning("File not found: '{0}'.", filePath);
            else
                isExisting = true;
            return isExisting;
        }

        public static async Task<byte[]> GetBytesAsync(this Stream sourceStream)
        {
            using (var outputStream = new MemoryStream())
            {
                sourceStream.Position = 0;
                await sourceStream.CopyToAsync(outputStream);
                return outputStream.ToArray();
            }
        }

        public static async Task<MemoryStream> GetFileStreamAsync(string inputPath)
        {
            var outputStream = new MemoryStream();

            if (!FileCheckOk(inputPath, true))
                return outputStream;

            using (FileStream source = new FileStream(
                inputPath, FileMode.Open, FileAccess.Read,
                FileShare.Read, BufferSize, useAsync: true))
            {
                await source.CopyToAsync(outputStream);
            }
            outputStream.Position = 0;

            return outputStream;
        }

        public static Stream GetFileStream(string filePath)
        {
            return GetFileStreamAsync(filePath).GetAwaiter().GetResult();
        }

        public static Stream GetFileOutputStream(string filePath, string fileName)
        {
            CreateDirectory(filePath);
            filePath = Path.Combine(filePath, fileName);
            return File.Create(filePath);
        }

        /// <summary>
        /// Check if the file is unavailable because it is:
        /// still being written to, being processed by another thread,
        /// or does not exist (has already been processed).
        /// </summary>
        /// <param name="file"></param>
        /// <returns>True if unavailable, false if not locked</returns>
        public static bool IsFileLocked(this FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(
                    FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                    stream.Close();
            }
            catch (IOException)
            {
                _logger.LogWarning("File is locked, close it then try again. '{0}'.", file?.FullName);
            }

            return false;
        }

        public static async Task<string> ReadFileAsync(
            string filePath, CancellationToken ct = default)
        {
            string content = string.Empty;
            if (FileCheckOk(filePath, true))
            {
                using (var stream = new FileStream(
                    filePath, FileMode.Open, FileAccess.Read,
                    FileShare.Read, BufferSize, useAsync: true))
                {
                    var bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, bytes.Length, ct);
                    //.ContinueWith(t => Logger.LogWarning(t.Exception,
                    //    $"Failed to read file: '{filePath}'."),
                    //    TaskContinuationOptions.OnlyOnFaulted);
                    content = Encoding.UTF8.GetString(bytes);
                }
                _logger.LogDebug("OK '{0}'.", filePath);
            }
            return content;
        }

        public static async Task<string> WriteToFileAsync(
            this Stream inputStream, string filePath, bool create = false, CancellationToken ct = default)
        {
            FileMode fileMode = create ? FileMode.Create : FileMode.Append;
            if (fileMode == FileMode.Create || fileMode == FileMode.CreateNew)
            {
                var directory = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(directory);
            }

            string fileName = string.Empty;
            filePath = filePath.Replace(Path.AltDirectorySeparatorChar, Path.PathSeparator);

            if (inputStream != null && FileCheckOk(filePath))
            {
                using (FileStream destination = new FileStream(
                    filePath, fileMode, FileAccess.Write,
                    FileShare.Write, BufferSize, useAsync: true))
                    {
                        inputStream.Position = 0;
                        await inputStream.CopyToAsync(destination, BufferSize, ct)
                            .ContinueWith(t =>
                            {
                                if (t.IsFaulted) _logger.LogWarning(t.Exception, "Failed to copy to '{0}'.", filePath);
                                else _logger.LogDebug("File copy {0}, '{1}'.", t.Status, filePath);
                            });
                    }
                    fileName = filePath;
            }

            return fileName;
        }
    }
}
