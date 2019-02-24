using System;
using System.Drawing;
using WildMouse.Unearth.Tesseract4;

namespace WildMouse.Unearth.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Simple Version
            var jabberwock = Image.FromFile(Environment.CurrentDirectory + @"\Images\Jabberwock.JPG");
            var text = TesseractHelper.OCRImageText(jabberwock);
            Console.WriteLine(text.TrimEnd());

            Console.WriteLine("---------------------------------------------------------------------");

            // More complex version showing JSON result. You can use getHOCR or getXHTML as well 
            var sorry = Image.FromFile(Environment.CurrentDirectory + @"\Images\ProjectBackground.png");
            var result = TesseractHelper.OCRImageResult(sorry, getJSON: true);
            Console.WriteLine(result.Text);
            Console.WriteLine();
            Console.WriteLine($"Mean Confidence: {result.MeanConfidence}");
            Console.WriteLine();
            Console.WriteLine(result.JSON);
            Console.WriteLine();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}
