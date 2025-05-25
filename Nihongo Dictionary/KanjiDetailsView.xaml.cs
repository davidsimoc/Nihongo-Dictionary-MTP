using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.IO;
using System.Media;

namespace Nihongo_Dictionary
{
    /// <summary>
    /// Interaction logic for KanjiDetailsView.xaml
    /// </summary>
    public partial class KanjiDetailsView : UserControl
    {
        private SpeechSynthesizer synthesizer;
        private readonly string speechSubscriptionKey = "8qYYhgUYZL3Ss0YCewf1EhF90PsHuGml5Og5xW5U9PRPmpkuNCbmJQQJ99BEAC5RqLJXJ3w3AAAYACOGFYxu"; 
        private readonly string speechRegion = "westeurope";

        public KanjiDetailsView()
        {
            InitializeComponent();
            InitializeSpeechSynthesizer();
        }

        private async void InitializeSpeechSynthesizer()
        {
            var config = SpeechConfig.FromSubscription(speechSubscriptionKey, speechRegion);
            //config.SpeechSynthesisVoiceName = "ja-JP-Aoi"; 
                                                           // Try another voice if "ja-JP-Aoi" is not working:
                                                            config.SpeechSynthesisVoiceName = "ja-JP-DaichiNeural";
                                                           // config.SpeechSynthesisVoiceName = "ja-JP-Mayu";
                                                           // config.SpeechSynthesisVoiceName = "ja-JP-Shiori";
            synthesizer = new SpeechSynthesizer(config, null);
        }

        private async void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string japaneseText = clickedButton.Tag as string;
                if (!string.IsNullOrEmpty(japaneseText))
                {
                    try
                    {
                        using var result = await synthesizer.SpeakTextAsync(japaneseText);
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            Console.WriteLine($"Speech synthesized for: {japaneseText}");

                            // Play the synthesized audio
                            using (var audioStream = new MemoryStream(result.AudioData))
                            {
                                using (var player = new SoundPlayer(audioStream))
                                {
                                    player.PlaySync(); // Play synchronously (blocks until playback is complete)
                                                       // Or use player.PlayAsync(); for asynchronous playback
                                }
                            }
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                            MessageBox.Show($"Speech synthesis canceled: {cancellation.Reason}\nError Details: {cancellation.ErrorDetails}", "Speech Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error during speech synthesis: {ex.Message}", "Speech Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public string KanjiSymbol
        {
            get { return KanjiSymbolTextBlock.Text; }
            set { KanjiSymbolTextBlock.Text = value; }
        }

        public string OnyomiMainReadings
        {
            get { return OnyomiMainReadingsTextBlock.Text; }
            set { OnyomiMainReadingsTextBlock.Text = value; }
        }

        public List<WordInfo> OnyomiWords
        {
            get { return (List<WordInfo>)OnyomiWordsItemsControl.ItemsSource; }
            set { OnyomiWordsItemsControl.ItemsSource = value; }
        }

        public string KunyomiMainReadings
        {
            get { return KunyomiMainReadingsTextBlock.Text; }
            set { KunyomiMainReadingsTextBlock.Text = value; }
        }

        public List<WordInfo> KunyomiWords
        {
            get { return (List<WordInfo>)KunyomiWordsItemsControl.ItemsSource; }
            set { KunyomiWordsItemsControl.ItemsSource = value; }
        }

        public List<WordInfo> SpecialReadings
        {
            get { return (List<WordInfo>)SpecialReadingsItemsControl.ItemsSource; }
            set { SpecialReadingsItemsControl.ItemsSource = value; }
        }

        public string Meaning
        {
            get { return MeaningTextBlock.Text; }
            set { MeaningTextBlock.Text = value; }
        }

        public List<Nihongo_Dictionary.ExampleInfo> Examples
        {
            get { return (List<Nihongo_Dictionary.ExampleInfo>)ExamplesItemsControl.ItemsSource; }
            set { ExamplesItemsControl.ItemsSource = value; }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new MainAppView(mainWindow);
            }
        }

    }
}