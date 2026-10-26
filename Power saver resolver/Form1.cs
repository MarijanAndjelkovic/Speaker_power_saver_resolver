using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Power_saver_resolver
{
    public partial class Form1 : Form
    {
        private WaveOutEvent waveOut;
        private AudioFileReader reader;
        private float currentVolume;
        private string settingsFile = Path.Combine(Application.StartupPath, "settings.txt");
        private bool volumeLoadedFromFile = false;
        private VolumeSampleProvider volumeProvider;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;

            Directory.CreateDirectory(Application.StartupPath);
            if (File.Exists(settingsFile))
            {
                string text = File.ReadAllText(settingsFile);
                if (float.TryParse(text, out float savedVol))
                {
                    currentVolume = savedVol;
                    volumeLoadedFromFile = true;
                }
            }

            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = 1M;
            numericUpDown1.Minimum = decimal.MinValue;
            numericUpDown1.Maximum = decimal.MaxValue;

            StartNoise();

            if (!volumeLoadedFromFile)
                currentVolume = 0.1f; 

            numericUpDown1.Value = (decimal)LinearToDb(currentVolume);

            if (volumeProvider != null)
                volumeProvider.Volume = currentVolume;

            this.FormClosing += Form1_FormClosing;
        }
        
        private void AddToStartup()
        {
            string appName = "PowerSaverResolver";
            string exePath = Application.ExecutablePath;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            key.SetValue(appName, exePath);
        }
        private void StartNoise()
        {
            string soundPath = Path.Combine(Application.StartupPath, "gas.wav");
            if (!File.Exists(soundPath))
            {
                MessageBox.Show("Nedostaje fajl: " + soundPath, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            reader = new AudioFileReader(soundPath);
            reader.Volume = 1.0f;

            var loop = new LoopStream(reader); 

            var sampleProvider = loop.ToSampleProvider(); 

            volumeProvider = new VolumeSampleProvider(sampleProvider);
            volumeProvider.Volume = currentVolume;

            waveOut = new WaveOutEvent();
            waveOut.Init(volumeProvider);
            waveOut.Play();

            File.WriteAllText(settingsFile, currentVolume.ToString());
        }
        public class LoopStream : WaveStream
        {
            private readonly WaveStream sourceStream;

            public LoopStream(WaveStream source)
            {
                this.sourceStream = source;
            }

            public override WaveFormat WaveFormat => sourceStream.WaveFormat;

            public override long Length => long.MaxValue; 

            public override long Position
            {
                get => sourceStream.Position;
                set => sourceStream.Position = value;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int read = sourceStream.Read(buffer, offset, count);
                if (read == 0)
                {
                    sourceStream.Position = 0; 
                    read = sourceStream.Read(buffer, offset, count);
                }
                return read;
            }
        }



        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            currentVolume = DbToLinear((float)numericUpDown1.Value);
            if (volumeProvider != null)
                volumeProvider.Volume = currentVolume;

            File.WriteAllText(settingsFile, currentVolume.ToString());
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;   
                this.Hide();      
                this.ShowInTaskbar = false; 
            }
            
        }

        private float DbToLinear(float db)
        {
            return (float)Math.Pow(10, db / 20.0);
        }

        private float LinearToDb(float linear)
        {
          
            if (linear <= 0)
                return -999;
            return (float)(20.0 * Math.Log10(linear));
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            AddToStartup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false; 
            Application.Exit();
        }
    }
}
