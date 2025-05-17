using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Linq;


namespace Nihongo_Dictionary
{
    /// <summary>
    /// Interaction logic for DictionaryControl.xaml
    /// </summary>
    public partial class DictionaryControl : UserControl
    {
        public DictionaryControl()
        {
            InitializeComponent();
            LoadKanjiDataFromXml();

        }
        private void LoadKanjiDataFromXml()
        {
            string xmlFilePath = "kanji.xml"; 

            if (!File.Exists(xmlFilePath))
            {
                MessageBox.Show($"Fișierul XML nu a fost găsit la calea: {xmlFilePath}");
                return;
            }

            try
            {
                // Încarcă documentul XML
                XDocument xmlDoc = XDocument.Load(xmlFilePath);

                // Selectează nodurile <kanji>
                var kanjiList = xmlDoc.Root.Elements("kanji").Select((kanjiElement, index) => new
                {
                    Index = index + 1,
                    Kanji = kanjiElement.Element("caracter")?.Value ?? "",
                    Onyomi = string.Join(", ", kanjiElement.Element("onyomi")?.Elements("reading").Select(r => r.Value) ?? Enumerable.Empty<string>()),
                    Kunyomi = string.Join(", ", kanjiElement.Element("kunyomi")?.Elements("reading").Select(r => r.Value) ?? Enumerable.Empty<string>()),
                    Meaning = kanjiElement.Element("meaning")?.Value ?? "",
                    OnyomiWords = kanjiElement.Element("onyomi_words")?.Elements("word").Select(w => new
                    {
                        Word = w.Element("text")?.Value ?? "",
                        Reading = w.Element("reading")?.Value ?? "",
                        Meaning = w.Element("meaning")?.Value ?? ""
                    }) ?? Enumerable.Empty<object>(),
                    KunyomiWords = kanjiElement.Element("kunyomi_words")?.Elements("word").Select(w => new
                    {
                        Word = w.Element("text")?.Value ?? "",
                        Reading = w.Element("reading")?.Value ?? "",
                        Meaning = w.Element("meaning")?.Value ?? ""
                    }) ?? Enumerable.Empty<object>(),
                    SpecialReadings = kanjiElement.Element("special_readings")?.Elements("word").Select(w => new
                    {
                        Word = w.Element("text")?.Value ?? "",
                        Reading = w.Element("reading")?.Value ?? "",
                        Meaning = w.Element("meaning")?.Value ?? ""
                    }) ?? Enumerable.Empty<object>(),
                    Examples = kanjiElement.Element("examples")?.Elements("example").Select(ex => new
                    {
                        Sentence = ex.Element("sentence")?.Value ?? "",
                        Reading = ex.Element("reading")?.Value ?? "",
                        Romanji = ex.Element("romanji")?.Value ?? "",
                        Meaning = ex.Element("meaning")?.Value ?? ""
                    }) ?? Enumerable.Empty<object>()
                }).ToList();

                // Setează sursa de date pentru DataGrid
                dgKanjiList.ItemsSource = kanjiList;
                
            }
            catch (XmlException ex)
            {
                MessageBox.Show($"Eroare la parsarea XML: {ex.Message}");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Eroare la citirea fișierului XML: {ex.Message}");
            }
        }
    }
}
