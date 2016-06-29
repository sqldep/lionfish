using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SSMSAddin
{
    class RegistryAccess
    {
        public RegistryAccess(string appName, bool showError)
        {
            this.BaseRegistryKey = Registry.LocalMachine;
            this.SubKey = "SOFTWARE\\" + appName;
        }

        public RegistryKey BaseRegistryKey { get; set;  }

        public string SubKey { get; set; }

        public bool ShowError { get; set; }
        public string Read(string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = BaseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(SubKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                    return null;
                }
            }
        }

        public bool Write(string KeyName, object Value)
        {
            try
            {
                // Setting
                RegistryKey rk = this.BaseRegistryKey;
                // I have to use CreateSubKey 
                // (create or open it if already exits), 
                // 'cause OpenSubKey open a subKey as read-only
                RegistryKey sk1 = rk.CreateSubKey(this.SubKey);
                // Save the value
                sk1.SetValue(KeyName.ToUpper(), Value);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Writing registry " + KeyName.ToUpper());
                return false;
            }
        }

        public bool DeleteKey(string KeyName)
        {
            try
            {
                // Setting
                RegistryKey rk = this.BaseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(this.SubKey);
                // If the RegistrySubKey doesn't exists -> (true)
                if (sk1 == null)
                    return true;
                else
                    sk1.DeleteValue(KeyName);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Deleting SubKey " + this.SubKey);
                return false;
            }
        }

        public bool DeleteSubKeyTree()
        {
            try
            {
                // Setting
                RegistryKey rk = this.BaseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(this.SubKey);
                // If the RegistryKey exists, I delete it
                if (sk1 != null)
                    rk.DeleteSubKeyTree(this.SubKey);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Deleting SubKey " + this.SubKey);
                return false;
            }
        }

        public int SubKeyCount()
        {
            try
            {
                // Setting
                RegistryKey rk = this.BaseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(this.SubKey);
                // If the RegistryKey exists...
                if (sk1 != null)
                    return sk1.SubKeyCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Retriving subkeys of " + this.SubKey);
                return 0;
            }
        }

        public int ValueCount()
        {
            try
            {
                // Setting
                RegistryKey rk = this.BaseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(this.SubKey);
                // If the RegistryKey exists...
                if (sk1 != null)
                    return sk1.ValueCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Retriving keys of " + this.SubKey);
                return 0;
            }
        }

        private void ShowErrorMessage(Exception e, string Title)
        {
            if (ShowError == true)
                MessageBox.Show(e.Message,
                        Title
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
        }
    }
}
