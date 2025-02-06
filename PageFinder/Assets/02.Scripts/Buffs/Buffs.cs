using UnityEngine;

namespace Buffs
{
    public class TemporaryMovementBuff : ITemporaryBuffCommand
    {
        private float elapsedTime;
        private IEntityState entityState;
        public float Duration { get; set; }

        public BuffType Type { get; set; }

        public float Value { get; set; }


        public TemporaryMovementBuff(IEntityState entityState)
        {
            this.entityState = entityState;
            elapsedTime = 0;
        }

        public void Execute()
        {
            
        }

        public void Tick()
        {
            elapsedTime += Time.deltaTime;
        }
    }
}
