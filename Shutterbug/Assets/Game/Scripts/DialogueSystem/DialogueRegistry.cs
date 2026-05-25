using Zenject;

namespace Game.Scripts.DialogueSystem
{
    public class DialogueRegistry
    {
        private DialogueDatabase _database;
        private DialogueService _dialogueService;

        [Inject]
        private void Construct(DialogueDatabase database, DialogueService dialogueService)
        {
            _database = database;
            _dialogueService = dialogueService;
        }
        
        public void TriggerEvent(KeyDialogueType gameEvent)
        {
            var entry = _database.Databased.Find(x => x.EventKey == gameEvent);
            if (entry.Dialogue != null)
            {
                _dialogueService.StartDialogue(entry.Dialogue);
            }
        }

    }
}