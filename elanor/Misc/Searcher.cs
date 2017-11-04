using Elanor.Properties;

namespace Elanor.Misc
{
    internal class Searcher
    {
        public static string LotroDirLookup()
        {
            var dir = Registry.LookupDisplayName(Resources.RegistryLookupKey32, Resources.RegistrySteamApp);

            return string.IsNullOrWhiteSpace(dir) ? Registry.LookupDisplayName(Resources.RegistryLookupKey64, Resources.RegistrySteamApp) : dir;
        }
    }
}
