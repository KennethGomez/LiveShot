using System;
using System.Collections.Generic;
using System.Text;
using LiveShot.UserInterface.Models;

namespace LiveShot.UserInterface.ViewModels
{
    public class CaptureScreenWindowViewModel : ViewModelBase
    {
        private readonly CaptureScreenModel _model;

        public CaptureScreenModel Model => _model;

        public CaptureScreenWindowViewModel()
        {
            _model = new CaptureScreenModel();
        }
    }
}