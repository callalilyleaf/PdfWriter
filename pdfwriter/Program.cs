
using System;
using System.Text;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
 
using System;
using System.IO;
using iTextSharp.text.pdf;
using Org.BouncyCastle.Asn1.X509.SigI;

// 3/28 Notes:
// 1. Insert the filename of the text entry form: "Place Name here"s
// 2. Find out if the reader and writer will actually read and change the text of all pdfs
//    Create the testing folder and files before actually run this

public class PdfWriter
{   
    public static void FillFormField(string outputPdfPath, string fieldName, string inputFileName)
    {
        string cleanName;
        string inputPdfPath;
        try
        {   
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            inputFileName = inputFileName.Replace(" ", ""); // Remove spaces
            string personNameWPdf = inputFileName.Replace("(", "").Replace(")", ""); // Remove parentheses from the field value
            string personNameWLevel = personNameWPdf.Remove(personNameWPdf.Length - 4); // Replace spaces with underscores
            Console.WriteLine("");
            Console.WriteLine("Certification person Name w/ Level Loading: " + personNameWLevel);
            string level = personNameWLevel.Last().ToString(); // get the certificate level

            string fullName = personNameWLevel.Remove(personNameWLevel.Length - 1); // get the clean Name, ready to put it
            cleanName = NameSplitter.AddSpaceBeforeLastName(fullName);

            cleanName = SanitizeName(cleanName);
            // NameSplitter.Test();
            Console.WriteLine("Inserting Student Name: " + cleanName + " for " + level + " level certificate.");

            // Get the base directory of the application
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            if (level == "B")
            {
                inputPdfPath = Path.Combine(baseDir, "certificateTemplate", "basic", "BasicCertificate-Editable.pdf");
            }
            else if (level == "S")
            {
                inputPdfPath = Path.Combine(baseDir, "certificateTemplate", "silver", "SilverCertificate-Editable.pdf");
            }
            else if (level == "G")
            {
                inputPdfPath = Path.Combine(baseDir, "certificateTemplate", "gold", "GoldCertificate-Editable.pdf");
            }
            else 
            {
                throw new Exception("Invalid level detected in the filename. Please ensure the filename contains with (B), (S), or (G).");
            }

            if (!File.Exists(inputPdfPath))
            {
                throw new FileNotFoundException($"Template PDF file not found: '{inputPdfPath}");
            }


            using (PdfReader reader = new PdfReader(inputPdfPath)) // use different templates here
            {
                using (FileStream fs = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, fs))
                    {
                        AcroFields fields = stamper.AcroFields;
                        
                        // according to the level, use different templates
                        fields.SetField(fieldName, cleanName);
                        
                        // Flatten the form to prevent further editing (optional)
                        stamper.FormFlattening = true;
                    }
                }
            }
            Console.WriteLine($"Field '{fieldName}' in '{inputPdfPath}' filled with '{cleanName}' and saved to '{outputPdfPath}'");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while inserting names: {e.Message}");
            Console.WriteLine($"Stack Trace: {e.StackTrace}");
        }
    }

    private static string SanitizeName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
            
        StringBuilder sb = new StringBuilder();
        foreach (char c in name)
        {
            // Only keep ASCII characters and common punctuation
            if ((c >= 32 && c <= 126) || c == ' ')
            {
                sb.Append(c);
            }
            else
            {
                // Replace non-ASCII characters with their closest ASCII equivalent
                if (c == 'é' || c == 'è' || c == 'ê' || c == 'ë') sb.Append('e');
                else if (c == 'à' || c == 'á' || c == 'â' || c == 'ã' || c == 'ä') sb.Append('a');
                else if (c == 'ì' || c == 'í' || c == 'î' || c == 'ï') sb.Append('i');
                else if (c == 'ò' || c == 'ó' || c == 'ô' || c == 'õ' || c == 'ö') sb.Append('o');
                else if (c == 'ù' || c == 'ú' || c == 'û' || c == 'ü') sb.Append('u');
                else sb.Append('?'); // Replace other special characters with a question mark
            }
        }
        return sb.ToString();
    }

    public static void ProcessPdfFilesInDirectory(string inputDirectory, string outputDirectory, string fieldName)
    {
        try
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string[] pdfFiles = Directory.GetFiles(inputDirectory, "*.pdf");

            foreach (string inputPdfPath in pdfFiles)
            {
                string inputFileName = Path.GetFileName(inputPdfPath);
                string outputPdfPath = Path.Combine(outputDirectory, inputFileName);
                FillFormField(outputPdfPath, fieldName, inputFileName);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while loading pdf paths: {e.Message}");
        }
    }


    public static void Main(string[] args)
    {
        string inputDirectory = "../originalpdf"; // Replace with your input directory for original PDFs
        string outputDirectory = "../correctedpdf"; // Replace with your output directory for modified PDFs
        string fieldName = "PlaceNameHere"; // Replace with the actual name of the form field

        ProcessPdfFilesInDirectory(inputDirectory, outputDirectory, fieldName);
    }
}
