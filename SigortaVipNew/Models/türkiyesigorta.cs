using System;
using System.IO;
using System.Net;

namespace SigortaVip.Models
{
    internal class türkiyesigorta
	{
		public bool getcids(out HttpWebResponse response)
		{
			response = null;

			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pusula.turkiyesigorta.com.tr/modul/uretim/hizliTrafikTeklifi.seam");

				request.KeepAlive = true;
				request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
				request.Headers.Add("sec-ch-ua-mobile", @"?0");
				request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36";
				request.Headers.Add("sec-ch-ua-platform", @"""Windows""");
				request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
				request.Accept = "*/*";
				request.Headers.Add("Origin", @"https://pusula.turkiyesigorta.com.tr");
				request.Headers.Add("Sec-Fetch-Site", @"same-origin");
				request.Headers.Add("Sec-Fetch-Mode", @"cors");
				request.Headers.Add("Sec-Fetch-Dest", @"empty");
				request.Referer = "https://pusula.turkiyesigorta.com.tr/modul/uretim/hizliTrafikTeklifi.seam";
				request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
				request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
				request.Headers.Set(HttpRequestHeader.Cookie, @"JSESSIONID=00003VulkIVs2u3zE2zR7BtZS_m:1f6uodshs; ckpusula=!/3PR57RMMpi+adQfL46eSO6U/ABavpJIHzuTlFICYpDYAzltHmtfb5seyisejlwBADk6g6VgnyZsQw==");

				request.Method = "POST";
				request.ServicePoint.Expect100Continue = false;

				string body = @"AJAXREQUEST=_viewRoot&form=form&form%3Adecorate%3AskId=5600361&form%3AdecorateAd%3AskAd=KAYALIK%20S%C4%B0GORTA%20ARACILIK%20H%C4%B0ZME%20TLER%C4%B0%20LTD.%C5%9ET%C4%B0.%20&form%3Aj_id220%3AselectKimlikNoTipi=KIMLIK_NO&form%3Aj_id220%3AtxtKimlikNo=34529130412&form%3Aj_id220%3AdogumTarihiInputDate=&form%3Aj_id220%3AdogumTarihiInputCurrentDate=05%2F2022&form%3Aj_id242%3Aj_id250=10027996637&form%3Aj_id242%3Aj_id251=HARUN%20DO%C4%9EAN&javax.faces.ViewState=j_id9&form%3AdecorateDevamBtn%3Aj_id394=form%3AdecorateDevamBtn%3Aj_id394&AJAX%3AEVENTS_COUNT=1&";
				byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
				request.ContentLength = postBytes.Length;
				Stream stream = request.GetRequestStream();
				stream.Write(postBytes, 0, postBytes.Length);
				stream.Close();

				response = (HttpWebResponse)request.GetResponse();
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
				else return false;
			}
			catch (Exception)
			{
				if (response != null) response.Close();
				return false;
			}

			return true;
		}
	}
}
