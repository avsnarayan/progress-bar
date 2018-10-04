﻿using Konsole;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickTest
{
    class Program
    {
        private static string[] Files = new[] { "12520437.cpx", "12520850.cpx", "@AudioToastIcon.png", "@EnrollmentToastIcon.png", "@VpnToastIcon.png", "@WirelessDisplayToast.png", "aadauthhelper.dll", "aadtb.dll", "aadWamExtension.dll", "AboveLockAppHost.dll", "accessibilitycpl.dll", "accountaccessor.dll", "AccountsRt.dll", "AcGenral.dll", "AcLayers.dll", "acledit.dll", "aclui.dll", "acppage.dll", "AcSpecfc.dll", "ActionCenter.dll", "ActionCenterCPL.dll", "ActivationClient.dll", "ActivationManager.dll", "activeds.dll", "activeds.tlb", "ActiveSyncProvider.dll", "actxprxy.dll", "AcWinRT.dll", "acwow64.dll", "AcXtrnal.dll", "adalsql.dll", "AdaptiveCards.dll", "AddressParser.dll", "AdmTmpl.dll", "adprovider.dll", "adrclient.dll", "adsldp.dll", "adsldpc.dll", "adsmsext.dll", "adsnt.dll" };

        static bool stopClock = false;
        // demo showing mixing and matching writing using con = new Writer(); with calling Console directly e.g. Console.WriteLine("");
        // it is only safe to call Console directly when all background threads have finished accessing Console using a ThreadSafeWriter.
        // so you can mix and match while background threads are not running,
        // and when threads start, you MUST only write to the console via a ThreadSafeWriter
        static void Main(string[] args)
        {
            Console.Clear();
            var clock = Task.Run(()=> RunClock());
            ProcessFakeFiles();
            stopClock = true;
            clock.Wait();
            Console.ResetColor();
        }

        static void RunClock()
        {
            var con = new ThreadsafeWriter();
            var x = con.Width - 10;
            // not the best way to signal to thread to stop, will do for demo
            while(!stopClock)
            {
                con.PrintAt(x, 0, DateTime.Now.ToString("HH:MM:ss"));
                Thread.Sleep(1000);
            }
        }

        static void ProcessFakeFiles()
        {
            var con = new ThreadsafeWriter();
            // Screen will have been cleared so we are at the top
            // write one empty line so that the first progress bar does not
            // overlap with the demo clock.
            con.WriteLine("");
            int numDirs = 15;            
            con.ForegroundColor = ConsoleColor.White;
            var r = new Random();
            var dirs = Files.Take(numDirs).Select(f => f.Split(new[] { '.' })[0]);

            var tasks = new List<Task>();
            var bars = new List<ProgressBar>();
            int cnt = dirs.Count();
            foreach (var d in dirs)
            {
                var dir = d;
                var files = Files.Take(r.Next(30) + 10).ToArray();
                var bar = new ProgressBar(files.Count());
                bars.Add(bar);
                bar.Refresh(0, d);
                tasks.Add(new Task(() => ProcessFakeFiles(d, files, bar)));
            }
            
            con.WriteLine("Press any key to start");
            Console.ReadKey(true);
            con.WriteLine("processing...       ");
            foreach (var t in tasks) t.Start();
            Task.WaitAll(tasks.ToArray());
            con.WriteLine(ConsoleColor.Yellow, "finished.           ");
        }

        public static void ProcessFakeFiles(string directory, string[] files, ProgressBar bar)
        {
            foreach (var file in files)
            {
                bar.Next(file);
                Thread.Sleep(150);
            }
        }
    }
}

