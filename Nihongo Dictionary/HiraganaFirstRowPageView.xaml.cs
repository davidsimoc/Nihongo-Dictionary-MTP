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

namespace Nihongo_Dictionary
{
    public partial class HiraganaFirstRowPageView : UserControl
    {
        private int currentStep = 0;
        private int currentExerciseIndex = 0;
        private List<string> userAnswers = new List<string>();
        private MainWindow _mainWindow;
        public HiraganaFirstRowPageView(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadCurrentStep();
            UpdateNavigationVisibility();
        }

        private List<LessonStep> lessonContent = new List<LessonStep>()
        {
            new LessonStep
            {
                Type = "info",
                Sections = new List<InfoSection>
                {
                    new InfoSection
                    {
                        Title = "Hiragana Basics: Introduction",
                        Content = new List<string>
                        {
                            "Hiragana is a fundamental Japanese script",
                            "Each character represents a syllable and there are 46 basic Hiragana characters",
                            "",
                            "In this section, we will cover the first row of Hiragana characters",
                        }
                    },
                    new InfoSection
                    {
                        Title = "The Hiragana Chart - First Row (あ-お)",
                        Characters = new List<HiraganaCharacter>
                        {
                            new HiraganaCharacter { Char = "あ", Pronunciation = "a", Helper = "Look closley, and find the letter A inside of it." },
                            new HiraganaCharacter { Char = "い", Pronunciation = "i", Helper = "To remember this kana, just think of a couple of eels hanging out.\n\nThey're upright because they're trying to mimic the letter i" },
                            new HiraganaCharacter { Char = "う", Pronunciation = "u", Helper = "To remember this kana, notice the U shape in it!\n\nThere's another similar hiragana つ, but that one isn't wearing a hat like U(you) are." },
                            new HiraganaCharacter { Char = "え", Pronunciation = "e", Helper = "Look at this kana and find the exotic bird laying exotic eggs inside of it." },
                            new HiraganaCharacter { Char = "お", Pronunciation = "o", Helper = "Can you see the letter o in there, two times? This one looks similar to あ except for one key difference: there are two letter o symbols visible in there." }
                        }
                    }
                }
            },
            new LessonStep
            {
                Type = "exerciseGroup",
                Exercises = new List<RecognitionExerciseData>
                {
                    new RecognitionExerciseData { Question = "Which character is \"a\"?", CorrectAnswer = "あ", Options = new List<string> { "い", "あ", "う", "え" } },
                    new RecognitionExerciseData { Question = "Which character is \"i\"?", CorrectAnswer = "い", Options = new List<string> { "あ", "い", "え", "お" } },
                    new RecognitionExerciseData { Question = "Which character is \"e\"?", CorrectAnswer = "え", Options = new List<string> { "い", "お", "え", "う" } }
                }
            }
        };

        private void LoadCurrentStep()
        {
            var step = lessonContent[currentStep];
            LessonContentStackPanel.Children.Clear();
            ExerciseContentControl.Content = null;
            CompletionPanel.Visibility = Visibility.Collapsed;

            if (step.Type == "info")
            {
                foreach (var section in step.Sections)
                {
                    if (!string.IsNullOrEmpty(section.Title))
                    {
                        var titleBlock = new TextBlock { Text = section.Title, Style = (Style)FindResource("SectionTitleStyle") };
                        LessonContentStackPanel.Children.Add(titleBlock);
                    }
                    if (section.Content != null)
                    {
                        foreach (var paragraph in section.Content)
                        {
                            var paragraphBlock = new TextBlock { Text = paragraph, Style = (Style)FindResource("ParagraphStyle"), TextWrapping = TextWrapping.Wrap };
                            LessonContentStackPanel.Children.Add(paragraphBlock);
                        }
                    }
                    if (section.Characters != null)
                    {
                        foreach (var character in section.Characters)
                        {
                            var container = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 0, 8) };
                            var charPronunciation = new StackPanel { Orientation = Orientation.Horizontal };
                            charPronunciation.Children.Add(new TextBlock { Text = character.Char, Style = (Style)FindResource("CharacterStyle") });
                            charPronunciation.Children.Add(new TextBlock { Text = $"Pronunciation: {character.Pronunciation}", Style = (Style)FindResource("PronunciationStyle"), Margin = new Thickness(16, 0, 0, 0) });
                            container.Children.Add(charPronunciation);
                            if (!string.IsNullOrEmpty(character.Helper))
                            {
                                var helperBlock = new TextBlock { Text = $"How to remember:\n\n{character.Helper}", Style = (Style)FindResource("HelperStyle"), TextWrapping = TextWrapping.Wrap };
                                container.Children.Add(helperBlock);
                            }
                            LessonContentStackPanel.Children.Add(container);
                        }
                    }
                }
                BottomNavigationPanel.Visibility = lessonContent.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (step.Type == "exerciseGroup")
            {
                if (step.Exercises != null && step.Exercises.Any())
                {
                    var exercise = step.Exercises[currentExerciseIndex];
                    var recognitionExerciseControl = new RecognitionExerciseControl(exercise, HandleRecognitionAnswer);
                    ExerciseContentControl.Content = recognitionExerciseControl;
                    BottomNavigationPanel.Visibility = Visibility.Visible;
                }
            }

            UpdateNavigationVisibility();
        }

        private void HandleRecognitionAnswer(string userAnswer)
        {
            userAnswers.Add(userAnswer);
            GoToNextStep(null, null);
        }

        private void GoToNextStep(object sender, RoutedEventArgs e)
        {
            var currentStepData = lessonContent[currentStep];
            if (currentStepData.Type == "exerciseGroup")
            {
                if (currentStepData.Exercises != null && currentExerciseIndex < currentStepData.Exercises.Count - 1)
                {
                    currentExerciseIndex++;
                    LoadCurrentStep();
                }
                else if (currentStep < lessonContent.Count - 1)
                {
                    currentStep++;
                    currentExerciseIndex = 0;
                    LoadCurrentStep();
                }
                else
                {

                    BottomNavigationPanel.Visibility = Visibility.Collapsed;
                    ExerciseContentControl.Content = null;
                    CompletionPanel.Visibility = Visibility.Visible;
                }
            }
            else if (currentStep < lessonContent.Count - 1)
            {
                currentStep++;
                LoadCurrentStep();
            }
            else
            {
                // Lecția s-a terminat (dacă ultimul pas nu e un exercitiu)
                BottomNavigationPanel.Visibility = Visibility.Collapsed;
                CompletionPanel.Visibility = Visibility.Visible;
            }
            UpdateNavigationVisibility();
        }

        private void GoToBackStep_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep > 0)
            {
                var currentStepData = lessonContent[currentStep];
                if (currentStepData.Type == "exerciseGroup" && currentExerciseIndex > 0)
                {
                    currentExerciseIndex--;
                }
                else
                {
                    currentStep--;
                    if (lessonContent[currentStep].Type == "exerciseGroup" && lessonContent[currentStep].Exercises != null)
                    {
                        currentExerciseIndex = lessonContent[currentStep].Exercises.Count - 1;
                    }
                    else
                    {
                        currentExerciseIndex = 0;
                    }
                }
                LoadCurrentStep();
            }
            UpdateNavigationVisibility();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

            if (_mainWindow?.MainContent.Content is LessonsControl lessonsControl)
            {
            }
            else if (_mainWindow != null)
            {
                _mainWindow.MainContent.Content = new HiraganaLessonsView();
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {

            if (_mainWindow != null)
            {
                _mainWindow.MainContent.Content = new MainAppView(_mainWindow); 
            }
        }

        private void UpdateNavigationVisibility()
        {
            BottomNavigationPanel.Visibility = lessonContent.Count > 1 && currentStep < lessonContent.Count - 1 ? Visibility.Visible : Visibility.Collapsed;
            if (currentStep == lessonContent.Count - 1)
            {
                BottomNavigationPanel.Visibility = Visibility.Collapsed;
                if (lessonContent[currentStep].Type != "exerciseGroup" || (lessonContent[currentStep].Exercises?.Count == currentExerciseIndex + 1))
                {
                    CompletionPanel.Visibility = Visibility.Visible;
                }
                else if (lessonContent[currentStep].Type == "exerciseGroup")
                {
                    BottomNavigationPanel.Visibility = Visibility.Visible;
                    var backButton = BottomNavigationPanel.Children.OfType<Button>().FirstOrDefault();
                    if (backButton != null) backButton.Visibility = currentStep > 0 || currentExerciseIndex > 0 ? Visibility.Visible : Visibility.Collapsed;
                    var nextButton = BottomNavigationPanel.Children.OfType<Button>().LastOrDefault();
                    if (nextButton != null) nextButton.Content = new TextBlock { Text = "Finish" };
                    if (nextButton != null) nextButton.Click -= GoToNextStep;
                    if (nextButton != null) nextButton.Click += DoneButton_Click;
                }
            }
            else
            {
                var backButton = BottomNavigationPanel.Children.OfType<Button>().FirstOrDefault();
                if (backButton != null) backButton.Visibility = currentStep > 0 || currentExerciseIndex > 0 ? Visibility.Visible : Visibility.Visible;
                var nextButton = BottomNavigationPanel.Children.OfType<Button>().LastOrDefault();
                if (nextButton != null)
                {
                    nextButton.Content = new TextBlock { Text = "Next" };
                    nextButton.Click -= DoneButton_Click;
                    nextButton.Click += GoToNextStep;
                }
            }
        }

    }
    public class LessonStep
    {
        public string Type { get; set; }
        public List<InfoSection> Sections { get; set; }
        public List<RecognitionExerciseData> Exercises { get; set; }
    }

    public class InfoSection
    {
        public string Title { get; set; }
        public List<string> Content { get; set; }
        public List<HiraganaCharacter> Characters { get; set; }
    }

    public class HiraganaCharacter
    {
        public string Char { get; set; }
        public string Pronunciation { get; set; }
        public string Helper { get; set; }
    }

    public class RecognitionExerciseData
    {
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> Options { get; set; }
    }
}
