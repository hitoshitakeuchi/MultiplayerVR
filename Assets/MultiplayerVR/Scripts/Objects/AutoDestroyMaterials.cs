using UnityEngine;
/// <summary>
/// 自身の破棄時に、自動的にMaterialを破棄する
/// </summary>
public class AutoDestroyMaterials : MonoBehaviour
{

    #region Parameters
    [System.NonSerialized]
    public bool IsRoot = true;

    #endregion

    void Start()
    {
        if (IsRoot)
        {
            foreach (var r in this.GetComponentsInChildren<Renderer>())
            {
                r.gameObject.AddComponent<AutoDestroyMaterials>().IsRoot = false;
            }
            foreach (var p in this.GetComponentsInChildren<ParticleSystem>())
            {
                p.gameObject.AddComponent<AutoDestroyMaterials>().IsRoot = false;
            }
        }
    }

    void OnDestroy()
    {
        var thisRenderer = this.GetComponent<Renderer>();
        if (thisRenderer != null && thisRenderer.materials != null)
        {
            foreach (var m in thisRenderer.materials)
            {
                DestroyImmediate(m);
            }
        }
    }

}
