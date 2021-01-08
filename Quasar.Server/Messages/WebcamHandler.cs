using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Common.Video.Codecs;
using Quasar.Server.Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Quasar.Server.Messages
{
    /// <summary>
    /// Handles messages for the interaction with the remote webcam.
    /// </summary>
    public class WebcamHandler : MessageProcessorBase<Bitmap>, IDisposable
    {
        /// <summary>
        /// States if the client is currently streaming desktop frames.
        /// </summary>
        public bool IsStarted { get; set; }

        /// <summary>
        /// Used in lock statements to synchronize access to <see cref="_codec"/> between UI thread and thread pool.
        /// </summary>
        private readonly object _syncLock = new object();

        /// <summary>
        /// Used in lock statements to synchronize access to <see cref="LocalResolution"/> between UI thread and thread pool.
        /// </summary>
        private readonly object _sizeLock = new object();

        /// <summary>
        /// The local resolution, see <seealso cref="LocalResolution"/>.
        /// </summary>
        private Size _localResolution;

        /// <summary>
        /// The local resolution in width x height. It indicates to which resolution the received frame should be resized.
        /// </summary>
        /// <remarks>
        /// This property is thread-safe.
        /// </remarks>
        public Size LocalResolution
        {
            get
            {
                lock (_sizeLock)
                {
                    return _localResolution;
                }
            }
            set
            {
                lock (_sizeLock)
                {
                    _localResolution = value;
                }
            }
        }

        /// <summary>
        /// Represents the method that will handle display changes.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="value">All currently available displays.</param>
        public delegate void DisplaysChangedEventHandler(object sender, List<Tuple<int, string>> value);

        /// <summary>
        /// Raised when a display changed.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event DisplaysChangedEventHandler DisplaysChanged;

        /// <summary>
        /// Reports changed displays.
        /// </summary>
        /// <param name="value">All currently available displays.</param>
        private void OnDisplaysChanged(List<Tuple<int, string>> value)
        {
            SynchronizationContext.Post(val =>
            {
                var handler = DisplaysChanged;
                handler?.Invoke(this, (List<Tuple<int, string>>) val);
            }, value);
        }

        /// <summary>
        /// The client which is associated with this remote desktop handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// The video stream codec used to decode received frames.
        /// </summary>
        private UnsafeStreamCodec _codec;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebcamHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public WebcamHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetWebcamResponse || message is GetWebcamDevicesResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetWebcamResponse d:
                    Execute(sender, d);
                    break;
                case GetWebcamDevicesResponse m:
                    Execute(sender, m);
                    break;
            }
        }

        /// <summary>
        /// Begins receiving frames from the client using the specified quality and display.
        /// </summary>
        /// <param name="quality">The quality of the remote desktop frames.</param>
        /// <param name="display">The display to receive frames from.</param>
        public void BeginReceiveFrames(int quality, int display)
        {
            lock (_syncLock)
            {
                IsStarted = true;
                _codec?.Dispose();
                _codec = null;
                _client.Send(new GetWebcam { CreateNew = true, Quality = quality, DisplayIndex = display });
            }
        }

        /// <summary>
        /// Ends receiving frames from the client.
        /// </summary>
        public void EndReceiveFrames()
        {
            lock (_syncLock)
            {
                _client.Send(new GetWebcam { Destroy = true });
                IsStarted = false;
            }
        }

        /// <summary>
        /// Refreshes the available displays of the client.
        /// </summary>
        public void RefreshDisplays()
        {
            _client.Send(new GetWebcamDevices());
        }

        private void Execute(ISender client, GetWebcamResponse message)
        {
            lock (_syncLock)
            {
                if (!IsStarted)
                    return;

                if (_codec == null || _codec.ImageQuality != message.Quality || _codec.Monitor != message.Webcam || _codec.Resolution != message.Resolution)
                {
                    _codec?.Dispose();
                    _codec = new UnsafeStreamCodec(message.Quality, message.Webcam, message.Resolution);
                }

                using (MemoryStream ms = new MemoryStream(message.Image))
                {
                    // create deep copy & resize bitmap to local resolution
                    OnReport(new Bitmap(_codec.DecodeData(ms), LocalResolution));
                }
                
                message.Image = null;

                client.Send(new GetWebcam {Quality = message.Quality, DisplayIndex = message.Webcam });
            }
        }

        private void Execute(ISender client, GetWebcamDevicesResponse message)
        {
            OnDisplaysChanged(message.WebcamDevices);
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
                lock (_syncLock)
                {
                    _codec?.Dispose();
                    IsStarted = false;
                }
            }
        }
    }
}
