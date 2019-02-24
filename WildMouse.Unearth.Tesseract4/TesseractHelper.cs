using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace WildMouse.Unearth.Tesseract4
{
    public static class TesseractHelper
    {
        public static string OCRImageText(Image theImage)
        {
            if (theImage is Bitmap)
            {
                var bmp = (Bitmap)theImage;
                return OCRImageText(bmp);
            }
            else
            {
                throw new ApplicationException("OCRImageWithTesseract: Image must be Bitmap");
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public static string OCRImageText(Bitmap theBmp)
        {
            var ocrText = string.Empty;
            try
            {
                using (var engine = new Tesseract.TesseractEngine(@".\tessdata\", "eng", Tesseract.EngineMode.LstmOnly))
                {
                    var pix = Tesseract.PixConverter.ToPix(theBmp);
                    using (var tessPage = engine.Process(pix))
                    {
                        ocrText = tessPage.GetText();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is AccessViolationException || ex is InvalidOperationException)
                {
                    Trace.TraceError("Tesseract Error: " + ex.Message);
                }
                else
                {
                    throw;
                }
            }
            return ocrText;
        }

        public static T4Result OCRImageResult(Image theImage, bool getHOCR = false, bool getXHTML = false, bool getJSON = false)
        {
            if (theImage is Bitmap)
            {
                var bmp = (Bitmap)theImage;
                return OCRImageResult(bmp, getHOCR, getXHTML, getJSON);
            }
            else
            {
                throw new ApplicationException("OCRImageWithTesseract: Image must be Bitmap");
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public static T4Result OCRImageResult(Bitmap theBmp, bool getHOCR = false, bool getXHTML = false, bool getJSON = false)
        {
            var result = new T4Result();
            try
            {
                using (var engine = new Tesseract.TesseractEngine(@".\tessdata\", "eng", Tesseract.EngineMode.LstmOnly))
                {
                    var pix = Tesseract.PixConverter.ToPix(theBmp);
                    using (var tessPage = engine.Process(pix, Tesseract.PageSegMode.Auto))
                    {
                        result.MeanConfidence = tessPage.GetMeanConfidence();
                        result.Text = tessPage.GetText();
                        if (getHOCR)
                            result.HOCR = tessPage.GetHOCRText(1);
                        if (getXHTML)
                            result.XHTML = tessPage.GetHOCRText(1, true);
                        if (getJSON)
                        {
                            result.JSON = GetJSON(tessPage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is AccessViolationException || ex is InvalidOperationException)
                {
                    Trace.TraceError("Tesseract Error: " + ex.Message);
                }
                else
                {
                    throw;
                }
            }
            return result;

        }

        /// <summary>
        /// This could be improved, but at least it shows how to use the page iterator
        /// </summary>
        /// <param name="tessPage">A Tesseract 4 page</param>
        /// <returns>JSON Representation of the page</returns>
        private static string GetJSON(Tesseract.Page tessPage)
        {
            var result = string.Empty;
            var structuredResult = new StructuredOCRResult() { Blocks = new List<StructuredOCRResult.Block>() };
            StructuredOCRResult.Block thisBlock = null;
            StructuredOCRResult.Para thisPara = null;
            StructuredOCRResult.TextLine thisLine = null;
            var iter = tessPage.GetIterator();
            do
            {
                if (iter.IsAtBeginningOf(Tesseract.PageIteratorLevel.Block))
                {
                    thisBlock = new StructuredOCRResult.Block()
                    { Paras = new List<StructuredOCRResult.Para>() };
                    structuredResult.Blocks.Add(thisBlock);
                }
                if (iter.IsAtBeginningOf(Tesseract.PageIteratorLevel.Para))
                {
                    thisPara = new StructuredOCRResult.Para()
                    { TextLines = new List<StructuredOCRResult.TextLine>() };
                    thisBlock.Paras.Add(thisPara);
                }
                if (iter.IsAtBeginningOf(Tesseract.PageIteratorLevel.TextLine))
                {
                    thisLine = new StructuredOCRResult.TextLine()
                    { Words = new List<string>() };
                    thisPara.TextLines.Add(thisLine);
                }
                var thisWord = iter.GetText(Tesseract.PageIteratorLevel.Word);
                if (thisLine == null)
                {
                    // Not structured as we expected - abandon
                    result = "{\"Error\":\"no line found\"}";
                    break;
                }
                thisLine.Words.Add(thisWord);
            } while (iter.Next(Tesseract.PageIteratorLevel.Word));

            if (string.IsNullOrWhiteSpace(result))
                result = JsonConvert.SerializeObject(structuredResult);

            return result;
        }
    }
}
