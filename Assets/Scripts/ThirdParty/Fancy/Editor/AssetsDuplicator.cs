using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Fancy
{
public static class AssetsDuplicator
{
	private struct StringsPair
	{
		public string Old;
		public string New;
	}

	[MenuItem("Assets/Duplicate Assets")]
	public static void DuplicateAssets()
	{
		List<StringsPair> files = new List<StringsPair>();
		List<StringsPair> folders = new List<StringsPair>();

		foreach (Object obj in Selection.objects)
		{
			string oldPath = AssetDatabase.GetAssetPath(obj);
			string newPath = GenerateName(oldPath);

			if (AssetDatabase.CopyAsset(oldPath, newPath))
			{
				if (Directory.Exists(oldPath))
				{
					folders.Add(new StringsPair
					{
						Old = oldPath,
						New = newPath
					});
				}
				else if (File.Exists(oldPath))
				{
					files.Add(new StringsPair
					{
						Old = oldPath,
						New = newPath
					});
				}
			}
			else
			{
				Debug.LogError($"Can not copy asset\n{oldPath}\n{newPath}");
			}
		}

		foreach (StringsPair folder in folders)
		{
			EditFolderDependencies(folder);
		}

		foreach (StringsPair file in files)
		{
			EditFileDependencied(file.New, files, folders);
		}

		Selection.objects = LoadNewObjects(files, folders);
	}

	private static string GenerateFileName(string fullFileName)
	{
		int i = 1;
		string directory = Path.GetDirectoryName(fullFileName);
		string name = Path.GetFileNameWithoutExtension(fullFileName) + "{0}";
		string extension = Path.GetExtension(fullFileName);

		while (File.Exists(fullFileName))
		{
			fullFileName = Path.Combine(directory, string.Format(name, $" {i++}") + extension);
		}

		return fullFileName;
	}

	private static string GenerateFolderName(string oldFolder)
	{
		int i = 1;
		string newFolder = oldFolder;

		while (Directory.Exists(newFolder))
		{
			newFolder = $"{oldFolder} {i++}";
		}

		return newFolder;
	}

	private static string GenerateName(string oldName)
	{
		if (File.Exists(oldName))
		{
			return GenerateFileName(oldName);
		}

		return GenerateFolderName(oldName);
	}

	private static void EditFolderDependencies(StringsPair folder)
	{
		try
		{
			StartAssetsEditing();

			foreach (string file in Directory.GetFiles(folder.Old, "*.*", SearchOption.AllDirectories))
			{
				List<StringsPair> guids = GetFolderGuids(file, folder);
				string newFile = file.Replace(folder.Old, folder.New);
				ReplaceGuids(newFile, guids);
			}
		}
		finally
		{
			FinishAssetsEditing();
		}
	}

	private static void EditFileDependencied(string file, List<StringsPair> selectionFiles,
		List<StringsPair> selectionFolders)
	{
		try
		{
			StartAssetsEditing();
			List<StringsPair> guids = GetFileGuids(file, selectionFiles, selectionFolders);
			ReplaceGuids(file, guids);
		}
		finally
		{
			FinishAssetsEditing();
		}
	}

	private static void StartAssetsEditing()
	{
		AssetDatabase.StartAssetEditing();
	}

	private static void FinishAssetsEditing()
	{
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}

	private static List<StringsPair> GetFolderGuids(string file, StringsPair folder)
	{
		List<StringsPair> guids = new List<StringsPair>();

		foreach (string oldDependency in AssetDatabase.GetDependencies(file))
		{
			if (oldDependency == file.Replace("\\", "/"))
			{
				continue;
			}

			if (oldDependency.StartsWith(folder.Old + "/"))
			{
				string oldGuid = AssetDatabase.AssetPathToGUID(oldDependency);
				string newDependency = oldDependency.Replace(folder.Old, folder.New);
				string newGuid = AssetDatabase.AssetPathToGUID(newDependency);
				guids.Add(new StringsPair
				{
					Old = oldGuid,
					New = newGuid
				});
			}
		}

		return guids;
	}

	private static List<StringsPair> GetFileGuids(string file, List<StringsPair> selectionFiles,
		List<StringsPair> selectionFolders)
	{
		IEnumerable<StringsPair> guids = new List<StringsPair>();

		foreach (string oldDependency in AssetDatabase.GetDependencies(file))
		{
			if (oldDependency == file.Replace("\\", "/"))
			{
				continue;
			}

			foreach (StringsPair selectionFile in selectionFiles)
			{
				if (oldDependency == selectionFile.Old)
				{
					string oldGuid = AssetDatabase.AssetPathToGUID(oldDependency);
					string newGuid = AssetDatabase.AssetPathToGUID(selectionFile.New);
					guids = guids.Append(new StringsPair
					{
						Old = oldGuid,
						New = newGuid
					});
				}
			}

			foreach (StringsPair selectionFolder in selectionFolders)
			{
				guids = guids.Concat(GetFolderGuids(file, selectionFolder));
			}
		}

		return guids.ToList();
	}

	private static void ReplaceGuids(string file, List<StringsPair> guids)
	{
		if (guids.Count == 0)
		{
			return;
		}

		string content = File.ReadAllText(file);

		foreach (StringsPair guid in guids)
		{
			content = content.Replace($"guid: {guid.Old}", $"guid: {guid.New}");
		}

		File.WriteAllText(file, content);
	}

	private static Object[] LoadNewObjects(List<StringsPair> files, List<StringsPair> folders)
	{
		var totalObjectsLenght = files.Count + folders.Count;
		var newObjects = new Object[totalObjectsLenght];
		var objectIndex = 0;
		foreach (var stringsPair in files)
		{
			var unityObject = AssetDatabase.LoadAssetAtPath<Object>(stringsPair.New);
			newObjects[objectIndex] = unityObject;
			objectIndex++;
		}

		foreach (var stringsPair in folders)
		{
			var unityObject = AssetDatabase.LoadAssetAtPath<Object>(stringsPair.New);
			newObjects[objectIndex] = unityObject;
			objectIndex++;
		}

		return newObjects;
	}
}
}