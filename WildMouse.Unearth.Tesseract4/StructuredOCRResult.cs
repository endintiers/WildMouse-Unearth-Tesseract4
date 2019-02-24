using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildMouse.Unearth.Tesseract4
{
    public class StructuredOCRResult
    {
        public List<Block> Blocks;
        public class Block
        {
            public List<Para> Paras;
        }
        public class Para
        {
            public List<TextLine> TextLines;
        }
        public class TextLine
        {
            public List<string> Words;
        }
    }
}
