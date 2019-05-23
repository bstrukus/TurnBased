/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core
{
    public class Battlefield : MonoBehaviour
    {
        [SerializeField]
        private Entity[] entities = null;

        [SerializeField]
        private Grid grid = null;
        public Grid Grid { get { return this.grid; } }

        [SerializeField]
        private Tilemap tilemap = null;
        public Tilemap Tilemap { get { return this.tilemap; } }

        public Vector3 UpDown    { get { return new Vector3(0.5f,  0.25f); } }
        public Vector3 RightLeft { get { return new Vector3(0.5f, -0.25f); } }

        #region MonoBehaviour
        private void Awake()
        {
            Debug.Assert(entities != null, "[Battlefield] No entities in the scene.");
        }

        private void Start()
        {
            PrintGridProperties();
        }
        #endregion

        public Entity CheckSquare(int x, int y)
        {
//             for (int i = 0; i < entities.Length; ++i)
//             {
//                 Vector3Int location = entities[i].transform.position;
//                 if (entities[i].transform)
//             }
            return null;
        }

        private void PrintGridProperties()
        {
            string[] lines = {
                "-------- GRID PROPERTIES --------",
                $"Cell Size: {this.Grid.cellSize}",
                $"Cell Gap: {this.Grid.cellGap}",
                $"Cell Layout: {this.Grid.cellLayout}",
                $"Cell Swizzle: {this.Grid.cellSwizzle}",
                $"Layout Cell Center: {this.Grid.GetLayoutCellCenter()}",
            };

            Logging.PrintLines(lines);
        }
    }
}