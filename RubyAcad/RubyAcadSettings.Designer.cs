﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4200
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RubyAcad {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class RubyAcadSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static RubyAcadSettings defaultInstance = ((RubyAcadSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new RubyAcadSettings())));
        
        public static RubyAcadSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:/ironruby-0.9.0/")]
        public string IronRubyHome {
            get {
                return ((string)(this["IronRubyHome"]));
            }
            set {
                this["IronRubyHome"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("lib/ironRuby;lib/Ruby/site_ruby/1.8;lib/Ruby/site_ruby;lib/Ruby/1.8;/bin")]
        public string IronRubyPaths {
            get {
                return ((string)(this["IronRubyPaths"]));
            }
            set {
                this["IronRubyPaths"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:/Program Files (x86)/AutoCAD Civil 3D 2009/")]
        public string AcadHome {
            get {
                return ((string)(this["AcadHome"]));
            }
            set {
                this["AcadHome"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("acmgd.dll;acdbmgd.dll;acmgdinternal.dll")]
        public string AcadRequires {
            get {
                return ((string)(this["AcadRequires"]));
            }
            set {
                this["AcadRequires"] = value;
            }
        }
    }
}
