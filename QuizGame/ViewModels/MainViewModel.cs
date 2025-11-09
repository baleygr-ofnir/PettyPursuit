using System.Windows.Input;
using QuizGame.Helpers;

namespace QuizGame.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentView;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }
    
    public ICommand NavigateToMenuCommand { get; }
    public ICommand NavigateToPlayCommand { get; }
    public ICommand NavigateToCreateCommand { get; }
    public ICommand NavigateToEditCommand { get; }

    public MainViewModel()
    {
        // Init commands
        NavigateToMenuCommand = new RelayCommand(o => NavigateToMenu());
        NavigateToPlayCommand = new RelayCommand(o => NavigateToPlay());
        NavigateToCreateCommand = new RelayCommand(o => NavigateToCreate());
        NavigateToEditCommand = new RelayCommand(o => NavigateToEdit());

        // Make sure menu is called first
        NavigateToMenu();
    }

    private void NavigateToMenu()
    {
        CurrentView = new MenuViewModel(this);
    }

    private void NavigateToPlay()
    {
        CurrentView = new PlayQuizViewModel(this);
    }
    
    private void NavigateToCreate()
    {
        CurrentView = new CreateQuizViewModel(this);
    }
    
    private void NavigateToEdit()
    {
        CurrentView = new EditQuizViewModel(this);
    }
}