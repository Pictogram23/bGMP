using System.Windows.Forms;

namespace bGMP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            ShowInTaskbar = false;
            BgmpFileManager bgmp = new BgmpFileManager();
        }
    }
}