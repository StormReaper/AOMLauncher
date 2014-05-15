using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AOMLauncher.Properties;
using Path = System.IO.Path;

namespace AOMLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool AutoLaunch = false;
        private DispatcherTimer AutoLaunchTimer = new DispatcherTimer();
        private const string _savegame = "savegame";
        private const string _aomFilename = "aomx.exe";
        private const string _launcherFilename = "launcher.exe";
        private string SteamPath = "";
        private string AOMEERootPath = "";
        private string AOMEESavedGamesPath = "";
        private string AOMFilenamePath = "";
        private string LauncherFilenamePath = "";
        private int secondsLeft = 0;
        private static long v = 76561197960265728;
        private long SteamId = 0;
        private List<string> launchArgs = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            LblStartTime.Content = "";
            LblErrors.Content = "";
            GetPathVariables();
            CbDelay.SelectedIndex = Settings.Default.Delay;
            AutoLaunch = Settings.Default.AutoLaunch;

            if (Settings.Default.NewsPopup)
            {
                var news = new NewsWindow();
                news.Topmost = true;
                news.Show();
            }
        }

        #region Timer Code

        private void InitTimer()
        {
            AutoLaunchTimer = new DispatcherTimer();
            AutoLaunchTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            AutoLaunchTimer.Interval = new TimeSpan(0, 0, 1);
            secondsLeft = Convert.ToInt32(CbDelay.SelectionBoxItem);
            if (AutoLaunch)
            {
                AutoLaunchTimer.Start();
            }
            
        }

        private void ResetTimer()
        {
            AutoLaunchTimer.Stop();
            secondsLeft = Convert.ToInt32(CbDelay.SelectionBoxItem);
            LblStartTime.Content = "";
            if (AutoLaunch)
            {
                AutoLaunchTimer.Start();
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (AutoLaunch && secondsLeft != 0)
            {
                secondsLeft--;
                LblStartTime.Content = String.Format("Launching in {0} seconds...", secondsLeft);
            }
            if (AutoLaunch && secondsLeft == 0)
            {
                LblStartTime.Content = "Launching! :)";
                AutoLaunchTimer.Stop();
                LaunchGame();
            }
        }

        #endregion

        #region Button Events
        private void btnAOE_Click(object sender, RoutedEventArgs e)
        {
            OpenWebPage(Settings.Default.AgeofEmpires);
        }

        private void btnSanctuary_Click(object sender, RoutedEventArgs e)
        {
            OpenWebPage(Settings.Default.RtsSanctuary);
        }

        private void btnHeaven_Click(object sender, RoutedEventArgs e)
        {
            OpenWebPage(Settings.Default.AOMHeaven);
        }

        private void btnSteam_Click(object sender, RoutedEventArgs e)
        {
            OpenWebPage(Settings.Default.SteamDiscussions);
        }

        private void btnWorkshop_Click(object sender, RoutedEventArgs e)
        {
            OpenWebPage(Settings.Default.SteamWorkshop);
        }

        private void btnLeaderboards_Click(object sender, RoutedEventArgs e)
        {
            OpenWebPage(Settings.Default.Leaderboards);
        }

        private void btnRecordedGames_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", AOMEESavedGamesPath);
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            if (!AutoLaunch)
            {
                LaunchGame();
            }
            else
            {
                ResetTimer();
                LaunchGame();
            }
        }

        #endregion

        #region Window Events
        private void Main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            new About().Show();
        }

        #endregion

        private void CbAutoLaunch_Changed(object sender, RoutedEventArgs e)
        {
            if (CbAutoLaunch.IsChecked.HasValue)
            {
                bool isChecked = CbAutoLaunch.IsChecked.Value;

                if (isChecked)
                {
                    AutoLaunch = true;
                    if (this.IsLoaded)
                    {
                        ResetTimer();
                    }
                }
                else
                {
                    AutoLaunch = false;
                    if (this.IsLoaded)
                    {
                        ResetTimer();
                    }
                }
            }
        }

        #region Private Methods
        private void LaunchGame()
        {
            //have to provide the path of the launcher to start aom:ee
            string args = "";
            foreach (var launchArg in launchArgs)
            {
                args += launchArg + " ";
            }
            Process.Start(AOMFilenamePath, args);
            this.Close();
        }

        private void OpenWebPage(string url)
        {
            Process.Start(url);
        }

        private string GetPathVariables()
        {
            var args = Environment.GetCommandLineArgs();
            foreach (var s in args)
            {
                if (s == "-reset")
                {
                    Settings.Default.Reset();
                    Settings.Default.Reload();
                }
                else
                {
                    launchArgs.Add(s);
                }
            }

            string steam = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"../../..")).FullName;
            AOMEERootPath = Environment.CurrentDirectory;

            if (File.Exists(Path.Combine(AOMEERootPath, _aomFilename)))
            {
                AOMFilenamePath = Path.Combine(SteamPath, _aomFilename);
            }
            else
            {
                BtnLaunch.IsEnabled = false;
                LblErrors.Content = "Error - Please place this file in the AOM:EE Installation folder!";
            }
            if (File.Exists(Path.Combine(AOMEERootPath, _launcherFilename)))
            {
                LauncherFilenamePath = Path.Combine(AOMEERootPath, _launcherFilename);
            }
            if (Directory.Exists(Path.Combine(AOMEERootPath, _savegame)))
            {
                AOMEESavedGamesPath = Path.Combine(AOMEERootPath, _savegame);
            }
            else
            {
                BtnRecordedGames.IsEnabled = false;
            }
            //leaderboards specific to user, steamid detection
            //if (Directory.Exists(steam) && File.Exists(Path.Combine(steam, "steam.exe")))
            //{
            //    DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(steam, "userdata"));
            //    DirectoryInfo[] userdata = directoryInfo.GetDirectories();
            //    if (userdata.Length == 1)
            //    {
            //        try
            //        {
            //            SteamId = v + Convert.ToInt64(userdata[0].Name);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex);
            //        }
            //    }
            //    else
            //    {
            //        BtnLeaderboards.IsEnabled = false;
            //    }
            //}
            return "";
        }
        #endregion

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            InitTimer();
        }

    }
}
