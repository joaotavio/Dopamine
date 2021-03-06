﻿using Dopamine.Common.Services.Playback;
using Dopamine.Common.Services.Playlist;
using Dopamine.Common.Services.Provider;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Dopamine.Common.Presentation.ViewModels.Base
{
    public abstract class ContextMenuViewModelBase : BindableBase
    {
        #region Variables
        private IProviderService providerService;
        private IPlaylistService playlistService;
        private IPlaybackService playbackService;
        private ObservableCollection<SearchProvider> contextMenuSearchProviders;
        private ObservableCollection<PlaylistViewModel> contextMenuPlaylists;
        #endregion

        #region Commands
        public DelegateCommand<string> SearchOnlineCommand { get; set; }
        public DelegateCommand<string> AddPlayingTrackToPlaylistCommand { get; set; }
        #endregion

        #region Properties
        public bool HasContextMenuPlaylists => this.ContextMenuPlaylists != null && this.ContextMenuPlaylists.Count > 0;

        public ObservableCollection<SearchProvider> ContextMenuSearchProviders
        {
            get { return this.contextMenuSearchProviders; }
            set
            {
                SetProperty<ObservableCollection<SearchProvider>>(ref this.contextMenuSearchProviders, value);
                OnPropertyChanged(() => this.HasContextMenuSearchProviders);
            }
        }

        public ObservableCollection<PlaylistViewModel> ContextMenuPlaylists
        {
            get { return this.contextMenuPlaylists; }
            set
            {
                SetProperty<ObservableCollection<PlaylistViewModel>>(ref this.contextMenuPlaylists, value);
                OnPropertyChanged(() => this.HasContextMenuPlaylists);
            }
        }
        #endregion

        #region Construction
        public ContextMenuViewModelBase(IUnityContainer container)
        {
            // Dependency injection
            this.providerService = container.Resolve<IProviderService>();
            this.playlistService = container.Resolve<IPlaylistService>();
            this.playbackService = container.Resolve<IPlaybackService>();

            // Commands
            this.SearchOnlineCommand = new DelegateCommand<string>((id) => this.SearchOnline(id));
            this.AddPlayingTrackToPlaylistCommand = new DelegateCommand<string>(async(playlistName) => await this.AddPlayingTrackToPlaylistAsync(playlistName));

            // Events
            this.providerService.SearchProvidersChanged += (_, __) => { this.GetSearchProvidersAsync(); };
            this.playlistService.PlaylistAdded += (_) => this.GetContextMenuPlaylistsAsync();
            this.playlistService.PlaylistDeleted += (_) => this.GetContextMenuPlaylistsAsync();

            // Initialize the search providers in the ContextMenu
            this.GetSearchProvidersAsync();

            // Initialize the playlists in the ContextMenu
            this.GetContextMenuPlaylistsAsync();
        }
        #endregion

        #region Private
        private async void GetSearchProvidersAsync()
        {
            this.ContextMenuSearchProviders = null;

            List<SearchProvider> providersList = await this.providerService.GetSearchProvidersAsync();
            var localProviders = new ObservableCollection<SearchProvider>();

            await Task.Run(() =>
            {
                foreach (SearchProvider vp in providersList)
                {
                    localProviders.Add(vp);
                }
            });

            this.ContextMenuSearchProviders = localProviders;
        }

        private async void GetContextMenuPlaylistsAsync()
        {
            try
            {
                // Unbind to improve UI performance
                this.ContextMenuPlaylists = null;

                List<string> playlists = await this.playlistService.GetPlaylistsAsync();

                // Populate an ObservableCollection
                var playlistViewModels = new ObservableCollection<PlaylistViewModel>();

                await Task.Run(() =>
                {
                    foreach (string playlist in playlists)
                    {
                        playlistViewModels.Add(new PlaylistViewModel { Name = playlist });
                    }

                });

                // Re-bind to update the UI
                this.ContextMenuPlaylists = playlistViewModels;

            }
            catch (Exception)
            {
                // If loading from the database failed, create and empty Collection.
                this.ContextMenuPlaylists = new ObservableCollection<PlaylistViewModel>();
            }
        }

        private async Task AddPlayingTrackToPlaylistAsync(string playlistName)
        {
        }
        #endregion

        #region Protected
        protected bool HasContextMenuSearchProviders => this.ContextMenuSearchProviders != null && this.ContextMenuSearchProviders.Count > 0;
        #endregion

        #region Abstract
        protected abstract void SearchOnline(string id);
        #endregion
    }
}
