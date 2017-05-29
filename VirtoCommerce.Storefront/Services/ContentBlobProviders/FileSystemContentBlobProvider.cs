﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Services
{
    public class FileSystemContentBlobProvider : IContentBlobProvider, IStaticContentBlobProvider
    {
        private readonly string _basePath;

        // Keep links to file watchers to prevent GC to collect it
        private readonly FileSystemWatcher[] _fileSystemWatchers;

        public FileSystemContentBlobProvider(string basePath)
        {
            _basePath = basePath;
            _fileSystemWatchers = MonitorThemeFileSystemChanges(basePath);
        }

        #region IContentBlobProvider Members
        public event FileSystemEventHandler Changed;
        public event RenamedEventHandler Renamed;

        /// <summary>
        /// Open blob for read 
        /// </summary>
        /// <param name="path">blob relative path /folder/blob.md</param>
        /// <returns></returns>
        public virtual Stream OpenRead(string path)
        {
            path = NormalizePath(path);
            return File.OpenRead(path);
        }

        /// <summary>
        /// Open blob for write by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>blob stream</returns>
        public virtual Stream OpenWrite(string path)
        {
            path = NormalizePath(path);
            var folderPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return File.Open(path, FileMode.Create);
        }

        /// <summary>
        /// Check that blob or folder with passed path exist
        /// </summary>
        /// <param name="path">relative path /folder/blob.md</param>
        /// <returns></returns>
        public virtual bool PathExists(string path)
        {
            path = NormalizePath(path);
            var retVal = Directory.Exists(path);
            if (!retVal)
            {
                retVal = File.Exists(path);
            }
            return retVal;
        }

        /// <summary>
        /// Search blob content in specified folder
        /// </summary>
        /// <param name="path">relative folder path in which the search  Example: /folder</param>
        /// <param name="searchPattern">search blob name pattern can be used mask (*, ? symbols)</param>
        /// <param name="recursive"> recursive search</param>
        /// <returns>Returns relative path for all found blobs  example: /folder/blob.md </returns>
        public virtual IEnumerable<string> Search(string path, string searchPattern, bool recursive)
        {
            var retVal = new List<string>();
            path = NormalizePath(path);
            searchPattern = searchPattern.TrimStart(Path.PathSeparator);
            if (Directory.Exists(Path.GetDirectoryName(Path.Combine(path, searchPattern))))
            {
                var files = Directory.GetFiles(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                                     .Select(x => GetRelativePath(x));
                retVal.AddRange(files);
            }
            return retVal;
        }
        #endregion

        protected virtual string GetRelativePath(string path)
        {
            return path.Replace(_basePath, string.Empty).Replace('\\', '/');
        }

        protected virtual string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            path = path.Replace("/", "\\");
            path = path.Replace(_basePath, string.Empty);
            return Path.Combine(_basePath, path.TrimStart('\\'));
        }

        private FileSystemWatcher[] MonitorThemeFileSystemChanges(string path)
        {
            var result = new List<FileSystemWatcher>();
            if (Directory.Exists(path))
            {
                result.Add(SetFileSystemWatcher(path));

                var symbolicLinks = GetSymbolicLinks(path);
                foreach (var symbolicLink in symbolicLinks)
                {
                    result.Add(SetFileSystemWatcher(symbolicLink));
                }
            }
            return result.ToArray();
        }

        private FileSystemWatcher SetFileSystemWatcher(string path)
        {
            var fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = path;
            fileSystemWatcher.IncludeSubdirectories = true;

            FileSystemEventHandler handler = (sender, args) =>
            {
                RaiseChangedEvent(args);
            };
            RenamedEventHandler renamedHandler = (sender, args) =>
            {
                RaiseRenamedEvent(args);
            };
            var throttledHandler = handler.Throttle(TimeSpan.FromSeconds(5));
            // Add event handlers.
            fileSystemWatcher.Changed += throttledHandler;
            fileSystemWatcher.Created += throttledHandler;
            fileSystemWatcher.Deleted += throttledHandler;
            fileSystemWatcher.Renamed += renamedHandler;

            // Begin watching.
            fileSystemWatcher.EnableRaisingEvents = true;

            return fileSystemWatcher;
        }

        private IEnumerable<string> GetSymbolicLinks(string path)
        {
            var result = new List<string>();
            var directories = Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);
            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                if ((directoryInfo.Attributes & FileAttributes.ReparsePoint) != 0)
                {
                    result.Add(directory);
                }
            }
            return result;
        }

        protected virtual void RaiseChangedEvent(FileSystemEventArgs args)
        {
            var changedEvent = Changed;
            if (changedEvent != null)
            {
                changedEvent(this, args);
            }
        }

        protected virtual void RaiseRenamedEvent(RenamedEventArgs args)
        {
            var renamedEvent = Renamed;
            if (renamedEvent != null)
            {
                renamedEvent(this, args);
            }
        }
    }
}
