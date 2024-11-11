// See https://aka.ms/new-console-template for more information
using System.Formats.Tar;
using System.IO;
using ThermoBiz = ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.RawFileReader;

if ( args.Length != 1 || !Directory.Exists(args[0]))
{
    Console.WriteLine("Incorrect arguments, please supply folder path");
}
else
{
    string FolderPath =args[0];
    RawRenameFolder(FolderPath);
}

void RawRenameFolder(string folderPath)
{
    string[] Files = Directory.GetFiles(folderPath, "*.raw");
    foreach (string file in Files)
    {
        RawRenameFile(file);
    }
}

void RawRenameFile(string file)
{
    IRawDataPlus rawFile = RawFileReaderAdapter.FileFactory(file);
    if (!rawFile.IsOpen)
    {
        Console.WriteLine(" RawFile Error: File could not be opened: " + file);
        Console.WriteLine(rawFile.FileError.WarningMessage);
        Console.WriteLine(rawFile.FileError.ErrorMessage);
        Console.WriteLine(rawFile.FileError.ErrorCode);
        throw new IOException("Failed to open RAW file.");
    }
    if (rawFile.IsError)
    {
        Console.WriteLine(" RawFile Error: reader error: " + file);
        throw new IOException("Error while opening RAW file.");
    }
    rawFile.SelectInstrument(ThermoBiz.Device.MS, 1);
    rawFile.IncludeReferenceAndExceptionData = true;

    string currentFileName = rawFile.FileName;
    string sampleID = rawFile.SampleInformation.SampleId; 
    
    if (currentFileName.Contains(sampleID))
    {
        rawFile.Dispose();
        return;
    }
    else
    {
        string abridgedName = currentFileName.Remove(currentFileName.Length - 4, 4);
        string newFileName = abridgedName + "_" + sampleID + ".raw";
        rawFile.Dispose();

        File.Move(currentFileName, newFileName);
    }

}