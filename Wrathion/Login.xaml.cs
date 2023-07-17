using System;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Web.WebView2.Core;

namespace Wrathion
{
    public partial class Login : Window
    {
        private const string JavaScript = @"
            setTimeout(
               ()=>{
                    const data = JSON.parse(localStorage.user);
                    const r = JSON.stringify({token:data['token'],userId:data['userId'], tenantCode:data['tenantCode'], userProjectId: data['preUserProjectId'], realName: data['realName']});
                    window.chrome.webview.postMessage(r);                
                }
            ,3000)
        ";

        public Login()
        {
            InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += ReciveMsg;
        }

        private void WebView_OnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            var url = webView.CoreWebView2.Source;
            if (url == "https://weiban.mycourse.cn/#/")
            {
                webView.CoreWebView2.ExecuteScriptAsync(JavaScript);
            }
        }

        private void ReciveMsg(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            MessageBox.Show(args.TryGetWebMessageAsString());
        }
    }
}