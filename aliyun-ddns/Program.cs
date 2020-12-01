using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Aliyun.Api;
using Aliyun.Api.DNS.DNS20150109.Request;

namespace aliyun_ddns
{
	class Program
	{
		private const  string APP_NAME = "aliyun-ddns";
		private static string _logFile = "";

		private static string GetLocalIP(string ipServer)
        {
			var ipRequest = (HttpWebRequest) WebRequest.Create(ipServer);
			ipRequest.AutomaticDecompression = DecompressionMethods.None | DecompressionMethods.GZip |
			                                 DecompressionMethods.Deflate;
			ipRequest.UserAgent = APP_NAME;
			string htmlSource;
			using (var ipResponse = ipRequest.GetResponse())
			{
				using (var responseStream = ipResponse.GetResponseStream())
				{
					using (var streamReader = new StreamReader(responseStream))
					{
						htmlSource = streamReader.ReadToEnd();
					}
				}
			}

			var ip = Regex.Match(htmlSource, @"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))", RegexOptions.IgnoreCase).Value;

			return ip;
        }

		private static void Printf(string msg)
        {
			var sTime = DateTime.Now.ToLocalTime().ToString();
			var sLog = string.Format("[{0}] {1}", sTime, msg);
			Console.WriteLine(sLog);
			File.AppendAllText(_logFile, sLog+"\r\n", Encoding.Default);
        }

		private static void Main(string[] args)
		{
			try
			{
				if (args.Length != 5)
                {
					Console.WriteLine("Usage: {0} <accessKeyId> <accessKeySecret> <domainName> <subDomainName> <getIpServer>", APP_NAME);
					return;
                }

				_logFile = string.Format(@"{0}{1}.log", AppDomain.CurrentDomain.BaseDirectory, APP_NAME);
				Printf("============================================================");

				// parse arguments
				var accessKeyId     = args[0].Trim(); // Access Key ID
				var accessKeySecret = args[1].Trim(); // Access Key Secret
				var domainName      = args[2].Trim(); // Domain Name
				var subDomainName   = args[3].Trim(); // Sub Domain Name
				var getIpServer     = args[4].Trim(); // Get IP Server
				Printf(string.Format("{0}.{1}:", subDomainName, domainName));
				
				// get local IP
				Printf(" -> Get Local IP ...");
				var localIP = GetLocalIP(getIpServer);
				Printf(string.Format(" -> Local IP: " + localIP));

				// get remote IP
				Printf(" -> Get Remote IP ...");
				var aliyunClient = new DefaultAliyunClient("http://dns.aliyuncs.com/", accessKeyId, accessKeySecret);
				var req          = new DescribeDomainRecordsRequest() { DomainName = domainName };
				var getResponse  = aliyunClient.Execute(req);
				if (getResponse.IsError)
                {
					Printf(" -> Get Remote IP ... Fail.");
					return;
                }
				var updateRecord = getResponse.DomainRecords.FirstOrDefault(rec => rec.RR == subDomainName && rec.Type == "A");
				if (updateRecord == null)
                {
					Printf(" -> Get Remote IP ... Fail.");
					return;
                }
				var remoteIP = updateRecord.Value;
				Printf(string.Format(" -> Remote IP: " + remoteIP));

				// update IP
				if (remoteIP != localIP)
				{
					Printf(" -> Update IP ...");
					var changeValueRequest = new UpdateDomainRecordRequest()
					{
						RecordId = updateRecord.RecordId,
						Value    = localIP,
						Type     = "A",
						RR       = subDomainName,
					};
					var updateResponse = aliyunClient.Execute(changeValueRequest);
					if (updateResponse.IsError)
					{
						Printf(" -> Update IP ... Fail.");
						return;
					}

					Printf(" -> Update IP ... Done.");
				}
				else
                {
					Printf(" -> IP No Change, Skipped.");
                }
			}
			catch (Exception ex)
			{
				Printf(ex.ToString());
			}
			
			Printf("============================================================");
		}
	}
}