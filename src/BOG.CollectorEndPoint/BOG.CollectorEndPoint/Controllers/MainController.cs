using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace BOG.CollectorEndPoint.Controllers
{
	[ApiController]
	[Produces("text/plain")]
	[Route("api")]
	public class MainController : ControllerBase
	{
		private readonly ILogger<MainController> _logger;
		private readonly IConfiguration _config;
		private readonly IHttpContextAccessor _accessor;

		/// <summary>
		/// Instantiated via injection
		/// </summary>
		/// <param name="logger">(injected)</param>
		/// <param name="config">(injected)</param>
		/// <param name="accessor">(injected)</param>
		public MainController(ILogger<MainController> logger, IConfiguration config, IHttpContextAccessor accessor)
		{
			_logger = logger;
			_config = config;
			_accessor = accessor;
		}

		/// <summary>
		/// Endpoint to capture the request object.
		/// </summary>
		/// <returns>200</returns>
		[Route("event", Name = "Event")]
		[HttpGet]
		[HttpPost]
		[HttpPut]
		[HttpDelete]
		[HttpOptions]
		[HttpPatch]
		[HttpHead]
		[RequestSizeLimit(1048576)]
		[ProducesResponseType(200, Type = typeof(string))]
		[ProducesResponseType(500)]
		[Produces("application/json")]
		public IActionResult Event()
		{
			var outputFile = Path.GetTempFileName();
			if (!string.IsNullOrWhiteSpace(_config.GetValue<string>("EventLogFile")))
			{
				outputFile = _config.GetValue<string>("EventLogFile");
			}
			var output = new StringBuilder();
			output.AppendLine();
			output.AppendLine($"Received on:    {DateTime.Now.ToString("s")}");
			output.AppendLine($"Received from:  {_accessor.HttpContext.Connection.RemoteIpAddress}:{_accessor.HttpContext.Connection.RemotePort}");
			output.AppendLine($"Protocol:       {_accessor.HttpContext.Request.Protocol}");
			output.AppendLine($"Scheme:         {_accessor.HttpContext.Request.Scheme}");
			output.AppendLine($"Method:         {_accessor.HttpContext.Request.Method}");
			output.AppendLine($"Host:           {_accessor.HttpContext.Request.Host}");
			output.AppendLine($"Path:           {_accessor.HttpContext.Request.Path}");
			output.AppendLine($"Headers:  ================");
			foreach (var item in _accessor.HttpContext.Response.Headers)
			{
				output.AppendLine($"{item.Key}: {item.Value}");
			}
			output.AppendLine($"====================================================================");
			output.AppendLine($"ContentType:    {_accessor.HttpContext.Request.ContentType}");
			output.AppendLine($"ContentLength:  {_accessor.HttpContext.Request.ContentLength}");
			output.AppendLine($"======= BODY =======================================================");
			using (var sr = new StreamReader(_accessor.HttpContext.Request.Body))
			{
				output.AppendLine(sr.ReadToEndAsync().Result);
			}
			Console.WriteLine(output.ToString());
			using (var sw = new StreamWriter(outputFile, true))
			{
				sw.WriteLine(output.ToString());
			}

			return StatusCode(200);
		}
	}
}
