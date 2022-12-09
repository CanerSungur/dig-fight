using UnityEngine;

namespace DigFight
{
    public class ParallaxBackground : MonoBehaviour
    {
        private float _startPosition;

        [Header("-- SETUP --")]
        [SerializeField] private GameObject _cameraObj;
        [SerializeField] private float _parallaxEffect;

        private void Start()
        {
            _startPosition = transform.position.x;
        }

        private void Update()
        {
            float distance = _cameraObj.transform.position.x * _parallaxEffect;
            transform.position = new Vector3(_startPosition + distance, transform.position.y, transform.position.z);
        }
    }
}
