using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Telerik.Windows.Controls;

namespace CKTVideoChat.View
{
    /// <summary>
    /// Interaction logic for MainDock.xaml
    /// </summary>
    public partial class MainDock : UserControl
    {
        public MainDock()
        {
            InitializeComponent();
            //// Need for downloading the grid assembly.
            //Telerik.Windows.Controls.RadGridView fakeGrid = new Telerik.Windows.Controls.RadGridView();
        }
    }
}
