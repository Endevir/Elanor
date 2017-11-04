using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using shared;

namespace Xavian
{
    public class ExtractMarker
    {
        public ExtractMarker(Dictionary<PatchContentType, CheckBox> fileBoxes, Dictionary<PatchContentType, CheckBox> dbBoxes)
        {
            ToFile = fileBoxes.Where(cb => cb.Value.Checked).Select(cb => cb.Key).ToList();
            ToDatabase = dbBoxes.Where(cb => cb.Value.Checked).Select(cb => cb.Key).ToList();
            Overall = ToFile.Union(ToDatabase).ToList();
        }

        public List<PatchContentType> ToFile { get; }
        public List<PatchContentType> ToDatabase { get; }
        public List<PatchContentType> Overall { get; }
    }
}
