using UnityEngine;
using ZestCore.Utility;

namespace ZestGames
{
    public class TakeScreenshot : MonoBehaviour
    {
        private void Update()
        {
            Screenshot.TakeAScreenshot();
        }
    }
}
