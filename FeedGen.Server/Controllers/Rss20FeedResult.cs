using System;
using System.IO;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace FeedGen.Server.Controllers {
    public class Rss20FeedResult : ActionResult {
        private readonly SyndicationFeedFormatter _formatter;

        public Rss20FeedResult( SyndicationFeed feed ) {
            _formatter = new Rss20FeedFormatter(
                feed ?? throw new ArgumentNullException( nameof( feed ) ),
                serializeExtensionsAsAtom: false
            );
        }

        public override void ExecuteResult( ActionContext context ) {
            using var stream = BuildFeed();
            context.HttpContext.Response.StatusCode = 200;
            context.HttpContext.Response.ContentType = "application/xml";
            context.HttpContext.Response.ContentLength = stream.Length;

            stream.CopyTo( context.HttpContext.Response.Body );
        }

        public override Task ExecuteResultAsync( ActionContext context ) {
            using var stream = BuildFeed();
            context.HttpContext.Response.StatusCode = 200;
            context.HttpContext.Response.ContentType = "application/xml";
            context.HttpContext.Response.ContentLength = stream.Length;

            return stream.CopyToAsync( context.HttpContext.Response.Body );
        }

        private Stream BuildFeed() {
            var ms = new MemoryStream();

            using (var writer = XmlWriter.Create( ms )) {
                _formatter.WriteTo( writer );
            }

            ms.Position = 0;
            return ms;
        }
    }
}
