﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Utils;

namespace AudioStreaming
{
    public class NoiseEliminator
    {
        byte[] _capture;
        int _counter;

        public NoiseEliminator(byte[] inputCapture)
        {
            _counter = 0;
            _capture = inputCapture;
        }

        public byte[] EliminateNoise()
        {
            string filename = this.ProcessNoise();
            FileStream fs = null;
            bool retry = true;
            int maxRetry = 5, tryCount = 0;
            while (retry && tryCount < maxRetry)
            {
                try
                {
                    fs = new FileStream(filename, FileMode.Open);
                    retry = false;
                }
                catch (Exception ex)
                {
                    Tools.Instance.Logger.LogError(ex.ToString());
                    retry = true;
                    Thread.Sleep(100);
                }
                tryCount++;
            }
            byte[] clear = null;
            if (fs != null)
            {
                clear = new byte[fs.Length];
                fs.Read(clear, 0, clear.Length);
                fs.Close();
                fs.Dispose();
            }
            try
            {
                File.Delete(filename);
            }
            catch { }
            return clear != null ? clear : this._capture;
        }

        string ProcessNoise()
        {
            bool exists = true;
            string filename = "";
            while (exists)
            {
                if (File.Exists("noisy" + _counter.ToString() + ".wav"))
                {
                    exists = true;
                    _counter++;
                }
                else
                {
                    filename = "noisy" + _counter.ToString() + ".wav";
                    exists = false;
                }
            }
            FileStream fs = new FileStream(filename, FileMode.CreateNew);
            fs.Write(_capture, 0, _capture.Length);
            fs.Close();
            fs.Dispose();

            string soxDir = Directory.GetCurrentDirectory() + "\\sox\\";

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = soxDir + "sox.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = filename + " −n trim 0 1 noiseprof | play " + filename + " noisered";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch(Exception ex)
            {
                Tools.Instance.Logger.LogError(ex.ToString());
            }
           
            return filename;
        }
    }
}
