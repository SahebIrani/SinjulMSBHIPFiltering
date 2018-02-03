using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SinjulMSBHIPFiltering.Models;

namespace SinjulMSBHIPFiltering.Controllers
{
	public class HomeController: Controller
	{
		private IHttpContextAccessor _accessor;

		public HomeController ( IHttpContextAccessor accessor )
		{
			_accessor=accessor;
		}

		public IActionResult Index ( )
		{
			return View( );
		}

		public IActionResult GetIP ( )
		{
			ViewData[ "GetIP" ] = _accessor.HttpContext.Connection.RemoteIpAddress.ToString( );
			return View( );
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