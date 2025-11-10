using System.Windows.Input;
using QuizGame.Helpers;

namespace QuizGame.ViewModels;

public class MenuViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    
    public ICommand NavigateToPlayCommand { get; }
    public ICommand NavigateToCreateCommand { get; }
    public ICommand NavigateToEditCommand { get; }

    public MenuViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        NavigateToPlayCommand = _mainViewModel.NavigateToPlayCommand;
        NavigateToCreateCommand = _mainViewModel.NavigateToCreateCommand;
        NavigateToEditCommand = _mainViewModel.NavigateToEditCommand;
    }
}