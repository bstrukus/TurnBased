/*
 * Ben's TurnBased Strategy Game
 */

 using UnityEngine;
namespace Core
{
    public class Logging : MonoBehaviour
    {
        static public void PrintLines(string[] lines)
        {
            const string border = "========================================\n";
            string outputString = "";
            foreach (var line in lines)
            {
                outputString += line + "\n";
            }
            outputString += border;
            Debug.Log(outputString);
        }
    }
}