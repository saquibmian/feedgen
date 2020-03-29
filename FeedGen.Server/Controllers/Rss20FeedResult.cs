using System;
using System.IO;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace FeedGen.Server.Controllers {
    public class Rss20FeedResult : ActionResult {
        private readonly SyndicationFeed _feed;

        public Rss20FeedResult( SyndicationFeed feed ) {
            _feed = feed ?? throw new ArgumentNullException( nameof( feed ) );
        }

        public override void ExecuteResult( ActionContext context ) {
            using var ms = new MemoryStream();

            using (var writer = XmlWriter.Create( ms )) {
                new Rss20FeedFormatter( _feed ).WriteTo( writer );
            }

            ms.Position = 0;
            context.HttpContext.Response.StatusCode = 200;
            context.HttpContext.Response.ContentType = "application/xml";
            context.HttpContext.Response.ContentLength = ms.Length;

            ms.CopyTo( context.HttpContext.Response.Body );
        }

        public override Task ExecuteResultAsync( ActionContext context ) {
            using var ms = new MemoryStream();

            using (var writer = XmlWriter.Create( ms )) {
                new Rss20FeedFormatter( _feed ).WriteTo( writer );
            }

            ms.Position = 0;
            context.HttpContext.Response.StatusCode = 200;
            context.HttpContext.Response.ContentType = "application/xml";
            context.HttpContext.Response.ContentLength = ms.Length;

            return ms.CopyToAsync( context.HttpContext.Response.Body );
        }
    }
}
