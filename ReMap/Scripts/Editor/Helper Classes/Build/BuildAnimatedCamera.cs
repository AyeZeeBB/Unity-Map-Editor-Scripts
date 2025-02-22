using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildAnimatedCamera
    {
        public static async Task< StringBuilder > BuildAnimatedCameraObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Animated Camera" );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;

                case BuildType.DataTable:
                    // Empty
                    break;

                case BuildType.LiveMap:
                    // Empty
                    break;
            }

            // Build the code
            foreach ( var obj in objectData )
            {
                var script = ( AnimatedCameraScript )Helper.GetComponentByEnum( obj, ObjectType.AnimatedCamera );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code,
                            $"    ReMapCreateCamera( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {Helper.ReplaceComma( script.AngleOffset )}, {Helper.ReplaceComma( script.MaxLeft )}, {Helper.ReplaceComma( script.MaxRight )}, {Helper.ReplaceComma( script.RotationTime )}, {Helper.ReplaceComma( script.TransitionTime )}, true )" );
                        break;

                    case BuildType.EntFile:
                        // Empty
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        LiveMap.AddToGameQueue(
                            $"ReMapCreateCamera( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles( obj )}, {Helper.ReplaceComma( script.AngleOffset )}, {Helper.ReplaceComma( script.MaxLeft )}, {Helper.ReplaceComma( script.MaxRight )}, {Helper.ReplaceComma( script.RotationTime )}, {Helper.ReplaceComma( script.TransitionTime )}, true )" );
                        break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;

                case BuildType.DataTable:
                    // Empty
                    break;

                case BuildType.LiveMap:
                    // Empty
                    break;
            }

            await Helper.Wait();

            return code;
        }
    }
}