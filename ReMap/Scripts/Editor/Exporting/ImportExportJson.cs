using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ImportExportJson
{
    static SaveJson save = new SaveJson();

    static string[] protectedModels =
    {
        "_vertical_zipline",
        "_non_vertical_zipline"
    };

    [MenuItem("ReMap/Import/Json", false, 51)]
    public static async void ImportJson()
    {
        var path = EditorUtility.OpenFilePanel("Json Import", "", "json");
        if (path.Length == 0)
            return;

        ReMapConsole.Log("[Json Import] Reading file: " + path, ReMapConsole.LogType.Warning);
        EditorUtility.DisplayProgressBar("Starting Import", "Reading File" , 0);
        string json = System.IO.File.ReadAllText(path);
        SaveJson myObject = JsonUtility.FromJson<SaveJson>(json);

        await ImportProps(myObject.Props);
        await ImportJumppads(myObject.JumpPads);
        await ImportButtons(myObject.Buttons);
        await ImportBubbleSheilds(myObject.BubbleShields);
        await ImportWeaponRacks(myObject.WeaponRacks);
        await ImportLootBins(myObject.LootBins);
        await ImportZipLines(myObject.ZipLines, myObject.LinkedZipLines, myObject.VerticalZipLines, myObject.NonVerticalZipLines);
        await ImportDoors(myObject.Doors);
        await ImportTriggers(myObject.Triggers);
        await ImportSounds(myObject.Sounds);

        ReMapConsole.Log("[Json Import] Finished", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ImportProps(List<PropsClass> Props)
    {
        int i = 0;
        foreach(PropsClass prop in Props)
        {
            ReMapConsole.Log("[Json Import] Importing: " + prop.Name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Props", "Importing: " + prop.Name, (i + 1) / (float)Props.Count);
            i++;

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(prop.Name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {prop.Name}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = prop.Position;
            obj.transform.eulerAngles = prop.Rotation;
            obj.name = prop.Name;
            obj.gameObject.transform.localScale = prop.Scale;

            PropScript script = obj.GetComponent<PropScript>();
            PropScriptClass propScript = prop.script;
            script.fadeDistance = propScript.FadeDistance;
            script.allowMantle = propScript.AllowMantle;
            script.realmID = propScript.RealmID;

            if (prop.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( prop.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
        }
    }

    private static async Task ImportJumppads(List<JumpPadsClass> JumpPads)
    {
        int i = 0;
        foreach(JumpPadsClass jumppad in JumpPads)
        {
            ReMapConsole.Log("[Json Import] Importing: custom_jumppad", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Jumppads", "Importing: custom_jumppad " + i, (i + 1) / (float)JumpPads.Count);

            string Model = "custom_jumppad";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = jumppad.Position;
            obj.transform.eulerAngles = jumppad.Rotation;
            obj.name = Model;
            obj.gameObject.transform.localScale =  jumppad.Scale;

            PropScript script = obj.GetComponent<PropScript>();
            PropScriptClass propScript = jumppad.script;
            propScript.FadeDistance = script.fadeDistance;
            propScript.AllowMantle = script.allowMantle;
            propScript.RealmID = script.realmID;

            if (jumppad.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( jumppad.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportButtons(List<ButtonsClass> Buttons)
    {
        int i = 0;
        foreach(ButtonsClass button in Buttons)
        {
            ReMapConsole.Log("[Json Import] Importing: custom_button", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Buttons", "Importing: custom_button " + i, (i + 1) / (float)Buttons.Count);

            string Model = "custom_button";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = button.Position;
            obj.transform.eulerAngles = button.Rotation;
            obj.name = Model;

            ButtonScripting script = obj.GetComponent<ButtonScripting>();
            script.OnUseCallback = button.OnUseCallback;
            script.UseText = button.UseText;

            if (button.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( button.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportBubbleSheilds(List<BubbleShieldsClass> BSheilds)
    {
        int i = 0;
        foreach(BubbleShieldsClass sheild in BSheilds)
        {
            ReMapConsole.Log("[Json Import] Importing: " + sheild.Model, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing BubbleShields", "Importing: " + sheild.Model, (i + 1) / (float)BSheilds.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(sheild.Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {sheild.Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = sheild.Position;
            obj.transform.eulerAngles = sheild.Rotation;
            obj.name = sheild.Model;
            obj.gameObject.transform.localScale = sheild.Scale;

            BubbleScript script = obj.GetComponent<BubbleScript>();
            string[] split = sheild.Color.Split(" ");
            script.shieldColor.r = byte.Parse(split[0].Replace("\"", ""));
            script.shieldColor.g = byte.Parse(split[1].Replace("\"", ""));
            script.shieldColor.b = byte.Parse(split[2].Replace("\"", ""));

            if (sheild.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( sheild.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportWeaponRacks(List<WeaponRacksClass> WeaponRacks)
    {
        int i = 0;
        foreach(WeaponRacksClass weaponrack in WeaponRacks)
        {
            ReMapConsole.Log("[Json Import] Importing: " + weaponrack.Weapon, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing WeaponRacks", "Importing: " + weaponrack.Weapon, (i + 1) / (float)WeaponRacks.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(weaponrack.Weapon);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {weaponrack.Weapon}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = weaponrack.Position;
            obj.transform.eulerAngles = weaponrack.Rotation;
            obj.name = weaponrack.Weapon;

            WeaponRackScript script = obj.GetComponent<WeaponRackScript>();
            script.respawnTime = weaponrack.RespawnTime;

            if (weaponrack.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( weaponrack.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportLootBins(List<LootBinsClass> LootBins)
    {
        int i = 0;
        foreach(LootBinsClass lootbin in LootBins)
        {
            ReMapConsole.Log("[Json Import] Importing: custom_lootbin", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing LootBins", "Importing: custom_lootbin " + i, (i + 1) / (float)LootBins.Count);

            string Model = "custom_lootbin";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = lootbin.Position;
            obj.transform.eulerAngles = lootbin.Rotation;
            obj.name = Model;

            LootBinScript script = obj.GetComponent<LootBinScript>();
            script.lootbinSkin = lootbin.Skin;

            if (lootbin.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( lootbin.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportZipLines(List<ZipLinesClass> Ziplines, List<LinkedZipLinesClass> LinkedZiplines, List<VerticalZipLinesClass> VerticalZiplines, List<NonVerticalZipLinesClass> NonVerticalZiplines)
    {
        int i = 0;
        foreach(ZipLinesClass zipline in Ziplines)
        {
            ReMapConsole.Log("[Json Import] Importing: custom_zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Ziplines", "Importing: custom_zipline " + i, (i + 1) / (float)Ziplines.Count);

            string Model = "custom_zipline";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            foreach (Transform child in obj.transform)
            {
                if (child.name == "zipline_start")
                    child.transform.position = zipline.Start;
                else if (child.name == "zipline_end")
                    child.transform.position = zipline.End;
            }

            if (zipline.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( zipline.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        foreach(LinkedZipLinesClass zipline in LinkedZiplines)
        {
            ReMapConsole.Log("[Json Import] Importing: Linked Zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing LinkedZiplines", "Importing: Linked Zipline " + i, (i + 1) / (float)LinkedZiplines.Count);

            GameObject obj = new GameObject("custom_linked_zipline");
            obj.AddComponent<DrawLinkedZipline>();
            obj.AddComponent<LinkedZiplineScript>();
            
            foreach(Vector3 v in zipline.Nodes)
            {
                GameObject child = new GameObject("zipline_node");
                child.transform.position = v;
                child.transform.parent = obj.transform;
                    
                i++;
            }

            LinkedZiplineScript script = obj.GetComponent<LinkedZiplineScript>();
            script.enableSmoothing = zipline.IsSmoothed;
            script.smoothType = zipline.SmoothType;
            script.smoothAmount = zipline.SmoothAmount;

            if (zipline.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( zipline.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        foreach(VerticalZipLinesClass zipline in VerticalZiplines)
        {
            ReMapConsole.Log("[Json Import] Importing: Vertical Zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing VerticalZiplines", "Importing: Vertical Zipline " + i, (i + 1) / (float)VerticalZiplines.Count);

            string Model = zipline.ZiplineType;
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;

            DrawVerticalZipline script = obj.GetComponent<DrawVerticalZipline>();

            obj.transform.position = zipline.ZiplinePosition;
            obj.transform.eulerAngles = zipline.ZiplineAngles;

            script.armOffset = zipline.ArmOffset;
            script.heightOffset = zipline.HeightOffset;
            script.anglesOffset = zipline.AnglesOffset;
            script.fadeDistance = zipline.FadeDistance;
            script.scale = zipline.Scale;
            script.width = zipline.Width;
            script.speedScale = zipline.SpeedScale;
            script.lengthScale = zipline.LengthScale;
            script.preserveVelocity = zipline.PreserveVelocity;
            script.dropToBottom = zipline.DropToBottom;
            script.autoDetachStart = zipline.AutoDetachStart;
            script.autoDetachEnd = zipline.AutoDetachEnd;
            script.restPoint = zipline.RestPoint;
            script.pushOffInDirectionX = zipline.PushOffInDirectionX;
            script.isMoving = zipline.IsMoving;
            script.detachEndOnSpawn = zipline.DetachEndOnSpawn;
            script.detachEndOnUse = zipline.DetachEndOnUse;

            foreach( VCPanelsClass panelInfo in zipline.Panels )
            {
                UnityEngine.Object loadedPrefabResourcePanel = FindPrefabFromName( "mdl#" + panelInfo.Model);
                if (loadedPrefabResourcePanel == null)
                {
                    ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {panelInfo.Model}" , ReMapConsole.LogType.Error);
                    continue;
                }

                GameObject panel = PrefabUtility.InstantiatePrefab(loadedPrefabResourcePanel as GameObject) as GameObject;
                panel.transform.position = panelInfo.Position;
                panel.transform.eulerAngles = panelInfo.Angles;
                panel.transform.parent = CreateGameObjectWithCollectionPath( panelInfo.Collection ).transform;
                Array.Resize( ref script.panels, script.panels.Length + 1 );
                script.panels[script.panels.Length - 1] = panel;
            }

            script.panelTimerMin = zipline.PanelTimerMin;
            script.panelTimerMax = zipline.PanelTimerMax;
            script.panelMaxUse = zipline.PanelMaxUse;

            if (zipline.Collection != "")
            obj.transform.parent = CreateGameObjectWithCollectionPath( zipline.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        foreach(NonVerticalZipLinesClass zipline in NonVerticalZiplines)
        {
            ReMapConsole.Log("[Json Import] Importing: Non Vertical Zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing NonVerticalZiplines", "Importing: Vertical Zipline " + i, (i + 1) / (float)NonVerticalZiplines.Count);

            string Model = zipline.ZiplineType;
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;

            DrawNonVerticalZipline script = obj.GetComponent<DrawNonVerticalZipline>();

            obj.transform.position = zipline.ZiplineStartPosition;
            obj.transform.eulerAngles = zipline.ZiplineStartAngles;
            script.zipline.transform.Find("support_end").position = zipline.ZiplineEndPosition;
            script.zipline.transform.Find("support_end").eulerAngles = zipline.ZiplineEndAngles;
            script.armOffsetStart = zipline.ArmStartOffset;
            script.armOffsetEnd = zipline.ArmEndOffset;
            script.fadeDistance = zipline.FadeDistance;
            script.scale = zipline.Scale;
            script.width = zipline.Width;
            script.speedScale = zipline.SpeedScale;
            script.lengthScale = zipline.LengthScale;
            script.preserveVelocity = zipline.PreserveVelocity;
            script.dropToBottom = zipline.DropToBottom;
            script.autoDetachStart = zipline.AutoDetachStart;
            script.autoDetachEnd = zipline.AutoDetachEnd;
            script.restPoint = zipline.RestPoint;
            script.pushOffInDirectionX = zipline.PushOffInDirectionX;
            script.isMoving = zipline.IsMoving;
            script.detachEndOnSpawn = zipline.DetachEndOnSpawn;
            script.detachEndOnUse = zipline.DetachEndOnUse;

            foreach( VCPanelsClass panelInfo in zipline.Panels )
            {
                UnityEngine.Object loadedPrefabResourcePanel = FindPrefabFromName( "mdl#" + panelInfo.Model);
                if (loadedPrefabResourcePanel == null)
                {
                    ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {panelInfo.Model}" , ReMapConsole.LogType.Error);
                    continue;
                }

                GameObject panel = PrefabUtility.InstantiatePrefab(loadedPrefabResourcePanel as GameObject) as GameObject;
                panel.transform.position = panelInfo.Position;
                panel.transform.eulerAngles = panelInfo.Angles;
                panel.transform.parent = CreateGameObjectWithCollectionPath( panelInfo.Collection ).transform;
                Array.Resize( ref script.panels, script.panels.Length + 1 );
                script.panels[script.panels.Length - 1] = panel;
            }

            script.panelTimerMin = zipline.PanelTimerMin;
            script.panelTimerMax = zipline.PanelTimerMax;
            script.panelMaxUse = zipline.PanelMaxUse;

            if (zipline.Collection != "")
            obj.transform.parent = CreateGameObjectWithCollectionPath( zipline.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportDoors(List<DoorsClass> Doors)
    {
        int i = 0;
        foreach(DoorsClass door in Doors)
        {
            string Model = "custom_single_door";
            bool IsSingleOrDouble = false;
            switch(door.Type)
            {
                case "eMapEditorDoorType.Single":
                    Model = "custom_single_door";
                    IsSingleOrDouble = true;
                    break;
                case "eMapEditorDoorType.Double":
                    Model = "custom_double_door";
                    IsSingleOrDouble = true;
                    break;
                case "eMapEditorDoorType.Vertical":
                    Model = "custom_vertical_door";
                    break;
                case "eMapEditorDoorType.Horizontal":
                    Model = "custom_sliding_door";
                    break;
            }

            ReMapConsole.Log("[Json Import] Importing: " + Model, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Doors", "Importing: " + Model, (i + 1) / (float)Doors.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = door.Position;
            obj.transform.eulerAngles = door.Rotation;
            obj.name = Model;

            if(IsSingleOrDouble)
            {
                DoorScript script = obj.GetComponent<DoorScript>();
                script.goldDoor = door.Gold;
            }

            if (door.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( door.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportTriggers(List<TriggersClass> Triggers)
    {
        int i = 0;
        foreach(TriggersClass trigger in Triggers)
        {
            ReMapConsole.Log("[Json Import] Importing: Trigger", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Triggers", "Importing: Triggers" + i, (i + 1) / (float)Triggers.Count);

            string Model = "trigger_cylinder";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = trigger.Position;
            obj.transform.eulerAngles = trigger.Rotation;
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3(trigger.Radius, trigger.Height, trigger.Radius);

            TriggerScripting script = obj.GetComponent<TriggerScripting>();
            script.Debug = trigger.Debug;
            script.EnterCallback = trigger.EnterCallback;
            script.LeaveCallback = trigger.ExitCallback;

            if (trigger.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( trigger.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportSounds(List<SoundClass> Sounds)
    {
        int i = 0;
        foreach(SoundClass sound in Sounds)
        {
            ReMapConsole.Log("[Json Import] Importing: Sound", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Sounds", "Importing: Sounds" + i, (i + 1) / (float)Sounds.Count);

            string Model = "custom_sound";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = sound.Position;

            SoundScript script = obj.GetComponent<SoundScript>();
            script.radius = sound.Radius;
            script.isWaveAmbient = sound.IsWaveAmbient;
            script.enable = sound.Enable;
            script.soundName = sound.SoundName;

            int j = 0;
            foreach ( Vector3 polylineSegments in sound.PolylineSegments )
            {
                Array.Resize( ref script.polylineSegment, script.polylineSegment.Length + 1 );
                script.polylineSegment[j++] = polylineSegments;
            }

            if (sound.Collection != "")
            obj.gameObject.transform.parent = CreateGameObjectWithCollectionPath( sound.Collection ).transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    [MenuItem("ReMap/Export/Json", false, 51)]
    public static async void ExportJson()
    {
        Helper.FixPropTags();

        var path = EditorUtility.SaveFilePanel("Json Export", "", "mapexport.json", "json");
        if (path.Length == 0)
            return;

        EditorUtility.DisplayProgressBar("Starting Export", "" , 0);

        ResetJsonSave();
        await ExportProps();
        await ExportJumppads();
        await ExportButtons();
        await ExportBubbleSheilds();
        await ExportWeaponRacks();
        await ExportLootBins();
        await ExportZipLines();
        await ExportDoors();
        await ExportTriggers();
        await ExportSounds();

        ReMapConsole.Log("[Json Export] Writing to file: " + path, ReMapConsole.LogType.Warning);
        string json = JsonUtility.ToJson(save);
        System.IO.File.WriteAllText(path, json);

        ReMapConsole.Log("[Json Export] Finished.", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ExportProps()
    {
        int i = 0;
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        foreach(GameObject obj in PropObjects)
        {
            PropScript script = obj.GetComponent<PropScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing PropScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Props", "Exporting: " + obj.name, (i + 1) / (float)PropObjects.Length);
            
            PropsClass prop = new PropsClass();
            PropScriptClass propScript = new PropScriptClass();
            prop.Name = obj.name.Split(char.Parse(" "))[0];
            prop.Position = obj.transform.position;
            prop.Rotation = obj.transform.rotation.eulerAngles;
            prop.Scale = obj.transform.localScale;
            propScript.AllowMantle = script.allowMantle;
            propScript.FadeDistance = script.fadeDistance;
            propScript.RealmID = script.realmID;
            prop.script = propScript;

            prop.Collection = FindCollectionPath( obj );

            if ( !prop.Collection.Contains(protectedModels[0]) && !prop.Collection.Contains(protectedModels[1]) )
                save.Props.Add(prop);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportJumppads()
    {
        int i = 0;
        GameObject[] JumpPadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        foreach (GameObject obj in JumpPadObjects)
        {
            PropScript script = obj.GetComponent<PropScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing PropScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Jumppads", "Exporting: " + obj.name, (i + 1) / (float)JumpPadObjects.Length);

            JumpPadsClass jumpPad = new JumpPadsClass();
            PropScriptClass propScript = new PropScriptClass();
            jumpPad.Position = obj.transform.position;
            jumpPad.Rotation = obj.transform.rotation.eulerAngles;
            jumpPad.Scale = obj.transform.localScale;
            propScript.AllowMantle = script.allowMantle;
            propScript.FadeDistance = script.fadeDistance;
            propScript.RealmID = script.realmID;
            jumpPad.script = propScript;

            jumpPad.Collection = FindCollectionPath( obj );

            save.JumpPads.Add(jumpPad);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportButtons()
    {
        int i = 0;
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject obj in ButtonObjects)
        {
            ButtonScripting script = obj.GetComponent<ButtonScripting>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing ButtonScripting on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Buttons", "Exporting: " + obj.name, (i + 1) / (float)ButtonObjects.Length);

            ButtonsClass button = new ButtonsClass();
            button.Position = obj.transform.position;
            button.Rotation = obj.transform.rotation.eulerAngles;
            button.UseText = script.UseText;
            button.OnUseCallback = script.OnUseCallback;

            button.Collection = FindCollectionPath( obj );

            save.Buttons.Add(button);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportBubbleSheilds()
    {
        int i = 0;
        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        foreach (GameObject obj in BubbleShieldObjects)
        {
            BubbleScript script = obj.GetComponent<BubbleScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing BubbleScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting BubbleShields", "Exporting: " + obj.name, (i + 1) / (float)BubbleShieldObjects.Length);

            BubbleShieldsClass bubbleShield = new BubbleShieldsClass();
            bubbleShield.Position = obj.transform.position;
            bubbleShield.Rotation = obj.transform.rotation.eulerAngles;
            bubbleShield.Scale = obj.transform.localScale;
            bubbleShield.Color = script.shieldColor.r + " " + script.shieldColor.g + " " + script.shieldColor.b;
            bubbleShield.Model = obj.name.Split(char.Parse(" "))[0];

            bubbleShield.Collection = FindCollectionPath( obj );

            save.BubbleShields.Add(bubbleShield);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportWeaponRacks()
    {
        int i = 0;
        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        foreach (GameObject obj in WeaponRackObjects)
        {
            WeaponRackScript script = obj.GetComponent<WeaponRackScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing WeaponRackScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting LootBins", "Exporting: " + obj.name, (i + 1) / (float)WeaponRackObjects.Length);

            WeaponRacksClass weaponRack = new WeaponRacksClass();
            weaponRack.Position = obj.transform.position;
            weaponRack.Rotation = obj.transform.rotation.eulerAngles;
            weaponRack.Weapon = obj.name.Split(char.Parse(" "))[0];
            weaponRack.RespawnTime = script.respawnTime;

            weaponRack.Collection = FindCollectionPath( obj );

            save.WeaponRacks.Add(weaponRack);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportLootBins()
    {
        int i = 0;
        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        foreach (GameObject obj in LootBinObjects)
        {
            LootBinScript script = obj.GetComponent<LootBinScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing LootBinScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting LootBins", "Exporting: " + obj.name, (i + 1) / (float)LootBinObjects.Length);

            LootBinsClass lootBin = new LootBinsClass();
            lootBin.Position = obj.transform.position;
            lootBin.Rotation = obj.transform.rotation.eulerAngles;
            lootBin.Skin = script.lootbinSkin;

            lootBin.Collection = FindCollectionPath( obj );

            save.LootBins.Add(lootBin);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportZipLines()
    {
        int i = 0;
        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        foreach (GameObject obj in ZipLineObjects)
        {
            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Ziplines", "Exporting: " + obj.name, (i + 1) / (float)ZipLineObjects.Length);

            ZipLinesClass zipLine = new ZipLinesClass();
            foreach (Transform child in obj.transform)
            {
                if (child.name == "zipline_start")
                    zipLine.Start = child.gameObject.transform.position;
                else if (child.name == "zipline_end")
                    zipLine.End = child.gameObject.transform.position;
            }

            if(zipLine.Start == null || zipLine.End == null)
                continue;

            zipLine.Collection = FindCollectionPath( obj );

            save.ZipLines.Add(zipLine);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        foreach (GameObject obj in LinkedZipLineObjects)
        {
            LinkedZiplineScript script = obj.GetComponent<LinkedZiplineScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing LinkedZiplineScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Ziplines", "Exporting: " + obj.name, (i + 1) / (float)LinkedZipLineObjects.Length);

            List<Vector3> nodes = new List<Vector3>();
            LinkedZipLinesClass linkedZipLine = new LinkedZipLinesClass();
            foreach (Transform child in obj.transform)
                nodes.Add(child.gameObject.transform.position);

            linkedZipLine.Nodes = nodes;
            linkedZipLine.IsSmoothed = script.enableSmoothing;
            linkedZipLine.SmoothType = script.smoothType;
            linkedZipLine.SmoothAmount = script.smoothAmount;

            linkedZipLine.Collection = FindCollectionPath( obj );

            save.LinkedZipLines.Add(linkedZipLine);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        GameObject[] VerticalZipLineObjects = GameObject.FindGameObjectsWithTag("VerticalZipLine");
        foreach (GameObject obj in VerticalZipLineObjects)
        {
            DrawVerticalZipline script = obj.GetComponent<DrawVerticalZipline>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing DrawVerticalZipline on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Ziplines", "Exporting: " + obj.name, (i + 1) / (float)VerticalZipLineObjects.Length);

            VerticalZipLinesClass verticalZipLine = new VerticalZipLinesClass();

            verticalZipLine.ZiplineType = obj.name;
            verticalZipLine.ZiplinePosition = script.zipline.position;
            verticalZipLine.ZiplineAngles = script.zipline.eulerAngles;
            verticalZipLine.ArmOffset = script.armOffset;
            verticalZipLine.HeightOffset = script.heightOffset;
            verticalZipLine.AnglesOffset = script.anglesOffset;
            verticalZipLine.FadeDistance = script.fadeDistance;
            verticalZipLine.Scale = script.scale;
            verticalZipLine.Width = script.width;
            verticalZipLine.SpeedScale = script.speedScale;
            verticalZipLine.LengthScale = script.lengthScale;
            verticalZipLine.PreserveVelocity = script.preserveVelocity;
            verticalZipLine.DropToBottom = script.dropToBottom;
            verticalZipLine.AutoDetachStart = script.autoDetachStart;
            verticalZipLine.AutoDetachEnd = script.autoDetachEnd;
            verticalZipLine.RestPoint = script.restPoint;
            verticalZipLine.PushOffInDirectionX = script.pushOffInDirectionX;
            verticalZipLine.IsMoving = script.isMoving;
            verticalZipLine.DetachEndOnSpawn = script.detachEndOnSpawn;
            verticalZipLine.DetachEndOnUse = script.detachEndOnUse;

            List<VCPanelsClass> panels = new List<VCPanelsClass>();
            foreach (GameObject panel in script.panels)
            {
                VCPanelsClass panelClass = new VCPanelsClass();
                panelClass.Model = panel.name;
                panelClass.Position = panel.transform.position;
                panelClass.Angles = panel.transform.eulerAngles;
                panelClass.Collection = FindCollectionPath( panel );
                panels.Add(panelClass);
            }

            verticalZipLine.Panels = panels.ToArray();

            verticalZipLine.PanelTimerMin = script.panelTimerMin;
            verticalZipLine.PanelTimerMax = script.panelTimerMax;
            verticalZipLine.PanelMaxUse = script.panelMaxUse;

            verticalZipLine.Collection = FindCollectionPath( obj );

            save.VerticalZipLines.Add(verticalZipLine);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        GameObject[] NonVerticalZipLineObjects = GameObject.FindGameObjectsWithTag("NonVerticalZipLine");
        foreach (GameObject obj in NonVerticalZipLineObjects)
        {
            DrawNonVerticalZipline script = obj.GetComponent<DrawNonVerticalZipline>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing DrawNonVerticalZipline on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Ziplines", "Exporting: " + obj.name, (i + 1) / (float)NonVerticalZipLineObjects.Length);

            NonVerticalZipLinesClass nonVerticalZipLine = new NonVerticalZipLinesClass();

            nonVerticalZipLine.ZiplineType = obj.name;
            nonVerticalZipLine.ZiplineStartPosition = script.zipline.transform.Find("support_start").position;
            nonVerticalZipLine.ZiplineStartAngles = script.zipline.transform.Find("support_start").eulerAngles;
            nonVerticalZipLine.ZiplineEndPosition = script.zipline.transform.Find("support_end").position;
            nonVerticalZipLine.ZiplineEndAngles = script.zipline.transform.Find("support_end").eulerAngles;
            nonVerticalZipLine.ArmStartOffset = script.armOffsetStart;
            nonVerticalZipLine.ArmEndOffset = script.armOffsetEnd;
            nonVerticalZipLine.FadeDistance = script.fadeDistance;
            nonVerticalZipLine.Scale = script.scale;
            nonVerticalZipLine.Width = script.width;
            nonVerticalZipLine.SpeedScale = script.speedScale;
            nonVerticalZipLine.LengthScale = script.lengthScale;
            nonVerticalZipLine.PreserveVelocity = script.preserveVelocity;
            nonVerticalZipLine.DropToBottom = script.dropToBottom;
            nonVerticalZipLine.AutoDetachStart = script.autoDetachStart;
            nonVerticalZipLine.AutoDetachEnd = script.autoDetachEnd;
            nonVerticalZipLine.RestPoint = script.restPoint;
            nonVerticalZipLine.PushOffInDirectionX = script.pushOffInDirectionX;
            nonVerticalZipLine.IsMoving = script.isMoving;
            nonVerticalZipLine.DetachEndOnSpawn = script.detachEndOnSpawn;
            nonVerticalZipLine.DetachEndOnUse = script.detachEndOnUse;

            List<VCPanelsClass> panels = new List<VCPanelsClass>();
            foreach (GameObject panel in script.panels)
            {
                VCPanelsClass panelClass = new VCPanelsClass();
                panelClass.Model = panel.name;
                panelClass.Position = panel.transform.position;
                panelClass.Angles = panel.transform.eulerAngles;
                panelClass.Collection = FindCollectionPath( panel );
                panels.Add(panelClass);
            }

            nonVerticalZipLine.Panels = panels.ToArray();

            nonVerticalZipLine.PanelTimerMin = script.panelTimerMin;
            nonVerticalZipLine.PanelTimerMax = script.panelTimerMax;
            nonVerticalZipLine.PanelMaxUse = script.panelMaxUse;

            nonVerticalZipLine.Collection = FindCollectionPath( obj );

            save.NonVerticalZipLines.Add(nonVerticalZipLine);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportDoors()
    {
        int i = 0;
        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        foreach (GameObject obj in SingleDoorObjects)
        {
            DoorScript script = obj.GetComponent<DoorScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing DoorScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)SingleDoorObjects.Length);

            DoorsClass singleDoor = new DoorsClass();
            singleDoor.Position = obj.transform.position;
            singleDoor.Rotation = obj.transform.rotation.eulerAngles;
            singleDoor.Type = "eMapEditorDoorType.Single";
            singleDoor.Gold = script.goldDoor;

            singleDoor.Collection = FindCollectionPath( obj );

            save.Doors.Add(singleDoor);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        foreach (GameObject obj in DoubleDoorObjects)
        {
            DoorScript script = obj.GetComponent<DoorScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing DoorScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)DoubleDoorObjects.Length);

            DoorsClass doubleDoor = new DoorsClass();
            doubleDoor.Position = obj.transform.position;
            doubleDoor.Rotation = obj.transform.rotation.eulerAngles;
            doubleDoor.Type = "eMapEditorDoorType.Double";
            doubleDoor.Gold = script.goldDoor;

            doubleDoor.Collection = FindCollectionPath( obj );

            save.Doors.Add(doubleDoor);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        foreach (GameObject obj in VertDoorObjects)
        {
            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)VertDoorObjects.Length);

            DoorsClass vertDoor = new DoorsClass();
            vertDoor.Position = obj.transform.position;
            vertDoor.Rotation = obj.transform.rotation.eulerAngles;
            vertDoor.Type = "eMapEditorDoorType.Vertical";
            vertDoor.Gold = false;

            vertDoor.Collection = FindCollectionPath( obj );

            save.Doors.Add(vertDoor);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        GameObject[] HorDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        foreach (GameObject obj in HorDoorObjects)
        {
            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)HorDoorObjects.Length);

            DoorsClass horDoor = new DoorsClass();
            horDoor.Position = obj.transform.position;
            horDoor.Rotation = obj.transform.rotation.eulerAngles;
            horDoor.Type = "eMapEditorDoorType.Horizontal";
            horDoor.Gold = false;

            horDoor.Collection = FindCollectionPath( obj );

            save.Doors.Add(horDoor);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportTriggers()
    {
        GameObject[] Triggers = GameObject.FindGameObjectsWithTag("Trigger");
        int i = 0;
        foreach (GameObject obj in Triggers)
        {
            TriggerScripting script = obj.GetComponent<TriggerScripting>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing TriggerScripting on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Triggers", "Exporting: " + obj.name, (i + 1) / (float)Triggers.Length);

            TriggersClass trigger = new TriggersClass();
            trigger.Position = obj.transform.position;
            trigger.Rotation = obj.transform.rotation.eulerAngles;
            trigger.Radius = obj.transform.localScale.x;
            trigger.Height = obj.transform.localScale.y;
            trigger.EnterCallback = script.EnterCallback;
            trigger.ExitCallback = script.LeaveCallback;
            trigger.Debug = script.Debug;

            trigger.Collection = FindCollectionPath( obj );

            save.Triggers.Add(trigger);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportSounds()
    {
        GameObject[] Sounds = GameObject.FindGameObjectsWithTag("Sound");
        int i = 0;
        foreach (GameObject obj in Sounds)
        {
            SoundScript script = obj.GetComponent<SoundScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing SoundScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Sounds", "Exporting: " + obj.name, (i + 1) / (float)Sounds.Length);

            SoundClass sound = new SoundClass();
            sound.Position = script.soundModel.transform.position;
            sound.Radius = script.radius;
            sound.IsWaveAmbient = script.isWaveAmbient;
            sound.Enable = script.enable;
            sound.SoundName = script.soundName;

            List<Vector3> polylineSegment = new List<Vector3>();
            foreach ( Vector3 polylineSegments in script.polylineSegment )
            {
                polylineSegment.Add( polylineSegments );
            }
            sound.PolylineSegments = polylineSegment;

            sound.Collection = FindCollectionPath( obj );

            save.Sounds.Add(sound);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static void ResetJsonSave()
    {
        save = new SaveJson();
        save.Props = new List<PropsClass>();
        save.JumpPads = new List<JumpPadsClass>();
        save.Buttons = new List<ButtonsClass>();
        save.BubbleShields = new List<BubbleShieldsClass>();
        save.WeaponRacks = new List<WeaponRacksClass>();
        save.LootBins = new List<LootBinsClass>();
        save.ZipLines = new List<ZipLinesClass>();
        save.LinkedZipLines = new List<LinkedZipLinesClass>();
        save.VerticalZipLines = new List<VerticalZipLinesClass>();
        save.NonVerticalZipLines = new List<NonVerticalZipLinesClass>();
        save.Doors = new List<DoorsClass>();
        save.Triggers = new List<TriggersClass>();
        save.Sounds = new List<SoundClass>();
    }

    private static UnityEngine.Object FindPrefabFromName(string name)
    {
        //Find Model GUID in Assets
        string[] results = AssetDatabase.FindAssets(name);
        if (results.Length == 0)
            return null;

        //Get model path from guid and load it
        UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
        return loadedPrefabResource;
    }

    private static string FindCollectionPath( GameObject obj )
    {
        List<GameObject> parents = new List<GameObject>();
        GameObject currentParent = obj;
        string collectionPath = "";

        while (currentParent.transform.parent != null)
        {
            if ( currentParent != obj ) parents.Add(currentParent);
            currentParent = currentParent.transform.parent.gameObject;
        }

        if ( currentParent != obj ) parents.Add(currentParent);

        foreach (GameObject parent in parents)
        {
            Vector3 pos = parent.transform.position;
            Vector3 ang = parent.transform.eulerAngles;
            collectionPath = $"{parent.name}|{parent.transform.position}|{parent.transform.eulerAngles}/{collectionPath}";
        }

        int lastSlashIndex = collectionPath.LastIndexOf("/");
        if (lastSlashIndex >= 0)
        {
            collectionPath = collectionPath.Remove(lastSlashIndex, collectionPath.Length - lastSlashIndex);
        }

        return collectionPath.Replace("\r", "").Replace("\n", "");
    }

    private static GameObject CreateGameObjectWithCollectionPath( string collectionPath )
    {
        List<string> pathSubstring = new List<string>();
        int startIndex = 0;
        while (true)
        {
            int slashIndex = collectionPath.IndexOf("/", startIndex);
            if (slashIndex < 0)
            {
                pathSubstring.Add(collectionPath.Substring(startIndex));
                break;
            }
            else
            {
                pathSubstring.Add(collectionPath.Substring(startIndex, slashIndex - startIndex));
                startIndex = slashIndex + 1;
            }
        }
        string[] parents = pathSubstring.ToArray();

        GameObject folder;
        folder = GameObject.Find(parents[0].Split("|")[0]);
            if(folder == null)
        folder = new GameObject(parents[0].Split("|")[0]);

        string[] partsPosF = parents[0].Split(char.Parse("|"))[1].Replace("(", "").Replace(")", "").Replace(" ", "").Split(char.Parse(","));
        string[] partsAngF = parents[0].Split(char.Parse("|"))[2].Replace("(", "").Replace(")", "").Replace(" ", "").Split(char.Parse(","));

        float xPosF = float.Parse(partsPosF[0].Replace(".", ","));
        float yPosF = float.Parse(partsPosF[1].Replace(".", ","));
        float zPosF = float.Parse(partsPosF[2].Replace(".", ","));

        float xAngF = float.Parse(partsAngF[0].Replace(".", ","));
        float yAngF = float.Parse(partsAngF[1].Replace(".", ","));
        float zAngF = float.Parse(partsAngF[2].Replace(".", ","));

        folder.transform.position = new Vector3( xPosF, yPosF, zPosF );
        folder.transform.eulerAngles = new Vector3( xAngF, yAngF, zAngF );

        int folderNum = parents.Length;

        string path = parents[0].Split(char.Parse("|"))[0];

        if ( folderNum >= 2 )
        for ( int j = 1 ; j < folderNum ; j++ )
        {
            string parentName = parents[j].Split(char.Parse("|"))[0];
            string parentPosString = parents[j].Split(char.Parse("|"))[1];
            string parentAngString = parents[j].Split(char.Parse("|"))[2];

            string[] partsPos = parentPosString.Replace("(", "").Replace(")", "").Replace(" ", "").Split(char.Parse(","));
            string[] partsAng = parentAngString.Replace("(", "").Replace(")", "").Replace(" ", "").Split(char.Parse(","));

            path = path + "/" + parentName;
            GameObject newFolder;
            newFolder = GameObject.Find(path);
                if(newFolder == null)
            newFolder = new GameObject(parentName);

            float xPos = float.Parse(partsPos[0].Replace(".", ","));
            float yPos = float.Parse(partsPos[1].Replace(".", ","));
            float zPos = float.Parse(partsPos[2].Replace(".", ","));

            float xAng = float.Parse(partsAng[0].Replace(".", ","));
            float yAng = float.Parse(partsAng[1].Replace(".", ","));
            float zAng = float.Parse(partsAng[2].Replace(".", ","));

            newFolder.transform.position = new Vector3( xPos, yPos, zPos );
            newFolder.transform.eulerAngles = new Vector3( xAng, yAng, zAng );

            newFolder.transform.SetParent(folder.transform);

            folder = newFolder;
        }
        return folder;
    }
}

[Serializable]
public class PropScriptClass
{
    public bool AllowMantle;
    public float FadeDistance;
    public int RealmID;
}

[Serializable]
public class SaveJson
{
    public List<PropsClass> Props;
    public List<JumpPadsClass> JumpPads;
    public List<ButtonsClass> Buttons;
    public List<BubbleShieldsClass> BubbleShields;
    public List<WeaponRacksClass> WeaponRacks;
    public List<LootBinsClass> LootBins;
    public List<ZipLinesClass> ZipLines;
    public List<LinkedZipLinesClass> LinkedZipLines;
    public List<VerticalZipLinesClass> VerticalZipLines;
    public List<NonVerticalZipLinesClass> NonVerticalZipLines;
    public List<DoorsClass> Doors;
    public List<TriggersClass> Triggers;
    public List<SoundClass> Sounds;
}

[Serializable]
public class PropsClass
{
    public string Name;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public PropScriptClass script;
    public string Collection;
}

[Serializable]
public class JumpPadsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public PropScriptClass script;
    public string Collection;
}

[Serializable]
public class ButtonsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string UseText;
    public string OnUseCallback;
    public string Collection;
}

[Serializable]
public class BubbleShieldsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public string Color;
    public string Model;
    public string Collection;
}

[Serializable]
public class WeaponRacksClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string Weapon;
    public float RespawnTime;
    public string Collection;
}

[Serializable]
public class LootBinsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public int Skin;
    public string Collection;
}

[Serializable]
public class ZipLinesClass
{
    public Vector3 Start;
    public Vector3 End;
    public string Collection;
}

[Serializable]
public class LinkedZipLinesClass
{
    public bool IsSmoothed;
    public bool SmoothType;
    public int SmoothAmount;
    public List<Vector3> Nodes;
    public string Collection;
}

[Serializable]
public class VerticalZipLinesClass
{
    public string ZiplineType;
    public Vector3 ZiplinePosition;
    public Vector3 ZiplineAngles;
    public float ArmOffset;
    public float HeightOffset;
    public float AnglesOffset;
    public float FadeDistance;
    public float Scale;
    public float Width;
    public float SpeedScale;
    public float LengthScale;
    public bool PreserveVelocity;
    public bool DropToBottom;
    public float AutoDetachStart;
    public float AutoDetachEnd;
    public bool RestPoint;
    public bool PushOffInDirectionX;
    public bool IsMoving;
    public bool DetachEndOnSpawn;
    public bool DetachEndOnUse;
    public VCPanelsClass[] Panels;
    public float PanelTimerMin;
    public float PanelTimerMax;
    public int PanelMaxUse;
    public string Collection;
}

[Serializable]
public class NonVerticalZipLinesClass
{
    public string ZiplineType;
    public Vector3 ZiplineStartPosition;
    public Vector3 ZiplineStartAngles;
    public Vector3 ZiplineEndPosition;
    public Vector3 ZiplineEndAngles;
    public float ArmStartOffset;
    public float ArmEndOffset;
    public float FadeDistance;
    public float Scale;
    public float Width;
    public float SpeedScale;
    public float LengthScale;
    public bool PreserveVelocity;
    public bool DropToBottom;
    public float AutoDetachStart;
    public float AutoDetachEnd;
    public bool RestPoint;
    public bool PushOffInDirectionX;
    public bool IsMoving;
    public bool DetachEndOnSpawn;
    public bool DetachEndOnUse;
    public VCPanelsClass[] Panels;
    public float PanelTimerMin;
    public float PanelTimerMax;
    public int PanelMaxUse;
    public string Collection;
}

[Serializable]
public class VCPanelsClass
{
    public string Model;
    public Vector3 Position;
    public Vector3 Angles;
    public string Collection;
}

[Serializable]
public class DoorsClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string Type;
    public bool Gold;
    public string Collection;
}

[Serializable]
public class TriggersClass
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string EnterCallback;
    public string ExitCallback;
    public float Radius;
    public float Height;
    public bool Debug;
    public string Collection;
}

[Serializable]
public class SoundClass
{
    public Vector3 Position;
    public float Radius;
    public bool IsWaveAmbient;
    public bool Enable;
    public string SoundName;
    public List<Vector3> PolylineSegments;
    public string Collection;
}
