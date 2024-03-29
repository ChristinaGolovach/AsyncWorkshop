﻿using System.Threading.Tasks;
using SiteCopy.LinkRestrictions;
using SiteCopy;
using SiteCopy.Services.Factories;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Mirror mirror = new Mirror();

            string uri = "https://social.msdn.microsoft.com/Forums/en-US/e7279f10-fad4-4a9f-b7e2-c2e1d52c86aa/get-page-source-with-httpclient?forum=winappswithcsharp";
            string savePath = @"D:\EPAM\Laba\Days\Day10(Async)\Homework\TestHomwork";

            //TODO move this in Inject, but how pass argument to ctor if it will be known after user will enter the path
            IStorageFactory storageFactory = new FileStorageFactory(savePath);

            var result = mirror.GetSiteCopy(uri, storageFactory, 1, PathLinkRestriction.CurrentDomen, "png, gif, js");
            Task.WaitAll(result);

        }
    }
}
