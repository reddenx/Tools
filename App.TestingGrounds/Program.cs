﻿using System;
using App.TestingGrounds;
using System.Net;

namespace SMT.Utilities.Configuration
{
    public class Program
    {
        static void Main(string[] args)
        {
            //SpeechTests.Test();
            //TcpNetworkConnectionTests.Run();
            //FogBugzProxyTesting.Start();
            //SqlTesting.Run();
            //MouseInputTesting.Run();
            DynamicApiTesting.Run();
        }
    }
}