using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textDialogue;
    [SerializeField] private TextMeshProUGUI _textAuthor;
    [SerializeField] private Button _buttonNext;
    
    private PlayerController _playerController;
    private DialogueService _dialogueService;
    
    private Coroutine _typingCoroutine;
    private CancellationTokenSource _cts;
    private bool _isTyping = false;
    private string _currentFullText;

    [Inject]
    private void Construct(PlayerController playerController, DialogueService dialogueService)
    {
        _playerController = playerController;
        _dialogueService = dialogueService;
        
        _dialogueService.OnNextDialogue += UpdateDisplayDialogue;
        _dialogueService.OnDialogueEnded += HidePanel;
        _dialogueService.OnStartDialogue += ShowPanel;
        _buttonNext.onClick.AddListener(HandleClick);
        
        gameObject.SetActive(false);
    }

    private void HandleClick()
    {
        if (_isTyping)
            CompleteTyping();
        else
            _dialogueService.NextCurrentDialogue();
    }

    private void UpdateDisplayDialogue(DialogueData data)
    {
        _textAuthor.text = data.AuthorName;
        _currentFullText = data.TextDialogue;
        TypeText(_currentFullText).Forget();
    }
    private void HidePanel()
    {
        _playerController.Toggle(true);
        
        gameObject.SetActive(false);
        _textDialogue.text = "";
        _textAuthor.text = "";
    }
    private void ShowPanel()
    { 
        _playerController.Toggle(false);
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);
        _textDialogue.text = "";
        _textAuthor.text = "";
    }

    void OnDestroy()
    {
        _dialogueService.OnNextDialogue -= UpdateDisplayDialogue;
        _dialogueService.OnDialogueEnded -= HidePanel;
        _dialogueService.OnStartDialogue -= ShowPanel;
        _buttonNext.onClick.RemoveListener(HandleClick);
    }

    private async UniTaskVoid TypeText(string fullText)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _isTyping = true;

        // Скрываем все символы ДО присвоения текста
        _textDialogue.maxVisibleCharacters = 0;
        _textDialogue.text = fullText;

        // Принудительно строим меш всё ещё скрытым
        _textDialogue.ForceMeshUpdate();

        // Получаем общее количество символов (включая теги!)
        // Для простоты используем textInfo.characterCount,
        // но можно высчитать только видимые, если есть rich text
        int totalCharacters = _textDialogue.textInfo.characterCount;

        try
        {
            for (int i = 0; i <= totalCharacters; i++)
            {
                _textDialogue.maxVisibleCharacters = i;
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: _cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _isTyping = false;
        }
    }
    
    private void CompleteTyping()
    {
        _cts?.Cancel();
        _textDialogue.maxVisibleCharacters = _textDialogue.textInfo.characterCount; 
        _isTyping = false;
    }
}
