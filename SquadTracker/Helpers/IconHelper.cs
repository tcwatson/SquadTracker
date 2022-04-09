namespace Torlando.SquadTracker.Helpers
{
    public static class IconHelper
    {
        public static string GetIconPath(uint elite, uint core, bool useColoredIcons)
        {
            var profName = Specialization.GetEliteName(elite, core);
            var folder = useColoredIcons ? "professions_colored" : "professions_monochrome";
            return @$"{folder}\{profName}.png";
        }
    }
}
