using Org.BouncyCastle.Utilities.Net;
using Quasar.Server.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Quasar.Server.Utilities
{
    public static class NoIpUpdater
    {
        private static bool _running;

        public static void Start()
        {
            if (_running) return;
            Thread updateThread = new Thread(BackgroundUpdater) {IsBackground = true};
            updateThread.Start();
        }

        private static void BackgroundUpdater()
        {
            _running = true;
            string IPv4 = null;
            string IPv6 = null;
            string URL = "";
            string IPAddresses = "";
            while (Settings.EnableNoIPUpdater)
            {
                try
                {
                    IPv4 = TryGetWanIp();
                    IPv6 = TryGetWanIp(true);
                    if(IPv4 == null && IPv6 == null)
                    {
                        URL = string.Format("https://dynupdate.no-ip.com/nic/update?hostname={0}", Settings.NoIPHost);
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(IPv4) && string.IsNullOrEmpty(IPv6))
                        {
                            IPAddresses = IPv4;
                        }
                        else if(string.IsNullOrEmpty(IPv4) && !string.IsNullOrEmpty(IPv6))
                        {
                            IPAddresses = IPv6;
                        }
                        else
                        {
                            IPAddresses = IPv4+","+IPv6;
                        }
                        URL = string.Format("https://dynupdate.no-ip.com/nic/update?hostname={0}&myip={1}", Settings.NoIPHost, IPAddresses);
                    }

                    Debug.WriteLine(URL);

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
                    request.Proxy = null;
                    request.UserAgent = string.Format("Quasar No-Ip Updater/2.0 {0}", Settings.NoIPUsername);
                    request.Timeout = 10000;
                    request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", Settings.NoIPUsername, Settings.NoIPPassword)))));
                    request.Method = "GET";

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        //gathered
                    }
                }
                catch(Exception)
                {
                    //empty exception
                }

                Thread.Sleep(TimeSpan.FromMinutes(10));
            }
            _running = false;
        }

        /// <summary>
        /// Tries to retrieves our ownWAN IP.
        /// </summary>
        /// <returns>The WAN IP as string if successful, otherwise <c>null</c>.</returns>
        /// <param name="IPv6">Retrieve public IPv6</param>
        private static string TryGetWanIp(bool IPv6 = false)
        {
            string wanIp = "";

            try
            {
                string urlExtra = "";
                if (IPv6) urlExtra = "6";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api" + urlExtra + ".ipify.org/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0";
                request.Proxy = null;
                request.Timeout = 5000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            wanIp = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
                //empty exception
            }

            return wanIp;
        }
    }
}
