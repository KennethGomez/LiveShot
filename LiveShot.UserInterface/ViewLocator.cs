// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using LiveShot.UserInterface.ViewModels;

namespace LiveShot.UserInterface
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            string? fullName = data.GetType().FullName;

            if (fullName == null) 
                return new TextBlock {Text = "Not Found"};

            string name = fullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null) 
                return (Control) Activator.CreateInstance(type)!;

            return new TextBlock {Text = "Not Found: " + name};

        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}