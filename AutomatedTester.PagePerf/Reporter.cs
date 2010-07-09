﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace AutomatedTester.PagePerf
{
    public static class Reporter
    {
        private static XmlNode node;
        private static Dictionary<string,object> breakDown = new Dictionary<string, object>();

        public static void Process(string harContents)
        {
            node = JsonToXml(harContents);
            XmlNodeList responses = node.SelectNodes("//entries/response");            
            breakDown["responsecount"] = responses.Count;
            breakDown["loadtimes"] = GetLoadTimes();

            breakDown["totalSizeOfPage"] = TotalSizeOfPage();

            // Writing HAR file just for debugging 
            File.WriteAllText(@"c:\development\har.xml", node.OuterXml.ToString());
        }

        private static int TotalSizeOfPage()
        {
            XmlNodeList bodysizes = node.SelectNodes("//entries/response/bodySize");
            int totalSize = 0;
            foreach (XmlNode size in bodysizes)
            {
                int itemSize = (Int32.Parse(size.InnerText));
                if (itemSize > 0)
                {
                    totalSize += itemSize;
                }
            }

            XmlNodeList headersizes = node.SelectNodes("//entries/response/headersSize");
            foreach (XmlNode size in headersizes)
            {
                int itemSize = (Int32.Parse(size.InnerText));
                if (itemSize > 0)
                {
                    totalSize += itemSize;
                }
            }

            return totalSize/1024;
        }

        private static double GetLoadTimes()
        {
            XmlNode time = node.SelectSingleNode("//onLoad");
            double loadTimes = Int32.Parse(time.InnerText);

            return loadTimes/1000;
        }

        private static XmlNode JsonToXml(string harContents)
        {
            return JsonConvert.DeserializeXmlNode(harContents);
        }
    }
}