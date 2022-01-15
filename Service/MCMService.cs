using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using DLG.ToolBox.Log;

namespace Service
{
    public class MCMService : ServiceBase
    {
        private static readonly Logger Log = Logger.getInstance();
        private Container _components;
        private static BusinessLogic _bl;

        //create the Mutex to stop multiple programs from running
        static readonly Mutex _mutex = new Mutex(true, "Global\\{tgH9dolvcNp-KMSJRCcAc-9pmXLtvwn9qg-ww0YoBch}");

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        // The main entry point for the process
        public static void Main(string[] args)
        {
            if (_mutex.WaitOne(TimeSpan.Zero, true))
            {
                //Start the log system
                Log.Initialize("DLGMCM_", "", Settings.LogDirectory, LogIntervalType.IT_PER_DAY, LogLevel.D, true, false);

                if (Settings.Error)
                {
                    Log.AddError("Settings are null, what did you do wrong??");
                    Console.WriteLine("Press any key to Exit!!");
                    Console.ReadLine();
                    return;
                }

                if (args.Length > 0 && args[0] == "-console")
                {
                    Console.Title = string.Format("DLG MCM Service Ver ({0})", Utils.buildDate());
                    StartMCMService();
                }
                else
                {
                    Run(new ServiceBase[] {new MCMService()});
                }
            }
            else
            {
                MessageBox.Show("Only one instance at a time");
            }
        }

        protected override void OnStart(string[] args)
        {
            new Thread(StartMCMService).Start();
        }

        protected static void StartMCMService()
        {
            //Dump Settings for Debugging
            //**shows only to console
            //Settings.DumpSettings();

            //pipe dream of stats logging :(
            //SetConsoleCtrlHandler(new HandlerRoutine(catchClose), true);

            _bl = new BusinessLogic();

            if (!_bl.start()) return;

            while(true)
            {
                Console.Write(">");
                var command = Console.ReadLine();
                if(command.Equals("exit") || command.Equals("quit"))
                {
                    Console.WriteLine(_bl.shutDown());
                }
                else if (command.Equals("stats"))
                {
                    _bl.printStats();
                }
                else if (command.Equals("runnow"))
                {
                    _bl.forceRun();
                }
                else if (command.Equals("pause"))
                {
                    _bl.pause(true);
                }
                else if (command.Equals("resume"))
                {
                    _bl.pause(false);
                }
                else if (command.StartsWith("force"))
                {
                    var splitCommand = command.Split(' ');
                    if(splitCommand.Length == 1 || splitCommand[1] == "")
                    {
                        Console.WriteLine("Invalid product");
                    } else
                    {
                        _bl.forceUpdate(splitCommand[1]);
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown command: '{command}'. Valid commands are 'quit','exit','runnow','pause',''resume'");
                }
            }
        }
                
        // ReSharper disable UnusedMember.Local
        private void InitializeComponent()
        // ReSharper restore UnusedMember.Local
        {
            _components = new Container();
            ServiceName = "DLG MCM";
        }

        private static bool catchClose(CtrlTypes ctrlType)
        {
            // Put your own handler here
            var isclosing = false;
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    isclosing = true;
                    Console.WriteLine("CTRL+C received!");
                    break;

                case CtrlTypes.CTRL_BREAK_EVENT:
                    isclosing = true;
                    Console.WriteLine("CTRL+BREAK received!");
                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:
                    isclosing = true;
                    Console.WriteLine("Program being closed!");
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    isclosing = true;
                    Console.WriteLine("User is logging off!");
                    break;

            }
            if(isclosing)
                _bl.shutDown();

            return true;
        }

        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion
    }
}