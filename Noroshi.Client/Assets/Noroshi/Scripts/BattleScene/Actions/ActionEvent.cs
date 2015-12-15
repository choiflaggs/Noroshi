using Noroshi.BattleScene.Actions.Attributes;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene.Actions
{
    public class ActionEvent
    {
        public ActionEvent(IActionExecutor executor, IActionTarget target)
        {
            Executor = executor;
            Target   = target;
        }

        public void SetHPDamage(int damage)
        {
            HPDamage = damage;
        }
        public void SetHPDamage(int damage, DamageType? damageType)
        {
            HPDamage   = damage;
            DamageType = damageType;
        }
        public void SetEnergyDamage(int damage)
        {
            EnergyDamage = damage;
        }
        public void SetHitCharacterEffectID(uint characterEffectId, bool isInterruptHitCharacterEffect = false)
        {
            HitCharacterEffectID = characterEffectId;
            IsInterruptHitCharacterEffect = isInterruptHitCharacterEffect;
            // TODO : ちゃんと外からセット。
            HitSoundID = 1;
        }

        public void SetTargetStateID(TargetStateID targetStateId)
        {
            TargetStateID = targetStateId;
        }

        public void SetAttribute(IAttribute attribute)
        {
            Attribute = attribute;
        }

        public void SetAttribute(uint attributeId, float coefficient)
        {
            AttributeID          = attributeId;
            AttributeCoefficient = coefficient;
        }

        public void SetForceRemoveAttribute()
        {
            IsForceRemoveAttribute = true;
        }

        public void RemoveAttribute()
        {
            Attribute = null;
            AttributeID = null;
            AttributeCoefficient = null;
        }

        public void SetDodge()
        {
            Dodge = true;
        }

        public void ReduceHPDamage(int damage)
        {
            HPDamage -= damage;
            if (HPDamage < 0) HPDamage = 0;
        }

        /// アクション実行者
        public IActionExecutor Executor { get; private set; }
        /// アクション対象者
        public IActionTarget Target { get; private set; }
        /// 回避成立
        public bool Dodge { get; private set; }
        public DamageType? DamageType { get; private set; }
        /// ダメージ有りの場合はダメージ（回復の場合は負で格納）
        public int? HPDamage { get; private set; }
        /// エネルギーダメージ有りの場合はダメージ（回復の場合は負で格納）
        public int? EnergyDamage { get; private set; }
        /// 付与属性ID（設定されていない場合は付与しない）
        public uint? AttributeID { get; private set; }
        /// 付与属性係数
        public float? AttributeCoefficient { get; private set; }
        /// 対象の状態ID。キャラクター内部状態遷移をさせたいに設定。
        public TargetStateID? TargetStateID { get; private set; }
        /// ヒットに表示するエフェクト
        public uint? HitCharacterEffectID { get; private set; }
        public bool IsInterruptHitCharacterEffect { get; private set; }
        public uint? HitSoundID { get; private set; }
        public IAttribute Attribute { get; private set; }
        public bool IsForceRemoveAttribute { get; private set; }
    }
}