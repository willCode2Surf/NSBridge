using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.IsolatedStorage;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Security.AccessControl;


namespace NSB.Utils
{
	/// <summary>
	/// 
	/// </summary>
	public class IOUtils
	{
		/// <summary>
		/// 
		/// </summary>
		public static FileStream OpenFile(string path, FileMode mode)
		{
			return File.Open(path, mode);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static FileStream OpenFileStreamRead(string path)
		{
			return File.OpenRead(path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static StreamReader OpenFileStreamText(string path)
		{
			return File.OpenText(path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static FileStream OpenFileStreamWrite(string path)
		{
			return File.OpenWrite (path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static void FileAppendAllText(string path, string contents)
		{
			File.AppendAllText(path, contents);

		}

		/// <summary>
		/// 
		/// </summary>
		public static byte[] ReadAllBytes(string path)
		{
			return File.ReadAllBytes(path);

		}
		/// <summary>
		/// 
		/// </summary>
		public static string[] ReadAllLines(string path)
		{
			return File.ReadAllLines(path);

		}
		/// <summary>
		/// 
		/// </summary>
		public static string ReadAllText(string path)
		{
			return File.ReadAllText(path);

		}
		/// <summary>
		/// 
		/// </summary>
		public static void ReplaceFromBackup(string source, string dest, string backup)
		{
			File.Replace (source, dest, backup);

		}



		/// <summary>
		/// 
		/// </summary>
		public static void FileAppendAllLines(string path, IEnumerable<string> contents) 
		{
			File.AppendAllLines (path, contents);
		}
		/// <summary>
		/// 
		/// </summary>
		public static void CopyFile(string sourcePath, string destinationPath)
		{
			File.Copy(sourcePath, destinationPath);

		}
		/// <summary>
		/// 
		/// </summary>
		public static void WriteBinary(string path, byte[] binary)
		{
			File.WriteAllBytes (path, binary);
		}
		/// <summary>
		/// 
		/// </summary>
		public static void WriteContents(string path, string[] contents)
		{
			File.WriteAllLines(path, contents);
		}
		/// <summary>
		/// 
		/// </summary>
		public static void WriteAllText(string path, string contents)
		{
			File.WriteAllText (path, contents);                 	

		}
		/// <summary>
		/// 
		/// </summary>
		public static bool FileExists(string path)
		{
			return File.Exists (path);

		}
		/// <summary>
		/// 
		/// </summary>
		public static void DeleteFile(string path)
		{
			File.Delete (path);

		}	
		/// <summary>
		/// 
		/// </summary>
		public static DirectoryInfo CreateDirectory(string path)
		{
			return Directory.CreateDirectory (path);

		} 	
		/// <summary>
		/// 
		/// </summary>
		public static void DeleteDirectory(string path)
		{
			Directory.Delete (path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static IEnumerable<string> EnumerateDirectories(string path)
		{
			return Directory.EnumerateDirectories (path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static IEnumerable<string> EnumerateFiles(string path)
		{
			return Directory.EnumerateFiles (path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static IEnumerable<string> EnumerateAllEntries(string path)
		{
			return Directory.EnumerateFileSystemEntries (path);


		}
		/// <summary>
		/// 
		/// </summary>
		public static bool DirectoryExists(string path)
		{
			return Directory.Exists (path);

		}
		/// <summary>
		/// 
		/// </summary>
		public static string[] GetDirectories(string path)
		{
			return Directory.GetDirectories (path);
		} 

		/// <summary>
		/// 
		/// </summary>
		public static string GetDirectoryRoot(string path)
		{
			return Directory.GetDirectoryRoot (path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static string[] GetDirectoryFiles(string path)
		{
			return Directory.GetFiles (path);

		}
		/// <summary>
		/// 
		/// </summary>
		public static string[] GetAllFileEntries(string path)
		{
			return Directory.GetFileSystemEntries (path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static string[] GetLogicalDrives()
		{
			return Directory.GetLogicalDrives ();
		}
		/// <summary>
		/// 
		/// </summary>
		public static DirectoryInfo GetParent(string path)
		{
			return Directory.GetParent (path);

		}
		/// <summary>
		/// 
		/// </summary>
		public static void SetDirectoryAccessControl(string path, DirectorySecurity security)
		{
			Directory.SetAccessControl (path, security);
		}

		/// <summary>
		///  Set's the applications current workibng directory.
		/// </summary>
		public static void SetCurrentDirectory(string path)
		{
			Directory.SetCurrentDirectory (path);
		}
		/// <summary>
		/// 
		/// </summary>
		public static DirectorySecurity GetDirectoryAccessControl(string path){


			return Directory.GetAccessControl (path);
		}

		public static void ChangePathExtension(string path, string extension)
		{
			Path.ChangeExtension (path, extension);
		}


		public static string CombinePaths(string path1, string path2)
		{
			return Path.Combine(path1, path2);
		}

		public static string GetPathDirectoryName(string path)
		{
			return Path.GetDirectoryName (path);
		}

		public static string GetPathExtension(string path)
		{
			return Path.GetExtension (path);
		}

		public static string GetPathFileName(string path)
		{
			return Path.GetFileName (path);
		}

		public static string GetPathFileNameWithoutExtension(string path)
		{
			return Path.GetFileNameWithoutExtension (path);
		}
		public static string GetPathFullPath(string path)
		{
			return Path.GetFullPath (path);
		}

		public static string GetRandomFileName()
		{
			return Path.GetRandomFileName();
		}
		public static string GetTempFileName()
		{
			return Path.GetTempFileName ();
		}
		public static string GetTempPath()
		{
			return Path.GetTempPath ();		
		}
		public static bool PathHasExtension(string path)
		{
			return Path.HasExtension (path);		
		}
		public static bool IsPathRooted(string path)
		{
			return Path.IsPathRooted(path);	

		}
		public static MemoryMappedFile CreateMemoryMappedFileFromFile(string path)
		{
			return MemoryMappedFile.CreateFromFile (path);
		}

		public static MemoryMappedFile CreateNewMemoryMappedFile(string mapName, long capactity)
		{
			return MemoryMappedFile.CreateNew (mapName, capactity);
		}
		public static MemoryMappedFile  CreateOrOpenMemoryMappedFile(string mapName, long capactity)
		{
			return MemoryMappedFile.CreateOrOpen(mapName, capactity);
		}

		public static MemoryMappedFile  OpenMemoryMappedFile(string mapName)
		{
			return MemoryMappedFile.OpenExisting(mapName);
		}

	}
}