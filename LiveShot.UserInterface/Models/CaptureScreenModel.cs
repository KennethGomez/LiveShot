using LiveShot.UserInterface.Objects;

namespace LiveShot.UserInterface.Models
{
    public class CaptureScreenModel : ModelBase
    {
        private Selection? _selection;

        public Selection? Selection
        {
            get => _selection;
            set
            {
                _selection = value;
                
                RaisePropertyChanged(nameof(Selection));
            }
        }
    }
}