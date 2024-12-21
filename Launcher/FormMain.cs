using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Launcher;

public class FormMain : Form
{
	private string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

	private IContainer components;

	private Button button1;

	public FormMain()
	{
		InitializeComponent();
	}

	private void button1_Click(object sender, EventArgs e)
	{
		string[] fileSystemEntries = Directory.GetFileSystemEntries(path, "*.cpn");
		foreach (string text in fileSystemEntries)
		{
			repackSCN("x \"" + text + "\" \"" + Path.Combine(path, Path.GetFileNameWithoutExtension(text)) + "\"");
		}
		string[] fileSystemEntries2 = Directory.GetFileSystemEntries(path, "*.cpx");
		foreach (string text2 in fileSystemEntries2)
		{
			repackSCN("x \"" + text2 + "\" \"" + Path.Combine(path, Path.GetFileNameWithoutExtension(text2)) + "\"");
		}
		processScr("*.scn");
		processScr("*.scx");
		processCmp("*.cpn", "*.scn");
		processCmp("*.cpx", "*.scx");
	}

	private void processScr(string ext)
	{
		string[] fileSystemEntries = Directory.GetFileSystemEntries(path, ext, SearchOption.AllDirectories);
		foreach (string text in fileSystemEntries)
		{
			List<byte> list = new List<byte>(File.ReadAllBytes(text));
			int num = BitConverter.ToInt32(new byte[4]
			{
				list[4],
				list[5],
				list[6],
				list[7]
			}, 0) + 8;
			List<byte> list2 = new List<byte>(list);
			list2.RemoveRange(0, num);
			list2 = new List<byte>(Decompress(list2.ToArray()));
			int count = list2.Count;
			for (int j = 0; j < count; j++)
			{
				if (j + 20 >= count || list2[j] != 0 || list2[j + 1] != 115 || list2[j + 8] != 111 || list2[j + 9] != 32 || list2[j + 10] != 100 || list2[j + 19] != 121 || list2[j + 20] != 0)
				{
					continue;
				}
				j += 21;
				int num2 = BitConverter.ToInt32(new byte[4]
				{
					list2[j],
					list2[j + 1],
					list2[j + 2],
					list2[j + 3]
				}, 0);
				List<byte> list3 = new List<byte>(list2);
				list2.RemoveRange(j + 4, num2);
				list3.RemoveRange(j + 4 + num2, count - (j + 4 + num2));
				list3.RemoveRange(0, j + 4);
				File.WriteAllBytes(text + ".tmp", list3.ToArray());
				List<string> list4 = new List<string>(File.ReadAllLines(text + ".tmp"));
				File.Delete(text + ".tmp");
				int count2 = list4.Count;
				for (int k = 0; k < count2; k++)
				{
					if (list4[k].IndexOf("ability-to-maintain-distance", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						list4[k] = "\t(set-difficulty-parameter ability-to-maintain-distance 100)";
					}
				}
				File.WriteAllLines(text + ".tmp", list4);
				list3 = new List<byte>(File.ReadAllBytes(text + ".tmp"));
				File.Delete(text + ".tmp");
				list2.InsertRange(j + 4, list3);
				byte[] bytes = BitConverter.GetBytes(list3.Count);
				list2[j] = bytes[0];
				list2[j + 1] = bytes[1];
				list2[j + 2] = bytes[2];
				list2[j + 3] = bytes[3];
				list2 = new List<byte>(Compress(list2.ToArray()));
				list.RemoveRange(num, list.Count - num);
				list2.InsertRange(0, list);
				File.WriteAllBytes(text, list2.ToArray());
				break;
			}
		}
	}

	private void processCmp(string ext1, string ext2)
	{
		string[] fileSystemEntries = Directory.GetFileSystemEntries(path, ext1);
		foreach (string text in fileSystemEntries)
		{
			string text2 = "c \"" + text + "\" " + Path.GetFileNameWithoutExtension(text);
			string[] fileSystemEntries2 = Directory.GetFileSystemEntries(Path.Combine(path, Path.GetFileNameWithoutExtension(text)), ext2);
			foreach (string text3 in fileSystemEntries2)
			{
				text2 = text2 + " \"" + text3 + "\"";
			}
			repackSCN(text2);
		}
	}

	private void repackSCN(string args)
	{
		Process process = new Process();
		process.StartInfo.FileName = Path.Combine(path, "rge_campaign.exe");
		process.StartInfo.WorkingDirectory = path;
		process.StartInfo.Arguments = args;
		process.Start();
		process.WaitForExit();
	}

	public static byte[] Compress(byte[] data)
	{
		byte[] array = null;
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
		{
			deflateStream.Write(data, 0, data.Length);
		}
		return memoryStream.ToArray();
	}

	public static byte[] Decompress(byte[] data)
	{
		byte[] array = null;
		using MemoryStream memoryStream = new MemoryStream();
		using (MemoryStream stream = new MemoryStream(data))
		{
			using DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
			deflateStream.CopyTo(memoryStream);
		}
		return memoryStream.ToArray();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher.FormMain));
		this.button1 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.button1.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.button1.Location = new System.Drawing.Point(12, 12);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(130, 35);
		this.button1.TabIndex = 0;
		this.button1.Text = "Start";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		base.ClientSize = new System.Drawing.Size(154, 59);
		base.Controls.Add(this.button1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormMain";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "NoKiting";
		base.ResumeLayout(false);
	}
}
