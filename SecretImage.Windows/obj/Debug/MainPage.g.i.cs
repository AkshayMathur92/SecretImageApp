﻿

#pragma checksum "C:\Users\amathur\Documents\SecretImage.Source-v1.0.0.0\SecretImage.Windows\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EF1631A3A328B903F612EF46586AC2D0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SecretImage
{
    partial class MainPage : global::SecretImage.PageBase
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::SecretImage.Controls.PageHeaderControl Header; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Hub Container; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualState SnappedView; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualState FullscreenView; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///MainPage.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            Header = (global::SecretImage.Controls.PageHeaderControl)this.FindName("Header");
            Container = (global::Windows.UI.Xaml.Controls.Hub)this.FindName("Container");
            SnappedView = (global::Windows.UI.Xaml.VisualState)this.FindName("SnappedView");
            FullscreenView = (global::Windows.UI.Xaml.VisualState)this.FindName("FullscreenView");
        }
    }
}


