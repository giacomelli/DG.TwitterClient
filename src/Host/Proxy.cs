using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;

namespace DG.TwitterClient.Host
{
    public class Proxy : IWebProxy
    {
        private Uri m_proxyAddress;

        public Proxy()
        {
            m_proxyAddress = new Uri(ConfigurationManager.AppSettings["proxyAddress"]);
            Credentials = new NetworkCredential(ConfigurationManager.AppSettings["proxyUserName"], ConfigurationManager.AppSettings["proxyPassword"]);
        }

        #region IWebProxy Members

        public ICredentials Credentials
        {
            get;            
            set;
        }

        public Uri GetProxy(Uri destination)
        {
            return m_proxyAddress;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        #endregion
    }
}
