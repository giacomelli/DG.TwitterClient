#region Usings
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Linq;
#endregion

namespace DG.TwitterClient.WpfHost.Controls
{    
    [ContentProperty("StatusText")]
    public sealed class StatusTextBlock : TextBlock
    {
        #region Eventos
        public event RequestNavigateEventHandler RequestNavigate;
        #endregion

        #region Campos
        public static readonly DependencyProperty StatusTextProperty = 
            DependencyProperty.Register(
            "StatusText",
            typeof(string),
            typeof(StatusTextBlock),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, StatusTextBlock.StatusTextPropertyChangedCallback, null));

        public static readonly DependencyProperty HyperlinkBrushProperty = 
            DependencyProperty.Register(
            "HyperlinkBrush",
            typeof(Brush),
            typeof(StatusTextBlock),
            new FrameworkPropertyMetadata(Brushes.Blue, FrameworkPropertyMetadataOptions.None, StatusTextBlock.HyperlinkBrushPropertyChangedCallback, null));
        

        private static readonly Regex s_uriMatchingRegex = new Regex(@"(?<url>[a-zA-Z]+:\/\/[a-zA-Z0-9]+([\-\.]{1}[a-zA-Z0-9]+)*\.[a-zA-Z]{2,5}(:[0-9]{1,5})?([a-zA-Z0-9_\-\.\~\%\+\?\=\&\;\|/]*)?)|(?<emailAddress>[^\s]+@[a-zA-Z0-9]+([\-\.]{1}[a-zA-Z0-9]+)*\.[a-zA-Z]{2,5})|(?<toTwitterScreenName>\@[a-zA-Z0-9\-_]+)", RegexOptions.Compiled);
        #endregion

        #region Construtores
        public StatusTextBlock()
        {            
        }
        #endregion

        #region Propriedades
        public string StatusText
        {
            get
            {
                return (string)GetValue(StatusTextBlock.StatusTextProperty);
            }

            set
            {
                SetValue(StatusTextBlock.StatusTextProperty, value);
            }
        }
        
        public Brush HyperlinkBrush
        {
            get
            {
                return (Brush) GetValue(StatusTextBlock.HyperlinkBrushProperty);
            }

            set
            {
                SetValue(StatusTextBlock.HyperlinkBrushProperty, value);
            }
        }
        #endregion

        #region Métodos
        
        internal static IEnumerable<Inline> GenerateInlinesFromRawEntryText(string entryText)
        {
            int startIndex = 0;
            Match match = StatusTextBlock.s_uriMatchingRegex.Match(entryText);

            while (match.Success)
            {
                if (startIndex != match.Index)
                {
                    yield return new Run(StatusTextBlock.DecodeStatusEntryText(entryText.Substring(startIndex, match.Index - startIndex)));
                }

                Hyperlink hyperLink = new Hyperlink(new Run(match.Value));   
                hyperLink.TextDecorations = null;
                hyperLink.MouseMove += new System.Windows.Input.MouseEventHandler(hyperLink_MouseMove);
                hyperLink.MouseLeave += new System.Windows.Input.MouseEventHandler(hyperLink_MouseLeave);

                string uri = match.Value;

                if (match.Groups["emailAddress"].Success)
                {
                    uri = "mailto:" + uri;
                }
                else if (match.Groups["toTwitterScreenName"].Success)
                {
                    uri = "http://twitter.com/" + uri.Substring(1);
                }

                hyperLink.NavigateUri = new Uri(uri);                
                hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(hyperLink_RequestNavigate);

                yield return hyperLink;

                startIndex = match.Index + match.Length;

                match = match.NextMatch();
            }

            if (startIndex != entryText.Length)
            {
                yield return new Run(StatusTextBlock.DecodeStatusEntryText(entryText.Substring(startIndex)));
            }
        }

        static void hyperLink_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Hyperlink)sender).TextDecorations = null;
        }

        static void hyperLink_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Hyperlink)sender).TextDecorations = System.Windows.TextDecorations.Underline;
        }

        static void hyperLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ((StatusTextBlock)((Hyperlink)e.OriginalSource).Parent).OnRequestNavigate(e);
        }

        private void OnRequestNavigate(RequestNavigateEventArgs e)
        {
            if (RequestNavigate != null)
            {
                RequestNavigate(this, e);
            }
        }

        internal static string DecodeStatusEntryText(string text)
        {
            return text.Replace("&gt;", ">").Replace("&lt;", "<");
        }

        private static void StatusTextPropertyChangedCallback(DependencyObject target, DependencyPropertyChangedEventArgs eventArgs)
        {
            var textBlock = (StatusTextBlock)target;

            textBlock.Inlines.Clear();

            string newValue = eventArgs.NewValue as string;

            if (newValue != null)
            {
                textBlock.Inlines.AddRange(StatusTextBlock.GenerateInlinesFromRawEntryText(newValue));
            }
        }

        private static void HyperlinkBrushPropertyChangedCallback(DependencyObject target, DependencyPropertyChangedEventArgs eventArgs)
        {
            var textBlock = (StatusTextBlock)target;
            var hyperlinksQuery = textBlock.Inlines.Where(i => i is Hyperlink);

            foreach (var link in hyperlinksQuery)
            {
                link.Foreground = (Brush)eventArgs.NewValue;
            }
        }        
        #endregion
    }
}
