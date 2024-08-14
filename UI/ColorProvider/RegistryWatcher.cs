using System;
using System.Management;
using System.Security.Principal;

namespace FluentUI
{
    internal sealed class RegistryWatcher
    {
        private readonly ManagementEventWatcher _ManagementEventWatcher;

        /// <summary>sample keyPath = @"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Accent";</summary>
        internal RegistryWatcher(String doubleEscapedPath, String valueName, Action<object, EventArrivedEventArgs> action)
        {
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            WqlEventQuery query = new($@"SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{currentUser.User.Value}\\{doubleEscapedPath}' AND ValueName='{valueName}'");

            _ManagementEventWatcher = new(query);

            _ManagementEventWatcher.EventArrived += action.Invoke;
            _ManagementEventWatcher.Start();
        }

        ~RegistryWatcher()
        {
            _ManagementEventWatcher.Stop();
            _ManagementEventWatcher.Dispose();
        }
    }
}