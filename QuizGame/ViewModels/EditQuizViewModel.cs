using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuizGame.Helpers;
using QuizGame.Models;
using QuizGame.Services;

namespace QuizGame.ViewModels;

public class EditQuizViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
        private Quiz _selectedQuiz;
        private QuestionViewModel _selectedQuestion;
        private string _quizName;

        public ObservableCollection<Quiz> AvailableQuizzes { get; set; }
        public ObservableCollection<QuestionViewModel> Questions { get; set; }

        public Quiz SelectedQuiz
        {
            get => _selectedQuiz;
            set => SetProperty(ref _selectedQuiz, value);
        }

        public QuestionViewModel SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                SetProperty(ref _selectedQuestion, value);
                OnPropertyChanged(nameof(IsQuestionSelectedVisibility));
            }
        }

        public string QuizCategory
        {
            get => _quizName;
            set => SetProperty(ref _quizName, value);
        }

        public Visibility IsQuizLoadedVisibility => 
            Questions?.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility IsQuestionSelectedVisibility => 
            SelectedQuestion != null ? Visibility.Visible : Visibility.Collapsed;

        public ICommand BackToMenuCommand { get; }
        public ICommand LoadQuizCommand { get; }
        public ICommand EditQuestionCommand { get; }
        public ICommand SaveChangesCommand { get; }

        public EditQuizViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Questions = new ObservableCollection<QuestionViewModel>();

            BackToMenuCommand = new RelayCommand(o => _mainViewModel.NavigateToMenuCommand.Execute(null));
            LoadQuizCommand = new RelayCommand(o => LoadQuiz(), o => SelectedQuiz != null);
            EditQuestionCommand = new RelayCommand(o => EditQuestion(), o => SelectedQuestion != null);
            SaveChangesCommand = new RelayCommand(o => SaveChanges());

            LoadAvailableQuizzes();
        }

        private async void LoadAvailableQuizzes()
        {
            // TODO: Load quizzes asynchronously from AppData/Local/YourAppName
            AvailableQuizzes = new ObservableCollection<Quiz>();
        }

        private async void LoadQuiz()
        {
            if (SelectedQuiz == null) return;

            // TODO: Load full quiz data asynchronously
            Questions.Clear();
            QuizCategory = SelectedQuiz.Category;
            
            // Convert Question models to QuestionViewModels
            foreach (var question in SelectedQuiz.Questions)
            {
                Questions.Add(new QuestionViewModel
                {
                    QuestionNumber = Questions.Count + 1,
                    Statement = question.Statement,
                    // Map other properties
                });
            }

            OnPropertyChanged(nameof(IsQuizLoadedVisibility));
        }

        private void EditQuestion()
        {
            // Question is already bound, editing happens automatically
            // This could trigger additional UI feedback
        }

        private async Task SaveChanges()
        {
            var updatedQuiz = new Quiz
            {
                Category = QuizCategory,
                Questions = Questions.Select(qvm => new Question
                {
                    Statement = qvm.Statement,
                    Answers = {qvm.FirstAnswer, qvm.SecondAnswer, qvm.ThirdAnswer, qvm.FourthAnswer},
                    CorrectAnswer = qvm.CorrectAnswer
                })
            }
            await QuizFileService.SaveAsJson()
        }
}