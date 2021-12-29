using System;

namespace LiveShot.API.Background.ContextOptions
{
    public interface IContextOption
    {
        public string Title { get; }
        public void OnClick(object? sender, EventArgs e);
    }
}