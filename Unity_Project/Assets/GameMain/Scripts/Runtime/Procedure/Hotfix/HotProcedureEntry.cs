using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

namespace Game.Runtime
{
    public sealed class HotProcedureEntry : HotProcedure
    {
        public override bool UseNativeDialog { get { return true; } }

        public override string HotProcedureLogicTypeFullName { get { return null; } }

        private bool IsStart = false;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {

        }


        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {

        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if (!IsStart)
            {
                IsStart = true;
                procedureOwner.SetData("NextSceneId", new VarInt(GameEntry.Config.GetInt("Scene.Menu")));
                ChangeState<HotProcedureChangeScene>(procedureOwner);
            }
        }


        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
        }


        protected override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
        {
        }

    }
}
