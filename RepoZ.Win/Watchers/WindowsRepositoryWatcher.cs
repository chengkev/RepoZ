﻿using RepoZ.Shared;
using RepoZ.Win.Watchers;
using System;
using System.IO;
using System.Threading.Tasks;
using RepoZ.Shared.Git;

namespace RepoZ.Win
{
	public class WindowsRepositoryWatcher : IRepositoryWatcher
	{
		private string _path;
		private FileSystemWatcher _watcher;
		private IRepositoryReader _repositoryReader;

		public WindowsRepositoryWatcher(IRepositoryReader repositoryHelper)
		{
			_repositoryReader = repositoryHelper;
		}

		public Action<RepositoryInfo> OnChangeDetected { get; set; }

		public void Setup(string path)
		{
			_path = path;
			_watcher = new FileSystemWatcher(_path, "HEAD");
			_watcher.Created += _watcher_Created;
			_watcher.Changed += _watcher_Changed;
			_watcher.Deleted += _watcher_Deleted;
			_watcher.IncludeSubdirectories = true;
		}

		public void Watch()
		{
			_watcher.EnableRaisingEvents = true;
		}

		public void Stop()
		{
			_watcher.EnableRaisingEvents = false;
		}

		private void _watcher_Deleted(object sender, FileSystemEventArgs e)
		{
			if (!isHead(e.FullPath))
				return;
		}

		private void _watcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (!isHead(e.FullPath))
				return;

			eatRepo(e.FullPath);
		}

		private void _watcher_Created(object sender, FileSystemEventArgs e)
		{
			if (!isHead(e.FullPath))
				return;

			Task.Run(() => Task.Delay(5000))
				.ContinueWith(t => eatRepo(e.FullPath));
		}

		private bool isHead(string fullPath)
		{
			return fullPath.IndexOf(@".git\HEAD", StringComparison.OrdinalIgnoreCase) > -1;
		}

		private void eatRepo(string path)
		{
			var repo = _repositoryReader.ReadRepository(path);

			if (repo.WasFound)
			{
				//_repositories.AddOrUpdate(repo.Path, repo.CurrentBranch, (k, v) => repo.CurrentBranch);
				OnChangeDetected?.Invoke(repo);
			}
		}
	}
}
