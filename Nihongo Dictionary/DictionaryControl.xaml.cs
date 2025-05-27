using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;


namespace Nihongo_Dictionary
{
    // Clasa pentru a reprezenta o intrare Kanji
    public class KanjiEntry : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public string Kanji { get; set; }
        public string Onyomi { get; set; }
        public string Kunyomi { get; set; }
        public string Meaning { get; set; }
        // Adaugă alte proprietăți dacă este necesar
        public List<WordInfo> OnyomiWords { get; set; }
        public List<WordInfo> KunyomiWords { get; set; }
        public List<WordInfo> SpecialReadings { get; set; }
        public List<ExampleInfo> Examples { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class WordInfo
    {
        public string Word { get; set; }
        public string Reading { get; set; }
        public string Meaning { get; set; }
    }

    public class ExampleInfo
    {
        public string Sentence { get; set; }
        public string Reading { get; set; }
        public string Romanji { get; set; }
        public string Meaning { get; set; }
    }

    /// <summary>
    /// Interaction logic for DictionaryControl.xaml
    /// </summary>
    public partial class DictionaryControl : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<KanjiEntry> _kanjiList;
        private ObservableCollection<KanjiEntry> _originalKanjiList;

        public ICommand DeleteKanjiCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DictionaryControl()
        {
            InitializeComponent();
            _kanjiList = new ObservableCollection<KanjiEntry>();
            _originalKanjiList = new ObservableCollection<KanjiEntry>();
            LoadKanjiDataFromXml();
            dgKanjiList.ItemsSource = _kanjiList;
            dgKanjiList.SelectionChanged += DgKanjiList_SelectionChanged; 
            DataContext = this;

            DeleteKanjiCommand = new RelayCommand<string>(DeleteKanji);
        }

        public void DeleteKanji(string kanjiToDelete)
        {
            if (string.IsNullOrWhiteSpace(kanjiToDelete))
            {
                MessageBox.Show("Vă rugăm să introduceți un Kanji valid.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var itemToDelete = _kanjiList.FirstOrDefault(k => k.Kanji == kanjiToDelete);
            if (itemToDelete == null)
            {
                MessageBox.Show($"Kanji-ul '{kanjiToDelete}' nu a fost găsit în listă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"Sigur doriți să ștergeți '{kanjiToDelete}'?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _kanjiList.Remove(itemToDelete);
                _originalKanjiList.Remove(itemToDelete);
                SaveKanjiDataToXml();
                OnPropertyChanged(nameof(KanjiList));
                MessageBox.Show($"Kanji-ul '{kanjiToDelete}' a fost șters cu succes.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveKanjiDataToXml()
        {
            string FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kanji.xml");
            XDocument xmlDoc = new XDocument(
                new XElement("kanji_list",
                    _kanjiList.Select(kanjiEntry =>
                        new XElement("kanji",
                            new XElement("caracter", kanjiEntry.Kanji),
                            new XElement("onyomi", kanjiEntry.Onyomi.Split(',').Select(o => new XElement("reading", o.Trim()))),
                            new XElement("kunyomi", kanjiEntry.Kunyomi.Split(',').Select(k => new XElement("reading", k.Trim()))),
                            new XElement("meaning", kanjiEntry.Meaning),
                            new XElement("onyomi_words", kanjiEntry.OnyomiWords?.Select(w => new XElement("word",
                                new XElement("text", w.Word),
                                new XElement("reading", w.Reading),
                                new XElement("meaning", w.Meaning)))),
                            new XElement("kunyomi_words", kanjiEntry.KunyomiWords?.Select(w => new XElement("word",
                                new XElement("text", w.Word),
                                new XElement("reading", w.Reading),
                                new XElement("meaning", w.Meaning)))),
                            new XElement("special_readings", kanjiEntry.SpecialReadings?.Select(w => new XElement("word",
                                new XElement("text", w.Word),
                                new XElement("reading", w.Reading),
                                new XElement("meaning", w.Meaning)))),
                            new XElement("examples", kanjiEntry.Examples?.Select(ex => new XElement("example",
                                new XElement("sentence", ex.Sentence),
                                new XElement("reading", ex.Reading),
                                new XElement("romanji", ex.Romanji),
                                new XElement("meaning", ex.Meaning)))))
                    )
                )
            );

            try
            {
                xmlDoc.Save(FileName);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Eroare la salvarea fișierului XML: {ex.Message}");
            }
        }

    public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Predicate<T> _canExecute;

            public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                bool canExecute = _canExecute == null || _canExecute((T)parameter);
                Console.WriteLine($"CanExecute called with parameter: {parameter}, result: {canExecute}");
                return canExecute;
            }
            public void Execute(object parameter)
            {
                Console.WriteLine($"Execute called with parameter: {parameter}");
                _execute((T)parameter);
            }
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }

        private void LoadKanjiDataFromXml()
        {
            string FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kanji.xml");

            if (!File.Exists(FileName))
            {
                MessageBox.Show($"Fișierul XML nu a fost găsit la calea: {FileName}");
                return;
            }

            try
            {
                // Încarcă documentul XML
                XDocument xmlDoc = XDocument.Load(FileName);

                // Selectează nodurile <kanji>
                var kanjiList = xmlDoc.Root.Elements("kanji").Select((kanjiElement, index) => new KanjiEntry
                {
                    Index = index + 1,
                    Kanji = kanjiElement.Element("caracter")?.Value ?? "",
                    Onyomi = string.Join(", ", kanjiElement.Element("onyomi")?.Elements("reading").Select(r => r.Value) ?? Enumerable.Empty<string>()),
                    Kunyomi = string.Join(", ", kanjiElement.Element("kunyomi")?.Elements("reading").Select(r => r.Value) ?? Enumerable.Empty<string>()),
                    Meaning = kanjiElement.Element("meaning")?.Value ?? "",
                    OnyomiWords = kanjiElement.Element("onyomi_words")?.Elements("word").Select(w => new WordInfo
                    {
                        Word = w.Element("text")?.Value ?? "",
                        Reading = w.Element("reading")?.Value ?? "",
                        Meaning = w.Element("meaning")?.Value ?? ""
                    }).ToList() ?? new List<WordInfo>(),
                    KunyomiWords = kanjiElement.Element("kunyomi_words")?.Elements("word").Select(w => new WordInfo
                    {
                        Word = w.Element("text")?.Value ?? "",
                        Reading = w.Element("reading")?.Value ?? "",
                        Meaning = w.Element("meaning")?.Value ?? ""
                    }).ToList() ?? new List<WordInfo>(),
                    SpecialReadings = kanjiElement.Element("special_readings")?.Elements("word").Select(w => new WordInfo
                    {
                        Word = w.Element("text")?.Value ?? "",
                        Reading = w.Element("reading")?.Value ?? "",
                        Meaning = w.Element("meaning")?.Value ?? ""
                    }).ToList() ?? new List<WordInfo>(),
                    Examples = kanjiElement.Element("examples")?.Elements("example").Select(ex => new ExampleInfo
                    {
                        Sentence = ex.Element("sentence")?.Value ?? "",
                        Reading = ex.Element("reading")?.Value ?? "",
                        Romanji = ex.Element("romanji")?.Value ?? "",
                        Meaning = ex.Element("meaning")?.Value ?? ""
                    }).ToList() ?? new List<ExampleInfo>()
                }).ToList();

                _originalKanjiList = new ObservableCollection<KanjiEntry>(kanjiList);  //store the original list.
                _kanjiList = new ObservableCollection<KanjiEntry>(kanjiList); // Also set the _kanjiList
                dgKanjiList.ItemsSource = _kanjiList;
                OnPropertyChanged(nameof(KanjiList)); //Raise the event.

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
        private void AddWordButton_Click(object sender, RoutedEventArgs e)
        {
            var addWordControl = new AddWordControl();
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = addWordControl;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                _kanjiList = _originalKanjiList;
            }
            else
            {
                _kanjiList = new ObservableCollection<KanjiEntry>(_originalKanjiList.Where(k => k.Kanji.ToLower().Contains(searchText) || k.Meaning.ToLower().Contains(searchText) || k.Onyomi.ToLower().Contains(searchText) || k.Kunyomi.ToLower().Contains(searchText)));
            }
            dgKanjiList.ItemsSource = _kanjiList;
            OnPropertyChanged(nameof(KanjiList)); // INotifyPropertyChanged
        }

        public ObservableCollection<KanjiEntry> KanjiList
        {
            get { return _kanjiList; }
            set
            {
                _kanjiList = value;
                OnPropertyChanged(nameof(KanjiList));
            }
        }

        private void DgKanjiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                KanjiEntry selectedKanji = (KanjiEntry)e.AddedItems[0];

                var detailsView = new KanjiDetailsView();

                detailsView.KanjiSymbol = selectedKanji.Kanji;
                detailsView.OnyomiMainReadings = selectedKanji.Onyomi;
                detailsView.OnyomiWords = selectedKanji.OnyomiWords?.ToList() ?? new List<WordInfo>();
                detailsView.KunyomiMainReadings = selectedKanji.Kunyomi;
                detailsView.KunyomiWords = selectedKanji.KunyomiWords?.ToList() ?? new List<WordInfo>();
                detailsView.SpecialReadings = selectedKanji.SpecialReadings?.ToList() ?? new List<WordInfo>();
                detailsView.Meaning = selectedKanji.Meaning;
                detailsView.Examples = selectedKanji.Examples?.ToList() ?? new List<ExampleInfo>(); // Trimite lista de ExampleInfo

                if (Window.GetWindow(this) is MainWindow mainWindow)
                {
                    mainWindow.MainContent.Content = detailsView;
                }
            }
        }


    }
}