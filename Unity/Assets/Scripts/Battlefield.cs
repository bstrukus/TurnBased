/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;

namespace Core
{
    public class Battlefield : MonoBehaviour
    {
        [SerializeField]
        private Entity[] entities = null;

        [SerializeField]
        private Grid grid = null;
        public Grid Grid { get { return this.grid; } }

        public Vector3 UpDown { get; private set; }
        public Vector3 RightLeft { get; private set; }

        #region MonoBehaviour
        private void Awake()
        {
            Debug.Assert(entities != null, "[Battlefield] No entities in the scene.");
            

            //this.UpDown
        }

        private void Start()
        {
            PrintOutGridProperties();

//                 Debug.Log("Printing out all of the Grid properties I'm interested in.");
//                 Debug.Log($"Cell Size: {this.Battlefield.Grid.cellSize}");
//                 Debug.Log($"Cell Gap: {this.Battlefield.Grid.cellGap}");
//                 Debug.Log($"Cell Layout: {this.Battlefield.Grid.cellLayout}");
//                 Debug.Log($"Cell Swizzle: {this.Battlefield.Grid.cellSwizzle}");
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

        private void PrintOutGridProperties()
        {
            string[] lines = {
                "Grid Properties:",
                "Foo",
                "Bar"
            };

            string gridProperties = "";
            foreach (var line in lines)
            {
                gridProperties += line + "\n";
            }
            Debug.Log(gridProperties);
        }
    }
}