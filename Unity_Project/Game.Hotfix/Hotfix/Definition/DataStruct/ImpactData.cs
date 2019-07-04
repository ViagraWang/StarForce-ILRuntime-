using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Hotfix
{
    /// <summary>
    /// 碰撞数据
    /// </summary>
    public class ImpactData : IReference
    {
        public CampType Camp { get; private set; }

        public int HP { get; private set; }

        public int Attack { get; private set; }

        public int Defense { get; private set; }

        public ImpactData Fill(CampType camp, int hp, int attack, int defense)
        {
            Camp = camp;
            HP = hp;
            Attack = attack;
            Defense = defense;
            return this;
        }

        public void Clear()
        {
            Camp = default;
            HP = default;
            Attack = default;
            Defense = default;
        }
    }
}
