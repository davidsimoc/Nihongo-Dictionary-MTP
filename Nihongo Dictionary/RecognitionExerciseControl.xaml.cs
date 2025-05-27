using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nihongo_Dictionary
{
    public partial class RecognitionExerciseControl : UserControl
    {
        private RecognitionExerciseData _exerciseData;
        private System.Action<string> _answerCallback;
        private List<Button> _optionButtons;

        public RecognitionExerciseControl(RecognitionExerciseData exerciseData, System.Action<string> answerCallback)
        {
            InitializeComponent();
            _exerciseData = exerciseData;
            _answerCallback = answerCallback;
            _optionButtons = new List<Button> { OptionButton1, OptionButton2, OptionButton3, OptionButton4 };
            LoadExercise();
        }

        private void LoadExercise()
        {
            QuestionTextBlock.Text = _exerciseData.Question;

            if (_exerciseData.Options != null && _exerciseData.Options.Count == _optionButtons.Count)
            {
                for (int i = 0; i < _optionButtons.Count; i++)
                {
                    _optionButtons[i].Content = _exerciseData.Options[i];
                    _optionButtons[i].Tag = _exerciseData.Options[i];
                }
            }
            else
            {
                MessageBox.Show("Error: Incorrect number of options for the exercise.");
            }

            foreach (var button in _optionButtons)
            {
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4444")); // Default color
                button.IsEnabled = true;
            }
        }

        private async void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string userAnswer = clickedButton.Tag.ToString();
                bool isCorrect = (userAnswer == _exerciseData.CorrectAnswer);

                foreach (var button in _optionButtons)
                {
                    button.IsEnabled = false;
                }

                if (isCorrect)
                {
                    clickedButton.Background = Brushes.Green;
                    await Task.Delay(500);
                    _answerCallback?.Invoke(userAnswer); 
                }
                else
                {
                    clickedButton.Background = Brushes.Red;
                    // Highlight the correct answer
                    foreach (var button in _optionButtons)
                    {
                        if (button.Tag.ToString() == _exerciseData.CorrectAnswer)
                        {
                            button.Background = Brushes.Green;
                        }
                    }
                    await Task.Delay(1500); 
                    _answerCallback?.Invoke(userAnswer); 
                }
            }
        }
    }
}