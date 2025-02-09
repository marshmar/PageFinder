using UnityEngine;

namespace Buffs
{
    public class TemporaryMovementBuff : ITemporaryBuffCommand
    {
        private float elapsedTime;
        private float duration;

        private IEntityState entityState;

        public TemporaryMovementBuff(IEntityState entityState)
        {
            this.entityState = entityState;
            elapsedTime = 0;
        }

        public override void Execute()
        {
            
        }

        public override void Tick()
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= Duration)
            {
                EndBuff();
            }
        }

        public override void EndBuff()
        {

        }
    }
}
