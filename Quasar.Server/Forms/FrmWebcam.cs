using Quasar.Common.Messages;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Networking;
using Quasar.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Quasar.Server.Forms
{
    public partial class FrmWebcam : Form
    {
        /// <summary>
        /// The client which can be used for the webcam.
        /// </summary>
        private readonly Client _connectClient;

        /// <summary>
        /// The message handler for handling the communication with the client.
        /// </summary>
        private readonly WebcamHandler _webcamHandler;

        /// <summary>
        /// Holds the opened webcam form for each client.
        /// </summary>
        private static readonly Dictionary<Client, FrmWebcam> OpenedForms = new Dictionary<Client, FrmWebcam>();

        /// <summary>
        /// Creates a new webcam form for the client or gets the current open form, if there exists one already.
        /// </summary>
        /// <param name="client">The client used for the webcam form.</param>
        /// <returns>
        /// Returns a new webcam form for the client if there is none currently open, otherwise creates a new one.
        /// </returns>
        public static FrmWebcam CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmWebcam r = new FrmWebcam(client);
            r.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, r);
            return r;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmWebcam"/> class using the given client.
        /// </summary>
        /// <param name="client">The client used for the webcam form.</param>
        public FrmWebcam(Client client)
        {
            _connectClient = client;
            _webcamHandler = new WebcamHandler(client);

            RegisterMessageHandler();
            InitializeComponent();
        }

        /// <summary>
        /// Called whenever a client disconnects.
        /// </summary>
        /// <param name="client">The client which disconnected.</param>
        /// <param name="connected">True if the client connected, false if disconnected</param>
        private void ClientDisconnected(Client client, bool connected)
        {
            if (!connected)
            {
                this.Invoke((MethodInvoker)this.Close);
            }
        }

        /// <summary>
        /// Registers the webcam message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            _connectClient.ClientState += ClientDisconnected;
            _webcamHandler.DisplaysChanged += DisplaysChanged;
            _webcamHandler.ProgressChanged += UpdateImage;
            MessageHandler.Register(_webcamHandler);
        }

        /// <summary>
        /// Unregisters the webcam message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(_webcamHandler);
            _webcamHandler.DisplaysChanged -= DisplaysChanged;
            _webcamHandler.ProgressChanged -= UpdateImage;
            _connectClient.ClientState -= ClientDisconnected;
        }

        /// <summary>
        /// Starts the webcam stream and begin to receive webcam frames.
        /// </summary>
        private void StartStream()
        {
            ToggleConfigurationControls(true);

            picWebcam.Start();
            // Subscribe to the new frame counter.
            picWebcam.SetFrameUpdatedEvent(frameCounter_FrameUpdated);

            this.ActiveControl = picWebcam;

            _webcamHandler.BeginReceiveFrames(barQuality.Value, cbWebcams.SelectedIndex);
        }

        /// <summary>
        /// Stops the webcam stream.
        /// </summary>
        private void StopStream()
        {
            ToggleConfigurationControls(false);

            picWebcam.Stop();
            // Unsubscribe from the frame counter. It will be re-created when starting again.
            picWebcam.UnsetFrameUpdatedEvent(frameCounter_FrameUpdated);

            this.ActiveControl = picWebcam;

            _webcamHandler.EndReceiveFrames();
        }

        /// <summary>
        /// Toggles the activatability of configuration controls in the status/configuration panel.
        /// </summary>
        /// <param name="started">When set to <code>true</code> the configuration controls get enabled, otherwise they get disabled.</param>
        private void ToggleConfigurationControls(bool started)
        {
            btnStart.Enabled = !started;
            btnStop.Enabled = started;
            barQuality.Enabled = !started;
            cbWebcams.Enabled = !started;
        }

        /// <summary>
        /// Toggles the visibility of the status/configuration panel.
        /// </summary>
        /// <param name="visible">Decides if the panel should be visible.</param>
        private void TogglePanelVisibility(bool visible)
        {
            panelTop.Visible = visible;
            btnShow.Visible = !visible;
            this.ActiveControl = picWebcam;
        }

        /// <summary>
        /// Called whenever the displays changed.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="displays">The currently available displays.</param>
        private void DisplaysChanged(object sender, List<Tuple<int, string>> displays)
        {
            cbWebcams.Items.Clear();
            foreach (Tuple<int, string> device in displays)
            {
                cbWebcams.Items.Add(device.Item2);
            }
            cbWebcams.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates the current webcam image by drawing it to the webcam picturebox.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="bmp">The new webcam image to draw.</param>
        private void UpdateImage(object sender, Bitmap bmp)
        {
            picWebcam.UpdateImage(bmp, false);
        }

        private void FrmWebcam_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Webcam", _connectClient);

            OnResize(EventArgs.Empty); // trigger resize event to align controls 

            _webcamHandler.RefreshDisplays();
        }

        /// <summary>
        /// Updates the title with the current frames per second.
        /// </summary>
        /// <param name="e">The new frames per second.</param>
        private void frameCounter_FrameUpdated(FrameUpdatedEventArgs e)
        {
            this.Text = string.Format("{0} - FPS: {1}", WindowHelper.GetWindowTitle("Webcam", _connectClient), e.CurrentFramesPerSecond.ToString("0.00"));
        }

        private void FrmWebcam_FormClosing(object sender, FormClosingEventArgs e)
        {
            // all cleanup logic goes here
            if (_webcamHandler.IsStarted) StopStream();
            UnregisterMessageHandler();
            _webcamHandler.Dispose();
            picWebcam.Image?.Dispose();
        }

        private void FrmWebcam_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                return;

            _webcamHandler.LocalResolution = picWebcam.Size;
            panelTop.Left = (this.Width - panelTop.Width) / 2;
            btnShow.Left = (this.Width - btnShow.Width) / 2;
            btnHide.Left = (panelTop.Width - btnHide.Width);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cbWebcams.Items.Count == 0)
            {
                MessageBox.Show("No webcam detected.\nPlease wait till the client sends a list with available displays.",
                    "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartStream();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopStream();
        }

        #region Webcam Configuration

        private void barQuality_Scroll(object sender, EventArgs e)
        {
            int value = barQuality.Value;
            lblQualityShow.Text = value.ToString();

            if (value < 25)
                lblQualityShow.Text += " (low)";
            else if (value >= 85)
                lblQualityShow.Text += " (best)";
            else if (value >= 75)
                lblQualityShow.Text += " (high)";
            else if (value >= 25)
                lblQualityShow.Text += " (mid)";

            this.ActiveControl = picWebcam;
        }

        #endregion

        private void btnHide_Click(object sender, EventArgs e)
        {
            TogglePanelVisibility(false);
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            TogglePanelVisibility(true);
        }
    }
}
