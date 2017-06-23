using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace MyWaifu
{
    public partial class ImageViewer : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(
        [MarshalAs(UnmanagedType.LPTStr)] string lpClassName,
        [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(
            IntPtr hWndChild,
            IntPtr hWndNewParent);

        Image waifu;

        public ImageViewer()
        {
            InitializeComponent();
            try
            {
                InitializeComponent();
                //this.TransparencyKey = this.BackColor;
                IntPtr hwndf = this.Handle;
                IntPtr hwndParent = FindWindow("Progman", null);
                SetParent(hwndf, hwndParent);

                this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
                this.DoubleBuffered = true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// https://github.com/NeuroWhAI/YoutubeWallpaper/blob/master/YoutubeWallpaper/BehindDesktopIcon.cs
        /// </summary>
        public static class BehindDesktopIcon
        {
            public static bool FixBehindDesktopIcon(IntPtr formHandle)
            {
                IntPtr progman = WinApi.FindWindow("Progman", null);

                if (progman == IntPtr.Zero)
                    return false;


                IntPtr workerw = IntPtr.Zero;

                // 여러번 시도함.
                for (int step = 0; step < 8; ++step)
                {
                    // 한번씩은 건너뜀.
                    if (step % 2 == 0)
                    {
                        IntPtr result = IntPtr.Zero;
                        WinApi.SendMessageTimeout(progman,
                            0x052C,
                            new IntPtr(0),
                            IntPtr.Zero,
                            WinApi.SendMessageTimeoutFlags.SMTO_NORMAL,
                            10000,
                            out result);
                    }


                    WinApi.EnumWindows(new WinApi.EnumWindowsProc((tophandle, topparamhandle) =>
                    {
                        IntPtr p = WinApi.FindWindowEx(tophandle,
                            IntPtr.Zero,
                            "SHELLDLL_DefView",
                            IntPtr.Zero);

                        if (p != IntPtr.Zero)
                        {
                            workerw = WinApi.FindWindowEx(IntPtr.Zero,
                                tophandle,
                                "WorkerW",
                                IntPtr.Zero);
                        }

                        return true;
                    }), IntPtr.Zero);


                    if (workerw == IntPtr.Zero)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        break;
                    }
                }

                if (workerw == IntPtr.Zero)
                    return false;
                
                WinApi.ShowWindow(workerw, 0/*HIDE*/);
                WinApi.SetParent(formHandle, progman);
                
                return true;
            }
        }

        private void ImageViewer_Load(object sender, EventArgs e)
        {
            BehindDesktopIcon.FixBehindDesktopIcon(this.Handle);
        }
    }
}
