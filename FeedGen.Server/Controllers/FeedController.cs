using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using FeedGen.Server.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace FeedGen.Server.Controllers {
    [ApiController]
    [Route( "feed" )]
    public class FeedController : ControllerBase {
        private readonly FeedFactory _feedFactory;
        public FeedController( FeedFactory feedFactory ) {
            _feedFactory = feedFactory;
        }

        [HttpGet]
        [Route( "{dir}.xml" )]
        public ActionResult Get( string dir ) {
            var feed = _feedFactory.Create( dir );

            var stream = new MemoryStream();
            var xml = XmlWriter.Create( stream );
            new Rss20FeedFormatter( feed ).WriteTo( xml );

            return new FileStreamResult( stream, new MediaTypeHeaderValue( "application/xml" ) ) {
                FileDownloadName = "feed.xml"
            };
        }
    }
}
