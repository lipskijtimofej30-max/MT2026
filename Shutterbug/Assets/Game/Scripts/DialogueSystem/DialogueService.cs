using System;

public class DialogueService
{
    private int _currentIndex = 0;
    private DialogueData _currentDialogueData;
    private DialogueConversation _currentDialogue;
    private bool _isActive = false;
    
    public event Action<DialogueData> OnNextDialogue;
    public event Action OnDialogueEnded;
    public event Action OnStartDialogue;
    public DialogueData CurrentDialogueData => _currentDialogueData;
    public DialogueConversation CurrentDialogue =>  _currentDialogue;
    

    public void StartDialogue(DialogueConversation dialogue)
    {
        if (dialogue == null || dialogue.Lines.Count == 0) return;
        _isActive = true;
        _currentIndex = 0;
        _currentDialogue = dialogue;
        _currentDialogueData = dialogue.Lines[_currentIndex];
        
        OnStartDialogue?.Invoke();
        OnNextDialogue?.Invoke(_currentDialogueData);
    }
    public void NextDialogue(DialogueConversation dialogue)
    {
       if (!_isActive) return;

        _currentIndex++;
        if (_currentIndex >= dialogue.Lines.Count)
        {
            _isActive = false;
            _currentIndex = 0; 
            OnDialogueEnded?.Invoke();
            return;
        }
        _currentDialogueData = dialogue.Lines[_currentIndex];
        OnNextDialogue?.Invoke(_currentDialogueData);
    }
    
    public void NextCurrentDialogue()
    {
        if (!_isActive) return;
        _currentIndex++;
        if (_currentIndex >= _currentDialogue.Lines.Count)
        {
            _isActive = false;
            _currentIndex = 0; 
            OnDialogueEnded?.Invoke();
            return;
        }
        _currentDialogueData = _currentDialogue.Lines[_currentIndex];
        OnNextDialogue?.Invoke(_currentDialogueData);
    }
}
