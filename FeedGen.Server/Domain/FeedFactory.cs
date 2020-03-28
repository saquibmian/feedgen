using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml.Serialization;

namespace FeedGen.Server.Domain {
    [XmlRoot( "rss" )]
    public class FeedFactory {
        private static readonly ISet<string> s_allowedExtensions = ImmutableHashSet.Create(
            ".mp3"
        );
        private readonly ApplicationConfiguration _config;

        public FeedFactory( ApplicationConfiguration config ) {
            _config = config;
        }

        public SyndicationFeed Create( string dir ) {
            var title = File.ReadAllText( Path.Combine( _config.RootDirectory, dir, "title.txt" ) );
            var description = File.ReadAllText( Path.Combine( _config.RootDirectory, dir, "description.txt" ) );
            var items = Directory.EnumerateFiles( Path.Combine( _config.RootDirectory, dir ) )
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
                title,
                description,
                new Uri( $"{_config.ExternalAddress}/feed/{dir}.xml" ),
                items
            ) {
                Generator = "FeedGen",
            };
            return feed;
        }
    }
}
