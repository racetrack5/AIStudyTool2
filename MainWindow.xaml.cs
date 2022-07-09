using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using WindowsInput;
using WindowsInput.Native;

namespace AIStudyTool2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public ObservableCollection<USleepOutput> o_USleep;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Logger(string Message)
        {
            string Output = String.Format("{0} - {1}\n", DateTime.Now, Message);
            this.Dispatcher.Invoke(() =>
            {
                ui_Log.Text += Output;
            });
        }

        public void Thread_Transfer(List<USleepOutput> o_USleep, string s_Process, int i_Delay, bool ProFusion)
        {
            try
            {
                var p = Process.GetProcessesByName(s_Process)[0];
                var pointer = p.Handle;

                InputSimulator ins = new InputSimulator();

                try
                {
                    if (p != null)
                    {
                        IntPtr h = p.MainWindowHandle;
                        SetForegroundWindow(h);

                        for (int i = 0; i < o_USleep.Count; i++)
                        {
                            if (o_USleep[i].Epoch == "Wake")
                            {
                                if (ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.VK_W);
                                else if (!ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.NUMPAD0);
                            }
                            else if (o_USleep[i].Epoch == "N1")
                            {
                                if (ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.VK_1);
                                else if (!ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.NUMPAD1);
                            }
                            else if (o_USleep[i].Epoch == "N2")
                            {
                                if (ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.VK_2);
                                else if (!ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.NUMPAD2);
                            }
                            else if (o_USleep[i].Epoch == "N3")
                            {
                                if (ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.VK_3);
                                else if (!ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.NUMPAD3);
                            }
                            else if (o_USleep[i].Epoch == "REM")
                            {
                                if (ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.VK_R);
                                else if (!ProFusion)
                                    ins.Keyboard.KeyPress(VirtualKeyCode.NUMPAD5);
                            }

                            Logger(String.Format("Sending epoch {0} ({1} of {2})", o_USleep[i].Epoch, Convert.ToString(i), Convert.ToString(o_USleep.Count)));

                            Thread.Sleep(Convert.ToInt32(i_Delay));
                        }
                    }
                }
                catch
                {
                    Logger(String.Format("Unable to send data to ProFusion PSG.\n"));
                }
            }
            catch
            {
                Logger(String.Format("Unable to locate process.\n"));
            }
        }
        void Import_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            try
            {
                List<USleepOutput> USleepEpochs = new List<USleepOutput>();

                if (openFileDialog.ShowDialog() == true)
                {
                    StreamReader h_File = new StreamReader(openFileDialog.FileName);
                    string? Line;

                    try
                    {
                        while ((Line = h_File.ReadLine()) != null)
                        {
                            /*
                             * U-Sleep outputs epochs line by line with no delimiters.
                             * string[] Constituents = Line.Split(',');
                             */

                            if (Line.StartsWith("EPOCH") || Line.StartsWith("START"))
                            {
                                Logger("Discarding header data...");
                            }
                            else
                            {
                                USleepEpochs.Add(new USleepOutput()
                                {
                                    Epoch = Line
                                });
                            }
                        }

                        o_USleep = new ObservableCollection<USleepOutput>(USleepEpochs);

                        Logger(String.Format("Imported {0} epochs.\n", Convert.ToString(o_USleep.Count)));

                        Transfer.IsEnabled = true;
                    }
                    catch
                    {
                        Logger(String.Format("Unable to read U-Sleep output."));
                    }
                }
            }
            catch
            {
                Logger(String.Format("Unable to read U-Sleep output."));
            }
        }

        void Transfer_Click(object sender, RoutedEventArgs e)
        {
            List<USleepOutput> uSleepOutputs = new List<USleepOutput>();
            uSleepOutputs = o_USleep.ToList();

            string a = ProcessT.Text;
            int b = Convert.ToInt32(Delay.Text);
            bool ProFusion = true;

            if (RemLogicC.IsChecked == true)
                ProFusion = false;

            Thread Worker = new Thread(() => Thread_Transfer(uSleepOutputs, a, b, ProFusion));
            Worker.Start();
        }
    }
}