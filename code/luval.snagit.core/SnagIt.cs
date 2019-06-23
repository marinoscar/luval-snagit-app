using System;
using System.Collections.Generic;
using System.Text;
using SNAGITLib;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing;

namespace luval.snagit.core
{
    public class SnagIt
    {
        private SNAGITLib.VideoCaptureClass snagVideo;

        public DateTime UtcRecordStartTime { get; private set; }

        public SnagIt()
        {
            snagVideo = new SNAGITLib.VideoCaptureClass();
        }

        private void Initialize()
        {
            SetParameters();
            HookEvents();
        }

        private void SetParameters()
        {
            snagVideo.Input = SNAGITLib.snagVideoInput.sviRegion;
            snagVideo.InputRegionOptions.SelectionMethod = snagRegionSelectionMethod.srsmFixed;

            float dpiX, dpiY;

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = 96f / graphics.DpiX;
                dpiY = 96f / graphics.DpiY;
            };

            snagVideo.InputRegionOptions.Height = Convert.ToInt16(Screen.PrimaryScreen.WorkingArea.Height * dpiY);
            snagVideo.InputRegionOptions.Width = Convert.ToInt16(Screen.PrimaryScreen.WorkingArea.Width * dpiX);



            snagVideo.InputRegionOptions.StartX = 0;
            snagVideo.InputRegionOptions.StartY = 0;
            snagVideo.InputRegionOptions.UseStartPosition = true;

            snagVideo.UseMagnifierWindow = false;
            snagVideo.IncludeCursor = false;
            snagVideo.EnablePreviewWindow = false;
            snagVideo.HideRecordingUI = true;




            SNAGITLib.MP4FormatClass mp4Format = new SNAGITLib.MP4FormatClass()
            {
                DisableMOOVAtomOptimization = false
            };
            snagVideo.VideoFormat = mp4Format;

            snagVideo.OutputVideoFile.FileNamingMethod = snagOuputFileNamingMethod.sofnmAuto;
            snagVideo.OutputVideoFile.Directory = Environment.CurrentDirectory;
            snagVideo.OutputVideoFile.Filename = "SNAG-CAPTURE";
            snagVideo.OutputVideoFile.AutoFilePrefix = "SNAG-CAPTURE";
            snagVideo.OutputVideoFile.AutoNumPrefixDigits = 10;
        }

        private void HookEvents()
        {
            snagVideo.OnRecorderStateChange += SnagVideo_OnRecorderStateChange;
            snagVideo.OnSelectedRecordingRegion += SnagVideo_OnSelectedRecordingRegion;
        }

        private void SnagVideo_OnSelectedRecordingRegion(int xOffset, int yOffset, int rectWidth, int rectHeight)
        {
            Debug.WriteLine(string.Format("Region: {0},{1},{2}, {3}", xOffset, yOffset, rectHeight, rectWidth));
        }

        private void SnagVideo_OnRecorderStateChange(snagRecorderState newState)
        {
            Debug.WriteLine(string.Format("OnRecorderStateChange: {0}", newState));
        }

        public void StartRecording()
        {
            Initialize();
            SelectFullScreen();
            UtcRecordStartTime = DateTime.UtcNow;
            var StartThread = new Thread(new ThreadStart(() => StartThreadWorker()));
            StartThread.Start();
        }

        public void StopRecording()
        {
            var StopThread = new Thread(new ThreadStart(() => StopThreadWorker()));
            StopThread.Start();
        }

        public void SelectFullScreen()
        {
            var FullScreenThread = new Thread(new ThreadStart(() => SelectFullScreenThread()));
            FullScreenThread.Start();
        }

        public void SelectFullScreenThread()
        {
            Thread.Sleep(4000);
            try { SendKeys.Send("F"); } catch { }
            Debug.WriteLine("Send Key F");
        }

        private void StopThreadWorker()
        {
            try
            {
                snagVideo.Stop();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error stopping recording: {0}", snagVideo.LastRecorderError), ex);
            }
        }

        private void StartThreadWorker()
        {
            try
            {
                snagVideo.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error starting recording: {0}", snagVideo.LastRecorderError), ex);
            }
        }
    }
}
