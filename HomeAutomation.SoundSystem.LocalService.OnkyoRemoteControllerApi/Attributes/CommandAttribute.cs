using System;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Attributes
{
    public sealed class CommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Regex { get; set; }
        public string DisplayName { get; set; }

        public CommandAttribute(string name)
        {
            Name = name;
        }

        public CommandAttribute(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}
