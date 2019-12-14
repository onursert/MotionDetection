using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;

namespace MotionDetection
{
    public partial class MotionDetection : Form
    {
        public MotionDetection()
        {
            InitializeComponent();
        }

        FilterInfoCollection devices;
        VideoCaptureDevice camera = new VideoCaptureDevice();

        MotionDetector detector;
        float detectionLevel;

        private void Form1_Load(object sender, EventArgs e)
        {
            detector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting());
            detectionLevel = 0;

            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach(FilterInfo d in devices)
            {
                comboBox1.Items.Add(d.Name);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (camera.IsRunning == true)
            {
                camera.Stop();
            }

            camera = new VideoCaptureDevice(devices[comboBox1.SelectedIndex].MonikerString);
            videoSourcePlayer1.VideoSource = camera;
            videoSourcePlayer1.Start();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            detectionLevel = 0;
            videoSourcePlayer1.SignalToStop();
            camera.Stop();
        }

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            detectionLevel = detector.ProcessFrame(image);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (detectionLevel > 0 && checkBox1.Checked == true)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                player.SoundLocation = "beep.wav";
                player.Play();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (camera.IsRunning == true)
            {
                detectionLevel = 0;
                videoSourcePlayer1.SignalToStop();
                camera.Stop();
            }
        }
    }
}