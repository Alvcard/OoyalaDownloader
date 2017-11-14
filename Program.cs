using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ooyala.API;
using System.Net;
using System.IO;
using System.Web;

namespace OoyalaDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Download Directory
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Downloads"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Downloads");
                Console.WriteLine("Download Directory Created");
            }
            // Create Progress file
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "progress.csv"))
            {
                using (StreamWriter file = File.CreateText(AppDomain.CurrentDomain.BaseDirectory + "progress.csv"))
                {
                    file.WriteLine("embed_code,original_file_name,name,description");
                }
                Console.WriteLine("CSV Created");
            }
            
            // Connect to OoyalaAPI
            OoyalaAPI api = new OoyalaAPI("<API Key>", "<Secret Key>");
            Dictionary<String, String> parameters = new Dictionary<String, String>
            {
                { "limit", "5" },
                { "where", "asset_type = 'video'" }
            };

            bool NextPage = true;

            while (NextPage)
            {
                string OoyalaResponce = string.Empty;
                try
                {
                    OoyalaResponce = JsonConvert.SerializeObject(api.get("assets", parameters));
                    if (JObject.Parse(OoyalaResponce) == null)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(DateTime.Now + " waiting 60 seonds");
                    System.Threading.Thread.Sleep(60000);
                    OoyalaResponce = JsonConvert.SerializeObject(api.get("assets", parameters));
                }
                parameters.Clear();

                JObject o = JObject.Parse(OoyalaResponce);
                foreach (var item in o["items"])
                {
                    string filename = item["original_file_name"].ToString();
                    filename = filename.Replace(@"\", "");

                    string videoDownloadLinks = string.Empty;
                    try
                    {
                        videoDownloadLinks = JsonConvert.SerializeObject(api.get("assets/" + item["embed_code"] + "/streams", parameters));
                        if (JArray.Parse(videoDownloadLinks) == null)
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(DateTime.Now + " waiting 60 seonds");
                        System.Threading.Thread.Sleep(60000);
                        videoDownloadLinks = JsonConvert.SerializeObject(api.get("assets/" + item["embed_code"] + "/streams", parameters));
                    }

                    JArray videoDownloadResponce = JArray.Parse(videoDownloadLinks);
                    foreach (var videoSource in videoDownloadResponce)
                    {
                        if (videoSource["is_source"].ToString() == "True")
                        {
                            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Downloads\\" + filename))
                            {
                                Console.WriteLine("File Existed " + filename);
                            }
                            else
                            {
                                OoyalaFileDownload(videoSource["url"].ToString(), filename);
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "progress.csv", true))
                                {
                                    file.WriteLine(item["embed_code"] + "," + item["original_file_name"] + "," + item["name"] + "," + item["description"]);
                                }
                            }
                        }
                    }
                }

                if (o["next_page"] != null)
                {
                    NameValueCollection qscoll = HttpUtility.ParseQueryString(cleanNextpage(o["next_page"].ToString()));

                    foreach (var k in qscoll.AllKeys)
                    {
                        parameters.Add(k, qscoll[k]);
                    }
                    NextPage = true;
                }
                else
                {
                    NextPage = false;
                }
            }
        }

        static void OoyalaFileDownload(string url, string VideoTitle)
        {
            Console.WriteLine(DateTime.Now + " Download Started for: " + VideoTitle);
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, AppDomain.CurrentDomain.BaseDirectory + "Downloads\\" + VideoTitle);
                    Console.WriteLine(VideoTitle + " Downloaded.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(VideoTitle + " Failed. " + ex);
            }
        }

        static string cleanNextpage(string nextpage)
        {
            nextpage = nextpage.Substring(11, nextpage.Length - 11);
            return nextpage;
        }
    }
}
