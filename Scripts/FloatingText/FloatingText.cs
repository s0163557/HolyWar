using TMPro;
using UnityEngine;

namespace HolyWar.FloatText
{
    public class FloatingText : MonoBehaviour
    {
        protected bool free;
        public bool IsFree { get {return free; } }

        [SerializeField]
        protected float floatingTime = 5f;
        [SerializeField]
        protected Color startColor;
        [SerializeField]
        protected Color endColor;

        protected float timer = 0f;
        [SerializeField]
        protected float ySpeed = 0.02f;
        [SerializeField]
        protected float dy = 0.5f;
        protected TextMeshPro tmComponent;
        protected Vector4 dColor;

        public void Awake()
        {
            tmComponent = GetComponent<TextMeshPro>();
        }

        public void Start()
        {
            free = true;
            tmComponent.color = endColor;
        }

        public void Call(Vector3 position, string value)
        {
            free = false;
            tmComponent.color = startColor;
            transform.position = new Vector3(position.x, position.y - dy, position.z);
            dColor = new Vector4((startColor.r - endColor.r)/floatingTime, 
                (startColor.g - endColor.g) / floatingTime, 
                (startColor.b - endColor.b)/ floatingTime, 
                (startColor.a - endColor.a)/ floatingTime);
            tmComponent.text = value;
        }

        public void Update()
        {
            if (!free)
            {
                if (timer < floatingTime)
                {
                    timer += Time.deltaTime;
                    transform.Translate(0, ySpeed * Time.deltaTime, 0);
                    tmComponent.color = new Color(tmComponent.color.r - dColor.x * Time.deltaTime / floatingTime,
                        tmComponent.color.g - dColor.y * Time.deltaTime / floatingTime,
                        tmComponent.color.b - dColor.z * Time.deltaTime / floatingTime,
                        tmComponent.color.a - dColor.w * Time.deltaTime / floatingTime);
                }
                else
                {
                    timer = 0f;
                    free = true;
                    tmComponent.color = endColor;
                }
            }
        }
    }
}