using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Build
{
    public enum BuildType
    {
        Script,
        EntFile,
        Precache,
        DataTable,
        LiveMap
    }

    public class Build
    {
        public static bool IgnoreCounter = false;

        public static async Task< string > BuildObjectsWithEnum( ObjectType objectType, BuildType buildType, bool selection = false )
        {
            // Does not generate if the type of object are flaged hide
            if ( !Helper.GetBoolFromObjectsToHide( objectType ) )
                return "";

            var code = new StringBuilder();

            var objectData = Helper.GetAllObjectTypeWithEnum( objectType, selection );

            // Dynamic Counter
            if ( !IgnoreCounter )
                IncrementToCounter( objectType, buildType, objectData );

            switch ( objectType )
            {
                case ObjectType.AnimatedCamera:
                    AppendCode( ref code, await BuildAnimatedCamera.BuildAnimatedCameraObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.BubbleShield:
                    AppendCode( ref code, await BuildBubbleShield.BuildBubbleShieldObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.Button:
                    AppendCode( ref code, await BuildButton.BuildButtonObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.DoubleDoor:
                    AppendCode( ref code, await BuildDoubleDoor.BuildDoubleDoorObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.CameraPath:
                    AppendCode( ref code, await BuildCameraPath.BuildCameraPathObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.FuncWindowHint:
                    AppendCode( ref code, await BuildFuncWindowHint.BuildFuncWindowHintObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.HorzDoor:
                    AppendCode( ref code, await BuildHorzDoor.BuildHorzDoorObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.InvisButton:
                    AppendCode( ref code, await BuildInvisButton.BuildInvisButtonObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.Jumppad:
                    AppendCode( ref code, await BuildJumppad.BuildJumppadObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.JumpTower:
                    AppendCode( ref code, await BuildJumpTower.BuildJumpTowerObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.LinkedZipline:
                    AppendCode( ref code, await BuildLinkedZipline.BuildLinkedZiplineObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.LootBin:
                    AppendCode( ref code, await BuildLootBin.BuildLootBinObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.NonVerticalZipLine:
                    AppendCode( ref code, await BuildNonVerticalZipline.BuildNonVerticalZipLineObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.Prop:
                    AppendCode( ref code, await BuildProp.BuildPropObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.RespawnableHeal:
                    AppendCode( ref code, await BuildRespawnableHeal.BuildRespawnableHealObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.SingleDoor:
                    AppendCode( ref code, await BuildSingleDoor.BuildSingleDoorObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.Sound:
                    AppendCode( ref code, await BuildSound.BuildSoundObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.NewLocPair:
                    AppendCode( ref code, await BuildNewLocPair.BuildNewLocPairObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.SpawnPoint:
                    AppendCode( ref code, await BuildSpawnPoint.BuildSpawnPointObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.SpeedBoost:
                    AppendCode( ref code, await BuildSpeedBoost.BuildSpeedBoostObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.TextInfoPanel:
                    AppendCode( ref code, await BuildTextInfoPanel.BuildTextInfoPanelObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.Trigger:
                    AppendCode( ref code, await BuildTrigger.BuildTriggerObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.VerticalDoor:
                    AppendCode( ref code, await BuildVerticalDoor.BuildVerticalDoorObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.VerticalZipLine:
                    AppendCode( ref code, await BuildVerticalZipline.BuildVerticalZipLineObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.WeaponRack:
                    AppendCode( ref code, await BuildWeaponRack.BuildWeaponRackObjects( objectData, buildType ), 0 );
                    break;

                case ObjectType.ZipLine:
                    AppendCode( ref code, await BuildZipline.BuildZiplineObjects( objectData, buildType ), 0 );
                    break;
            }

            return code.ToString();
        }

        internal static void AppendCode( ref string code, string line = "", int idx = 1 )
        {
            code += line;
            PageBreak( ref code, idx );
        }

        internal static void AppendCode( ref string code, StringBuilder line, int idx = 1 )
        {
            code += line.ToString();
            PageBreak( ref code, idx );
        }

        internal static void AppendCode( ref StringBuilder code, string line = "", int idx = 1 )
        {
            code.Append( line );
            PageBreak( ref code, idx );
        }

        internal static void AppendCode( ref StringBuilder code, StringBuilder line, int idx = 1 )
        {
            code.Append( line );
            PageBreak( ref code, idx );
        }

        internal static void PageBreak( ref string code, int idx = 1 )
        {
            for ( int i = 0; i < idx; i++ )
                code += Environment.NewLine;
        }

        internal static void PageBreak( ref StringBuilder code, int idx = 1 )
        {
            for ( int i = 0; i < idx; i++ )
                code.Append( Environment.NewLine );
        }

        private static void IncrementToCounter( ObjectType objectType, BuildType buildType, GameObject[] objectData )
        {
            int objectDataLength = objectData.Length;

            switch ( objectType )
            {
                case ObjectType.Prop:
                    objectDataLength = objectData.Count( o =>
                    {
                        var component = Helper.GetComponentByEnum( o, ObjectType.Prop );
                        return component != null && !( ( PropScript ) component ).ClientSide;
                    } );
                    break;

                case ObjectType.ZipLine:
                case ObjectType.LinkedZipline:
                case ObjectType.VerticalZipLine:
                case ObjectType.NonVerticalZipLine:
                case ObjectType.DoubleDoor:
                case ObjectType.InvisButton:
                    objectDataLength *= 2;
                    break;

                case ObjectType.AnimatedCamera:
                    objectDataLength *= 3;
                    break;

                case ObjectType.JumpTower:
                case ObjectType.SpeedBoost:
                    objectDataLength *= 4;
                    break;

                case ObjectType.RespawnableHeal:
                    objectDataLength *= 6;
                    break;
            }

            switch ( buildType )
            {
                case BuildType.Precache:

                    var PrecacheList = new List< string >();
                    foreach ( var obj in objectData )
                    {
                        string model = UnityInfo.GetObjName( obj );
                        if ( PrecacheList.Contains( model ) )
                            continue;
                        PrecacheList.Add( model );
                    }
                    Helper.IncrementEntityCount( PrecacheList.Count );

                    break;
                case BuildType.LiveMap:
                    Helper.IncrementSendEntityCount( objectDataLength );
                    break;

                default:
                    Helper.IncrementEntityCount( objectDataLength );
                    break;
            }
        }
    }
}