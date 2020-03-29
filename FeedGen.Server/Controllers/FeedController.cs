using FeedGen.Server.Domain;
using Microsoft.AspNetCore.Mvc;

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
            if (feed == null) {
                return NotFound();
            }

            return new Rss20FeedResult( feed );
        }
    }
}
