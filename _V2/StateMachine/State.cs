namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public abstract class State : MonoBehaviour
    {
        public int priority = 1;
        public abstract void OnStateEnter();
        public abstract Task OnStateExit();

        public abstract State Tick();
    }
}
