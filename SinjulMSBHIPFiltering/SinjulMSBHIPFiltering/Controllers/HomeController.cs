using System.Diagnostics;
using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SinjulMSBHIPFiltering.Filters;
using SinjulMSBHIPFiltering.Models;

namespace SinjulMSBHIPFiltering.Controllers
{
	public class HomeController: Controller
	{
		private IHttpContextAccessor _accessor;
		private readonly IHostingEnvironment _hostingEnvironment;
		private ILogger<HomeController> _logger;

		public HomeController ( IHttpContextAccessor accessor ,
						IHostingEnvironment hostingEnvironment ,
						ILogger<HomeController> logger
					    )
		{
			_accessor=accessor;
			_hostingEnvironment = hostingEnvironment;
			_logger = logger;
		}

		public IActionResult Index ( )
		{
			return View( );
		}

		[ServiceFilter( typeof( ClientIdCheckFilter ) )]
		public IActionResult GetIP ( )
		{
			_logger.LogDebug( "successful get.." );

			ViewData[ "GetIP" ] = _accessor.HttpContext.Connection.RemoteIpAddress.ToString( );

			//download link GeoLite2-City.mmdb => https://dev.maxmind.com/geoip/geoip2/geolite2/
			//http://geolite.maxmind.com/download/geoip/database/GeoLite2-City.tar.gz
			using ( var reader = new DatabaseReader( _hostingEnvironment.ContentRootPath + "\\GeoLite2-City.mmdb" ) )
			{
				// Determine the IP Address of the request
				var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString() == "::1"
					//Your public IP address
					? "31.58.123.0"
					: HttpContext.Connection.RemoteIpAddress.ToString()
				;

				// Get the city from the IP Address
				var city = reader.City(ipAddress);

				return View( city );
			}
		}

		public IActionResult About ( )
		{
			ViewData[ "Message" ] = "Your application description page.";

			return View( );
		}

		public IActionResult Contact ( )
		{
			ViewData[ "Message" ] = "Your contact page.";

			return View( );
		}

		public IActionResult Error ( )
		{
			return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
		}
	}
}