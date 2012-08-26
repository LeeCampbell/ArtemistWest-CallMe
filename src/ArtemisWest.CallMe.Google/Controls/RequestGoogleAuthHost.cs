﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ArtemisWest.CallMe.Google.Controls
{
    [TemplatePart(Name = WebBrowesrTemplateName, Type = typeof(WebBrowser))]
    public class RequestGoogleAuthHost : Control
    {
        private const string WebBrowesrTemplateName = "PART_WebBrowser";
        private WebBrowser _webBrowser;

        static RequestGoogleAuthHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RequestGoogleAuthHost), new FrameworkPropertyMetadata(typeof(RequestGoogleAuthHost)));
        }

        #region AuthorizationUri DependencyProperty

        public Uri AuthorizationUri
        {
            get { return (Uri)GetValue(AuthorizationUriProperty); }
            set { SetValue(AuthorizationUriProperty, value); }
        }

        public static readonly DependencyProperty AuthorizationUriProperty =
            DependencyProperty.Register("AuthorizationUri", typeof(Uri), typeof(RequestGoogleAuthHost),
                                        new PropertyMetadata());

        #endregion

        #region AuthorizationCode DependencyProperty

        public string AuthorizationCode
        {
            get { return (string)GetValue(AuthorizationCodeProperty); }
            set { SetValue(AuthorizationCodeProperty, value); }
        }

        public static readonly DependencyProperty AuthorizationCodeProperty =
            DependencyProperty.Register("AuthorizationCode", typeof(string), typeof(RequestGoogleAuthHost),
                                        new PropertyMetadata(null));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_webBrowser != null)
                _webBrowser.LoadCompleted -= OnWebBrowserLoadCompleted;
            _webBrowser = (WebBrowser)Template.FindName(WebBrowesrTemplateName, this);
            if (_webBrowser != null)
                _webBrowser.LoadCompleted += OnWebBrowserLoadCompleted;
        }

        private void OnWebBrowserLoadCompleted(object sender, NavigationEventArgs e)
        {
            dynamic doc = _webBrowser.Document;
            string authorizationCode = null;
            if (TryParseSuccessCode(doc.title, out authorizationCode))
            {
                AuthorizationCode = authorizationCode;
            }
        }

        private static bool TryParseSuccessCode(string documentTitle, out string successCode)
        {
            if (!string.IsNullOrWhiteSpace(documentTitle))
            {
                if (documentTitle.StartsWith("Success code="))
                {
                    successCode = documentTitle.Substring("Success code=".Length);
                    return true;
                }
            }
            successCode = null;
            return false;
        }
    }
}
