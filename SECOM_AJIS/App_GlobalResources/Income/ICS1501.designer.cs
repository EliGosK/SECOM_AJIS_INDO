//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスによって ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションを付けて
    // /str を使用するか、または Visual Studio プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Web.Application.StronglyTypedResourceProxyBuilder", "14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ICS150 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ICS150() {
        }
        
        /// <summary>
        ///   このクラスで使用されるキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.ICS150", global::System.Reflection.Assembly.Load("App_GlobalResources"));
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、現在のスレッドの CurrentUICulture
        ///   プロパティをオーバーライドします。
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
        ///   Download に類似したローカライズされた文字列を検索します。
        /// </summary>
        internal static string btnDownload {
            get {
                return ResourceManager.GetString("btnDownload", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Group name に類似したローカライズされた文字列を検索します。
        /// </summary>
        internal static string lblGroupname {
            get {
                return ResourceManager.GetString("lblGroupname", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Payment date に類似したローカライズされた文字列を検索します。
        /// </summary>
        internal static string lblPaymentDate {
            get {
                return ResourceManager.GetString("lblPaymentDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Specify search criteria に類似したローカライズされた文字列を検索します。
        /// </summary>
        internal static string lblSearchCriteria {
            get {
                return ResourceManager.GetString("lblSearchCriteria", resourceCulture);
            }
        }
    }
}
