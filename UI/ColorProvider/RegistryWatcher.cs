using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Principal;

namespace FluentUI
{
    internal sealed class RegistryWatcher
    {
        internal readonly Boolean SuccessfullySubscribed = false;

        private static readonly List<ManagementEventWatcher> _ManagementEventWatchers = new(2);

        /// <summary>sample keyPath = @"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Accent";</summary>
        internal RegistryWatcher(String doubleEscapedPath, String valueName, Action<Object, EventArrivedEventArgs> action)
        {
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            WqlEventQuery query = new($@"SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{currentUser.User.Value}\\{doubleEscapedPath}' AND ValueName='{valueName}'");

            ManagementEventWatcher managementEventWatcher = new(query);

            try
            {
                managementEventWatcher.Start(); // will fail when WMI quota was reached
                managementEventWatcher.EventArrived += action.Invoke;

                _ManagementEventWatchers.Add(managementEventWatcher);

                SuccessfullySubscribed = true;
            }
            catch
            {
                managementEventWatcher.Dispose();
            }
        }

        internal static void Dispose()
        {
            for (UInt16 i = 0; i < _ManagementEventWatchers.Count; ++i)
            {
                _ManagementEventWatchers[i]?.Stop(); // free unmanaged resources
                _ManagementEventWatchers[i]?.Dispose();
            }
        }
    }
}