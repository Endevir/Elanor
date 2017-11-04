using System.IO;
using System.Linq;

namespace shared
{
    public sealed class ClientDat
    {
        public static readonly ClientDat Undefined = new ClientDat(-1, "N/A");
        public static readonly ClientDat LocalEnglish = new ClientDat(0, "client_local_English.dat");
        public static readonly ClientDat General = new ClientDat(1, "client_general.dat");
        public static readonly ClientDat Sound = new ClientDat(2, "client_sound.dat");
        public static readonly ClientDat Surface = new ClientDat(3, "client_surface.dat");
        public static readonly ClientDat Highres = new ClientDat(4, "client_highres.dat");

        public readonly int Id;
        public readonly string Name;
        public static string Directory;

        private ClientDat(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public string GetPath(string prefix)
        {
            return Path.Combine(prefix, Name);
        }

        public string FullPath => Path.Combine(Directory, Name);

        public bool Exists => File.Exists(FullPath);

        public static ClientDat WhoIs(string filename)
        {
            var clinetDat = Undefined;
            var safeFileName = Path.GetFileName(filename);          
            foreach (var dat in Collection)
            {
                if (string.CompareOrdinal(dat.Name.ToLowerInvariant(), safeFileName?.ToLowerInvariant()) == 0)
                {
                    clinetDat = dat;
                }
            }

            Directory = Path.GetDirectoryName(filename);

            return clinetDat;
        }

        public static ClientDat GetById(int id)
        {
            return Collection.FirstOrDefault(cdat => cdat.Id == id);
        }

        public static readonly ClientDat[] Collection = {LocalEnglish, General, Sound, Surface, Highres};
    }
}
