﻿using Quasar.Common.Messages;
using Quasar.Common.Helpers;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Networking;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Quasar.Server.Properties;

namespace Quasar.Server.Forms
{
    public partial class FrmAudio : Form
    {
        /// <summary>
        /// The client which can be used for the remote audio.
        /// </summary>
        private readonly Client _connectClient;

        /// <summary>
        /// The message handler for handling the communication with the client.
        /// </summary>
        private readonly AudioHandler _remoteAudioHandler;

        /// <summary>
        /// Holds the opened remote audio form for each client.
        /// </summary>
        private static readonly Dictionary<Client, FrmAudio> OpenedForms = new Dictionary<Client, FrmAudio>();

        /// <summary>
        /// Creates a new remote audio form for the client or gets the current open form, if there exists one already.
        /// </summary>
        /// <param name="client">The client used for the remote audio form.</param>
        /// <returns>
        /// Returns a new remote audio form for the client if there is none currently open, otherwise creates a new one.
        /// </returns>
        public static FrmAudio CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmAudio r = new FrmAudio(client);
            r.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, r);
            return r;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmAudio"/> class using the given client.
        /// </summary>
        /// <param name="client">The client used for the remote audio form.</param>
        public FrmAudio(Client client)
        {
            _connectClient = client;
            _remoteAudioHandler = new AudioHandler(client);

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
        /// Registers the remote audio message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            _connectClient.ClientState += ClientDisconnected;
            _remoteAudioHandler.MicrophoneChanged += MicrophoneChanged;
            MessageHandler.Register(_remoteAudioHandler);
        }

        /// <summary>
        /// Unregisters the remote audio message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(_remoteAudioHandler);
            _remoteAudioHandler.MicrophoneChanged -= MicrophoneChanged;
            _connectClient.ClientState -= ClientDisconnected;
        }

        /// <summary>
        /// Starts the remote audio stream and begin to receive audio frames.
        /// </summary>
        private void StartStream()
        {
            ToggleConfigurationControls(true);
            _remoteAudioHandler.BeginReceiveAudio(cbDevices.SelectedIndex);
        }

        /// <summary>
        /// Stops the remote audio stream.
        /// </summary>
        private void StopStream()
        {
            ToggleConfigurationControls(false);

            _remoteAudioHandler.EndReceiveAudio(cbDevices.SelectedIndex);
        }

        /// <summary>
        /// Toggles the activatability of configuration controls in the status/configuration panel.
        /// </summary>
        /// <param name="started">When set to <code>true</code> the configuration controls get enabled, otherwise they get disabled.</param>
        private void ToggleConfigurationControls(bool started)
        {
            barQuality.Enabled = !started;
            cbDevices.Enabled = !started;
        }

        /// <summary>
        /// Called whenever the remote microphones changed.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="devices">The currently available microphone devices.</param>
        private void MicrophoneChanged(object sender, List<Tuple<int, string>> devices)
        {
            cbDevices.Items.Clear();
            foreach (Tuple<int, string> device in devices)
            {
                cbDevices.Items.Add(device.Item2);
            }
            cbDevices.SelectedIndex = 0;
        }

        private void FrmRemoteAudio_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Audio", _connectClient);

            _remoteAudioHandler.RefreshMicrophones();
        }

        private void FrmAudio_FormClosing(object sender, FormClosingEventArgs e)
        {
            // all cleanup logic goes here
            if (_remoteAudioHandler.IsStarted) StopStream();
            UnregisterMessageHandler();
            _remoteAudioHandler.Dispose();
        }

        private void FrmAudio_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                return;
        }

        #region  Audio Configuration

        private void barQuality_Scroll(object sender, EventArgs e)
        {
            int value = barQuality.Value;
            if (value == 1)
            {
                lblQualityShow.Text = "1000";
                _remoteAudioHandler._bitrate = 1000;
            }
            else if (value == 2)
            {
                lblQualityShow.Text = "2000";
                _remoteAudioHandler._bitrate = 2000;
            }
            else if (value == 3)
            {
                lblQualityShow.Text = "4000";
                _remoteAudioHandler._bitrate = 4000;
            }
            else if (value == 4)
            {
                lblQualityShow.Text = "8000";
                _remoteAudioHandler._bitrate = 8000;
            }
            else if (value == 5)
            {
                lblQualityShow.Text = "11025";
                _remoteAudioHandler._bitrate = 11025;
            }
            else if (value == 6)
            {
                lblQualityShow.Text = "22050";
                _remoteAudioHandler._bitrate = 22050;
            }
            else if (value == 7)
            {
                lblQualityShow.Text = "32000";
                _remoteAudioHandler._bitrate = 32000;
            }
            else if (value == 8)
            {
                lblQualityShow.Text = "44100";
                _remoteAudioHandler._bitrate = 44100;
            }
            else if (value == 9)
            {
                lblQualityShow.Text = "48000";
                _remoteAudioHandler._bitrate = 48000;
            }
            else if (value == 10)
            {
                lblQualityShow.Text = "64000";
                _remoteAudioHandler._bitrate = 64000;
            }
            else if (value == 11)
            {
                lblQualityShow.Text = "88200";
                _remoteAudioHandler._bitrate = 88200;
            }
            else if (value == 12)
            {
                lblQualityShow.Text = "96000";
                _remoteAudioHandler._bitrate = 96000;
            }

            if (value < 8)
                lblQualityShow.Text += " (low)";
            else if (value == 8)
                lblQualityShow.Text += " (best)";
            else if (value >= 9)
                lblQualityShow.Text += " (high)";
        }
        #endregion

        private void pbStart_Click(object sender, EventArgs e)
        {
            if (cbDevices.Items.Count == 0)
            {
                MessageBox.Show("No microphone detected.\nPlease wait till the client sends a list with micrphones.",
                    "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            StartStream();
            pbStart.Image = Resources.audio_start_disabled;
            pbStart.Enabled = false;
            pbStop.Image = Resources.audio_stop;
            pbStop.Enabled = true;
        }

        private void pbStop_Click(object sender, EventArgs e)
        {
            StopStream();
            pbStart.Image = Resources.audio_start;
            pbStart.Enabled = true;
            pbStop.Image = Resources.audio_stop_disabled;
            pbStop.Enabled = false;
        }
    }
}
