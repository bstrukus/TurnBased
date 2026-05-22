using UnityEngine;
using System.Collections.Generic;

namespace Tactics
{
    public enum CellHighlight { None, Move, Attack, Selected }

    public class GridSystem : MonoBehaviour
    {
        public const int Width = 20;
        public const int Depth = 20;

        [Header("Height generation")]
        [SerializeField] private float noiseScale = 0.35f;
        [SerializeField] private int maxHeight = 4;

        [Header("Materials (auto-created if null)")]
        [SerializeField] private Material defaultMat;
        [SerializeField] private Material moveMat;
        [SerializeField] private Material attackMat;
        [SerializeField] private Material selectedMat;

        private GridCell[,] cells = new GridCell[Width, Depth];

        public GridCell GetCell(int x, int z)
        {
            if (x < 0 || x >= Width || z < 0 || z >= Depth) return null;
            return cells[x, z];
        }

        // Returns the grid cell hit by a screen-space ray (used for unit placement queries)
        public GridCell GetCellAtWorldXZ(float worldX, float worldZ)
        {
            return GetCell(Mathf.RoundToInt(worldX), Mathf.RoundToInt(worldZ));
        }

        public Vector3 GridCenter => new Vector3(Width * 0.5f, 0f, Depth * 0.5f);

        private void Awake()
        {
            EnsureMaterials();
            GenerateCells();
            BuildVisuals();
        }

        private void EnsureMaterials()
        {
            if (defaultMat  == null) defaultMat  = CreateMat(new Color(0.55f, 0.45f, 0.35f));
            if (moveMat     == null) moveMat     = CreateMat(new Color(0.25f, 0.55f, 1.00f, 0.8f));
            if (attackMat   == null) attackMat   = CreateMat(new Color(1.00f, 0.25f, 0.25f, 0.8f));
            if (selectedMat == null) selectedMat = CreateMat(new Color(1.00f, 0.90f, 0.20f, 0.9f));
        }

        private Material CreateMat(Color color)
        {
            var mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            return mat;
        }

        private void GenerateCells()
        {
            // Offset the noise so each play/scene looks different
            float ox = Random.Range(0f, 100f);
            float oz = Random.Range(0f, 100f);

            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    float n = Mathf.PerlinNoise(ox + x * noiseScale, oz + z * noiseScale);
                    int h = Mathf.RoundToInt(n * maxHeight);
                    cells[x, z] = new GridCell(x, z, h);
                }
            }
        }

        private void BuildVisuals()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    GridCell cell = cells[x, z];

                    // Pillar: fills from y=0 to the surface height
                    float pillarH = cell.Height + 1f;
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.name = $"Cell_{x}_{z}";
                    go.transform.parent = transform;
                    go.transform.localScale = new Vector3(0.95f, pillarH, 0.95f);
                    // Position so top face sits at y = cell.Height
                    go.transform.position = new Vector3(x, cell.Height - pillarH * 0.5f + 0.5f, z);
                    go.GetComponent<Renderer>().material = defaultMat;

                    cell.Visual = go;
                    var selector = go.AddComponent<CellSelector>();
                    selector.Initialize(cell);
                }
            }
        }

        public void SetHighlight(GridCell cell, CellHighlight type)
        {
            if (cell?.Visual == null) return;
            var r = cell.Visual.GetComponent<Renderer>();
            r.material = type switch
            {
                CellHighlight.Move     => moveMat,
                CellHighlight.Attack   => attackMat,
                CellHighlight.Selected => selectedMat,
                _                      => defaultMat,
            };
        }

        public void SetHighlights(IEnumerable<GridCell> cells, CellHighlight type)
        {
            foreach (var c in cells) SetHighlight(c, type);
        }

        public void ClearAllHighlights()
        {
            for (int x = 0; x < Width; x++)
                for (int z = 0; z < Depth; z++)
                    SetHighlight(cells[x, z], CellHighlight.None);
        }
    }
}
