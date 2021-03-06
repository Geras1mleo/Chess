#pragma checksum "..\..\..\Tables\NewGameWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5216AA7A1E96F0EDDC5DAD3DF6792D5B0FDEA85A6D0C9164707B13AAA3ECAE42"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Chess.Tables;
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


namespace Chess.Tables {
    
    
    /// <summary>
    /// NewGameWindow
    /// </summary>
    public partial class NewGameWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\Tables\NewGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Nickname;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Tables\NewGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ErrorNick;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\Tables\NewGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LobbyID;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Tables\NewGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ErrorLobby;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\Tables\NewGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NewLobby;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Tables\NewGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Connect;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\Tables\NewGameWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Cancel;
        
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
            System.Uri resourceLocater = new System.Uri("/Chess;component/tables/newgamewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Tables\NewGameWindow.xaml"
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
            
            #line 8 "..\..\..\Tables\NewGameWindow.xaml"
            ((Chess.Tables.NewGameWindow)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Nickname = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.ErrorNick = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.LobbyID = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.ErrorLobby = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.NewLobby = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\..\Tables\NewGameWindow.xaml"
            this.NewLobby.Click += new System.Windows.RoutedEventHandler(this.NewLobby_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Connect = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\..\Tables\NewGameWindow.xaml"
            this.Connect.Click += new System.Windows.RoutedEventHandler(this.Confirm_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Cancel = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\..\Tables\NewGameWindow.xaml"
            this.Cancel.Click += new System.Windows.RoutedEventHandler(this.Cancel_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

