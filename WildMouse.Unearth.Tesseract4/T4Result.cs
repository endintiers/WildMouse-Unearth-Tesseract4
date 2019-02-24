using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildMouse.Unearth.Tesseract4
{
    public class T4Result
    {
        public float MeanConfidence { get; set; }
        public string Text { get; set; }
        public string JSON { get; set; }
        public string HOCR { get; set; }
        public string XHTML { get; set; }
    }
}
