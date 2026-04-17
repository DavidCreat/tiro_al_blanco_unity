using UnityEngine;
using TiroAlBlanco.Data;

namespace TiroAlBlanco.Gameplay
{
    public class TargetBuilder : MonoBehaviour
    {
        [Header("Ring Settings")]
        [SerializeField] private int ringCount = 4;
        [SerializeField] private float outerRadius = 0.5f;
        [SerializeField] private float thickness = 0.05f;

        [Header("Material")]
        [SerializeField] private Material targetMaterial;

        private Transform ringParent;

        public void Build(TargetData data)
        {
            ClearRings();

            ringParent = new GameObject("Rings").transform;
            ringParent.SetParent(transform, false);

            Color[] ringColors = BuildRingColors(data);

            for (int i = 0; i < ringCount; i++)
            {
                float t = (float)i / ringCount;
                float radius = outerRadius * (1f - t);
                float zOffset = i * -0.001f;

                GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ring.name = $"Ring_{i}";
                ring.transform.SetParent(ringParent, false);
                ring.transform.localPosition = new Vector3(0f, 0f, zOffset);
                ring.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                ring.transform.localScale = new Vector3(radius * 2f, thickness, radius * 2f);

                Destroy(ring.GetComponent<Collider>());

                Renderer rend = ring.GetComponent<Renderer>();
                if (targetMaterial != null) rend.material = new Material(targetMaterial);

                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", ringColors[i]);
                rend.SetPropertyBlock(block);
            }

            // Outer trigger â€” get existing or add new, always set as trigger
            SphereCollider outerCol = GetComponent<SphereCollider>();
            if (outerCol == null) outerCol = gameObject.AddComponent<SphereCollider>();
            outerCol.radius = outerRadius;
            outerCol.isTrigger = true;

            AddCenterZone();
        }

        private void AddCenterZone()
        {
            var existing = transform.Find("CenterZone");
            if (existing != null) DestroyImmediate(existing.gameObject);

            GameObject czGO = new GameObject("CenterZone");
            czGO.transform.SetParent(transform, false);
            czGO.AddComponent<CenterZone>();

            SphereCollider czCol = czGO.AddComponent<SphereCollider>();
            czCol.radius = outerRadius * 0.25f;   // bullseye = 25% of outer radius
            czCol.isTrigger = true;
        }

        private Color[] BuildRingColors(TargetData data)
        {
            Color[] colors = new Color[ringCount];
            for (int i = 0; i < ringCount; i++)
                colors[i] = i % 2 == 0 ? data.ringColor : data.targetColor;
            return colors;
        }

        private void ClearRings()
        {
            if (ringParent != null) DestroyImmediate(ringParent.gameObject);

            // Remove all child objects that are ring visuals (children except CenterZone)
            var toDestroy = new System.Collections.Generic.List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.name != "CenterZone")
                    toDestroy.Add(child.gameObject);
            }
            foreach (var go in toDestroy)
                DestroyImmediate(go);

            var cz = transform.Find("CenterZone");
            if (cz != null) DestroyImmediate(cz.gameObject);
        }
    }
}
