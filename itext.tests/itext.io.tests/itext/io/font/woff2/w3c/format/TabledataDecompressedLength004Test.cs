using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataDecompressedLength004Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-decompressed-length-004";
        }

        protected internal override String GetTestInfo() {
            return "The transformed length of the glyf table in the directory is decreased by 1, making the decompressed length of the table data greater than the sum of transformed table lengths.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
