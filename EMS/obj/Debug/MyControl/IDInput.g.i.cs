﻿#pragma checksum "..\..\..\MyControl\IDInput.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "6519F5938DB36E89A15746AFB5A8476216D30A433AF31BA7952CBC3999CBE73D"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using EMS.MyControl;
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


namespace EMS.MyControl {
    
    
    /// <summary>
    /// IDInput
    /// </summary>
    public partial class IDInput : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\MyControl\IDInput.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox P1;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\MyControl\IDInput.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox P2;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\MyControl\IDInput.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox P3;
        
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
            System.Uri resourceLocater = new System.Uri("/EMS;component/mycontrol/idinput.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MyControl\IDInput.xaml"
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
            this.P1 = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\..\MyControl\IDInput.xaml"
            this.P1.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.P1_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.P2 = ((System.Windows.Controls.TextBox)(target));
            
            #line 20 "..\..\..\MyControl\IDInput.xaml"
            this.P2.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.P2_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.P3 = ((System.Windows.Controls.TextBox)(target));
            
            #line 22 "..\..\..\MyControl\IDInput.xaml"
            this.P3.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.P3_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
