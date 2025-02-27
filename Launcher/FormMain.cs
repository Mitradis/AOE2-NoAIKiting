using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Launcher
{
    public partial class FormMain : Form
    {
        string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public FormMain()
        {
            InitializeComponent();
        }

        void button1_Click(object sender, EventArgs e)
        {
            foreach (string line in Directory.GetFileSystemEntries(path, "*.cpn"))
            {
                repackSCN("x \"" + line + "\" \"" + Path.Combine(path, Path.GetFileNameWithoutExtension(line)) + "\"");
            }
            foreach (string line in Directory.GetFileSystemEntries(path, "*.cpx"))
            {
                repackSCN("x \"" + line + "\" \"" + Path.Combine(path, Path.GetFileNameWithoutExtension(line)) + "\"");
            }
            processScr("*.scn");
            processScr("*.scx");
            processCmp("*.cpn", "*.scn");
            processCmp("*.cpx", "*.scx");
        }

        void processScr(string ext)
        {
            foreach (string line in Directory.GetFileSystemEntries(path, ext, SearchOption.AllDirectories))
            {
                List<byte> data1 = new List<byte>(File.ReadAllBytes(line));
                int head = BitConverter.ToInt32(new byte[] { data1[4], data1[5], data1[6], data1[7] }, 0) + 8;
                List<byte> data2 = new List<byte>(data1);
                data2.RemoveRange(0, head);
                data2 = new List<byte>(Decompress(data2.ToArray()));
                int count = data2.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i + 20 < count && data2[i] == 0 && data2[i + 1] == 115 && data2[i + 8] == 111 && data2[i + 9] == 32 && data2[i + 10] == 100 && data2[i + 19] == 121 && data2[i + 20] == 0)
                    {
                        i += 21;
                        int length = BitConverter.ToInt32(new byte[] { data2[i], data2[i + 1], data2[i + 2], data2[i + 3] }, 0);
                        List<byte> file = new List<byte>(data2);
                        data2.RemoveRange(i + 4, length);
                        file.RemoveRange(i + 4 + length, count - (i + 4 + length));
                        file.RemoveRange(0, i + 4);
                        File.WriteAllBytes(line + ".tmp", file.ToArray());
                        List<string> list = new List<string>(File.ReadAllLines(line + ".tmp"));
                        File.Delete(line + ".tmp");
                        int count2 = list.Count;
                        for (int j = 0; j < count2; j++)
                        {
                            if (list[j].IndexOf("ability-to-maintain-distance", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                list[j] = "	(set-difficulty-parameter ability-to-maintain-distance 100)";
                            }
                        }
                        File.WriteAllLines(line + ".tmp", list);
                        file = new List<byte>(File.ReadAllBytes(line + ".tmp"));
                        File.Delete(line + ".tmp");
                        data2.InsertRange(i + 4, file);
                        byte[] newlength = BitConverter.GetBytes(file.Count);
                        data2[i] = newlength[0];
                        data2[i + 1] = newlength[0 + 1];
                        data2[i + 2] = newlength[0 + 2];
                        data2[i + 3] = newlength[0 + 3];
                        data2 = new List<byte>(Compress(data2.ToArray()));
                        data1.RemoveRange(head, data1.Count - head);
                        data2.InsertRange(0, data1);
                        File.WriteAllBytes(line, data2.ToArray());
                        break;
                    }
                }
            }
        }

        void processCmp(string ext1, string ext2)
        {
            foreach (string line in Directory.GetFileSystemEntries(path, ext1))
            {
                string arg = "c \"" + line + "\" " + Path.GetFileNameWithoutExtension(line);
                foreach (string file in Directory.GetFileSystemEntries(Path.Combine(path, Path.GetFileNameWithoutExtension(line)), ext2))
                {
                    arg += " \"" + file + "\"";
                }
                repackSCN(arg);
            }
        }

        void repackSCN(string args)
        {
            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(path, "rge_campaign.exe");
            process.StartInfo.WorkingDirectory = path;
            process.StartInfo.Arguments = args;
            process.Start();
            process.WaitForExit();
        }

        static byte[] Compress(byte[] data)
        {
            byte[] compressArray = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    deflateStream.Write(data, 0, data.Length);
                }
                compressArray = memoryStream.ToArray();
            }
            return compressArray;
        }

        static byte[] Decompress(byte[] data)
        {
            byte[] decompressedArray = null;
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                using (MemoryStream compressStream = new MemoryStream(data))
                {
                    using (DeflateStream deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(decompressedStream);
                    }
                }
                decompressedArray = decompressedStream.ToArray();
            }
            return decompressedArray;
        }
    }
}
