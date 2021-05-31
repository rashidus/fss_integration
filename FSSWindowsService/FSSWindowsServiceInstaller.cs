using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace FSSService
{
    [RunInstaller(true)]
    public partial class FSSWindowsServiceInstaller : System.Configuration.Install.Installer
    {
        public FSSWindowsServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
