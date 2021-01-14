using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;

public class VertexShaderInstancing : MonoBehaviour
{
    public Material material;
    public Mesh     instancingTargetMesh;
    public int      numOfInstances;

    private ComputeBuffer vertexBuffer;

    void Start()
    {
        Vector3[] vertices = instancingTargetMesh.triangles
            .Select(i => instancingTargetMesh.vertices[i]).ToArray();

        vertexBuffer = new ComputeBuffer
            (instancingTargetMesh.triangles.Length, Marshal.SizeOf(typeof(Vector3)));
        vertexBuffer.SetData(vertices);

        material.SetBuffer("_VertexBuffer", vertexBuffer);
    }

    void OnRenderObject()
    {
        // DrawProcedual は ComputeBuffer などから直接データを読み込んで描画します。
        // http://docs.unity3d.com/ja/current/ScriptReference/Graphics.DrawProcedural.html
        // 
        // DrawProcedual(描画形式*三角形や点など,
        //               頂点数,
        //               インスタンス数*実行回数)
        // MeshTopology は LINE など線を描画する目的などに変更することができます。

        material.SetPass(0);

        Graphics.DrawProceduralNow(MeshTopology.Triangles,
                                instancingTargetMesh.triangles.Length,
                                numOfInstances);
    }

    void OnDestroy()
    {
        vertexBuffer.Release();
    }
}