using System;
using System.Collections.Generic;
using System.Text;
using SNAGITLib;
using System.Windows.Forms;
using System.Threading;

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

        private void SetParameters()
        {
            snagVideo.InputRegionOptions.SelectionMethod = snagRegionSelectionMethod.srsmFixed;
            snagVideo.InputRegionOptions.Height = Convert.ToInt16(SystemInformation.VirtualScreen.Height);
            snagVideo.InputRegionOptions.Width = Convert.ToInt16(SystemInformation.VirtualScreen.Width);
            snagVideo.InputRegionOptions.StartX = 0;
            snagVideo.InputRegionOptions.StartY = 0;
            snagVideo.InputRegionOptions.UseStartPosition = false;

            snagVideo.UseMagnifierWindow = false;
            snagVideo.IncludeCursor = false;
            snagVideo.EnablePreviewWindow = false;
            snagVideo.HideRecordingUI = true;

            snagVideo.ForegroundPreview = true;
            //snagVideo.MuteMic = true;
            snagVideo.ShowVideoCountdown = false;
            

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


        public void StartRecording()
        {
            SetParameters();
            UtcRecordStartTime = DateTime.UtcNow;
            var StartThread = new Thread(new ThreadStart(() => StartThreadWorker()));
            StartThread.Start();
        }

        public void StopRecording()
        {
            var StopThread = new Thread(new ThreadStart(() => StopThreadWorker()));
            StopThread.Start();
        }

        private void StopThreadWorker()
        {
            try
            {
                snagVideo.Stop();
            }
            catch(Exception ex)
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
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error starting recording: {0}", snagVideo.LastRecorderError), ex);
            }
        }
    }
}
