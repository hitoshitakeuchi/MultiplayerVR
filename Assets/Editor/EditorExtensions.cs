using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorExtensions
{
	[MenuItem ("Custom/Select All/Materials")]
	public static void SelectAllMaterial ()
	{
		Selection.objects = Selection.GetFiltered (typeof(Material), SelectionMode.DeepAssets);
	}

	[MenuItem ("Custom/Select All/Meshes")]
	public static void SelectAllMesh ()
	{
		Selection.objects = Selection.GetFiltered (typeof(Mesh), SelectionMode.DeepAssets);
	}

	[MenuItem ("Custom/Select All/Textures")]
	public static void SelectAllTexture ()
	{
		Selection.objects = Selection.GetFiltered (typeof(Texture), SelectionMode.DeepAssets);
	}

	[MenuItem ("Custom/Copy/MovieTexture")]
	public static void CopyMovieTexture ()
	{
		var mt = (MovieTexture)Selection.activeObject;
		if (mt == null)
			return;
		var newMt = MovieTexture.Instantiate<MovieTexture> (mt);
		AssetDatabase.CreateAsset (newMt, "Assets/" + newMt.name + "mt.asset");
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}

}
