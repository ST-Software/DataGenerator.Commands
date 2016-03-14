using Microsoft.Extensions.CommandLineUtils;

namespace DataGenerator.Commands
{
    public static class CommandOptionExtensions
    {
        public static void AssignDefault(this CommandOption option, string defaultValue)
        {
            if (!option.HasValue() && defaultValue != null)
            {
                option.Values.Add(defaultValue);
            }
        }
    }
}