using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace mp3Fixer
{
    public partial class Form1 : Form
    {
        private TextBoxTraceListener _textBoxListener;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _textBoxListener = new TextBoxTraceListener(txtLog);
            Trace.Listeners.Add(_textBoxListener);
        }

        private void btnTestGo_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Starting MP3 Fixer");

            var tFix = new Thread(new ThreadStart(new mp3FixThread().DoMp3Fix));
            tFix.Start();
        }
    }

    //Debug Trace listener
    public class TextBoxTraceListener : TraceListener
    {
        private readonly TextBox _target;
        private readonly StringSendDelegate _invokeWrite;

        public TextBoxTraceListener(TextBox target)
        {
            _target = target;
            _invokeWrite = new StringSendDelegate(SendString);
        }

        public override void Write(string message)
        {
            _target.Invoke(_invokeWrite, new object[] { message });
        }

        public override void WriteLine(string message)
        {
            _target.Invoke(_invokeWrite, new object[] { message + Environment.NewLine });
        }

        private delegate void StringSendDelegate(string message);
        private void SendString(string message)
        {
            // No need to lock text box as this function will only 
            // ever be executed from the UI thread
            _target.AppendText(message);
            if (_target.Lines.Length >= 1000)
                _target.Lines[1].Remove(0);
        }
    }
}
