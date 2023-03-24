using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class MapExport
{

    [MenuItem("ReMap/Export/Map With Origin Offset/Whole Script", false, 50)]
    public static void ExportWholeScriptOffset()
    {
        ExportMap(Helper.ExportType.WholeScriptOffset);
    }

    [MenuItem("ReMap/Export/Map With Origin Offset/Map Only", false, 50)]
    public static void ExportMapOnlyOffset()
    {
        ExportMap(Helper.ExportType.MapOnlyOffset);
    }

    [MenuItem("ReMap/Export/Map Without Origin Offset/Whole Script", false, 50)]
    public static void ExportWholeScript()
    {
        ExportMap(Helper.ExportType.WholeScript);
    }

    [MenuItem("ReMap/Export/Map Without Origin Offset/Map Only", false, 50)]
    public static void ExportOnlyMap()
    {
        ExportMap(Helper.ExportType.MapOnly);
    }

    [MenuItem("ReMap/Export/script.ent", false, 50)]
    public static void ExportScriptEntCode()
    {
        var path = EditorUtility.SaveFilePanel("Script.ent", "", "scriptentexport.ent", "ent");
        if (path.Length == 0) {
            ReMapConsole.Log("[Script.ent Export] No path selected, canceling export", ReMapConsole.LogType.Error);
            return;
        }

        ReMapConsole.Log("[Script.ent Export] Starting Export", ReMapConsole.LogType.Warning);

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        Helper.UseStartingOffset = true;
        string entCode = Build_.Props( null, Build_.BuildType.Ent );

        ReMapConsole.Log("[Script.ent Export] Writing to file: " + path, ReMapConsole.LogType.Warning);
        System.IO.File.WriteAllText(path, entCode + "\u0000");
        ReMapConsole.Log("[Script.ent Export] Finished", ReMapConsole.LogType.Success);
    }

    [MenuItem("ReMap/Export/sound.ent", false, 50)]
    public static void ExportSoundEntCode()
    {
        var path = EditorUtility.SaveFilePanel("Sound.ent Export", "", "soundentexport.ent", "ent");
        if (path.Length == 0) {
            ReMapConsole.Log("[Sound.ent Export] No path selected, canceling export", ReMapConsole.LogType.Error);
            return;
        }

        ReMapConsole.Log("[Sound.ent Export] Starting Export", ReMapConsole.LogType.Warning);

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build_.Sounds();

        ReMapConsole.Log("[Sound.ent Export] Writing to file: " + path, ReMapConsole.LogType.Warning);
        System.IO.File.WriteAllText(path, entCode + "\u0000");
        ReMapConsole.Log("[Sound.ent Export] Finished", ReMapConsole.LogType.Success);
    }


    /// <summary>
    /// Exports map code
    /// </summary>
    private static void ExportMap(Helper.ExportType type)
    {
        var path = EditorUtility.SaveFilePanel("Map Export", "", "mapexport.txt", "txt");
        if (path.Length == 0) {
            ReMapConsole.Log("[Map Export] No path selected, canceling export", ReMapConsole.LogType.Error);
            return;
        }

        ReMapConsole.Log("[Map Export] Starting Export", ReMapConsole.LogType.Warning);

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        Helper.UseStartingOffset = false;
        if (type == Helper.ExportType.MapOnlyOffset || type == Helper.ExportType.WholeScriptOffset)
            Helper.UseStartingOffset = true;

        string funcName = SceneManager.GetActiveScene().name.Replace(" ", "_");

        string mapcode = Helper.ReMapCredit() + "\n";
        mapcode += $"void function {funcName}_Init()" + "\n{\n" + $"{Build_.Props( null, Build_.BuildType.Precache)}" + "\n" + $"    {funcName}()" + "\n" + "\n}\n\n";
        mapcode += $"void function {funcName}()" + "\n{\n" +  Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction );

        if (type == Helper.ExportType.MapOnly || type == Helper.ExportType.MapOnlyOffset)
            mapcode = Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction );

        //Build Map Code
        mapcode += Helper.BuildMapCode();

        if (type == Helper.ExportType.WholeScript || type == Helper.ExportType.WholeScriptOffset)
            mapcode += "}";

        ReMapConsole.Log("[Map Export] Writing to file: " + path, ReMapConsole.LogType.Warning);
        System.IO.File.WriteAllText(path, mapcode);
        ReMapConsole.Log("[Map Export] Finished", ReMapConsole.LogType.Warning);
    }
} 