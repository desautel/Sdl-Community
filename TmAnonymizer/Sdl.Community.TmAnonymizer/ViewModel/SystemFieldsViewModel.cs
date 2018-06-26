﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class SystemFieldsViewModel:ViewModelBase
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<UniqueUsername> _uniqueUsernames;
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private readonly BackgroundWorker _backgroundWorker;
		private ICommand _selectAllCommand;
		private ICommand _removeUserCommand;
		private ICommand _applyChangesCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly List<AnonymizeTranslationMemory> _anonymizeTranslationMemories;
		private IList _selectedItems;
		private WaitWindow _waitWindow;


		public SystemFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_uniqueUsernames = new ObservableCollection<UniqueUsername>();
			_selectedItems = new List<UniqueUsername>();
			_sourceSearchResults = new ObservableCollection<SourceSearchResult>();
			_translationMemoryViewModel = translationMemoryViewModel;
			if (_tmsCollection != null)
			{
				var serverTms = _tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
				var fileBasedTms = _tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
				if (serverTms.Any())
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					foreach (var serverTm in serverTms)
					{
						UniqueUsernames = Helpers.SystemFields.GetUniqueServerBasedSystemFields(serverTm, translationProvider);
					}
					
				}

				if (fileBasedTms.Any())
				{
					foreach (var fileTm in fileBasedTms)
					{
						UniqueUsernames = Helpers.SystemFields.GetUniqueFileBasedSystemFields(fileTm);
					}
				}
			}
			
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;
			_translationMemoryViewModel.PropertyChanged += _translationMemoryViewModel_PropertyChanged;
			_anonymizeTranslationMemories = new List<AnonymizeTranslationMemory>();
		}

		public IList SelectedItems
		{
			get => _selectedItems;
			set
			{
				_selectedItems = value;
				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		public ObservableCollection<UniqueUsername> UniqueUsernames
		{
			get => _uniqueUsernames;

			set
			{
				if (Equals(value, _uniqueUsernames))
				{
					return;
				}
				_uniqueUsernames = value;
				OnPropertyChanged(nameof(UniqueUsernames));
			}
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllUserNames, true));
		public ICommand RemoveUserCommand => _removeUserCommand ?? (_removeUserCommand = new CommandHandler(RemoveAllUserNames, true));
		public ICommand ApplyChangesCommand => _applyChangesCommand ?? (_applyChangesCommand = new CommandHandler(ApplyChanges, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));
		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));


		private void SelectAllUserNames()
		{
			throw new NotImplementedException();
		}

		private void RemoveAllUserNames()
		{
			throw new NotImplementedException();
		}

		private void ApplyChanges()
		{
			foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
			{
				if (!tm.IsServerTm)
				{
					Helpers.SystemFields.AnonymizeFileBasedSystemFields(tm, UniqueUsernames.ToList());
				}

				else if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);

					Helpers.SystemFields.AnonymizeServerBasedSystemFields(tm, UniqueUsernames.ToList(), translationProvider);
				}
			}
			RefreshSystemFields();

		}

		private void RefreshSystemFields()
		{
			if (_tmsCollection != null)
			{
				UniqueUsernames = new ObservableCollection<UniqueUsername>();
				var serverTms = _tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
				var fileBasedTms = _tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
				if (fileBasedTms.Any())
				{
					foreach (var fileTm in fileBasedTms)
					{
						var names = Helpers.SystemFields.GetUniqueFileBasedSystemFields(fileTm);
						foreach (var name in names)
						{
							System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
							{
								UniqueUsernames.Add(name);
							});
						}
					}
				}
				if (serverTms.Any())
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					foreach (var serverTm in serverTms)
					{
						var names = Helpers.SystemFields.GetUniqueServerBasedSystemFields(serverTm, translationProvider);
						foreach (var name in names)
						{
							System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
							{
								UniqueUsernames.Add(name);
							});
						}
					}

				}
			}
		}

		private void Import()
		{
			throw new NotImplementedException();
		}

		private void Export()
		{
			throw new NotImplementedException();
		}

		private void _translationMemoryViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//if (e.PropertyName.Equals("TmsCollection"))
			//{
			//	//removed from tm collection
			//	RefreshPreviewWindow();
			//}
			if ((e.PropertyName.Equals("IsSelected")) && _tmsCollection != null)
			{
				//var tmViewModel = sender as TranslationMemoryViewModel;
				//var selectedTms = tmViewModel.SelectedItems;
				//foreach (var tm in collection)
				//{

				//}
				//var checkedTms = selectedTms.Where(t => t.IsSelected).ToList();
				
			}
		}

		private void _tmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (TmFile newTm in e.NewItems)
				{
					if (!newTm.IsServerTm)
					{
						UniqueUsernames = Helpers.SystemFields.GetUniqueFileBasedSystemFields(newTm);
					}
					
					newTm.PropertyChanged += NewTm_PropertyChanged;
				}
			}
		}

		private void NewTm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				_backgroundWorker.RunWorkerAsync(sender);
			}

		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (_waitWindow != null)
			{
				_waitWindow.Close();
			}
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{

			var tm = e.Argument as TmFile;
			System.Windows.Application.Current.Dispatcher.Invoke(delegate
			{
				_waitWindow = new WaitWindow();
				_waitWindow.Show();
			});
			if (tm.IsSelected)
			{
				if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					var names = Helpers.SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
					foreach (var name in names)
					{
						System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
						{
							UniqueUsernames.Add(name);
						});
						
					}
				}
				else
				{
					var names = Helpers.SystemFields.GetUniqueFileBasedSystemFields(tm);
					foreach (var name in names)
					{
						System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
						{
							UniqueUsernames.Add(name);
						});
					}
				}
			}
			else
			{
				if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					var names = Helpers.SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
					var newList = UniqueUsernames.ToList();
					foreach (var name in names)
					{
						newList.RemoveAll(n => n.Username == name.Username);
					}
					UniqueUsernames = new ObservableCollection<UniqueUsername>(newList);
				}
				else
				{
					var names = Helpers.SystemFields.GetUniqueFileBasedSystemFields(tm);
					var newList = UniqueUsernames.ToList();
					foreach (var name in names)
					{
						newList.RemoveAll(n => n.Username == name.Username);
					}
					UniqueUsernames = new ObservableCollection<UniqueUsername>(newList);
				}
			}
		}

		public ObservableCollection<SourceSearchResult> SourceSearchResults
		{
			get => _sourceSearchResults;

			set
			{
				if (Equals(value, _sourceSearchResults))
				{
					return;
				}
				_sourceSearchResults = value;
				OnPropertyChanged(nameof(SourceSearchResults));
			}
		}
	}
}