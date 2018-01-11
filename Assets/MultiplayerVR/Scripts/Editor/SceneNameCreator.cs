using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MultiplayerVR.Editor
{
    public static class SceneNameCreator
    {
        private static readonly string[] INVALUD_CHARS =
        {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        ",", "<"
    };

        private const string ITEM_NAME = "Tools/Create/Scene Name Class";

        private const string OUTPUT_CLASS_FILE_PATH = "Assets/MultiplayerVR/Scripts/Constant/SceneName.cs";

        [MenuItem(ITEM_NAME)]
        public static void Create()
        {
            if (!CanCreate())
            {
                return;
            }

            CreateScript();

            EditorUtility.DisplayDialog(Path.GetFileNameWithoutExtension(OUTPUT_CLASS_FILE_PATH), string.Format("{0}の作成が完了しました", OUTPUT_CLASS_FILE_PATH), "OK");
        }

        public static void CreateScript()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("namespace MultiplayerVR.Constant");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("\t/// <summary>");
            stringBuilder.AppendLine("\t/// シーン名を定数で管理するクラス");
            stringBuilder.AppendLine("\t/// </summary>");
            stringBuilder.AppendFormat("\tpublic static class {0}", Path.GetFileNameWithoutExtension(OUTPUT_CLASS_FILE_PATH)).AppendLine();
            stringBuilder.AppendLine("\t{");

            var namePair = EditorBuildSettings.scenes
                .Where(v => v.enabled)
                .Select(v => Path.GetFileNameWithoutExtension(v.path))
                .Distinct()
                .Select(v => new
                {
                    className = RemoveInvalidChars(v),
                    sceneName = v
                })
                .ToList();

            namePair.ForEach(v => stringBuilder.Append("\t\t").AppendFormat(@"public const string {0} = ""{1}"";", v.className, v.sceneName).AppendLine());

            stringBuilder.AppendLine();
            stringBuilder.Append("\t\t").AppendLine("public static readonly HashSet<string> Names = new HashSet<string>()");
            stringBuilder.Append("\t\t").AppendLine("{");

            namePair.ForEach(v => stringBuilder.AppendFormat("\t\t\t{0},", v.className).AppendLine());

            stringBuilder.Append("\t\t").AppendLine("};");
            stringBuilder.Append("\t").AppendLine("}");
            stringBuilder.AppendLine("}");

            var directoryName = Path.GetDirectoryName(OUTPUT_CLASS_FILE_PATH);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(OUTPUT_CLASS_FILE_PATH, stringBuilder.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        }

        [MenuItem(ITEM_NAME, true)]
        public static bool CanCreate()
        {
            return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
        }

        public static string RemoveInvalidChars(string str)
        {
            Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
            return str;
        }
    }
}