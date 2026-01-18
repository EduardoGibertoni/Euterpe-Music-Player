using System.Collections.Generic;
using System.IO;
using Euterpe.Models;

namespace Euterpe.Services
{
    public static class AlbumService
    {
        public static List<Album> LoadAlbums(string rootPath)
        {
            var albums = new List<Album>();

            if (!Directory.Exists(rootPath))
                return albums;

            foreach (var artistFolder in Directory.GetDirectories(rootPath))
            {
                // Caso 1: artista → vários álbuns
                foreach (var albumFolder in Directory.GetDirectories(artistFolder))
                {
                    var album = AlbumScanner.Scan(albumFolder);
                    if (album != null)
                        albums.Add(album);
                }

                // Caso 2: artista → arquivos direto (um álbum só)
                var directAlbum = AlbumScanner.Scan(artistFolder);
                if (directAlbum != null)
                    albums.Add(directAlbum);
            }

            return albums;
        }
    }
}
