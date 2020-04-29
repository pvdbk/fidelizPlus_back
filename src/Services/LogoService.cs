using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace fidelizPlus_back.Services
{
	public class LogoService
	{
		public const int MAX_SIZE = 0xFFFFF;
		public string TMP_PREFIX = "tmp";
		public string LOGO_PREFIX = "logo";
		public const string MIME_TYPE = "image/png";
		public const string EXTENSION = ".png";
		public string LogosPath { get; }
		TraderService TraderService { get; }

		public LogoService(IConfiguration configuration, TraderService traderService)
		{
			LogosPath = configuration.GetSection("Paths")["Logos"];
			TraderService = traderService;
		}

		private async Task Resize(Stream picStream, string newName)
		{
			picStream.Position = 0;
			using (HttpContent input = new StreamContent(picStream))
			using (HttpContent op = new StringContent("fixedWidth"))
			using (HttpContent width = new StringContent("70"))
			using (MultipartFormDataContent formData = new MultipartFormDataContent())
			using (HttpClient client = new HttpClient())
			{
				input.Headers.ContentType = new MediaTypeHeaderValue(MIME_TYPE);
				op.Headers.ContentType = null;
				width.Headers.ContentType = null;
				formData.Add(op, "op");
				formData.Add(width, "width");
				formData.Add(input, "input", "pic" + EXTENSION);
				using (HttpResponseMessage response = await client.PostAsync("https://img-resize.com/resize", formData))
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						using (FileStream resized = File.Create(LogosPath + "/" + newName + EXTENSION))
						{
							await response.Content.CopyToAsync(resized);
						}
					}
					else
					{
						throw new AppException("Corrupted file", 400);
					}
				}
			}
		}

		public async Task Save(int traderId, IFormFile formFile)
		{
			if(formFile == null)
			{
				throw new AppException("Unspecified file", 400);
			}
			if (formFile.Length > MAX_SIZE)
			{
				throw new AppException("File too voluminous", 400);
			}
			if (formFile.ContentType != MIME_TYPE)
			{
				throw new AppException($"Only {MIME_TYPE}", 400);
			}
			TraderService.CheckCredentials(traderId);
			using (var originalStream = new MemoryStream())
			{
				await formFile.CopyToAsync(originalStream);
				await Resize(originalStream, $"{TMP_PREFIX}{traderId}");
			}
		}

		public void Confirm(int traderId)
		{
			TraderService.CheckCredentials(traderId);
			string tmpPath = $"{LogosPath}/{TMP_PREFIX}{traderId}{EXTENSION}";
			if (!File.Exists(tmpPath))
			{
				throw new AppException("Temporary file not found", 404);
			}
			File.Copy(tmpPath, $"{LogosPath}/{LOGO_PREFIX}{traderId}{EXTENSION}");
			File.Delete(tmpPath);
		}

		public void Get(int traderId, string prefix)
		{
			string filePath = $"{LogosPath}/{prefix}{traderId}{EXTENSION}";
			if (!File.Exists(filePath))
			{
				throw new AppException("Logo not found", 404);
			}
			byte[] ret;
			using (FileStream file = File.OpenRead(filePath))
			using (BinaryReader br = new BinaryReader(file))
			{
				ret = br.ReadBytes((int)file.Length);
			}
			throw new SendPic(ret);
		}
	}
}
