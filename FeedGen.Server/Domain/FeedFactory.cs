using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.Json;

namespace FeedGen.Server.Domain {
    public class FeedFactory {
        private static readonly ISet<string> s_allowedExtensions = ImmutableHashSet.Create(
            ".mp3"
        );
        private readonly ApplicationConfiguration _config;

        public FeedFactory( ApplicationConfiguration config ) {
            _config = config;
        }

        public SyndicationFeed? Create( string dir ) {
            var podcastDir = Path.Combine( _config.RootDirectory, dir );
            if (!Directory.Exists( podcastDir )) {
                return null;
            }

            var feedInfo = JsonSerializer.Deserialize<FeedInfo>(
                File.ReadAllText( Path.Combine( podcastDir, "feed.json" ) ),
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                }
            );

            var items = Directory.EnumerateFiles( podcastDir )
                .Where( file => s_allowedExtensions.Contains( Path.GetExtension( file ) ) )
                .Select( file => new FileInfo( file ) )
                .Select( file => {
                    var fullUrl = $"{_config.ExternalAddress}/media/{dir}/{file.Name}";
                    return new SyndicationItem(
                        title: file.Name,
                        content: SyndicationContent.CreateUrlContent( new Uri( fullUrl ), "audio/mpeg" ),
                        itemAlternateLink: new Uri( fullUrl ),
                        id: fullUrl,
                        lastUpdatedTime: file.CreationTimeUtc
                    );
                } );

            var feed = new SyndicationFeed(
                feedInfo.Title,
                feedInfo.Description,
                new Uri( $"{_config.ExternalAddress}/feed/{dir}.xml" ),
                items
            ) {
                Generator = "FeedGen",
                ImageUrl = new Uri( $"{_config.ExternalAddress}/media/{dir}/image.jpg" ),
            };
            feed.Authors.Add( new SyndicationPerson( "saquib.mian@gmail.com" ) );
            return feed;
        }
    }
}
