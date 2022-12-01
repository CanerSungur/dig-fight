using UnityEngine;

namespace DigFight
{
    public class Debris : MonoBehaviour
    {
        private DebrisHandler _debrisHandler;
        private Rigidbody _rigidbody;

        public void Init(DebrisHandler debrisHandler)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
                _debrisHandler = debrisHandler;
            }

            _rigidbody.isKinematic = true;
        }

        public void Release()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 5f), Random.Range(1f, 2f)) * _debrisHandler.ReleaseForce, ForceMode.Impulse);
            DestroyAfterDelay();
        }

        private void DestroyAfterDelay()
        {
            Destroy(gameObject, 5f);
        }
    }
}
