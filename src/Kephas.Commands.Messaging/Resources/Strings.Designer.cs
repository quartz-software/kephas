﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kephas.Commands.Messaging.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple command types found for &apos;{0}&apos;: &apos;{1}&apos;..
        /// </summary>
        internal static string DefaultCommandRegistry_AmbiguousCommandName {
            get {
                return ResourceManager.GetString("DefaultCommandRegistry_AmbiguousCommandName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command type for &apos;{0}&apos; not found..
        /// </summary>
        internal static string DefaultCommandRegistry_CommandNotFound {
            get {
                return ResourceManager.GetString("DefaultCommandRegistry_CommandNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please provide one command name to see the information about that command. Example: &apos;help &lt;command&gt;&apos;..
        /// </summary>
        internal static string MissingCommandName_Warning {
            get {
                return ResourceManager.GetString("MissingCommandName_Warning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No commands with the filter &apos;{0}*&apos; found..
        /// </summary>
        internal static string NoMatchingCommands_Warning {
            get {
                return ResourceManager.GetString("NoMatchingCommands_Warning", resourceCulture);
            }
        }
    }
}
