using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;

public class SimpleInstancing : MonoBehaviour
{
    public Shader simpleInstancingShader;

    public Mesh instancingTargetMesh;
    public int numOfInstances;

    private Material material;
    private ComputeBuffer vertexBuffer;

    void Start()
    {
        Vector3[] vertices = instancingTargetMesh.triangles
            .Select(i => instancingTargetMesh.vertices[i]).ToArray();

        this.vertexBuffer = new ComputeBuffer
            (this.instancingTargetMesh.triangles.Length, Marshal.SizeOf(typeof(Vector3)));
        this.vertexBuffer.SetData(vertices);

        this.material = new Material(this.simpleInstancingShader);
        this.material.SetBuffer("_VertexBuffer", this.vertexBuffer);
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

        this.material.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Triangles,
                                this.instancingTargetMesh.triangles.Length,
                                this.numOfInstances);
    }

    void OnDestroy()
    {
        DestroyImmediate(this.material);
        this.vertexBuffer.Release();
    }
}