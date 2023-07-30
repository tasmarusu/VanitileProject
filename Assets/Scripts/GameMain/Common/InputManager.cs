using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// インプット操作
    /// </summary>
    public class InputManager : SingletonMono<InputManager>
    {
        public float Horizontal { get { return Input.GetAxis("Horizontal"); } }
        public float Vertical { get { return Input.GetAxis("Vertical"); } }
        public bool Jump { get { return Input.GetKeyDown(KeyCode.Space); } }
    }
}