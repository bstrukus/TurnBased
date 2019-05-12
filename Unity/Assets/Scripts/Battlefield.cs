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

        #region MonoBehaviour
        private void Awake()
        {
            Debug.Assert(entities != null, "[Battlefield] No entities in the scene.");
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
    }
}