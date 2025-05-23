﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoArm.Models;

namespace AutoArm
{
    public enum AutoArmState
    {
        Disabled,
        WaitingForStart,
        WaitingForDelay,
        Verified,
        NotVerified,
    }

    public enum AutoArmErrorState
    {
        AlreadyEnabled,
        ButtonNotVisible,
        CaptureNotEnabled,
    }

    internal class LynxInterface : IDisposable
    {
        public event EventHandler<AutoArmState>? AutoArmStateChanged;
        public event EventHandler<AutoArmErrorState>? AutoArmWarning;

        private Main mainUi;
        private UdpListener? udpListener;
        private AutoArmState autoArmState = AutoArmState.WaitingForStart;

        public LynxInterface(Main mainUi)
        {
            this.mainUi = mainUi;
            // Start listening
            ListenUdpScoreboard();
        }

        public void Dispose()
        {
            if (udpListener != null)
            {
                udpListener.Dispose();
                udpListener = null;
            }
            ArmStateChanged(AutoArmState.Disabled);
        }

        private void ArmStateChanged(AutoArmState newState)
        {
            autoArmState = newState;
            AutoArmStateChanged?.Invoke(this, newState);
        }

        #region SendKeys

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        private void SendKeysToLynx()
        {
            // Do the thing
            Process? p = Process.GetProcessesByName("lynx").FirstOrDefault();
            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);
                Debug.Print("Sending keys");
                SendKeys.SendWait("%t");
            }
        }

        private ButtonAnalysis GetToggleState()
        {
            var buttonWatcher = mainUi.lynxButtonWatcher;
            if (buttonWatcher == null)
            {
                Debug.WriteLine("Button watcher not initialized.");
                return ButtonAnalysis.NOT_GREY;
            }
            var (analysis, bitmap) = buttonWatcher.AnalyzeScreen();
            return analysis;
        }

        private void ToggleAndVerify()
        {
            // Check current state before sending keys. If capture is already enabled, don't send toggle
            var toggleState = GetToggleState();
            if (toggleState == ButtonAnalysis.OK)
            {
                // Capture is already enabled. Not sending toggle
                AutoArmWarning?.Invoke(this, AutoArmErrorState.AlreadyEnabled);
                ArmStateChanged(AutoArmState.Verified);
                return;
            }
            else if (toggleState == ButtonAnalysis.NOT_GREY)
            {
                AutoArmWarning?.Invoke(this, AutoArmErrorState.ButtonNotVisible);
                ArmStateChanged(AutoArmState.NotVerified);
            }

            // Send keys
            SendKeysToLynx();

            // Wait for a bit to let the keys be sent
            Thread.Sleep(1000);

            // Check the state again
            toggleState = GetToggleState();

            if (toggleState == ButtonAnalysis.RED)
            {
                AutoArmWarning?.Invoke(this, AutoArmErrorState.CaptureNotEnabled);
                ArmStateChanged(AutoArmState.NotVerified);
            }
            else if (toggleState == ButtonAnalysis.NOT_GREY)
            {
                AutoArmWarning?.Invoke(this, AutoArmErrorState.ButtonNotVisible);
                ArmStateChanged(AutoArmState.NotVerified);
            }
            else
            {
                ArmStateChanged(AutoArmState.Verified);
            }
        }

        #endregion SendKeys

        #region ScoreboardListeners

        /**
         * This listens for the normal scoreboard data. In normal circumstances, it is passed along to the serial port.
         */
        private void ListenUdpScoreboard()
        {
            Action<string> onDataReceived = (data) => HandleScoreboardData(data);

            udpListener = new UdpListener(mainUi.udpPort, onDataReceived);
            udpListener.Start();
        }

        private void HandleScoreboardData(string data)
        {
            // Process data
            Debug.WriteLine("Received scoreboard data: " + data);
            // Parse data to ScoreboardData object
            ScoreboardData? scoreboardData =
                Newtonsoft.Json.JsonConvert.DeserializeObject<ScoreboardData>(data);
            if (scoreboardData == null)
            {
                Debug.WriteLine("Failed to parse scoreboard data.");
                return;
            }
            if (scoreboardData.TimeArmed != null)
            {
                if (scoreboardData.TimeArmed == true)
                {
                    mainUi.UpdateLynxStatus("Event armed");
                }
                else
                {
                    mainUi.UpdateLynxStatus("Event not armed");
                }
                ArmStateChanged(AutoArmState.WaitingForStart);
            }
            else if (scoreboardData.timeRunning != null)
            {
                var timeSeconds = ConvertTimeToSeconds(scoreboardData.timeRunning);
                Debug.Print("Time running: " + timeSeconds);
                mainUi.UpdateLynxStatus("Running: " + scoreboardData.timeRunning);
                if (autoArmState == AutoArmState.WaitingForStart)
                {
                    // Waiting for time threshold
                    ArmStateChanged(AutoArmState.WaitingForDelay);
                }
                else if (
                    autoArmState == AutoArmState.WaitingForDelay
                    && timeSeconds >= mainUi.delay
                )
                {
                    // Time threshold reached
                    ToggleAndVerify();
                }
            }
            else
            {
                mainUi.UpdateLynxStatus("Unknown");
                ArmStateChanged(AutoArmState.WaitingForStart);
            }
        }

        #endregion ScoreboardListeners

        private double ConvertTimeToSeconds(string time)
        {
            string[] parts = time.Split(':');
            if (parts.Length != 2)
            {
                return double.Parse(time);
            }
            var minutes = double.Parse(parts[0]);
            var seconds = double.Parse(parts[1]);
            return minutes * 60 + seconds;
        }
    }
}
