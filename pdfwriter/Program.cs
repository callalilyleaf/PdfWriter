
using System;
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
    public static void FillFormField(string inputPdfPath, string outputPdfPath, string fieldName, string inputFileName)
    {
        string cleanName = "";
        try
        {   
            inputFileName = inputFileName.Replace(" ", ""); // Remove spaces
            string personNameWPdf = inputFileName.Replace("(", "").Replace(")", ""); // Remove parentheses from the field value
            string personNameWLevel = personNameWPdf.Remove(personNameWPdf.Length - 4); // Replace spaces with underscores
            Console.WriteLine("Certification person Name w/ Level Loaded: " + personNameWLevel);
            Console.WriteLine("");
            string level = personNameWLevel.Last().ToString(); // get the certificate level

            string fullName = personNameWLevel.Remove(personNameWLevel.Length - 1); // get the clean Name, ready to put it
            cleanName = NameSplitter.AddSpaceBeforeLastName(fullName);
            NameSplitter.Test();
            Console.WriteLine("Inserting Student Name: " + cleanName + " for " + level + " level certificate.");
            
            if (level == "B")
            {
                inputPdfPath = "Certificate PDF writer/pdfwriter/certificateTemplate/basic";
            }
            else if (level == "S")
            {
                inputPdfPath = "Certificate PDF writer/pdfwriter/certificateTemplate/silver";
            }
            else if (level == "G")
            {
                inputPdfPath = "Certificate PDF writer/pdfwriter/certificateTemplate/gold";
            }
            else 
            {
                inputPdfPath = "Certificate PDF writer/pdfwriter/certificateTemplate/basic"; // default to basic if no level is found
                throw new Exception("Invalid level detected in the filename. Please ensure the filename contains with (B), (S), or (G).");
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
            Console.WriteLine($"An error occurred: {e.Message}");
        }
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
                FillFormField(inputPdfPath, outputPdfPath, fieldName, inputFileName);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }


    public static void Main(string[] args)
    {
        string inputDirectory = "C:/Users/calla/桌面/programming/Career Center/Certificate PDF writer/pdfwriter/originalpdf"; // Replace with your input directory
        string outputDirectory = "C:/Users/calla/桌面/programming/Career Center/Certificate PDF writer/pdfwriter/correctedpdf"; // Replace with your output directory
        string fieldName = "PlaceNameHere"; // Replace with the actual name of the form field

        ProcessPdfFilesInDirectory(inputDirectory, outputDirectory, fieldName);
    }
}