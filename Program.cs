using ExifLibrary;

// string file = Environment.GetCommandLineArgs()[0];

// So that decimal points are dots etc.
System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

string[] filePaths = Directory.GetFiles(@"f:\icloud\", "IMG_*.mov");
foreach (var filePath in filePaths)
{
    Console.WriteLine($"Processing  -> {filePath}");

    var ext = Path.GetExtension(filePath);
    var fileWithoutExtension = Path.GetFullPath(filePath).Substring(0, filePath.Length-ext.Length);

    var sourceTagFile = fileWithoutExtension + ".jpeg";

    if (!File.Exists(sourceTagFile))
    {
        sourceTagFile = fileWithoutExtension + ".jpg";
        if (!File.Exists(sourceTagFile))
            continue;
    }

    var sourceTagImage = ImageFile.FromFile(sourceTagFile);

    string strCmdText = string.Empty;

    var dateTimeTag = sourceTagImage.Properties.Get<ExifDateTime>(ExifTag.DateTimeOriginal);
    if (dateTimeTag != null)
        strCmdText += $" -DateTimeOriginal=\"{dateTimeTag.Value.ToString("yyyy:MM:dd hh:mm:ss")}\" ";

    var altitudeTag = sourceTagImage.Properties.Get<ExifURational>(ExifTag.GPSAltitude);
    if (altitudeTag != null)
        strCmdText += " -GPSAltitude=" + altitudeTag.ToFloat();

    var altitudeRefTag = sourceTagImage.Properties.Get<ExifEnumProperty<GPSAltitudeRef>>(ExifTag.GPSAltitudeRef);
    if (altitudeRefTag != null)
         strCmdText += " -GPSAltitudeRef=" + (altitudeRefTag == GPSAltitudeRef.AboveSeaLevel ? 0 : 1);

    var latitudeTag = sourceTagImage.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLatitude);
    if (latitudeTag != null)
        strCmdText += " -GPSLatitude=" + latitudeTag;

    var latitudeRefTag = sourceTagImage.Properties.Get<ExifEnumProperty<GPSLatitudeRef>>(ExifTag.GPSLatitudeRef);
    if (latitudeRefTag != null)
        strCmdText += " -GPSLatitudeRef=" + (latitudeRefTag == GPSLatitudeRef.North ? "N" : "S");

    var longtitudeTag = sourceTagImage.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLongitude);
    if (longtitudeTag != null)
        strCmdText += " -GPSLongitude=" + longtitudeTag;

    var longtitudeRefTag = sourceTagImage.Properties.Get<ExifEnumProperty<GPSLongitudeRef>>(ExifTag.GPSLongitudeRef);
    if (longtitudeRefTag != null)
        strCmdText += " -GPSLongitudeRef=" + (longtitudeRefTag == GPSLongitudeRef.East ? "E" : "W");

    strCmdText += " -overwrite_original " + filePath;
    Console.WriteLine("Using the exiftool");
    Console.WriteLine("\t" + strCmdText);

    var process = System.Diagnostics.Process.Start(@"f:\exiftool.exe",strCmdText);
    process.WaitForExit();

    Console.WriteLine("");
}