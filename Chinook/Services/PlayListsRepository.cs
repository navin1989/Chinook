﻿using AutoMapper;
using Azure;
using Azure.Core;
using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Composition;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Chinook.Services
{
    public class PlayListsRepository : IPlayListsRepository
    {
        private readonly ChinookContext _dbContext;
        public PlayListsRepository(ChinookContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Creating playlist per user
        public async Task<List<Models.Playlist>> CreatePlaylists(PlaylistTrack playlistTrack, string playListName, string userId)
        {
            var track = _dbContext.Tracks.FirstOrDefault(t => t.TrackId == playlistTrack.TrackId);
            Random rnd = new Random();
            int index = rnd.Next();
            try
            {
                if (CheckForPlaylistExistence(_dbContext, userId , playListName))
                {
                    var playlists = _dbContext.Playlists.Include(p => p.UserPlaylists).Where(p => p.UserPlaylists.Any(up => up.UserId == userId));
                    var list = playlists.First(p => p.Name == playListName);
                    list.Tracks.Add(track);
                    _dbContext.Playlists.Update(list);
                }
                else
                {
                    var playList = new Chinook.Models.Playlist() { PlaylistId = index, Name = playListName };
                    playList.Tracks.Add(track);
                    _dbContext.Playlists.Add(playList);

                    var userPlayList = new Chinook.Models.UserPlaylist() { PlaylistId = index, UserId = userId };
                    _dbContext.UserPlaylists.Add(userPlayList);
                }

                await _dbContext.SaveChangesAsync();
                return _dbContext.Playlists.Where(p=> p.UserPlaylists.Any(up=>up.UserId == userId)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get action
        //Return a single "PlayList" data 
        public async Task<ClientModels.Playlist> GetPlayList(string userId, long playListId)
        {
            var list = _dbContext.Playlists
                        .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
                        .Where(p => p.PlaylistId == playListId)
                        .Select(p => new ClientModels.Playlist()
                        {
                            Name = p.Name,
                            Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                            {
                                AlbumTitle = t.Album.Title,
                                ArtistName = t.Album.Artist.Name,
                                TrackId = t.TrackId,
                                TrackName = t.Name,
                                IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == "My Favorites")).Any()
                            }).ToList()
                        })
                       .FirstOrDefault();

            return list != null ? list : new ClientModels.Playlist();
        }

        //Get action
        //Returns List of "PlayList" data 
        async Task<List<Models.Playlist>> IPlayListsRepository.GetPlayLists(string userId)
        {
            try
            {
                var up = _dbContext.UserPlaylists.Where(up => up.UserId == userId);
                return await _dbContext.Playlists.Where(p => up.Any(a => a.PlaylistId == p.PlaylistId)).ToListAsync(); //Filters only the current user's playlists
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool CheckForPlaylistExistence(ChinookContext dbContext, string userId, string playListName)
        {
            var playlists = dbContext.Playlists.Include(p => p.UserPlaylists).Where(p => p.UserPlaylists.Any(up => up.UserId == userId));
            bool playListExist = playlists.Any(p => p.Name == playListName);
            return playListExist;
        }
    }
}
