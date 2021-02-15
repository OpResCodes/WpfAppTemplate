using GalaSoft.MvvmLight.Messaging;

namespace Attalo.WPF.Utilities.Messages
{
    public class ItemSelectionMessage : NotificationMessage
    {
        public ItemSelectionMessage(object sender, bool selectionState) : base(sender,"selection")
        {
            SelectionState = selectionState;
        }

        public bool SelectionState { get; set; }

    }
}
