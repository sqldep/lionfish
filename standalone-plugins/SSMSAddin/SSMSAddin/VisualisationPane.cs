using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SSMSAddin
{
    [Guid("E9C60F2B-F01B-4e3e-A551-C09C62E5F584")]
    public partial class VisualisationPane : UserControl
    {
        public VisualisationPane()
        {
            InitializeComponent();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
