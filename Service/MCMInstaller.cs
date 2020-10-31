using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;


namespace Service
{
    [RunInstaller(true)]
    public class MCMInstaller : Installer
    {
        public MCMInstaller()
        {
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.DisplayName = "DLG MCM";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "DLD MCM";

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
