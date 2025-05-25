using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Nihongo_Dictionary
{
    /// <summary>
    /// Interaction logic for AddWordControl.xaml
    /// </summary>
    public partial class AddWordControl : UserControl
    {
        private const string FileName = "kanji.xml";
        private MainWindow _mainWindow;

        public AddWordControl()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e )
        {
            try
            {
                XDocument doc;
                XElement kanjiDictionary;
                if(File.Exists(FileName))
                {
                    doc = XDocument.Load(FileName);
                    kanjiDictionary = doc.Root;
                }
                else
                {
                    doc = new XDocument(
                        new XElement("kanji_dictionary")
                        );
                    kanjiDictionary = doc.Root;
                }
                XElement newKanji = new XElement("kanji",
                   new XElement("caracter", txtCaracter.Text),
                   new XElement("onyomi",
                       txtOnyomiSingle.Text.Split(new[] { ',', '，', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(reading => new XElement("reading", reading))),
                   new XElement("kunyomi",
                       txtKunyomiSingle.Text.Split(new[] { ',', '，', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(reading => new XElement("reading", reading))),
                   new XElement("meaning", txtMeaning.Text),
                   new XElement("onyomi_words",
                       CreateWordElements(txtOnyomiWord1Text.Text, txtOnyomiWord1Reading.Text, txtOnyomiWord1Meaning.Text),
                       CreateWordElements(txtOnyomiWord2Text.Text, txtOnyomiWord2Reading.Text, txtOnyomiWord2Meaning.Text),
                       CreateWordElements(txtOnyomiWord3Text.Text, txtOnyomiWord3Reading.Text, txtOnyomiWord3Meaning.Text)),
                   new XElement("kunyomi_words",
                       CreateWordElements(txtKunyomiWord1Text.Text, txtKunyomiWord1Reading.Text, txtKunyomiWord1Meaning.Text),
                       CreateWordElements(txtKunyomiWord2Text.Text, txtKunyomiWord2Reading.Text, txtKunyomiWord2Meaning.Text),
                       CreateWordElements(txtKunyomiWord3Text.Text, txtKunyomiWord3Reading.Text, txtKunyomiWord3Meaning.Text)),
                   new XElement("special_readings",
                       CreateWordElements(txtSpecialReading1Text.Text, txtSpecialReading1Reading.Text, txtSpecialReading1Meaning.Text),
                       CreateWordElements(txtSpecialReading2Text.Text, txtSpecialReading2Reading.Text, txtSpecialReading2Meaning.Text),
                       CreateWordElements(txtSpecialReading3Text.Text, txtSpecialReading3Reading.Text, txtSpecialReading3Meaning.Text)),
                   new XElement("examples",
                       CreateExampleElements(txtExampleSentence1.Text, txtExampleReading1.Text, txtExampleRomanji1.Text, txtExampleMeaning1.Text),
                       CreateExampleElements(txtExampleSentence2.Text, txtExampleReading2.Text, txtExampleRomanji2.Text, txtExampleMeaning2.Text),
                       CreateExampleElements(txtExampleSentence3.Text, txtExampleReading3.Text, txtExampleRomanji3.Text, txtExampleMeaning3.Text))
               );

                kanjiDictionary?.Add(newKanji);

                doc.Save(FileName);
                MessageBox.Show("Cuvântul a fost salvat cu succes în fișierul XML!", "Salvare", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
            } catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare la salvarea în XML: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private XElement CreateWordElements(string text, string reading, string meaning)
        {
            if (!string.IsNullOrWhiteSpace(text) || !string.IsNullOrWhiteSpace(reading) || !string.IsNullOrWhiteSpace(meaning))
            {
                return new XElement("word",
                    new XElement("text", text),
                    new XElement("reading", reading),
                    new XElement("meaning", meaning));
            }
            return null;
        }

        private XElement CreateExampleElements(string sentence, string reading, string romanji, string meaning)
        {
            if (!string.IsNullOrWhiteSpace(sentence) || !string.IsNullOrWhiteSpace(reading) || !string.IsNullOrWhiteSpace(romanji) || !string.IsNullOrWhiteSpace(meaning))
            {
                return new XElement("example",
                    new XElement("sentence", sentence),
                    new XElement("reading", reading),
                    new XElement("romanji", romanji),
                    new XElement("meaning", meaning));
            }
            return null;
        }

        private void ClearForm()
        {
            txtCaracter.Clear();
            txtOnyomiSingle.Clear();
            txtKunyomiSingle.Clear();
            txtMeaning.Clear();

            txtOnyomiWord1Text.Clear();
            txtOnyomiWord1Reading.Clear();
            txtOnyomiWord1Meaning.Clear();
            txtOnyomiWord2Text.Clear();
            txtOnyomiWord2Reading.Clear();
            txtOnyomiWord2Meaning.Clear();
            txtOnyomiWord3Text.Clear();
            txtOnyomiWord3Reading.Clear();
            txtOnyomiWord3Meaning.Clear();

            txtKunyomiWord1Text.Clear();
            txtKunyomiWord1Reading.Clear();
            txtKunyomiWord1Meaning.Clear();
            txtKunyomiWord2Text.Clear();
            txtKunyomiWord2Reading.Clear();
            txtKunyomiWord2Meaning.Clear();
            txtKunyomiWord3Text.Clear();
            txtKunyomiWord3Reading.Clear();
            txtKunyomiWord3Meaning.Clear();

            chkSpecialReadings.IsChecked = false;
            // TODO: Curățarea câmpurilor Special Readings

            txtExampleSentence1.Clear();
            txtExampleReading1.Clear();
            txtExampleRomanji1.Clear();
            txtExampleMeaning1.Clear();
            txtExampleSentence2.Clear();
            txtExampleReading2.Clear();
            txtExampleRomanji2.Clear();
            txtExampleMeaning2.Clear();
            txtExampleSentence3.Clear();
            txtExampleReading3.Clear();
            txtExampleRomanji3.Clear();
            txtExampleMeaning3.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainControl = new MainAppView(_mainWindow);
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = mainControl;
            }
        }

    }
}
