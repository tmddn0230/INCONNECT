using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

namespace Rokoko.Core
{
    public struct CoreBoneData
    {
        public string name;
        public float[] hipPosition;
        public float[] hipRotation;
        public float[] spinePosition;
        public float[] spineRotation;
        public float[] chestPosition;
        public float[] chestRotation;
        public float[] neckPosition;
        public float[] neckRotation;
        public float[] headPosition;
        public float[] headRotation;
        public float[] leftFootPosition;
        public float[] leftFootRotation;
        public float[] rightFootPosition;
        public float[] rightFootRotation;
    }

    public class CoreBoneDataWrapper
    {
        public CoreBoneData[] actors;
    }

    public class StudioReceiver : UDPReceiver
    {
        public event EventHandler<LiveFrame_v4> onStudioDataReceived;
        public bool useLZ4Compression = true;
        public bool verbose = false;

        private string filePath = "path/to/your/file.txt"; // 파일 경로를 설정하세요.


        protected override void OnDataReceived(byte[] data, IPEndPoint endPoint)
        {
            LiveFrame_v4 liveFrame_V4 = null;
            try
            {
                base.OnDataReceived(data, endPoint);
                byte[] uncompressed;

                if (useLZ4Compression)
                {
                    // Decompress LZ4
                    uncompressed = LZ4Wrapper.Decompress(data);
                    if (uncompressed == null || uncompressed.Length == 0)
                    {
                        Debug.LogError("Incoming data are in bad format. Please ensure you are using JSON v3 as forward data format");
                        return;
                    }
                }
                else
                {
                    uncompressed = data;
                }

                // Convert from Json
                string text = Encoding.UTF8.GetString(uncompressed);
                if (verbose)
                {
                    //Debug.Log(text);
                }
                liveFrame_V4 = JsonUtility.FromJson<LiveFrame_v4>(text);

                if (liveFrame_V4 == null)
                {
                    Debug.LogError("Incoming data are in bad format. Please ensure you are using JSON v3 as forward data format");
                    return;
                }

                // Extract necessary data
                CoreBoneDataWrapper coreBoneDataWrapper = ExtractCoreBoneData(liveFrame_V4);
                string coreBoneDataJson = JsonUtility.ToJson(coreBoneDataWrapper);

                // Convert to JSON
                if (verbose)
                {
                    //Debug.LogError(this);
                    //Debug.Log(coreBoneDataJson);
                }

                // Use coreBoneDataJson as needed
            }
            catch { }

            onStudioDataReceived?.Invoke(this, liveFrame_V4);
        }

        private CoreBoneDataWrapper ExtractCoreBoneData(LiveFrame_v4 liveFrame)
        {
            List<CoreBoneData> coreBoneDataList = new List<CoreBoneData>();

            foreach (var actor in liveFrame.scene.actors)
            {
                CoreBoneData coreBoneData = new CoreBoneData
                {
                    name = actor.name,
                    hipPosition = new float[] { actor.body.hip.position.x, actor.body.hip.position.y, actor.body.hip.position.z },
                    hipRotation = new float[] { actor.body.hip.rotation.x, actor.body.hip.rotation.y, actor.body.hip.rotation.z, actor.body.hip.rotation.w },
                    spinePosition = new float[] { actor.body.spine.position.x, actor.body.spine.position.y, actor.body.spine.position.z },
                    spineRotation = new float[] { actor.body.spine.rotation.x, actor.body.spine.rotation.y, actor.body.spine.rotation.z, actor.body.spine.rotation.w },
                    chestPosition = new float[] { actor.body.chest.position.x, actor.body.chest.position.y, actor.body.chest.position.z },
                    chestRotation = new float[] { actor.body.chest.rotation.x, actor.body.chest.rotation.y, actor.body.chest.rotation.z, actor.body.chest.rotation.w },
                    neckPosition = new float[] { actor.body.neck.position.x, actor.body.neck.position.y, actor.body.neck.position.z },
                    neckRotation = new float[] { actor.body.neck.rotation.x, actor.body.neck.rotation.y, actor.body.neck.rotation.z, actor.body.neck.rotation.w },
                    headPosition = new float[] { actor.body.head.position.x, actor.body.head.position.y, actor.body.head.position.z },
                    headRotation = new float[] { actor.body.head.rotation.x, actor.body.head.rotation.y, actor.body.head.rotation.z, actor.body.head.rotation.w },
                    leftFootPosition = new float[] { actor.body.leftFoot.position.x, actor.body.leftFoot.position.y, actor.body.leftFoot.position.z },
                    leftFootRotation = new float[] { actor.body.leftFoot.rotation.x, actor.body.leftFoot.rotation.y, actor.body.leftFoot.rotation.z, actor.body.leftFoot.rotation.w },
                    rightFootPosition = new float[] { actor.body.rightFoot.position.x, actor.body.rightFoot.position.y, actor.body.rightFoot.position.z },
                    rightFootRotation = new float[] { actor.body.rightFoot.rotation.x, actor.body.rightFoot.rotation.y, actor.body.rightFoot.rotation.z, actor.body.rightFoot.rotation.w }
                };

                coreBoneDataList.Add(coreBoneData);
            }

            CoreBoneDataWrapper coreBoneDataWrapper = new CoreBoneDataWrapper
            {
                actors = coreBoneDataList.ToArray()
            };

            return coreBoneDataWrapper;

        }
    }


}
