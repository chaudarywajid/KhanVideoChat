#pragma checksum "..\..\..\MainWindow-orignal.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BD8FE4495DB0F9DBFB915B2AF1AEA790"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using OpenTok;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Animation;
using Telerik.Windows.Controls.Behaviors;
using Telerik.Windows.Controls.Carousel;
using Telerik.Windows.Controls.Docking;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.LayoutControl;
using Telerik.Windows.Controls.Legend;
using Telerik.Windows.Controls.Primitives;
using Telerik.Windows.Controls.RadialMenu;
using Telerik.Windows.Controls.TransitionEffects;
using Telerik.Windows.Controls.TreeView;
using Telerik.Windows.Controls.Wizard;
using Telerik.Windows.Data;
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;
using Telerik.Windows.Input.Touch;
using Telerik.Windows.Shapes;


namespace CKTVideoChat {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.UniformGrid SubscriberGrid;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal OpenTok.VideoRenderer PublisherVideo;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Telerik.Windows.Controls.RadPathButton ConnectButton;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Telerik.Windows.Controls.RadPathButton DisconnectButton;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Telerik.Windows.Controls.RadPathButton muteButton;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Telerik.Windows.Controls.RadPathButton videoButton;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Telerik.Windows.Controls.RadPathButton ConnectDisconnectButton;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button myButton;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\MainWindow-orignal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button testButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CKTVideoChat;component/mainwindow-orignal.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow-orignal.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SubscriberGrid = ((System.Windows.Controls.Primitives.UniformGrid)(target));
            return;
            case 2:
            this.PublisherVideo = ((OpenTok.VideoRenderer)(target));
            return;
            case 3:
            this.ConnectButton = ((Telerik.Windows.Controls.RadPathButton)(target));
            
            #line 46 "..\..\..\MainWindow-orignal.xaml"
            this.ConnectButton.Click += new System.Windows.RoutedEventHandler(this.Connect_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.DisconnectButton = ((Telerik.Windows.Controls.RadPathButton)(target));
            
            #line 53 "..\..\..\MainWindow-orignal.xaml"
            this.DisconnectButton.Click += new System.Windows.RoutedEventHandler(this.Disonnect_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.muteButton = ((Telerik.Windows.Controls.RadPathButton)(target));
            
            #line 61 "..\..\..\MainWindow-orignal.xaml"
            this.muteButton.Click += new System.Windows.RoutedEventHandler(this.Mute_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.videoButton = ((Telerik.Windows.Controls.RadPathButton)(target));
            
            #line 66 "..\..\..\MainWindow-orignal.xaml"
            this.videoButton.Click += new System.Windows.RoutedEventHandler(this.Video_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ConnectDisconnectButton = ((Telerik.Windows.Controls.RadPathButton)(target));
            
            #line 73 "..\..\..\MainWindow-orignal.xaml"
            this.ConnectDisconnectButton.Click += new System.Windows.RoutedEventHandler(this.Connect_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.myButton = ((System.Windows.Controls.Button)(target));
            return;
            case 9:
            this.testButton = ((System.Windows.Controls.Button)(target));
            
            #line 97 "..\..\..\MainWindow-orignal.xaml"
            this.testButton.Click += new System.Windows.RoutedEventHandler(this.testButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

