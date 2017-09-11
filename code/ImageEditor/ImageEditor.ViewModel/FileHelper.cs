using System.Drawing;
using System.IO;
using ImageEditor.ViewModel.model;
using Microsoft.Win32;

namespace ImageEditor.ViewModel
{
    public class FileHelper
    {
        private const string Filter =
            "JPEG (*.jpg, *.jpeg) | *.jpg; *.jpeg; |PNG ( *.png) | *.png |ImageEditor  (*.ief) | *.ief";


        //returns file path
        public static string ChooseFile()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = Filter
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }

            return null;
        }

        public static void SaveCanvasToFile(Canvas canvas)
        {
            var filePath = canvas.FilePath;
            if (string.IsNullOrEmpty(filePath)) return;
            var ext = Path.GetFileName(filePath)?.Split('.')[1];

            if (ext.ToLower().Equals("ief"))
            {
                SerializeCanvas(canvas);
                return;
            }

            var bitmap = canvas.ToBitmap();

            switch (ext)
            {
                case "jpeg":
                case "jpg":
                    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case "png":
                    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    break;
            }
        }

        public static void SerializeCanvas(Canvas canvas)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(canvas.Width);
                    writer.Write(canvas.Height);
                    writer.Write(canvas.Layers.Count);
                    foreach (var layer in canvas.Layers)
                    {
                        writer.Write(layer.X);
                        writer.Write(layer.Y);
                        writer.Write(layer.Height);
                        writer.Write(layer.Width);
                        writer.Write((double)layer.Opacity);
                        writer.Write(layer.Name);
                        var byteArray = layer.Raw.ToByteArray();
                        writer.Write(byteArray.Length);
                        writer.Write(byteArray);
                        writer.Write(layer.IsSelected);
                        writer.Write(layer.IsVisible);
                    }
                }
                
                File.WriteAllBytes(canvas.FilePath, stream.ToArray());
            }
        }

        public static Canvas DeserializeCanvasFromFile(string filePath)
        {
            Canvas canvas;
            var bytes = File.ReadAllBytes(filePath);
            
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    canvas = new Canvas(reader.ReadInt32(), reader.ReadInt32());

                    var length = reader.ReadInt32();
                    for (int i = 0; i < length; i++)
                    {
                        double x = reader.ReadDouble();
                        double y = reader.ReadDouble();
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();
                        var layer = new Layer(x, y, width, height)
                        {
                            Opacity = (float) reader.ReadDouble(),
                            Name = reader.ReadString(),
                            Raw = reader.ReadBytes(reader.ReadInt32()).ToFloatArray(),
                            IsSelected = reader.ReadBoolean(),
                            IsVisible = reader.ReadBoolean()
                        };
                        canvas.AddLayer(layer);
                    }
                }
            }
            
            return canvas;
        }

        public static Canvas ReadCanvasFromFile(string filePath)
        {
            var ext = Path.GetFileName(filePath)?.Split('.')[1];
            if (ext == null) return null;
            
            if (ext.ToLower().Equals("ief"))
            {
                return DeserializeCanvasFromFile(filePath);;
            }

            return new Bitmap(filePath).ToCanvas(filePath);
        }
    }
}