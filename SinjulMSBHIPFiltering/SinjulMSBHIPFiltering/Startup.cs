﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SinjulMSBHIPFiltering.Filters;
using SinjulMSBHIPFiltering.Middlewares;

namespace SinjulMSBHIPFiltering
{
	public class Startup
	{
		public Startup ( IConfiguration configuration )
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices ( IServiceCollection services )
		{
			//services.AddIPFiltering( this.Configuration.GetSection( "IPFiltering" ) );
			//services.AddIPFiltering(
			//  opts =>
			//  {
			//	  opts.DefaultBlockLevel = DefaultBlockLevel.All;
			//	  opts.HttpStatusCode = HttpStatusCode.NotFound;
			//	  opts.Blacklist = new List<string> { "192.168.0.100-192.168.1.200" };
			//	  opts.Whitelist = new List<string> { "192.168.0.10-192.168.10.20" , "fe80::/10" };
			//	  opts.IgnoredPaths = new List<string> { "get:/ignoreget" , "*:/ignore" };
			//  } );

			#region IP Range

			// rangeA.Begin is "192.168.0.0", and rangeA.End is "192.168.0.255".
			//var rangeA = IPAddressRange.Parse("192.168.0.0/255.255.255.0");
			//rangeA.Contains( IPAddress.Parse( "192.168.0.34" ) ); // is True.
			//rangeA.Contains( IPAddress.Parse( "192.168.10.1" ) ); // is False.
			//rangeA.ToCidrString( ); // is 192.168.0.0/24

			//// rangeB.Begin is "192.168.0.10", and rangeB.End is "192.168.10.20".
			//var rangeB1 = IPAddressRange.Parse("192.168.0.10 - 192.168.10.20");
			//rangeB1.Contains( IPAddress.Parse( "192.168.3.45" ) ); // is True.
			//rangeB1.Contains( IPAddress.Parse( "192.168.0.9" ) ); // is False.

			//// Support shortcut range description.
			//// ("192.168.10.10-20" means range of begin:192.168.10.10 to end:192.168.10.20.)
			//var rangeB2 = IPAddressRange.Parse("192.168.10.10-20");

			//// Support CIDR expression and IPv6.
			//var rangeC = IPAddressRange.Parse("fe80::/10");
			//rangeC.Contains( IPAddress.Parse( "fe80::d503:4ee:3882:c586%3" ) ); // is True.
			//rangeC.Contains( IPAddress.Parse( "::1" ) ); // is False.

			//// "Contains()" method also support IPAddressRange argument.
			//var rangeD1 = IPAddressRange.Parse("192.168.0.0/16");
			//var rangeD2 = IPAddressRange.Parse("192.168.10.0/24");
			//rangeD1.Contains( rangeD2 ); // is True.

			//// IEnumerable<IPAddress> support, it's lazy evaluation.
			//foreach ( var ip in IPAddressRange.Parse( "192.168.0.1/23" ) )
			//{
			//	Console.WriteLine( ip );
			//}

			//// You can use LINQ via "AsEnumerable()" method.
			//var longValues = IPAddressRange.Parse("192.168.0.1/23")
			// .AsEnumerable()
			// .Select(ip => System.BitConverter.ToInt32(ip.GetAddressBytes(), 0))
			// .Select(adr => adr.ToString("X8"));
			////Console.WriteLine( string.Join( "," , longValues );

			//// Constructors from IPAddress objects.
			//var ipBegin = IPAddress.Parse("192.168.0.1");
			//var ipEnd = IPAddress.Parse("192.168.0.128");
			//var ipSubnet = IPAddress.Parse("255.255.255.0");

			//var rangeE = new IPAddressRange(); // This means "0.0.0.0/0".
			//var rangeF = new IPAddressRange(ipBegin, ipEnd);
			//var rangeG = new IPAddressRange(ipBegin, maskLength: 24);
			//var rangeH = new IPAddressRange(ipBegin, IPAddressRange.SubnetMaskLength(ipSubnet));

			//// Calculates Cidr subnets
			//var rangeI = IPAddressRange.Parse("192.168.0.0-192.168.0.254");
			//rangeI.ToCidrString( );  // is 192.168.0.0/24

			#endregion IP Range

			services.AddMvc( );
			services.AddScoped<ClientIdCheckFilter>( );
			services.AddSingleton<IHttpContextAccessor , HttpContextAccessor>( );
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure ( IApplicationBuilder app , IHostingEnvironment env , ILoggerFactory loggerFactory )
		{
			if ( env.IsDevelopment( ) )
			{
				app.UseForwardedHeaders( new ForwardedHeadersOptions
				{
					ForwardedHeaders = ForwardedHeaders.XForwardedFor ,
					// IIS is also tagging a X-Forwarded-For header on, so we need to increase this limit,
					// otherwise the X-Forwarded-For we are passing along from the browser will be ignored
					ForwardLimit = 2
				} );

				app.UseStatusCodePages( );
				app.UseBrowserLink( );
				app.UseDeveloperExceptionPage( );
			}
			else
			{
				app.UseExceptionHandler( "/Home/Error" );
			}

			//loggerFactory.AddNLog( );

			//var configDir = "C:\\git\\ClientIpAspNetCore\\Logs";
			//var configDir = "C:\\inetpub\\wwwroot\\clientidaspnetcore\\Logs";

			//if ( configDir != string.Empty )
			//{
			//	var logEventInfo = NLog.LogEventInfo.CreateNullEvent();
			//	foreach ( FileTarget target in LogManager.Configuration.AllTargets.Where( t => t is FileTarget ) )
			//	{
			//		var filename = target.FileName.Render(logEventInfo).Replace("'", "");
			//		target.FileName = Path.Combine( configDir , filename );
			//	}

			//	LogManager.ReconfigExistingLoggers( );
			//}

			//app.UseIPFiltering( );

			app.UseMiddleware<AdminWhiteListMiddleware>( Configuration[ "AdminWhiteList" ] );

			app.UseStaticFiles( );

			app.UseMvc( routes =>
			 {
				 routes.MapRoute(
			    name: "default" ,
			    template: "{controller=Home}/{action=GetIP}/{id?}" );
			 } );
		}
	}
}